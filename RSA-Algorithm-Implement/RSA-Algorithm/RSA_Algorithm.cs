using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace RSA_Algorithm
{
    public static class RSA_Algorithm
    {
        public static (string,string) GenerateKeys(int keySize = 4096)
        {
            string publicKey;
            string privateKey;
            using (RSA rsa = RSA.Create(keySize))
            {
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);
            }

            return (publicKey, privateKey);
        }

        public static string EncryptText(string plainText, string publicKey)
        {
            string encryptedBase64 = string.Empty;
            try
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(publicKey);

                    byte[] plaintextBytes = Encoding.UTF8.GetBytes(plainText);
                    byte[] encryptedBytes = rsa.Encrypt(plaintextBytes, RSAEncryptionPadding.OaepSHA256);
                    encryptedBase64 = Convert.ToBase64String(encryptedBytes);                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encryption error: {ex.Message}");
            }

            return encryptedBase64;
        }

        public static string EncryptObject(object obj, string publicKey)
        {
            string encryptedBase64 = string.Empty;
            try
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(publicKey);

                    byte[] plaintextBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));
                    byte[] encryptedBytes = rsa.Encrypt(plaintextBytes, RSAEncryptionPadding.OaepSHA256);
                    encryptedBase64 = Convert.ToBase64String(encryptedBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Encryption error: {ex.Message}");
            }

            return encryptedBase64;
        }

        public static string DecryptText(string encryptedBase64, string privateKey)
        {
            string decryptedText = string.Empty;
            try
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(privateKey);
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
                    byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
                    decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption error: {ex.Message}");
            }
            return decryptedText;
        }

        public static T DecryptObject<T>(string encryptedBase64, string privateKey)
        {
            string decryptedText = string.Empty;
            try
            {
                using (RSA rsa = RSA.Create())
                {
                    rsa.FromXmlString(privateKey);
                    byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);
                    byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
                    decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption error: {ex.Message}");
            }
            return JsonSerializer.Deserialize<T>(decryptedText);
        }
    }
}
