namespace Admin.Domain.User
{
    public class ResetUserDetailByIdModel
    {
        public IEnumerable<ResetUserDetailsForPanVerification> userDetail { get; set; }
        public IEnumerable<DocumentDetail> documentDetail { get; set; }
    }

    public record ResetUserDetailsForPanVerification
    {
        public string UserId { get; set; }
        public string UserName { get; set; }    
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string PAN { get; set; }    
        public bool ResetSuccessfull { get; set; } = false;
    }

    public record DocumentDetail
    {
        public string DocumentId { get; set; }
        public string DocumentType { get; set; }
    }
}
