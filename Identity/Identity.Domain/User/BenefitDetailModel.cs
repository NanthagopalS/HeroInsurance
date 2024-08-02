namespace Identity.Domain.User
{
    public class BenefitDetailModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// BenefitsTitle
        /// </summary>
        public string BenefitsTitle { get; set; }
        /// <summary>
        /// BenefitsDescription
        /// </summary>
        public string BenefitsDescription { get; set; }
        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// CreatedBy
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// UpdatedBy
        /// </summary>
        public string UpdatedBy { get; set; }
        /// <summary>
        /// UpdatedOn
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }
    public class BenefitsDetailCreateResponseModel
    {
        public string Id { get; set; }
        public bool IsExists { get; set; }
    }
}
