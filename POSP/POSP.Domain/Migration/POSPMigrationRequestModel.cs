namespace POSP.Domain.Migration
{
    public record POSPMigrationRequestModel
    {
        public int Index { get; set; }
        public string posp_code { get; set; }
        public string name { get; set; }
        public string alias_name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string pan_number { get; set; }
        public string dob { get; set; }
        public string father_name { get; set; }
        public string gender { get; set; }
        public string adhar_no { get; set; }
        public string alternate_mobile { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string pincode { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string product_type { get; set; }
        public string noc_available { get; set; }
        public string bank_name { get; set; }
        public string ifsc_code { get; set; }
        public string account_holder_name { get; set; }
        public string account_number { get; set; }
        public string educational_qualification { get; set; }
        public string select_average_premium { get; set; }
        public string POSP { get; set; }
        public string BU { get; set; }
        public string created_date { get; set; }
        public string created_by { get; set; }
        public string sourced_by { get; set; }
        public string serviced_by { get; set; }
        public string posp_source { get; set; }
        public string general_training_start { get; set; }
        public string general_training_end { get; set; }
        public string life_insurance_training_start { get; set; }
        public string life_insurance_training_end { get; set; }
        public string exam_status { get; set; }
        public string exam_start { get; set; }
        public string exam_end { get; set; }
        public string IIBUploadStatus { get; set; }
        public string iib_upload_date { get; set; }
        public string AgreementStatus { get; set; }
    }
}
