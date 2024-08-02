namespace Insurance.Domain.HDFC;

public class HDFCSubmitPOSPCertificateResponse
{
    public string status { get; set; }
    public string statusDesc { get; set; }
    public string correlationID { get; set; }
    public string HDFCPOSPID { get; set; }
    public RespPOSP Resp_POSP { get; set; }
}
public class RespPOSP
{
    public string POSP_ID { get; set; }
}
