namespace Insurance.Domain.Bajaj;
public class BajajBreakinStatusCheckResponseModel
{
    public string errorMsg { get; set; }
    public string errorCode { get; set; }
    public Pinlist[] pinList { get; set; }
}

public class Pinlist
{
    public string stringval1 { get; set; }
    public string stringval2 { get; set; }
    public string stringval3 { get; set; }
    public string stringval4 { get; set; }
    public string stringval5 { get; set; }
    public string stringval6 { get; set; }
    public string stringval7 { get; set; }
    public string stringval8 { get; set; }
    public string stringval9 { get; set; }
    public string stringval10 { get; set; }
}
