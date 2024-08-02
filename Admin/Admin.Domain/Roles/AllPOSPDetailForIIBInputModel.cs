namespace Admin.Domain.Roles
{
    public class AllPOSPDetailForIIBInputModel
    {
        public string? Searchtext { get; set; }
        public string? CreatedBy { get; set; }
        public string? StatusType { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
    }
}
