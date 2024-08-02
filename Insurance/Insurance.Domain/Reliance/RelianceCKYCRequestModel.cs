using Newtonsoft.Json;

namespace Insurance.Domain.Reliance;

public class RelianceCKYCRequestModel
{
    public string PAN { get; set; }
    public string DOB { get; set; }
    public string CKYC { get; set; }
    public string MOBILE { get; set; }
    public string PINCODE { get; set; }
    public string BIRTHYEAR { get; set; }
    public string ReturnURL { get; set; }
    public string UNIQUEID { get; set; }
    public string CIN { get; set; }
    public string VOTERID { get; set; }

    [JsonProperty("DL_No ")]
    public string DL_No { get; set; }
    public string PASSPORT { get; set; }
    public string AADHAAR_NO { get; set; }
    public string FULLNAME { get; set; }
    public string GENDER { get; set; }
}