namespace ThirdPartyUtilities.Abstraction
{
    public interface ICustomUtility
    {
        DateTime ConvertToDateTime(string dateString);
        string Base64Encode(string plainText);
        string Base64Decode(string encodedText);
        string Base64DecodeForRequest(string encryptedString);
        string ConvertToDateTimeReturnFalse(string dateString);
    }
}
