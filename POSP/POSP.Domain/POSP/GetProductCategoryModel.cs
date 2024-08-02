namespace POSP.Domain.POSP
{
    public class GetProductCategoryModel
    {
        public string InsuranceTypeId { get; set; }
        public string InsuranceName { get; set; }
        public string InsuranceType { get; set; }
        public string ImageURL { get; set; }
        public bool IsActive { get; set; }
        public bool IsEnable { get; set; }
        public string ProductCategoryId { get; set; }
    }
}
