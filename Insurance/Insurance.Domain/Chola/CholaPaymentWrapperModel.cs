namespace Insurance.Domain.Chola;

public class CholaPaymentWrapperModel
{
    public string RequestId { get; set; }
    public string RequestDateTime { get; set; }
    public Data Data { get; set; }
}

public class Data
{
    public string policy_id { get; set; }
    public string tp_policy_no { get; set; }
    public string policy_no { get; set; }
    public string remarks { get; set; }
    public string DocumentId { get; set; }
    public string DocumentBase64 { get; set; }
    public byte[] PdfBase64 { get; set; }
    public string PolicyURL { get; set; }
}

public class PaymentRequestModel
{
    public string user_code { get; set; }
    public string payment_id { get; set; }
    public string total_amount { get; set; }
    public string payment_mode { get; set; }
    public string billdesk_txn_date { get; set; }
    public string billdesk_txn_amount { get; set; }
    public string billdesk_txn_ref_no { get; set; }
}





