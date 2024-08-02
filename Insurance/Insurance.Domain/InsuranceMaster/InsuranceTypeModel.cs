namespace Insurance.Domain.InsuranceMaster;
public class InsuranceTypeModel
{
    public string InsuranceTypeId { get; set; }
    public string InsuranceName { get; set; }
    public string InsuranceType { get; set; }
    public bool IsOfferApplicable { get; set; }
    public string OfferContent { get; set; }
}
