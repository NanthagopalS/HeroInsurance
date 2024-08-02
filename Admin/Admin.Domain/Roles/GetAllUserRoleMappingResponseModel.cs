namespace Admin.Domain.Roles
{
    public class GetAllUserRoleMappingResponseModel
    {
        public IEnumerable<UserRoleMappingDataModel>? UserRoleMappingDataModel { get; set; }
        public IEnumerable<UserRoleMappingPaginationModel>? UserRoleMappingPaginationModel { get; set; }

    }
    public class UserRoleMappingDataModel
    {
        public string? UserRoleMappingId { get; set; }
        public string? UserId { get; set; }
        public string? EmpId { get; set; }
        public string? EmployeeName { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public string? RoleTypeId { get; set; }
        public string? RoleTypeName { get; set; }
        public string? RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool StatusId { get; set; }
        public string? StatusName { get; set; }
    }
    public class UserRoleMappingPaginationModel
    {
        public int? CurrentPageIndex { get; set; }
        public int? PreviousPageIndex { get; set; }
        public int? NextPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
        public int? TotalRecord { get; set; }
    }
}
