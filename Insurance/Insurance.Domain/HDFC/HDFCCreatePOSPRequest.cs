namespace Insurance.Domain.HDFC;

public class HDFCCreatePOSPRequest
{
    public string TransactionID { get; set; }
    public ReqPOSP Req_POSP { get; set; }
}

public class ReqPOSP
{
    public string NUM_MOBILE_NO { get; set; }
    public string ADHAAR_CARD { get; set; }
    public string EMAILID { get; set; }
    public string NAME { get; set; }
    public string PAN_CARD { get; set; }
    public string STATE { get; set; }
    public string UNIQUE_CODE { get; set; }
    public string START_DT { get; set; }
    public string END_DT { get; set; }
    public string REGISTRATION_NO { get; set; }
}

