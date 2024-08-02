namespace Insurance.Domain;
public class InsurerModel
{
    public string InsurerId { get; set; }
    public string InsurerName { get; set; }
    public string Logo { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsCommercialActive { get; set; }
}
