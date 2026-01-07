using MedXDataCollection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace PUBLICKEY_PRIVATEKEY_PAIR
{
    public class RsaKeyService
    {
        public (string publicKey, string privateKey) GenerateKeys(int keySize = 2048)
        {
            using var rsa = RSA.Create(keySize);

            var publicKey = ExportPublicKey(rsa);
            var privateKey = ExportPrivateKey(rsa);

            return (publicKey, privateKey);
        }

        private string ExportPublicKey(RSA rsa)
        {
            var keyBytes = rsa.ExportSubjectPublicKeyInfo();
            return ToPem("PUBLIC KEY", keyBytes);
        }

        private string ExportPrivateKey(RSA rsa)
        {
            var keyBytes = rsa.ExportPkcs8PrivateKey();
            return ToPem("PRIVATE KEY", keyBytes);
        }

        private string ToPem(string title, byte[] data)
        {
            var base64 = Convert.ToBase64String(data);
            var sb = new StringBuilder();

            sb.AppendLine($"-----BEGIN {title}-----");

            for (int i = 0; i < base64.Length; i += 64)
                sb.AppendLine(base64.Substring(i, Math.Min(64, base64.Length - i)));

            sb.AppendLine($"-----END {title}-----");

            return sb.ToString();
        }
    }
}
