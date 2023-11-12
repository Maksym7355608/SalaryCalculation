using System.Security.Cryptography;
using System.Text;

namespace Identity.App.Helpers;

public class PasswordEncryptor
{
    private readonly string _encryptionKey;
    private readonly string _salt = "MySaltValue";

    public PasswordEncryptor(string encryptionKey)
    {
        _encryptionKey = encryptionKey;
    }

    public string EncryptPassword(string password)
    {
        using (var aesAlg = Aes.Create())
        {
            var keyDerivation = new Rfc2898DeriveBytes(_encryptionKey, Encoding.ASCII.GetBytes(_salt));

            aesAlg.Key = keyDerivation.GetBytes(32);
            aesAlg.IV = keyDerivation.GetBytes(16);

            var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (var msEncrypt = new MemoryStream())
            {
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(password);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public string DecryptPassword(string encryptedPassword)
    {
        using (var aesAlg = Aes.Create())
        {
            var keyDerivation = new Rfc2898DeriveBytes(_encryptionKey, Encoding.ASCII.GetBytes(_salt));

            aesAlg.Key = keyDerivation.GetBytes(32);
            aesAlg.IV = keyDerivation.GetBytes(16);

            var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedPassword)))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }
}