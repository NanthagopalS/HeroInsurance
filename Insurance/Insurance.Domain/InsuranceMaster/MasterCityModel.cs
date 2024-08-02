namespace Insurance.Domain.InsuranceMaster;

public class MasterCityModel
{
    public IEnumerable<CityList> MasterData { get; set; }
    public DefaultCity DefaultCity { get; set; }
}
public class CityList
{
    public string Name { get; set; }
    public string Value { get; set; }
}
public class DefaultCity
{
    public string DefaultCityName { get; set; }
    public string DefaultCityValue { get; set; }
}
