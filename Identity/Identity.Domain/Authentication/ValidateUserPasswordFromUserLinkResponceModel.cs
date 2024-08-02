namespace Identity.Domain.Authentication
{
    public class ValidateUserPasswordFromUserLinkResponceModel
    {
       public bool IsValid { get; set; }
       public string UserId { get; set; }
       public string Message { get; set; }
    }
}
