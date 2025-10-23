using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Dorisoy.Pan.Helper
{
    public class AesOperation
    {

        private static readonly byte[] SALT = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };


        public static byte[] EncryptStream(byte[] input, string password)
        {
            byte[] encrypted;
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
                aes.Key = pdb.GetBytes(32);
                aes.IV = pdb.GetBytes(16);
                ICryptoTransform encryptor = aes.CreateEncryptor();
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(input, 0, input.Length);
                        cryptoStream.FlushFinalBlock();
                        cryptoStream.Close();
                    }
                    encrypted = memoryStream.ToArray();
                }
            }
            return encrypted;
        }

        public static byte[] DecryptStream(byte[] input, string password)
        {

            byte[] decrypted;
            using (Aes aes = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT);
                aes.Key = pdb.GetBytes(32);
                aes.IV = pdb.GetBytes(16);
                ICryptoTransform decryptor = aes.CreateDecryptor();

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(input, 0, input.Length);
                        cryptoStream.Close();
                    }
                    decrypted = memoryStream.ToArray();
                }
            }
            return decrypted;
        }

        public static string Get128BitString(string keyToConvert)
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                b.Append(keyToConvert[i % keyToConvert.Length]);
            }
            keyToConvert = b.ToString();

            return keyToConvert;
        }

        public static byte[] ReadAsBytesAsync(IFormFile file)
        {

            byte[] fileBytes;

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                fileBytes = ms.ToArray();
                // act on the Base64 data
            }
            return fileBytes;
        }
    }

}
