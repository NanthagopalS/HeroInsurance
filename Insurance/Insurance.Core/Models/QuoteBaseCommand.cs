using Insurance.Domain.Quote;

namespace Insurance.Core.Models;
public class QuoteBaseCommand
{
    public string VehicleNumber { get; set; }
    public string VariantId { get; set; }
    public string RTOId { get; set; }
    public string VehicleTypeId { get; set; }
    public string RegistrationYear { get; set; }
    public bool IsVehicleNumberPresent { get; set; }
    public List<AddOnList> AddOnsList { get; set; }
    public List<DiscountList> DiscountList { get; set; }
    public List<AccessoriesList> AccessoryList { get; set; }
    public List<PACoverList> PACoverList { get; set; }
    public PreviousPolicyModel PreviousPolicy { get; set; }
    public PolicyDatesResponse PolicyDates { get; set; }
    public bool IsBrandNewVehicle { get; set; }
    public string LeadId { get; set; }
    public int IDV { get; set; }
    public bool IsDefaultPACoverRequired { get; set; }
    public bool IsFourWheeler { get; set; }
    public bool IsTwoWheeler { get; set; }
    public bool IsCommercial { get; set; }
    public string CategoryId { get; set; }
}


public class AddOnList
{
    public string AddOnId { get; set; }
    public string AddOnsExtensionId { get; set; }
}

public class DiscountList
{
    public string DiscountId { get; set; }
    public double DiscountValue { get; set; }
    public string DiscountExtensionId { get; set; }
}

public class AccessoriesList
{
    public string AccessoryId { get; set; }
    public int AccessoryValue { get; set; }
}

public class PACoverList
{
    public string PACoverId { get; set; }
    public double PACoverValue { get; set; }
    public string PACoverExtensionId { get; set; }
}

public class PreviousPolicyModel
{
    public bool IsPreviousPolicy { get; set; }
    public string PreviousPolicyNumber { get; set; }
    public string PreviousPolicyTypeId { get; set; }
    public string SAODInsurer { get; set; }
    public string SAODPolicyExpiryDate { get; set; }
    public string TPInsurer { get; set; }
    public string TPPolicyExpiryDate { get; set; }
    public bool IsPolicyExpired { get; set; }
    public bool IsPreviousYearClaim { get; set; }
    public string NCBId { get; set; }
    public string PreviousPolicyNumberSATP { get; set; }

}