using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Chola;

public class CholaBreakInResponseModel
{
    public string Status { get; set; }
    public string ReferenceNumber { get; set; }
    public string ChassisNumber { get; set; }
    public string EnigineNumber { get; set; }
    public string CustomerName { get; set; }
    public string RegistrationNumber { get; set; }
    public DateTime LastUpdatedDate { get; set; }
    public string AgencyId { get; set; }
    public string IntermediaryCode { get; set; }
    public string IntermediaryName { get; set; }
    public string status_flag { get; set; }
    public string dtdamount { get; set; }
    public string flag { get; set; }
    public string gvw { get; set; }
    public string vehile_make { get; set; }
    public string vehile_model { get; set; }
    public string varient { get; set; }
    public string subclass { get; set; }
    public string model_code { get; set; }
    public string RejectReason { get; set; }
    public string Remarks { get; set; }
}
