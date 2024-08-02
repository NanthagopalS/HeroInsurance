namespace Insurance.Domain.UnitedIndia
{
    public record UnitedIndiaCkycFetchModel
    {

        public string status { get; set; }
        public string oem_unique_identifier { get; set; }
        public string ckyc_no { get; set; }
        public string customer_name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string pincode { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public string email { get; set; }
        public string mobile_no { get; set; }
        public string pan { get; set; }
        public string verified_date_time { get; set; }
        public string url { get; set; }
        public string kyc_verification_status { get; set; }
        public string status_code { get; set; }
        public string status_message { get; set; }
        public string errormessage { get; set; }
        public string errorcode { get; set; }

    }
}
