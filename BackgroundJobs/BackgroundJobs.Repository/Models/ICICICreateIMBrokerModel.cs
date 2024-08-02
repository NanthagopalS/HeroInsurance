using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundJobs.Repository.Models;

public class ICICICreateIMBrokerModel
{
    public string PAN { get; set; }
    public string AadhaarNumber { get; set; }
    public string UserName { get; set; }
    public string MobileNo { get; set; }
    public string Gender { get; set; }
    public string EmailId { get; set; }
    public string DateofBirth { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string StateName { get; set; }
    public string CityName { get; set; }
    public string Pincode { get; set; }
    public string Country { get; set; }
    public string LastName { get; set; }
    public string FatherHusbandName { get; set; }
    public string CorrelationId { get; set; }
    public string POSPId { get; set; }
    public string UserId { get; set; }
    public string RoleName { get; set; }
    public string IMID { get; set; }
}