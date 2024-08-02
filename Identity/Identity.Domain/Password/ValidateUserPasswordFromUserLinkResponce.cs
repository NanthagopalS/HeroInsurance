namespace Identity.Domain.Password
{
    public record ValidateUserPasswordFromUserLinkResponce
    {
        public bool IsValid { get; set; }
        public string UserId { get; set; }
        public string GuId { get; set; }
    }
}
