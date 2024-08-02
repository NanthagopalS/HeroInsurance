using System.Security.Cryptography;
using System.Text;

namespace Insurance.Persistence.ICIntegration.Implementation;

public class ICICIEncrypt
{
    RSACryptoServiceProvider rsaCryptoServiceProvider;
    RSA rsa;

    public ICICIEncrypt()
    {
        //rsaCryptoServiceProvider = new RSACryptoServiceProvider();
        rsa = RSA.Create();
    }

    public void ImportPrivateKey(string fileName)
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsa.importrsaprivatekey?view=netcore-3.1
        // https://vcsjones.dev/2019/10/07/key-formats-dotnet-3/
        // Read the file
        string text = File.ReadAllText(fileName);
        // Remove the "-----BEGIN RSA PRIVATE KEY-----" etc.
        text = text.Replace("-----BEGIN RSA PRIVATE KEY-----", "");
        text = text.Replace("-----END RSA PRIVATE KEY-----", "");
        text = text.Replace("\r", "");
        text = text.Replace("\n", "");
        // Decode from Base64
        var privateKeyBytes = Convert.FromBase64String(text);
        // Now import
        int bytesRead;
        rsa.ImportRSAPrivateKey(privateKeyBytes, out bytesRead);
    }

    public void ImportPublicKey(string fileName)
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsa.importrsaprivatekey?view=netcore-3.1
        // https://vcsjones.dev/2019/10/07/key-formats-dotnet-3/
        // Read the file
        string text = fileName;
        // Remove the "-----BEGIN PUBLIC KEY-----" etc.
        text = text.Replace("-----BEGIN PUBLIC KEY-----", "");
        text = text.Replace("-----END PUBLIC KEY-----", "");
        text = text.Replace("\r", "");
        text = text.Replace("\n", "");
        // Decode from Base64
        var privateKeyBytes = Convert.FromBase64String(text);
        // Now import
        int bytesRead;
        rsa.ImportSubjectPublicKeyInfo(privateKeyBytes, out bytesRead);
    }

    public string Encrypt(String s)
    {
        byte[] dataToEncrypt = Encoding.UTF8.GetBytes(s);
        var encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(encryptedData);
    }

    public string Decrypt(string s)
    {
        byte[] dataToDecrypt = Convert.FromBase64String(s);
        var decryptedData = rsa.Decrypt(dataToDecrypt, RSAEncryptionPadding.Pkcs1);
        return Encoding.UTF8.GetString(decryptedData);
    }
}
