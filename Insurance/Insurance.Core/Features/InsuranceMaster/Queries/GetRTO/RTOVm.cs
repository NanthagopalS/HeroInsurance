namespace Insurance.Core.Features.InsuranceMaster;

public class StateCityRTOYearVm
{
    public IEnumerable<YearVm> YearVms { get; set; }
    public IEnumerable<StateVm> StateVms { get; set; }
}

public class StateVm
{
    /// <summary>
    /// StateId
    /// </summary>
    public string StateId { get; set; }

    /// <summary>
    /// StateName
    /// </summary>
    public string StateName { get; set; }

    /// <summary>
    /// Collection of City
    /// </summary>
    public IEnumerable<CityVm> CityVms { get; set; }
}

public class CityVm
{
    /// <summary>
    /// CityId
    /// </summary>
    public string CityId { get; set; }

    /// <summary>
    /// CityName
    /// </summary>
    public string CityName { get; set; }

    /// <summary>
    /// Collection of RTO
    /// </summary>
    public IEnumerable<RTOVm> RTOVms { get; set; }
}

public class RTOVm
{
    /// <summary>
    /// RTOID
    /// </summary>
    public string RTOId { get; set; }

    /// <summary>
    /// RTOCOde
    /// </summary>
    public string RTOCode { get; set; }
}

public class YearVm
{
    /// <summary>
    /// YearId
    /// </summary>
    public string YearId { get; set; }

    /// <summary>
    /// Year
    /// </summary>
    public string Year { get; set; }
}