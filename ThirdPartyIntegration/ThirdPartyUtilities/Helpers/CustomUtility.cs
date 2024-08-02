using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using ThirdPartyUtilities.Abstraction;

namespace ThirdPartyUtilities.Helpers;

public class CustomUtility : ICustomUtility
{
    private readonly ILogger<CustomUtility> _logger;
    public CustomUtility(ILogger<CustomUtility> logger) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public DateTime ConvertToDateTime(string dateString)
    {
        string[] formats = {"dd-MM-yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss","dd/MM/yyyy","yyyy-MM-dd", "yyyy/MM/dd", "MM/dd/yyyy", "dd/MM/yyyy", "yyyyMMdd", "dd-MM-yyyy", "dd/MMM/yyyy",
        "MMM/dd/yyyy", "yyyy-MMM-dd", "MMM-dd-yyyy", "dd-MMM-yyyy", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss",
        "dd/MM/yyyy HH:mm:ss", "MMM dd, yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fffZ","MM/dd/yyyy HH:mm:ss","d/M/yyyy","dd/M/yyyy","d/MM/yyyy" };

        DateTime result;
        if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out result))
        {
            return result;
        }
        else
        {
            return DateTime.UtcNow;
        }
    }
    public string ConvertToDateTimeReturnFalse(string dateString)
    {
        string[] formats = {"MM/dd/yyyy hh:mm:ss t","M/dd/yyyy hh:mm:ss t","MM/dd/yyyy hh:mm:ss tt","M/dd/yyyy hh:mm:ss tt","dd-MM-yyyy HH:mm:ss", "dd/MM/yyyy HH:mm:ss","dd/MM/yyyy","yyyy-MM-dd", "yyyy/MM/dd", "MM/dd/yyyy", "dd/MM/yyyy", "yyyyMMdd", "dd-MM-yyyy", "dd/MMM/yyyy",
        "MMM/dd/yyyy", "yyyy-MMM-dd", "MMM-dd-yyyy", "dd-MMM-yyyy", "yyyy/MM/dd HH:mm:ss", "yyyy-MM-dd HH:mm:ss",
        "dd/MM/yyyy HH:mm:ss", "MMM dd, yyyy HH:mm:ss", "dd-MMM-yyyy HH:mm:ss", "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fffZ","MM/dd/yyyy HH:mm:ss","d/M/yyyy","dd/M/yyyy","d/MM/yyyy" };

        DateTime result;
        if (DateTime.TryParseExact(dateString, formats, CultureInfo.InvariantCulture,
            DateTimeStyles.None, out result))
        {
            _logger.LogInformation("ConvertToDateTimeReturnFalse:COnvertion successfull" + result.ToString("yyyy-MM-dd HH:mm:ss"));
            return result.ToString("yyyy-MM-dd HH:mm:ss");
        }
        else
        {
            _logger.LogError("ConvertToDateTimeReturnFalse:Failed to convert DATETIME" + dateString);
            return null;
        }
    }
    public string Base64Encode(string plainText)
    {
        byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }

    public string Base64Decode(string encodedText)
    {
        byte[] encodedBytes = Convert.FromBase64String(encodedText);
        return Encoding.UTF8.GetString(encodedBytes);
    }
    public string Base64DecodeForRequest(string encrytedData)
    {
        if (string.IsNullOrEmpty(encrytedData))
        {
            return null;
        }
        // decode the encoded data from Base64
        string decodedData = Base64Decode(encrytedData);

        //remove salt from both side
        string dataWithoutSalt = decodedData.Substring(4, decodedData.Length - 8);

        //decode the data without salt
        string decodeWithoutSalt = Base64Decode(dataWithoutSalt);

        return decodeWithoutSalt;

    }
}

