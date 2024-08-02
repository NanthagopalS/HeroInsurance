namespace Insurance.Core.Features.InsuranceMaster;
public class ModelVm
{
    /// <summary>
    /// ModelId
    /// </summary>
    public string ModelId { get; set; }

    /// <summary>
    /// Model Name
    /// </summary>
    public string ModelName { get; set; }

    /// <summary>
    /// IsPopular Model
    /// </summary>
    public bool IsPopularModel { get; set; }
}

public class FuelVm
{
    /// <summary>
    /// FuelId
    /// </summary>
    public string FuelId { get; set; }

    /// <summary>
    /// FuelName
    /// </summary>
    public string FuelName { get; set; }
}

public class MakeModelVm
{
    /// <summary>
    /// BrandId
    /// </summary>
    public string MakeId { get; set; }

    /// <summary>
    /// BrandName
    /// </summary>
    public string MakeName { get; set; }

    /// <summary>
    /// Image of Brand
    /// </summary>
    public string ImageURL { get; set; }

    /// <summary>
    /// Collection of Models
    /// </summary>
    public IEnumerable<ModelVm> ModelVms { get; set; }
}
