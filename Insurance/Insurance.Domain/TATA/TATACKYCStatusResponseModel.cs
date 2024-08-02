using Insurance.Domain.Quote;

namespace Insurance.Domain.TATA
{
    public class TATACKYCStatusResponseModel
    {
        public string RequestBody { get; set; }
        public string ResponseBody { get; set; }
        public TATAVerifyCKYCResponseDto TATACKYCResponse { get; set; }
        public SaveCKYCResponse SaveCKYCResponse { get; set; }
    }
}
