namespace Identity.Domain.User
{
    public record UpdateUserPasswordFromUserLinkResponceModel
    {
        public bool IsPasswordUpdated { get; set; }
        public string Message { get; set; }
        public bool IsValid { get; set; }
    }
}
