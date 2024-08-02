namespace Insurance.Domain.ICICI;

public class ICICIPaymentEnquiryResponse
{
    public int Status { get; set; }
    public string AuthCode { get; set; }
    public string PGtransactionId { get; set; }
    public string GatewayName { get; set; }
    public int GatewayId { get; set; }
    public string MerchantId { get; set; }
    public float Amount { get; set; }
}
