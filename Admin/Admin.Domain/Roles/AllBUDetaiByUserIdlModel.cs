namespace Admin.Domain.Roles
{
    public record AllBUDetaiByUserIdModel
    {
        public string BUId { get; set; }
        public string BUName { get; set; }
        public string HierarchyLevelId { get; set; }
        public string HierarchyLevelName { get; set; }
        public string RoleTypeName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedOn { get; set; }
        public bool IsSales { get; set; }
    }
}
