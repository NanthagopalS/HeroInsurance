namespace Identity.Domain.User
{
    public class AdminUpdateUserResponseModel
    {
        public string UserId { get; set; }
        public bool IsPasswordUpdated { get; set; }
        public string Message { get; set; }
    }
}
