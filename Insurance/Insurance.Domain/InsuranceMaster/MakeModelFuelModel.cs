namespace Insurance.Domain;

public class MakeModel
{
    public string MakeId { get; set; }
    public string MakeName { get; set; }
    public string ImageURL { get; set; }
}

public class Model
{
    public string ModelId { get; set; }
    public string ModelName { get; set; }
    public string MakeId { get; set; }
    public bool IsPopularModel { get; set; }
}

public class FuelModel
{
    public string FuelId { get; set; }
    public string FuelName { get; set; }
}

public class MakeModelModel
{
    public IEnumerable<MakeModel> MakeList { get; set; }
    public IEnumerable<Model> ModelList { get; set; }
}
