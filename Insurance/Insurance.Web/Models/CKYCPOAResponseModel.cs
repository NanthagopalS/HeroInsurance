namespace Insurance.Web.Models
{
    public class CKYCPOAResponseModel
    {
        public string statusCode { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
        public class Data
        {
            public string insurerId { get; set; }
            public string leadID { get; set; }
            public string transactionId { get; set; }
            public string kycId { get; set; }
            public string ckycNumber { get; set; }
            public string ckycStatus { get; set; }
            public string message { get; set; }
            public string insurerName { get; set; }
            public string name { get; set; }
            public string dob { get; set; }
            public string gender { get; set; }
            public string address { get; set; }
        }

    }
}
