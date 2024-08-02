namespace Insurance.Domain.Reliance;

public class RelianceStateCityMasterModel
{
    public string StateId { get; set; }
    public string CityId { get; set; }
    public string DistrictId { get; set; }
}

public class ReliancePreviousInsurer
{
    public string InsurerName { get; set; }
    public string InsurerId  { get; set; }
}

