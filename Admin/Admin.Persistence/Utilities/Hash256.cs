using Org.BouncyCastle.Security;
using System.Security.Cryptography;

namespace Admin.Persistence.Utilities;
/// <summary>
/// Hash256
/// </summary>
public static class Hash256
{
    private const int HashByteSize = 64; // to match the size of the PBKDF2-HMAC-SHA-1 hash 
    private const int Pbkdf2Iterations = 1000;
    private const int SaltIndex = 0;
    private const int Pbkdf2Index = 1;

    /// <summary>
    /// Hash256 Password
    /// </summary>
    public static string Hash256Password(string password)
    {
        //var cryptoProvider = new RSACryptoServiceProvider();
        byte[] salt = FunGetsalt();
        //cryptoProvider.GetBytes(salt);

        var hash = GetPbkdf2SH256Bytes(password, salt, Pbkdf2Iterations, HashByteSize);
        return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash);
    }

    private static byte[] GetPbkdf2SH256Bytes(string password, byte[] salt, int iterations, int outputBytes)
    {
        var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
        //pbkdf2.IterationCount = iterations;
        return pbkdf2.GetBytes(outputBytes);
    }

    private static byte[] FunGetsalt()
    {
        SecureRandom sr = SecureRandom.GetInstance("SHA256PRNG");
        byte[] salt = new byte[16];
        sr.NextBytes(salt);
        return salt;
    }

    /// <summary>
    /// Hash256 Validate Password
    /// </summary>
    public static bool ValidateSH256Password(string password, string correctHash)
    {
        char[] delimiter = { ':' };
        var split = correctHash.Split(delimiter);
        var iterations = Pbkdf2Iterations;
        var salt = Convert.FromBase64String(split[SaltIndex]);
        var hash = Convert.FromBase64String(split[Pbkdf2Index]);
        var testHash = GetPbkdf2SH256Bytes(password, salt, iterations, hash.Length);
        return SlowEquals(hash, testHash);
    }

    private static bool SlowEquals(byte[] a, byte[] b)
    {
        var diff = (uint)a.Length ^ (uint)b.Length;
        for (int i = 0; i < a.Length && i < b.Length; i++)
        {
            diff |= (uint)(a[i] ^ b[i]);
        }
        return diff == 0;
    }
}