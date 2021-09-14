using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NetConnMon.Domain.Configuration;

namespace NetConnMon.Domain.Logic
{
    public interface IEncryptor
    {
        string Encrypt(string text);
        string Decrypt(string text);
    }

    // largely built around these two sources that were shared with the world:
    // https://github.com/damienbod/SendingEncryptedData/blob/main/EncryptDecryptLib/SymmetricEncryptDecrypt.cs
    // https://dev.to/stratiteq/cryptography-with-practical-examples-in-net-core-1mc4
    public class Encryptor : IEncryptor
    {
        private EncryptionSettings encryptionSettings;

        public Encryptor(IOptions<EncryptionSettings> options)
        {
            encryptionSettings = options?.Value ?? null; 
            if (encryptionSettings == null)
                throw new NullReferenceException("EncryptionSettings required in Encryptor");
        }

        public Encryptor(EncryptionSettings settings) => encryptionSettings = settings;
        public Encryptor(string key, string iv)
        {
            encryptionSettings = new() { KeyBase64 = key, IVBase64 = iv };
        }
        public static EncryptionSettings CreateNewEncryptionSettings()
        {
            var key = GetEncodedRandomString(32); // for a 256 size
            Aes cipher = CreateCipher(key);

            return new EncryptionSettings() { 
                KeyBase64 = key, 
                IVBase64 = Convert.ToBase64String(cipher.IV) 
            };
        }

        /// <summary>
        /// Encrypt using AES
        /// </summary>
        /// <param name="text">any text</param>
        /// <returns>Returns an encrypted string</returns>
        public string Encrypt(string text)
        {
            Aes cipher = CreateCipher(encryptionSettings.KeyBase64);
            cipher.IV = Convert.FromBase64String(encryptionSettings.IVBase64);

            ICryptoTransform cryptTransform = cipher.CreateEncryptor();
            byte[] plaintext = Encoding.UTF8.GetBytes(text);
            byte[] cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);

            return Convert.ToBase64String(cipherText);
        }

        /// <summary>
        /// Decrypt using AES
        /// </summary>
        /// <param name="text">Base64 string for an AES encryption</param>
        /// <returns>Returns a string</returns>
        public string Decrypt(string encryptedText)
        {
            Aes cipher = CreateCipher(encryptionSettings.KeyBase64);
            cipher.IV = Convert.FromBase64String(encryptionSettings.IVBase64);

            ICryptoTransform cryptTransform = cipher.CreateDecryptor();
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            byte[] plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(plainBytes);
        }

        private static string GetEncodedRandomString(int length)
        {
            var base64 = Convert.ToBase64String(GenerateRandomBytes(length));
            return base64;
        }

        /// <summary>
        /// Create an AES Cipher using a base64 key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>AES</returns>
        private static Aes CreateCipher(string keyBase64)
        {
            // Default values: Keysize 256, Padding PKC27
            Aes cipher = Aes.Create();
            cipher.Mode = CipherMode.CBC; // Ensure the integrity of the ciphertext if using CBC
            cipher.Padding = PaddingMode.ISO10126;
            cipher.Key = Convert.FromBase64String(keyBase64);

            return cipher;
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            var byteArray = new byte[length];
            RandomNumberGenerator.Fill(byteArray);
            return byteArray;
        }

    }
}
