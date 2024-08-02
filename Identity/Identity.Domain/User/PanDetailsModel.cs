namespace Identity.Domain.User
{
    public class PanDetailsModel
    {
        public string PanVerificationId { get; set; }  
        public string Name { get; set; }
        public string DOB { get; set; }
        public string PanNumber { get; set; }
        public string EmailId { get; set; }
        public string FatherName { get; set; }

        public bool IsUserExists { get; set; }
        public string InvalidPanMsg { get; set; }

    }
}
