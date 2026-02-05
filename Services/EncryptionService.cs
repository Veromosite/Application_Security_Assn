using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Services
{
    public class EncryptionService
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

     public EncryptionService(IConfiguration configuration)
        {
       // Get encryption key and IV from configuration
        // IMPORTANT: In production, store these in Azure Key Vault or secure configuration
       var keyString = configuration["Encryption:Key"] ?? "32CharacterLongEncryptionKey!!"; // Must be 32 chars for AES-256
         var ivString = configuration["Encryption:IV"] ?? "16CharIVValue!!!"; // Must be 16 chars

            _key = Encoding.UTF8.GetBytes(keyString.PadRight(32).Substring(0, 32));
   _iv = Encoding.UTF8.GetBytes(ivString.PadRight(16).Substring(0, 16));
    }

        public string Encrypt(string plainText)
        {
     if (string.IsNullOrEmpty(plainText))
       return plainText;

  using (Aes aes = Aes.Create())
            {
    aes.Key = _key;
   aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
     aes.Padding = PaddingMode.PKCS7;

         ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

    using (MemoryStream msEncrypt = new MemoryStream())
     {
    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
          {
        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
     {
           swEncrypt.Write(plainText);
              }
      return Convert.ToBase64String(msEncrypt.ToArray());
 }
                }
            }
        }

        public string Decrypt(string cipherText)
  {
      if (string.IsNullOrEmpty(cipherText))
      return cipherText;

       try
            {
     using (Aes aes = Aes.Create())
         {
         aes.Key = _key;
         aes.IV = _iv;
         aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

       ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
  {
            using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
               {
      using (StreamReader srDecrypt = new StreamReader(csDecrypt))
   {
             return srDecrypt.ReadToEnd();
     }
    }
      }
      }
            }
        catch
       {
                return cipherText; // Return original if decryption fails
   }
        }
    }
}
