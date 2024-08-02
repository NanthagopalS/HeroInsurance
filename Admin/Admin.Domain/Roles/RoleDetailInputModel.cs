namespace Admin.Domain.Roles
{
    public class RoleDetailInputModel
    {
        public string? RoleName { get; set; }
        public string? RoleTypeId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? CurrentPageIndex { get; set; }
        public bool? IsActive { get; set; }
        public int? CurrentPageSize { get; set; }
    }
}
