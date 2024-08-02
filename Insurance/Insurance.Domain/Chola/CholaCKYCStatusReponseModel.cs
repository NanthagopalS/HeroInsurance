using Insurance.Domain.InsuranceMaster;

namespace Insurance.Domain.Chola
{
    public class CholaCKYCStatusReponseModel
    {
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public CholaCKYCResponseModel CholaCKYCResponse { get; set; }
        public CreateLeadModel CreateLeadModel { get; set; }
    }
}
