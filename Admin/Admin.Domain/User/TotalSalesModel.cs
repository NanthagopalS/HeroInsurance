namespace Admin.Domain.User
{
    public record TotalSalesResponseModel
    {
        public string ICId { get; set; }
        public string ICName { get; set; }
        public string ICLogoImageId { get; set; }
        public string ICLogoImageString { get; set; }
        public string ProductTypeId { get; set; }
        public string ProductTypeName { get; set; }
        public string TotalAmount { get; set; }
        public string LastSalesDate { get; set; }
    }
}
