using System.Security.Cryptography;
using System.Text;
using Force.Crc32;
using Aes = System.Security.Cryptography.Aes;

namespace Dorisoy.PanClient.Utils;

public class AesOperation
{

    private static readonly byte[] SALT = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };



    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static byte[] ReadFile(string filePath, bool decrypt = true)
    {
        var _pathHelper = new PathHelper();
        byte[] newBytes;
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            byte[] bytes = new byte[stream.Length];
            int numBytesToRead = (int)stream.Length;
            int numBytesRead = 0;
            while (numBytesToRead > 0)
            {
                // Read可以返回0到numBytesToRead之间的任何值.
                int n = stream.Read(bytes, numBytesRead, numBytesToRead);

                // 到达文件末尾时中断
                if (n == 0)
                    break;

                numBytesRead += n;
                numBytesToRead -= n;
            }

            if (decrypt)
                //解密文件
                newBytes = DecryptStream(bytes, _pathHelper.EncryptionKey);
            else
                newBytes = bytes;
        }
        return newBytes;
    }

    /// <summary>
    /// 加密流
    /// </summary>
    /// <param name="input"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public static byte[] EncryptStream(byte[] input, string password)
    {
        byte[] encrypted;
        using (Aes aes = Aes.Create())
        {
            var pdb = new Rfc2898DeriveBytes(password, SALT);
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

    /// <summary>
    /// 解密流
    /// </summary>
    /// <param name="input"></param>
    /// <param name="password"></param>
    /// <returns></returns>
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

    public static byte[] ReadAsBytesAsync(string filename)
    {
        return File.ReadAllBytes(filename);
    }

    public static uint Crc32CAlgorithmBigCrc(string fileName)
    {
        uint hash = 0;
        byte[] buffer = null;
        FileInfo fileInfo = new FileInfo(fileName);
        long fileLength = fileInfo.Length;
        int blockSize = 1024000000;
        decimal div = fileLength / blockSize;
        int blocks = (int)Math.Floor(div);
        int restBytes = (int)(fileLength - (blocks * blockSize));
        long offsetFile = 0;
        uint interHash = 0;
        //Crc32CAlgorithm crc32CAlgorithm = new Crc32CAlgorithm();
        bool firstBlock = true;
        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            buffer = new byte[blockSize];
            using (BinaryReader br = new BinaryReader(fs))
            {
                while (blocks > 0)
                {
                    blocks -= 1;
                    fs.Seek(offsetFile, SeekOrigin.Begin);
                    buffer = br.ReadBytes(blockSize);
                    if (firstBlock)
                    {
                        firstBlock = false;
                        interHash = Crc32CAlgorithm.Compute(buffer);
                        hash = interHash;
                    }
                    else
                    {
                        hash = Crc32CAlgorithm.Append(interHash, buffer);
                    }
                    offsetFile += blockSize;
                }
                if (restBytes > 0)
                {
                    Array.Resize(ref buffer, restBytes);
                    fs.Seek(offsetFile, SeekOrigin.Begin);
                    buffer = br.ReadBytes(restBytes);
                    hash = Crc32CAlgorithm.Append(interHash, buffer);
                }
                buffer = null;
            }
        }
        //MessageBox.Show(hash.ToString());
        //MessageBox.Show(hash.ToString("X"));
        return hash;
    }


    public static void SaveBytesToFile(string filename, byte[] bytesToWrite)
    {
        if (filename != null && filename.Length > 0 && bytesToWrite != null)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

            FileStream file = File.Create(filename);

            file.Write(bytesToWrite, 0, bytesToWrite.Length);

            file.Close();
        }
    }
}
