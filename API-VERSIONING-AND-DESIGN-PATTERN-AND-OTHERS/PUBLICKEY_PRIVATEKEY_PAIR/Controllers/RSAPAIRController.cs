using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PUBLICKEY_PRIVATEKEY_PAIR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RSAPAIRController : ControllerBase
    {
        private readonly RsaKeyService _rsaKeyService = new RsaKeyService();
        private string _publicKey;
        private string _privateKey;

        public RSAPAIRController()
        {
            var (publicKey, privateKey) = _rsaKeyService.GenerateKeys();
            _publicKey = publicKey;
            _privateKey = privateKey;
        }

        // GET: api/<RSAPAIRController>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new {
                publicKey = _publicKey,
                privateKey = _privateKey
            });
        }

        [HttpGet("encrypt")]
        public IActionResult Encrypt(string plainText)
        {
            var encryptedText = EncryptData(plainText, _publicKey);
            return Ok(new { encryptedText = Convert.ToBase64String(encryptedText) });
        }

        [HttpGet("decrypt")]
        public IActionResult Decrypt(string plainText)
        {
            var encryptedText = DecryptData(Convert.FromBase64String(plainText), _privateKey);
            return Ok(new { encryptedText = encryptedText });
        }

        private byte[] EncryptData(string plainText, string publicKeyPem)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem.ToCharArray());
            return rsa.Encrypt(Encoding.UTF8.GetBytes(plainText), RSAEncryptionPadding.Pkcs1);
        }

        private string DecryptData(byte[] cipherText, string privateKeyPem)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem.ToCharArray());
            var decryptedBytes = rsa.Decrypt(cipherText, RSAEncryptionPadding.Pkcs1);
            return Encoding.UTF8.GetString(decryptedBytes);
        }

    }
}
