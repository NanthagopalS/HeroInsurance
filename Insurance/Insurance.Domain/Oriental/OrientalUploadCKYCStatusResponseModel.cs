using Insurance.Domain.Quote;

namespace Insurance.Domain.Oriental;

public class OrientalUploadCKYCStatusResponseModel
{
    public string RequestBody { get; set; }
    public string ResponseBody { get; set; }
    public SaveCKYCResponse saveCKYCResponse { get; set; }
}
