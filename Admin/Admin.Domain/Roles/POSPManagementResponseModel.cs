namespace Admin.Domain.Roles
{
    public class POSPManagementResponseModel
    {
        public IEnumerable<POSPManagementDataModel>? POSPManagementDataModel { get; set; }
        public IEnumerable<POSPManagementPaginationModel>? POSPManagementPaginationModel { get; set; }
    }

    public class POSPManagementDataModel
    {
        public string UserId { get; set; }
        public string? POSPId { get; set; }
        public string? POSPName { get; set; }
        public string? MobileNumber { get; set; }
        public string? EmailId { get; set; }
        public string? CreatedBy { get; set; }
        public string? StageValue { get; set; }
        public string? StatusValue { get; set; }
        public string? RelationManager { get; set; }
        public string? TaggedPolicy { get; set; }
        public bool IsActive { get; set; }
    }
    public class POSPManagementPaginationModel
    {
        public int CurrentPageIndex { get; set; }
        public int PreviousPageIndex { get; set; }
        public int NextPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public int TotalRecord { get; set; }
    }
}

