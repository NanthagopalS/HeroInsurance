namespace Identity.Domain.User
{
    public class UpdateUserPasswordFromUserLinkModel
    {
        public string UserId { get; set; }
        public string NewPassWord { get; set; }
        public string ConfirmPassWord { get; set; }
        public string Guid { get; set; }
    }
}
