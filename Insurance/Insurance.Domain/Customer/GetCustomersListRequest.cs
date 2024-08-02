namespace Insurance.Domain.Customer
{
    public class GetCustomersListRequest
    {
        public string CustomerType { get; set; }
        public string SearchText { get; set; }
        public string PolicyType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int CurrentPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public string PolicyStatus { get; set; }
        public string PolicyNature { get; set; }
    }
}
