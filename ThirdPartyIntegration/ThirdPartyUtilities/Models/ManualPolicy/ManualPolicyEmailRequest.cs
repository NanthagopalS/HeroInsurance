namespace ThirdPartyUtilities.Models.ManualPolicy
{
    public class ManualPolicyEmailRequest
    {
        public int policyuploadedsuccessful { get; set; }
        public int policyfailed { get; set; }
        public int totalpolicy { get; set; }
        public string EmailId { get; set; }
        public string UserName { get; set; }
        public string dateAndTime { get; set; }

    }
}
