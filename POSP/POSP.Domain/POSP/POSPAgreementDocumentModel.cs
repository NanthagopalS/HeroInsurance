namespace POSP.Domain.POSP
{
    public class POSPAgreementDocumentModel
    {
        public string UserId { get; set; }
        public string DateofBirth { get; set; }
        public string AadhaarNumber { get; set; }
        public string PAN { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string POSP_Name { get; set; }
        public string SignatureDocumentId { get; set; }
        public string AgreementId { get; set; }
        public string SignatureImage { get; set; }
        public string ProcessType { get; set; }
        public string PreSignedAgreementId { get; set; }
        public string Gender { get; set; }
        public string FatherName { get; set; }
        public string POSPId { get; set; }
        public string PinCode { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string StampNumber { get; set; }
        public int IsRevoked { get; set; }
    }
}
