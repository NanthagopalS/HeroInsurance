namespace Admin.Domain.Roles
{
    public record RoleDetailInsertResponceModel
    {
        public bool Status { get; set; } = false;
        public string Message { get; set; }
    }
}
