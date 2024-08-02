namespace Insurance.Domain.ICICI;

public class ICICIPaymentRequestDto
{
    public string TransactionId { get; set; }
    public string Amount { get; set; }
    public string ApplicationId { get; set; }
    public string ReturnUrl { get; set; }
    public string AdditionalInfo1 { get; set; } // proposal number
    public string AdditionalInfo2 { get; set; } // customer Id
    public string AdditionalInfo3 { get; set; } // vehicle number
    public string AdditionalInfo4 { get; set; } // mobile number
}
