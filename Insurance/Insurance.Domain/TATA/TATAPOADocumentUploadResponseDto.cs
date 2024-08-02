namespace Insurance.Domain.TATA
{

    public class TATAPOADocumentUploadResponseDto
    {
        public int status { get; set; }
        public string message_txt { get; set; }
        public DocumentUpload_Data data { get; set; }
    }

    public class DocumentUpload_Data
    {
        public string doc_type { get; set; }
        public string image_quality { get; set; }
        public string req_id { get; set; }
        public bool success { get; set; }
        public string error_message { get; set; }
        public Verify_Data verify_data { get; set; }
        public DocumentUpload_Result result { get; set; }
        public bool verified { get; set; }
        public string id_num { get; set; }
        public string id_type { get; set; }
        public string customer_type { get; set; }
        public DateTime verified_at { get; set; }
        public string proposal_id { get; set; }
    }

    public class Verify_Data
    {
        public bool status { get; set; }
        public Body body { get; set; }
        public int code { get; set; }
        public bool invalid { get; set; }
    }

    public class Body
    {
        public int code { get; set; }
        public string status { get; set; }
        public Response response { get; set; }
    }

    public class Response
    {
        public string state { get; set; }
        public string ageBand { get; set; }
        public string mobileNumber { get; set; }
        public string gender { get; set; }
        public string aadhaarNumber { get; set; }
        public bool valid { get; set; }
        public bool status { get; set; }
        public string message { get; set; }
    }

    public class DocumentUpload_Result
    {
        public DocumentUpload_P_Address p_address { get; set; }
        public DocumentUpload_C_Address c_address { get; set; }
        public string registered_name { get; set; }
        public string gender { get; set; }
        public string age { get; set; }
    }

    public class DocumentUpload_P_Address
    {
        public string line1 { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
    }

    public class DocumentUpload_C_Address
    {
        public string line1 { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
    }


}