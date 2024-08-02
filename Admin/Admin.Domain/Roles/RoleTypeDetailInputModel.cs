namespace Admin.Domain.Roles
{
    public class RoleTypeDetailInputModel
    {
        public string? RoleName { get; set; }
        public string? RoleTypeId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? PageIndex { get; set; }
        public string? IsActive { get; set; }
    }
}
