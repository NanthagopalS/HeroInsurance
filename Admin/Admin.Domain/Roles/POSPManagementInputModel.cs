namespace Admin.Domain.Roles
{
    public class POSPManagementInputModel
    {
        public string? SearchText { get; set; }
        public int? POSPStatus { get; set; }
        public string? StageId { get; set; }
        public string? RelationManagerId { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public string? CreatedBy { get; set; }
    }
}
