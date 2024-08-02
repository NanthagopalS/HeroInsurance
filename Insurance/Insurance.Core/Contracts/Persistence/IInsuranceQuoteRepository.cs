using Insurance.Core.Features.InsuranceMaster;
using Insurance.Domain;
using Insurance.Domain.InsuranceMaster;
namespace Insurance.Core.Contracts.Persistence;
public interface IInsuranceMasterRepository
{
    Task<MakeModelModel> GetMakeModel(GetMakeModelQuery makeModelFuelQuery, CancellationToken cancellationToken);
    Task<IEnumerable<FuelModel>> GetFuelByModel(string modelId);
    Task<IEnumerable<VariantModel>> GetVariant(string modelId, string fuelId, CancellationToken cancellationToken);
    Task<RTOCityModel> GetRTO(CancellationToken cancellationToken);
    Task<IEnumerable<InsurerModel>> GetInsurer(bool? isCommercial, CancellationToken cancellationToken);
    Task<IEnumerable<NCBModel>> GetNCB(string policyExpiryDate, bool isPreviousPolicy, CancellationToken cancellationToken);
    Task<VehicleCodeDto> GetVehicleCode(int variantId, int rtoCodeId, int ncbId, int insurerId, CancellationToken cancellationToken);
    Task<IEnumerable<InsuranceTypeModel>> GeInsuranceType(string insuranceType, CancellationToken cancellationToken);
    Task<VehicleRegistrationModel> GetVehicleDetails(string vehicleNumber, string vehicleTypeId, CancellationToken cancellationToken);
    Task<IEnumerable<PreviousPolicyTypeModel>> GetPreviousPolicyType(string regDate, bool isBrandNew, string vehicleTypeId, CancellationToken cancellationToken);
    Task<IEnumerable<LeadModel>> CreateLead(CreateLeadModel createLeadModel, CancellationToken cancellationToken);
    Task<QuoteMasterModel> GetQuote(string vehicleTypeId, string policytypeid, CancellationToken cancellationToken);
    Task<IEnumerable<CityModel>> GetCity(CancellationToken cancellationToken);
    Task<IEnumerable<FinancierModel>> GetFinancier(CancellationToken cancellationToken);
    Task<IEnumerable<InsuranceMasterDataModel>> GetInsuranceMaster(CancellationToken cancellationToken);
    Task<MasterCityModel> GetStateCity(string insurerId, string pincode, string state, CancellationToken cancellationToken);
    Task<GetLeadPreviousPolicyTypeModel> GetLeadPreviousPolicyType(string vehicleTypeId, string vehicleNumber, string previousPolicyTypeId, string yearId, CancellationToken cancellationToken);
}
