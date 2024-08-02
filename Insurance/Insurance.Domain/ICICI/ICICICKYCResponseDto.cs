namespace Insurance.Domain.ICICI;


public class ICICICKYCResponseDto
{
    public KYCDetails kyc_details { get; set; }
    public string message { get; set; }
    public bool status { get; set; }
    public string statusMessage { get; set; }
    public string correlationId { get; set; }
}

public class KYCDetails
{
    public string il_kyc_ref_no { get; set; }
    public string certificate_type { get; set; }
    public string certificate_number { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string full_name { get; set; }
    public string gender { get; set; }
    public string dob { get; set; }
    public string mobile_number { get; set; }
    public string email { get; set; }
    public string ckyc_number { get; set; }
    public ICICIPermanentAddress permanent_address { get; set; }
    public ICICIAlternateAddress alternate_address { get; set; }
    public string customer_type { get; set; }
}

public class ICICIPermanentAddress
{
    public string address_line_1 { get; set; }
    public string address_line_2 { get; set; }
    public string address_line_3 { get; set; }
    public string pin_code { get; set; }
    public string city { get; set; }
    public string district { get; set; }
    public string state { get; set; }
}

public class ICICIAlternateAddress
{
    public string address_line_1 { get; set; }
    public string address_line_2 { get; set; }
    public string pin_code { get; set; }
    public string city { get; set; }
    public string district { get; set; }
    public string state { get; set; }
}

