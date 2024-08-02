namespace Identity.Domain.User
{
    public class AdminUserResponseModel
    {
        public string UserId { get; set; }
        public string EmailId { get; set; }
        public bool IsUserExists { get; set; }
        public string SuperRoleId { get; set; }
        public string SuperRoleName { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string AuthToken { get; set; }
        public string Message { get; set; }
        public bool CredentialsReseted { get; set; }

    }

    public class AdminUserValidationModel
    {
        public string UserId { get; set; }
        public string EmailId { get; set; }
        public bool IsUserExists { get; set; }
        public string SuperRoleId { get; set; }
        public string SuperRoleName { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string AuthToken { get; set; }
        public string Password { get; set; }
        public string Message { get; set; }
        public bool CredentialsReseted { get; set; } = false;
    }
}
