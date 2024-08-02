using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.InsuranceMaster;
using Insurance.Domain;
using Insurance.Domain.InsuranceMaster;
using Insurance.Persistence.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using ThirdPartyUtilities.Abstraction;

namespace Insurance.Persistence.Repository;
public class InsuranceMasterRepository : IInsuranceMasterRepository
{
    private readonly ApplicationDBContext _context;
    private readonly ISignzyService _signzyService;
    private readonly LogoConfig _logoConfig;
    private readonly IApplicationClaims _applicationClaims;
    public InsuranceMasterRepository(ApplicationDBContext context, ISignzyService signzyService, IOptions<LogoConfig> options, IApplicationClaims applicationClaims)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _signzyService = signzyService ?? throw new ArgumentNullException(nameof(signzyService));
        _logoConfig = options.Value;
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
    }

    /// <summary>
    /// Get GetMakeModelFuel List
    /// </summary>
    /// <param name="insuranceType"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<IEnumerable<InsuranceTypeModel>> GeInsuranceType(string insuranceType, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("InsuranceType", insuranceType, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<InsuranceTypeModel>("[dbo].[Insurance_GetInsuranceType]", parameters, commandType: CommandType.StoredProcedure);

        return result;
    }

    /// <summary>
    /// Get GetMakeModelFuel List
    /// </summary>
    /// <param name="makeModelFuelQuery"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<MakeModelModel> GetMakeModel(GetMakeModelQuery makeModelFuelQuery, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("VehicleType", makeModelFuelQuery.VehicleType, DbType.String, ParameterDirection.Input);
        parameters.Add("CVCategoryId", makeModelFuelQuery.CVCategoryId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetMakeModelFuel]", parameters, commandType: CommandType.StoredProcedure);

        MakeModelModel response = new()
        {
            MakeList = result.Read<MakeModel>(),
            ModelList = result.Read<Model>()
        };
        foreach (var item in response.MakeList)
        {
            if (!string.IsNullOrEmpty(item.ImageURL))
            {
                item.ImageURL = _logoConfig.MakeLogoURL + item.ImageURL;
            }
        }
        return response;
    }

    /// <summary>
    /// Get GetFuelByModel List
    /// </summary>
    /// <param name="modelId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<IEnumerable<FuelModel>> GetFuelByModel(string modelId)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("ModelId", modelId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<FuelModel>("[dbo].[Insurance_GetFuelTypeByModelId]",
            parameters,
            commandType: CommandType.StoredProcedure);

        return result;
    }

    /// <summary>
    /// Get GetVariant List
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<IEnumerable<VariantModel>> GetVariant(string modelId, string fuelId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("ModelId", modelId, DbType.String, ParameterDirection.Input);
        parameters.Add("FuelId", fuelId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<VariantModel>("[dbo].[Insurance_GetVariant]", parameters, commandType: CommandType.StoredProcedure);

        return result;
    }

    /// <summary>
    /// Get GetRTO List
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<RTOCityModel> GetRTO(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetRTO]",
                     commandType: CommandType.StoredProcedure);

        RTOCityModel response = new()
        {
            YearList = result.Read<YearModel>(),
            StateList = result.Read<StateModel>(),
            CityList = result.Read<CityModel>(),
            RTOList = result.Read<RTOModel>(),
        };

        return response;

    }

    /// <summary>
    /// Get GetInsurer List
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<IEnumerable<InsurerModel>> GetInsurer(bool? isCommercial, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("IsCommercial", isCommercial, DbType.Boolean, ParameterDirection.Input);
        var result = await connection.QueryAsync<InsurerModel>("[dbo].[Insurance_GetInsurer]", parameters,
                     commandType: CommandType.StoredProcedure);
        foreach (var item in result)
        {
            item.Logo = _logoConfig.InsurerLogoURL + item.Logo;
        }

        return result;
    }

    /// <summary>
    /// Get GetNCB List
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<IEnumerable<PreviousPolicyTypeModel>> GetPreviousPolicyType(string regDate, bool isBrandNew, string vehicleTypeId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("RegDate", regDate, DbType.String, ParameterDirection.Input);
        parameters.Add("IsBrandNew", isBrandNew, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", vehicleTypeId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<PreviousPolicyTypeModel>("[dbo].[Insurance_GetPreviousPolicyType]", parameters,
                     commandType: CommandType.StoredProcedure);

        return result;
    }

    /// <summary>
    /// Get GetNCB List
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<IEnumerable<NCBModel>> GetNCB(string policyExpiryDate, bool isPreviousPolicy, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("PolicyExpiryDate", policyExpiryDate, DbType.String, ParameterDirection.Input);
        parameters.Add("IsPreviousPolicy", isPreviousPolicy, DbType.Boolean, ParameterDirection.Input);
        var result = await connection.QueryAsync<NCBModel>("[dbo].[Insurance_GetNCB]", parameters,
                     commandType: CommandType.StoredProcedure);

        return result;
    }

    /// <summary>
    /// Get GetVehicleCode
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<VehicleCodeDto> GetVehicleCode(int variantId, int rtoCodeId, int ncbId, int insurerId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("Variant_Id", variantId, DbType.Int64, ParameterDirection.Input);
        parameters.Add("RTOCode_Id", rtoCodeId, DbType.Int64, ParameterDirection.Input);
        parameters.Add("NCB_Id", ncbId, DbType.Int64, ParameterDirection.Input);
        parameters.Add("Insurer_Id", insurerId, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryAsync<VehicleCodeDto>("[dbo].[GetVehicleCode]", parameters, commandType: CommandType.StoredProcedure);
        var vehicleCode = result.FirstOrDefault();

        return vehicleCode;
    }

    /// <summary>
    /// Get Vehicle Details from Response
    /// </summary>
    /// <param name="vehicleNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<VehicleRegistrationModel> GetVehicleDetails(string vehicleNumber, string vehicleTypeId, CancellationToken cancellationToken)
    {
        var vehicleDetails = await CheckVehicleDetailsFromDB(vehicleNumber, vehicleTypeId);
        if (vehicleDetails == null)
        {
            var vehicleResponse = await _signzyService.GetVehicleRegistrationDetails(vehicleNumber, cancellationToken);
            if (vehicleResponse != null)
            {
                //variant id retrieval logic based on score
                string _variantId = string.Empty;
                if (vehicleResponse.result?.mappings?.variantIds != null && vehicleResponse.result.mappings.variantIds.Any())
                    _variantId = vehicleResponse.result?.mappings?.variantIds.OrderByDescending(x => x.score)?.FirstOrDefault().variantId;

                using var connection = _context.CreateConnection();
                var parameters = new DynamicParameters();

                parameters.Add("regNo", vehicleResponse.result.regNo, DbType.String, ParameterDirection.Input);
                parameters.Add("class", vehicleResponse.result.@class, DbType.String, ParameterDirection.Input);
                parameters.Add("chassis", vehicleResponse.result.chassis, DbType.String, ParameterDirection.Input);
                parameters.Add("engine", vehicleResponse.result.engine, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleManufacturerName", vehicleResponse.result.vehicleManufacturerName, DbType.String, ParameterDirection.Input);
                parameters.Add("model", vehicleResponse.result.model, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleColour", vehicleResponse.result.vehicleColour, DbType.String, ParameterDirection.Input);
                parameters.Add("type", vehicleResponse.result.type, DbType.String, ParameterDirection.Input);
                parameters.Add("normsType", vehicleResponse.result.normsType, DbType.String, ParameterDirection.Input);
                parameters.Add("bodyType", vehicleResponse.result.bodyType, DbType.String, ParameterDirection.Input);
                parameters.Add("ownerCount", vehicleResponse.result.ownerCount, DbType.String, ParameterDirection.Input);
                parameters.Add("owner", vehicleResponse.result.owner, DbType.String, ParameterDirection.Input);
                parameters.Add("ownerFatherName", vehicleResponse.result.ownerFatherName, DbType.String, ParameterDirection.Input);
                parameters.Add("mobileNumber", vehicleResponse.result.mobileNumber, DbType.String, ParameterDirection.Input);
                parameters.Add("status", vehicleResponse.result.status, DbType.String, ParameterDirection.Input);
                parameters.Add("statusAsOn", vehicleResponse.result.statusAsOn, DbType.String, ParameterDirection.Input);
                parameters.Add("regAuthority", vehicleResponse.result.regAuthority, DbType.String, ParameterDirection.Input);
                parameters.Add("regDate", vehicleResponse.result.regDate, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleManufacturingMonthYear", vehicleResponse.result.vehicleManufacturingMonthYear, DbType.String, ParameterDirection.Input);
                parameters.Add("rcExpiryDate", vehicleResponse.result.rcExpiryDate, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleTaxUpto", vehicleResponse.result.vehicleTaxUpto, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleInsuranceCompanyName", vehicleResponse.result.vehicleInsuranceCompanyName, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleInsuranceUpto", vehicleResponse.result.vehicleInsuranceUpto, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleInsurancePolicyNumber", vehicleResponse.result.vehicleInsurancePolicyNumber, DbType.String, ParameterDirection.Input);
                parameters.Add("rcFinancer", vehicleResponse.result.rcFinancer, DbType.String, ParameterDirection.Input);
                parameters.Add("presentAddress", vehicleResponse.result.presentAddress, DbType.String, ParameterDirection.Input);
                parameters.Add("splitPresentAddress", JsonConvert.SerializeObject(vehicleResponse.result.splitPresentAddress), DbType.String, ParameterDirection.Input);
                parameters.Add("permanentAddress", vehicleResponse.result.permanentAddress, DbType.String, ParameterDirection.Input);
                parameters.Add("splitPermanentAddress", JsonConvert.SerializeObject(vehicleResponse.result.splitPermanentAddress), DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleCubicCapacity", vehicleResponse.result.vehicleCubicCapacity, DbType.String, ParameterDirection.Input);
                parameters.Add("grossVehicleWeight", vehicleResponse.result.grossVehicleWeight, DbType.String, ParameterDirection.Input);
                parameters.Add("unladenWeight", vehicleResponse.result.unladenWeight, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleCategory", vehicleResponse.result.vehicleCategory, DbType.String, ParameterDirection.Input);
                parameters.Add("rcStandardCap", vehicleResponse.result.rcStandardCap, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleCylindersNo", vehicleResponse.result.vehicleCylindersNo, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleSeatCapacity", vehicleResponse.result.vehicleSeatCapacity, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleSleeperCapacity", vehicleResponse.result.vehicleSleeperCapacity, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleStandingCapacity", vehicleResponse.result.vehicleStandingCapacity, DbType.String, ParameterDirection.Input);
                parameters.Add("wheelbase", vehicleResponse.result.wheelbase, DbType.String, ParameterDirection.Input);
                parameters.Add("vehicleNumber", vehicleResponse.result.vehicleNumber, DbType.String, ParameterDirection.Input);
                parameters.Add("puccNumber", vehicleResponse.result.puccNumber, DbType.String, ParameterDirection.Input);
                parameters.Add("puccUpto", vehicleResponse.result.puccUpto, DbType.String, ParameterDirection.Input);
                parameters.Add("blacklistStatus", vehicleResponse.result.blacklistStatus, DbType.String, ParameterDirection.Input);
                parameters.Add("blacklistDetails", JsonConvert.SerializeObject(vehicleResponse.result.blacklistDetails), DbType.String, ParameterDirection.Input);
                parameters.Add("challanDetails", JsonConvert.SerializeObject(vehicleResponse.result.challanDetails), DbType.String, ParameterDirection.Input);
                parameters.Add("permitIssueDate", vehicleResponse.result.permitIssueDate, DbType.String, ParameterDirection.Input);
                parameters.Add("permitNumber", vehicleResponse.result.permitNumber, DbType.String, ParameterDirection.Input);
                parameters.Add("permitType", vehicleResponse.result.permitType, DbType.String, ParameterDirection.Input);
                parameters.Add("permitValidFrom", vehicleResponse.result.permitValidFrom, DbType.String, ParameterDirection.Input);
                parameters.Add("permitValidUpto", vehicleResponse.result.permitValidUpto, DbType.String, ParameterDirection.Input);
                parameters.Add("nonUseStatus", vehicleResponse.result.nonUseStatus, DbType.String, ParameterDirection.Input);
                parameters.Add("nonUseFrom", vehicleResponse.result.nonUseFrom, DbType.String, ParameterDirection.Input);
                parameters.Add("nonUseTo", vehicleResponse.result.nonUseTo, DbType.String, ParameterDirection.Input);
                parameters.Add("nationalPermitNumber", vehicleResponse.result.nationalPermitNumber, DbType.String, ParameterDirection.Input);
                parameters.Add("nationalPermitUpto", vehicleResponse.result.nationalPermitUpto, DbType.String, ParameterDirection.Input);
                parameters.Add("nationalPermitIssuedBy", vehicleResponse.result.nationalPermitIssuedBy, DbType.String, ParameterDirection.Input);
                parameters.Add("isCommercial", vehicleResponse.result.isCommercial, DbType.String, ParameterDirection.Input);
                parameters.Add("nocDetails", vehicleResponse.result.nocDetails, DbType.String, ParameterDirection.Input);
                parameters.Add("VariantId", _variantId, DbType.String, ParameterDirection.Input);
                parameters.Add("VehicleTypeId", vehicleTypeId, DbType.String, ParameterDirection.Input);
                var result = await connection.QueryAsync<VehicleRegistrationModel>("[dbo].[Insurance_InsertVehicleRegistrationDetails]", parameters,
                             commandType: CommandType.StoredProcedure);



                return result.FirstOrDefault();
            }
        }
        return vehicleDetails;
    }

    private async Task<VehicleRegistrationModel> CheckVehicleDetailsFromDB(string vehicleNumber, string vehicleTypeId)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("regNo", vehicleNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", vehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("Condition", "CHECKVEHICLEDETAILS", DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<VehicleRegistrationModel>("[dbo].[Insurance_InsertVehicleRegistrationDetails]", parameters,
                     commandType: CommandType.StoredProcedure);
        var vehicleCode = result.FirstOrDefault();

        return vehicleCode;
    }
    /// <summary>
    /// Insert Lead Details into Database
    /// </summary>
    /// <param name="createLeadModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<LeadModel>> CreateLead(CreateLeadModel createLeadModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("VehicleTypeId", createLeadModel.VehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", createLeadModel.VehicleNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("VariantId", createLeadModel.VariantId, DbType.String, ParameterDirection.Input);
        parameters.Add("YearId", createLeadModel.YearId, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadName", createLeadModel.LeadName, DbType.String, ParameterDirection.Input);
        parameters.Add("PhoneNumber", createLeadModel.PhoneNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("Email", createLeadModel.Email, DbType.String, ParameterDirection.Input);
        parameters.Add("PrevPolicyTypeId", createLeadModel.PrevPolicyTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", createLeadModel.RTOId, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadId", createLeadModel.LeadID, DbType.String, ParameterDirection.Input);
        parameters.Add("IsBrandNew", createLeadModel.IsBrandNew, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("CreatedBy", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
        parameters.Add("RefLeadId", createLeadModel.RefLeadId, DbType.String, ParameterDirection.Input);
        parameters.Add("CarrierType", !string.IsNullOrEmpty(createLeadModel.CarrierType) ? Convert.ToInt32(createLeadModel.CarrierType) : null, DbType.Int32, ParameterDirection.Input);
        parameters.Add("UsageNatureId", !string.IsNullOrEmpty(createLeadModel.UsageNatureId) ? Convert.ToInt32(createLeadModel.UsageNatureId) : null, DbType.Int32, ParameterDirection.Input);
        parameters.Add("VehicleBodyId", !string.IsNullOrEmpty(createLeadModel.VehicleBodyId) ? Convert.ToInt32(createLeadModel.VehicleBodyId) : null, DbType.Int32, ParameterDirection.Input);
        parameters.Add("HazardousVehicleUse", !string.IsNullOrEmpty(createLeadModel.HazardousVehicleUse) ? Convert.ToInt32(createLeadModel.HazardousVehicleUse) : null, DbType.Int32, ParameterDirection.Input);
        parameters.Add("IfTrailer", !string.IsNullOrEmpty(createLeadModel.IfTrailer) ? Convert.ToInt32(createLeadModel.IfTrailer) : null, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("TrailerIDV", createLeadModel.TrailerIDV, DbType.String, ParameterDirection.Input);
        parameters.Add("CategoryId", !string.IsNullOrEmpty(createLeadModel.CategoryId) ? Convert.ToInt32(createLeadModel.CategoryId) : null, DbType.Int32, ParameterDirection.Input);
        parameters.Add("SubCategoryId", !string.IsNullOrEmpty(createLeadModel.SubCategoryId) ? Convert.ToInt32(createLeadModel.SubCategoryId) : null, DbType.Int32, ParameterDirection.Input);
        parameters.Add("UsageTypeId ", !string.IsNullOrEmpty(createLeadModel.UsageType) ? Convert.ToInt32(createLeadModel.UsageType) : null, DbType.Int32, ParameterDirection.Input);
        parameters.Add("PCVVehicleCategoryId", !string.IsNullOrEmpty(createLeadModel.PCVVehicleCategory) ? Convert.ToInt32(createLeadModel.PCVVehicleCategory) : null, DbType.Int32, ParameterDirection.Input);

        var result = await connection.QueryAsync<LeadModel>("[dbo].[Insurance_InsertLeadDetails]", parameters, commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<QuoteMasterModel> GetQuote(string vehicleTypeId, string policytypeid, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("VehicleTypeId", vehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("PolicyTypeId", policytypeid, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetQuoteMaster]", parameters,
            commandType: CommandType.StoredProcedure);

        QuoteMasterModel response = new()
        {
            DiscountList = result.Read<DiscountModel>(),
            AddOnsList = result.Read<AddonsModel>(),
            AccessoryList = result.Read<AccessoryModel>(),
        };

        var paCoverList = result.Read<PACoverModel>().ToList();
        var paCoverExtensionList = result.Read<PACoverExtensionModel>();
        var discountExtensionList = result.Read<DiscountExtensionModel>();
        var addOnExtensionList = result.Read<AddOnsExtensionModel>();

        foreach (var item in paCoverList)
        {
            var extModel = paCoverExtensionList
                .Where(x => x.PACoverId.Equals(item.PACoverId))
                .Select(d => new PACoverExtensionModel
                {
                    PACoverExtension = d.PACoverExtension,
                    PACoverExtensionId = d.PACoverExtensionId,
                    PACoverId = d.PACoverId
                }).ToList();
            item.PACoverExtensionList = extModel;
        }
        response.PACoverList = paCoverList;

        foreach (var item in response.AddOnsList)
        {
            var extModel = addOnExtensionList
                .Where(x => x.AddOnsId.Equals(item.AddOnId))
                .Select(d => new AddOnsExtensionModel
                {
                    AddOnsExtension = d.AddOnsExtension,
                    AddOnsExtensionId = d.AddOnsExtensionId,
                    AddOnsId = d.AddOnsId
                }).ToList();
            item.AddOnsExtensionList = extModel;
        }

        foreach (var item in response.DiscountList)
        {
            var extModel = discountExtensionList
                .Where(x => x.DiscountId.Equals(item.DiscountId))
                .Select(d => new DiscountExtensionModel
                {
                    DiscountExtension = d.DiscountExtension,
                    DiscountExtensionId = d.DiscountExtensionId,
                    DiscountId = d.DiscountId
                }).ToList();
            item.DiscountExtensionList = extModel;
        }

        return response;
    }

    public async Task<IEnumerable<CityModel>> GetCity(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<CityModel>("[dbo].[Insurance_GetCityMaster]", parameters,
                     commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<IEnumerable<FinancierModel>> GetFinancier(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<FinancierModel>("[dbo].[Insurance_GetFinancierMaster]", parameters,
                     commandType: CommandType.StoredProcedure);

        return result;
    }
    public async Task<MasterCityModel> GetStateCity(string insurerId, string pincode, string state, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("InsurerId", insurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("Pincode", pincode, DbType.String, ParameterDirection.Input);
        parameters.Add("State", state, DbType.String, ParameterDirection.Input);


        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetProposalMaster]",
            parameters, commandType: CommandType.StoredProcedure);

        MasterCityModel masterCityModel = new()
        {
            MasterData = result.Read<CityList>(),
            DefaultCity = result.Read<DefaultCity>().FirstOrDefault(),
        };
        return masterCityModel;
    }

    public async Task<GetLeadPreviousPolicyTypeModel> GetLeadPreviousPolicyType(string vehicleTypeId, string vehicleNumber, string previousPolicyTypeId, string yearId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("VehicleTypeId", vehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("PreviousPolicyTypeId", previousPolicyTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("YearId", yearId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<GetLeadPreviousPolicyTypeModel>("[dbo].[Insurance_GetLeadPreviousPolicyType]", parameters,
                     commandType: CommandType.StoredProcedure);

        return result.FirstOrDefault();
    }
    public async Task<IEnumerable<InsuranceMasterDataModel>> GetInsuranceMaster(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<InsuranceMasterDataModel>("[dbo].[Insurance_GetInsuranceMaster]", parameters,
                     commandType: CommandType.StoredProcedure);

        return result;
    }
}
