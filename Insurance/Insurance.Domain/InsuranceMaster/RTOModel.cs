namespace Insurance.Domain;
public class RTOCityModel
{
    public IEnumerable<YearModel> YearList { get; set; }
    public IEnumerable<StateModel> StateList { get; set; }
    public IEnumerable<CityModel> CityList { get; set; }
    public IEnumerable<RTOModel> RTOList { get; set; }

}

public class YearModel
{
    public string YearId { get; set; }
    public string Year { get; set; }
}
public class StateModel
{
    public string StateId { get; set; }
    public string StateName { get; set; }
}

public class CityModel
{
    public string CityId { get; set; }
    public string CityName { get; set; }
    public string StateId { get; set; }
}


public class RTOModel
{
    public string RTOId { get; set; }
    public string RTOCode { get; set; }
    public string CityId { get; set; }
}

public class LeadModel
{
    public string LeadId { get; set; }
}
