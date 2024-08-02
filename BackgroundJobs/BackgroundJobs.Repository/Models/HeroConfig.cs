namespace BackgroundJobs.Models;

public class HeroConfig
{
    public string BaseURL { get; set; }
    public string GoDigitBreakInURL { get; set; }
    public string ICICIBreakInURL { get; set; }
    public string PGPaymentURL { get; set; }
    public string BajajURL { get; set; }
    public string BajajTPURL { get; set; }
    public string ICICIURL { get; set; }
    public string GoDigitCKYCPaymentStatusURL { get; set; }
    public string ICICICKYCPaymentStatusURL { get; set; }
    public string CronExpForGoDigitBreakInStatus { get; set; }
    public string CronExpForGoDigitPaymentStatus { get; set; }
    public string CronExpForICICIBreakInStatus { get; set; }
    public string CronExpForICICIPaymentStatus { get; set; }
    public string BajajBreakInURL { get; set; }
    public string CronExpForBajajBreakInStatus { get; set; }
    public string CronExpForHDFCCKYCPOAStatus { get; set; }
    public string HdfcCkycPoaStatusURL { get; set; }
    public string HDFCInsurerId { get; set; }
    public string ICICIIMBrokerURL { get; set; }
    public string CronExpForICICICreateIMBroker { get; set; }
    public string HdfcCreatePOSPURL { get; set; }
    public string CronExpForHDFCCreatePOSP { get; set; }
    public string CronExpForCholaCKYCStatus { get; set; }
    public string CronExpForCholaBreakInStatus { get; set; }
    public string CholaInsurerId { get; set; }
    public string CholaCkycStatusURL { get; set; }
    public string CholaBreakinStatusURL { get; set; }
    public string CronExpForCholaPaymentStatus { get; set; }
    public string CholaPaymentStatusURL { get; set; }
    public string IFFCOBreakInURL { get; set; }
    public string CronExpForIFFCOBreakinStatus { get; set; }
    public string TATABreakInURL { get; set; }
    public string TATAVerifyPaymentURL { get; set; }
    public string CronExpForTATABreakinStatus { get; set; }
    public string CronExpForTATAPaymentStatus { get; set; }
    public string TATAInsurerId { get; set;  }
    public string CronExpForQuoteTrasactionUpdate { get; set; }
}
