using Org.BouncyCastle.Ocsp;
using System;

namespace Identity.Domain.Authentication;

public class POSPDetailsModel
{
    public string UserName { get; set; }
    public string UserId { get; set; }
    public string POSPId { get; set; }
    public string MobileNo { get; set; }
    public string DateofBirth { get; set; }
    public string EmailId { get; set; }
    public string AadhaarNumber { get; set; }
    public string PAN { get; set; }
    public string CityName { get; set; }
    public string RoleName { get; set; }
}
