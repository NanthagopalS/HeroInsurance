namespace Insurance.Domain.CommercialMaster
{
    public class CommercialCategory
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string IsOtherDetails { get; set; }
		public IEnumerable<CommercialSubCategory> SubCategory { get; set; }

	}

	public class CommercialSubCategory
    {
        public string SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }
        public string MasterCategoryId { get; set; }

	}

    public class CommercialVehicleCategory
    {
        public IEnumerable<CommercialCategory> Category { get; set; }
    }
}
