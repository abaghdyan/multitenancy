using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Multitenancy.Common.Data.Helpers;

public class SecurityHelper
{
    public static string Encrypt(string key, string data)
    {
        string encData = null;
        byte[][] keys = GetHashKeys(key);

        try
        {
            encData = EncryptStringToBytes_Aes(data, keys[0], keys[1]);
        }
        catch (CryptographicException) { }
        catch (ArgumentNullException) { }

        return encData;
    }

    public static string Decrypt(string key, string data)
    {
        string decData = null;
        byte[][] keys = GetHashKeys(key);

        try
        {
            decData = DecryptStringFromBytes_Aes(data, keys[0], keys[1]);
        }
        catch (CryptographicException) { }
        catch (ArgumentNullException) { }

        return decData;
    }

    private static byte[][] GetHashKeys(string key)
    {
        byte[][] result = new byte[2][];
        Encoding enc = Encoding.UTF8;

        var sha256 = SHA256.Create();

        byte[] rawKey = enc.GetBytes(key);
        byte[] rawIV = enc.GetBytes(key);

        byte[] hashKey = sha256.ComputeHash(rawKey);
        byte[] hashIV = sha256.ComputeHash(rawIV);

        Array.Resize(ref hashIV, 16);

        result[0] = hashKey;
        result[1] = hashIV;

        return result;
    }

    //source: https://msdn.microsoft.com/de-de/library/system.security.cryptography.aes(v=vs.110).aspx
    private static string EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
    {
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        byte[] encrypted;

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt =
                        new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }
        return Convert.ToBase64String(encrypted);
    }

    //source: https://msdn.microsoft.com/de-de/library/system.security.cryptography.aes(v=vs.110).aspx
    private static string DecryptStringFromBytes_Aes(string cipherTextString, byte[] Key, byte[] IV)
    {
        byte[] cipherText = Convert.FromBase64String(cipherTextString);

        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        if (Key == null || Key.Length <= 0)
            throw new ArgumentNullException("Key");
        if (IV == null || IV.Length <= 0)
            throw new ArgumentNullException("IV");

        string plaintext = null;

        using (var aesAlg = Aes.Create())
        {
            aesAlg.Key = Key;
            aesAlg.IV = IV;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var msDecrypt = new MemoryStream(cipherText))
            {
                using (var csDecrypt =
                        new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        return plaintext;
    }
}
