namespace ThirdPartyUtilities.Models.Signzy;
public class BankVerificationRequestModel
{
    public string task { get; set; }
    public BankEssentials essentials { get; set; }
}

public class BankEssentials
{
    public string beneficiaryMobile { get; set; }
    public string beneficiaryAccount { get; set; }
    public string beneficiaryName { get; set; }
    public string beneficiaryIFSC { get; set; }
    public string nameFuzzy { get; set; }
}

public class AuditTrail
{
    public string nature { get; set; }
    public string value { get; set; }
    public DateTime timestamp { get; set; }
}

public class BankTransfer
{
    public string response { get; set; }
    public string bankRRN { get; set; }
    public string beneName { get; set; }
    public string beneMMID { get; set; }
    public string beneMobile { get; set; }
    public string beneIFSC { get; set; }
}


public class BankResult
{
    public string active { get; set; }
    public string reason { get; set; }
    public string nameMatch { get; set; }
    public string mobileMatch { get; set; }
    public string signzyReferenceId { get; set; }
    public AuditTrail auditTrail { get; set; }
    public decimal nameMatchScore { get; set; }
    public BankTransfer bankTransfer { get; set; }
}

public class BankVerificationResponse
{
    public string task { get; set; }
    public BankEssentials essentials { get; set; }
    public string id { get; set; }
    public string patronId { get; set; }
    public BankResult result { get; set; }
}