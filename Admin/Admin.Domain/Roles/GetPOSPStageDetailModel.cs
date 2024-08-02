namespace Admin.Domain.Roles
{
    public class GetPOSPStageDetailModel
    {
        public string StageId { get; set; }
        public string StageName { get; set; }
        public string PriorityIndex { get; set; }
        public bool IsActive { get; set; }
        public int GroupNumber { get; set; }
        public int VisibleForFilters { get; set; }
    }
}
