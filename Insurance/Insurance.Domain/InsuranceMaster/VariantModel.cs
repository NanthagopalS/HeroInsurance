namespace Insurance.Domain;
public class VariantModel
{
    public string VariantId { get; set; }
    public string VariantName { get; set; }
}

public class VehicleCodeDto
{
    public string VehicleCode { get; set; }
    public string RegionCode { get; set; }
    public string VehicleManufactureCode { get; set; }
    public string VehicleModelCode { get; set; }
    public string RTOLocationCode { get; set; }
    public string State { get; set; }
    public string NCBName { get; set; }
    public string NCBValue { get; set; }
    public string GoInsCode { get; set; }
}

public class VehicleRegistrationModel
{
    public string VehicleId { get; set; }
    public string VehicleMake { get; set; }
    public string VehicleModel { get; set; }
    public string VariantName { get; set; }
    public string VehiceFuelType { get; set; }
    public string VehicleCC { get; set; }
    public string RegistrationDate { get; set; }
    public string RTOLocation { get; set; }
    public string RegistrationNumber { get; set; }
    public string VehicleTypeId { get; set; }
    public string VehicleClass { get; set; }
    public string VariantId { get; set;}
    public string TPExpiryDate { get; set; }
    public string PrevInsurerCompanyName { get; set; }
    public string PrevInsurerId { get; set; }
    public string PrevNCBId { get; set; }
    public string PrevNCBPercent { get; set; }
    public string PrevInsurancePolicyNumber { get; set; }
}