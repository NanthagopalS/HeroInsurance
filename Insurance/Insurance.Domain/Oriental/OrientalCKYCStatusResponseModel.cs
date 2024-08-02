using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;

namespace Insurance.Domain.Oriental;

public class OrientalCKYCStatusResponseModel
{
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public OrientalCKYCFetchResponseModel OrientalCKYCFetchResponseModel { get; set; }
    public CreateLeadModel CreateLeadModel { get; set; }
    public SaveCKYCResponse SaveCKYCResponse { get; set; }
}