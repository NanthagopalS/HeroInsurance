using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Reliance.Command.CKYC;
using Insurance.Core.Features.Reliance.Command.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.Reliance;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data;
using System.Globalization;

namespace Insurance.Persistence.Repository;

public class RelianceRepository : IRelianceRepository
{
    private readonly IRelianceService _relianceService;
    private readonly ApplicationDBContext _applicationDBContext;
    private readonly RelianceConfig _relianceConfig;
    private readonly IApplicationClaims _applicationClaims;
    private readonly IQuoteRepository _quoteRepository;
    public RelianceRepository(IRelianceService relianceService,
                          ApplicationDBContext applicationDBContext,
                          IOptions<RelianceConfig> options,
                          IApplicationClaims applicationClaims,
                          IQuoteRepository quoteRepository)
    {
        _relianceService = relianceService ?? throw new ArgumentNullException(nameof(relianceService));
        _applicationDBContext = applicationDBContext ?? throw new ArgumentNullException(nameof(applicationDBContext));
        _relianceConfig = options?.Value;
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
    }
    public async Task<QuoteResponseModel> GetQuote(GetRelianceQuery query, CancellationToken cancellationToken)
    {
        var quoteQuery = await QuoteMasterMapping(query, cancellationToken);

        if (quoteQuery is not null)
        {
            // Call Quote API
            var quoteResponse = await _relianceService.GetQuote(quoteQuery, cancellationToken);

            if (!string.IsNullOrEmpty(quoteResponse.QuoteResponseModel.ValidationMessage))
                return quoteResponse.QuoteResponseModel;

            SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
            {
                quoteResponseModel = quoteResponse.QuoteResponseModel,
                RequestBody = quoteResponse.RequestBody,
                ResponseBody = quoteResponse.ResponseBody,
                Stage = "Quote",
                InsurerId = _relianceConfig.InsurerId,
                LeadId = query.LeadId,
                MaxIDV = Convert.ToDecimal(quoteResponse.QuoteResponseModel?.MaxIDV),
                MinIDV = Convert.ToDecimal(quoteResponse.QuoteResponseModel?.MinIDV),
                RecommendedIDV = Convert.ToDecimal(quoteResponse.QuoteResponseModel?.IDV),
                TransactionId = quoteResponse.QuoteResponseModel?.ApplicationId,
                PolicyNumber = string.Empty,
                SelectedIDV = (quoteQuery.IsBrandNewVehicle && quoteQuery.IDVValue == 0) ? "1" : Convert.ToString(query.IDV),
                PolicyTypeId = query.PreviousPolicy.PreviousPolicyTypeId,
                IsPreviousPolicy = query.PreviousPolicy.IsPreviousPolicy,
                SAODInsurerId = query.PreviousPolicy.SAODInsurer,
                SATPInsurerId = query.PreviousPolicy.TPInsurer,
                SAODPolicyStartDate = Convert.ToDateTime(query.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                SAODPolicyExpiryDate = Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                SATPPolicyStartDate = Convert.ToDateTime(query.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                SATPPolicyExpiryDate = Convert.ToDateTime(query.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                RegistrationDate = Convert.ToDateTime(query.PolicyDates.RegistrationDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                VehicleNumber = query.IsVehicleNumberPresent ? query.VehicleNumber : quoteQuery.VehicleDetails.LicensePlateNumber,
                IsPreviousYearClaim = query.PreviousPolicy.IsPreviousYearClaim,
                PreviousYearNCB = query.PreviousPolicy.NCBId,
                IsBrandNew = query.IsBrandNewVehicle,
                RTOId = query.RTOId,
            };
            await _quoteRepository.SaveQuoteTransaction(saveQuoteTransactionModel, cancellationToken);


            if (quoteResponse.QuoteResponseModel.InsurerName != null)
            {
                quoteResponse.QuoteResponseModel.InsurerId = _relianceConfig.InsurerId;
                return quoteResponse.QuoteResponseModel;
            }
        }
        return default;
    }
    private async Task<QuoteQueryModel> QuoteMasterMapping(GetRelianceQuery query, CancellationToken cancellationToken)
    {
        var addOnId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList.Select(x => $"{x.AddOnId} ")) : string.Empty;
        var paCoverId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList.Select(x => $"{x.PACoverId} ")) : string.Empty;
        var accessoryId = (query.AccessoryList != null && query.AccessoryList.Any()) ? String.Join(",", query.AccessoryList.Select(x => $"{x.AccessoryId} ")) : string.Empty;
        var discountId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList.Select(x => $"{x.DiscountId}")) : string.Empty;
        var paCoverExtensionId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList.Select(x => $"{x.PACoverExtensionId}")) : string.Empty;
        var discountExtensionId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList.Select(x => $"{x.DiscountExtensionId}")) : string.Empty;
        var addOnsExtensionId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnsExtensionId} ")) : String.Empty;
        var previousInsurer = "DC765CBA-79F7-4E6B-86D0-245C7CF87CD3";
        if (query.PreviousPolicy != null)
        {
            previousInsurer = query.PreviousPolicy.TPInsurer != null ? query.PreviousPolicy.TPInsurer : query.PreviousPolicy.SAODInsurer;
        }

        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("PACoverId", paCoverId, DbType.String, ParameterDirection.Input);
        parameters.Add("AccessoryId", accessoryId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddonId", addOnId, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountId", discountId, DbType.String, ParameterDirection.Input);
        parameters.Add("InsurerId", _relianceConfig.InsurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", query.RTOId, DbType.String, ParameterDirection.Input);
        parameters.Add("VariantId", query.VariantId, DbType.String, ParameterDirection.Input);
        parameters.Add("NCBId", query.PreviousPolicy?.NCBId, DbType.String, ParameterDirection.Input);
        parameters.Add("PolicyTypeId", query.PolicyDates.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", query.VehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("PACoverExtensionId", paCoverExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountExtensionId", discountExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddOnsExtensionId", addOnsExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", query.VehicleNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadId", query.LeadId, DbType.String, ParameterDirection.Input);
        parameters.Add("IDV", query.IDV, DbType.Int32, ParameterDirection.Input);
        parameters.Add("PreviousInsurer", previousInsurer, DbType.String, ParameterDirection.Input);
        parameters.Add("RegistrationYear", query.RegistrationYear, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetRelianceQuoteMasterMapping]", parameters,
                     commandType: CommandType.StoredProcedure);

        var paCoverList = result.Read<PACoverModel>();
        var accessoryList = result.Read<AccessoryModel>();
        var addOnList = result.Read<AddonsModel>();
        var discountList = result.Read<DiscountModel>();
        var paCoverExtensionList = result.Read<PACoverExtensionModel>();
        var discountExtensionList = result.Read<DiscountExtensionModel>();
        var addOnsExtensionList = result.Read<AddOnsExtensionModel>();
        var codeList = result.Read<RTOVehiclePreviousInsurerModel>();
        var configNameValueList = result.Read<ConfigNameValueModel>();

        var quoteQuery = new QuoteQueryModel
        {
            AddOns = new Domain.GoDigit.AddOns(),
            Discounts = new Discounts(),
            PACover = new PACover(),
            Accessories = new Accessories(),
            VehicleDetails = new VehicleDetails(),
            PreviousPolicyDetails = new PreviousPolicy()
        };
        if (addOnList.Any())
        {
            foreach (var item in addOnList)
            {

                if (item.AddOns.Equals("ReturnToInvoice"))
                {
                    quoteQuery.AddOns.ReturnToInvoiceIdId = (addOnList?.Where(x => x.AddOns == "ReturnToInvoice").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsReturnToInvoiceRequired = true;
                }
                else if (item.AddOns.Equals("PartsDepreciation"))
                {
                    quoteQuery.AddOns.ZeroDebtId = (addOnList?.Where(x => x.AddOns == "PartsDepreciation").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsZeroDebt = true;
                }
                else if (item.AddOns.Equals("EngineProtection"))
                {
                    quoteQuery.AddOns.EngineProtectionId = (addOnList?.Where(x => x.AddOns == "EngineProtection").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsEngineProtectionRequired = true;
                }
                else if (item.AddOns.Equals("RoadSideAssistance"))
                {
                    quoteQuery.AddOns.RoadSideAssistanceId = (addOnList?.Where(x => x.AddOns == "RoadSideAssistance").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsRoadSideAssistanceRequired = true;
                }
                else if (item.AddOns.Equals("KeyAndLockReplacement"))
                {
                    quoteQuery.AddOns.KeyAndLockProtectionId = (addOnList?.Where(x => x.AddOns == "KeyAndLockReplacement").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsKeyAndLockProtectionRequired = true;
                }
                else if (item.AddOns.Equals("PersonalBelongings"))
                {
                    quoteQuery.AddOns.PersonalBelongingId = (addOnList?.Where(x => x.AddOns == "PersonalBelongings").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsPersonalBelongingRequired = true;
                }
                else if (item.AddOns.Equals("Consumables"))
                {
                    quoteQuery.AddOns.ConsumableId = (addOnList?.Where(x => x.AddOns == "Consumables").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsConsumableRequired = true;
                }
                else if (item.AddOns.Equals("DailyAllowance"))
                {
                    quoteQuery.AddOns.DailyAllowanceId = (addOnList?.Where(x => x.AddOns == "DailyAllowance").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsDailyAllowance = true;
                }
                else if (item.AddOns.Equals("EMIProtectorCover"))
                {
                    quoteQuery.AddOns.EMIProtectorId = (addOnList?.Where(x => x.AddOns == "EMIProtectorCover").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsEMIProtectorRequired = true;
                }
                else if (item.AddOns.Equals("GeoAreaExtension"))
                {
                    quoteQuery.AddOns.GeoAreaExtensionId = (addOnList?.Where(x => x.AddOns == "GeoAreaExtension").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsGeoAreaExtension = true;
                }
                else if (item.AddOns.Equals("LimitedtoOwnPremisesCover"))
                {
                    quoteQuery.AddOns.LimitedOwnPremisesId = (addOnList?.Where(x => x.AddOns == "LimitedtoOwnPremisesCover").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsLimitedOwnPremisesRequired = true;
                }
                else if (item.AddOns.Equals("LossofUseDownTimeProtCover"))
                {
                    quoteQuery.AddOns.LossOfDownTimeId = (addOnList?.Where(x => x.AddOns == "LossofUseDownTimeProtCover").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsLossOfDownTimeRequired = true;
                }
                else if (item.AddOns.Equals("NCBProtect"))
                {
                    quoteQuery.AddOns.NCBId = (addOnList?.Where(x => x.AddOns == "NCBProtect").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsNCBRequired = true;
                }
                else if (item.AddOns.Equals("RimProtection"))
                {
                    quoteQuery.AddOns.RimProtectionId = (addOnList?.Where(x => x.AddOns == "RimProtection").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsRimProtectionRequired = true;
                }
                else if (item.AddOns.Equals("RoadsideAssistanceAdvance"))
                {
                    quoteQuery.AddOns.RoadSideAssistanceAdvanceId = (addOnList?.Where(x => x.AddOns == "RoadsideAssistanceAdvance").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired = true;
                }
                else if (item.AddOns.Equals("RoadsideAssistanceWider"))
                {
                    quoteQuery.AddOns.RoadSideAssistanceWiderId = (addOnList?.Where(x => x.AddOns == "RoadsideAssistanceWider").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsRoadSideAssistanceWiderRequired = true;
                }
                else if (item.AddOns.Equals("TowingCover"))
                {
                    quoteQuery.AddOns.TowingId = (addOnList?.Where(x => x.AddOns == "TowingCover").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsTowingRequired = true;
                }
                else if (item.AddOns.Equals("TyreProtection"))
                {
                    quoteQuery.AddOns.TyreProtectionId = (addOnList?.Where(x => x.AddOns == "TyreProtection").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsTyreProtectionRequired = true;
                }

            }
        }
        if (accessoryList.Any())
        {
            foreach (var item in accessoryList)
            {
                if (item.Accessory.Equals("CNG"))
                {
                    quoteQuery.Accessories.CNGId = (accessoryList?.Where(x => x.Accessory == "CNG").Select(d => d.AccessoryId).FirstOrDefault());
                    quoteQuery.Accessories.IsCNG = true;
                    quoteQuery.Accessories.CNGValue = (int)(query.AccessoryList?.Where(x => x.AccessoryId == item.AccessoryId).Select(d => d.AccessoryValue).FirstOrDefault());
                }
                else if (item.Accessory.Equals("Electrical"))
                {
                    quoteQuery.Accessories.ElectricalId = (accessoryList?.Where(x => x.Accessory == "Electrical").Select(d => d.AccessoryId).FirstOrDefault());
                    quoteQuery.Accessories.IsElectrical = true;
                    quoteQuery.Accessories.ElectricalValue = (int)(query.AccessoryList?.Where(x => x.AccessoryId == item.AccessoryId).Select(d => d.AccessoryValue).FirstOrDefault());
                }
                else if (item.Accessory.Equals("NonElectrical"))
                {
                    quoteQuery.Accessories.NonElectricalId = (accessoryList?.Where(x => x.Accessory == "NonElectrical").Select(d => d.AccessoryId).FirstOrDefault());
                    quoteQuery.Accessories.IsNonElectrical = true;
                    quoteQuery.Accessories.NonElectricalValue = (int)(query.AccessoryList?.Where(x => x.AccessoryId == item.AccessoryId).Select(d => d.AccessoryValue).FirstOrDefault());
                }
            }
        }
        if (paCoverList.Any())
        {
            foreach (var item in paCoverList)
            {
                if (item.CoverName.Equals("UnnamedOWNERDRIVER"))
                {
                    quoteQuery.PACover.UnnamedOWNERDRIVERId = (paCoverList?.Where(x => x.CoverName == "UnnamedOWNERDRIVER").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedOWNERDRIVER = true;
                }
                else if (item.CoverName.Equals("UnnamedPaidDriver"))
                {
                    quoteQuery.PACover.PaidDriverId = (paCoverList?.Where(x => x.CoverName == "UnnamedPaidDriver").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsPaidDriver = true;
                }
                else if (item.CoverName.Equals("UnnamedPax"))
                {
                    quoteQuery.PACover.UnnamedPassengerId = (paCoverList?.Where(x => x.CoverName == "UnnamedPax").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedPassenger = true;
                }
                else if (item.CoverName.Equals("UnnamedPillionRider"))
                {
                    quoteQuery.PACover.UnnamedPillionRiderId = (paCoverList?.Where(x => x.CoverName == "UnnamedPillionRider").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedPillionRider = true;
                }
            }
        }
        if (discountList.Any())
        {
            foreach (var item in discountList)
            {
                if (item.DiscountName.Equals("AAMembership"))
                {
                    quoteQuery.Discounts.AAMemberShipId = (discountList?.Where(x => x.DiscountName == "AAMembership").Select(d => d.DiscountId).FirstOrDefault());
                    quoteQuery.Discounts.IsAAMemberShip = true;
                }
                else if (item.DiscountName.Equals("ARAIApprovedAntiTheftDevice"))
                {
                    quoteQuery.Discounts.AntiTheftId = (discountList?.Where(x => x.DiscountName == "ARAIApprovedAntiTheftDevice").Select(d => d.DiscountId).FirstOrDefault());
                    quoteQuery.Discounts.IsAntiTheft = true;
                }
                else if (item.DiscountName.Equals("LimitedThirdPartyCoverage"))
                {
                    quoteQuery.Discounts.LimitedTPCoverageId = (discountList?.Where(x => x.DiscountName == "LimitedThirdPartyCoverage").Select(d => d.DiscountId).FirstOrDefault());
                    quoteQuery.Discounts.IsLimitedTPCoverage = true;
                }
                else if (item.DiscountName.Equals("VoluntaryDeductible"))
                {
                    quoteQuery.Discounts.VoluntarilyDeductibleId = (discountList?.Where(x => x.DiscountName == "VoluntaryDeductible").Select(d => d.DiscountId).FirstOrDefault());
                    quoteQuery.Discounts.IsVoluntarilyDeductible = true;
                }
            }
        }
        if (discountExtensionList.Any())
        {
            quoteQuery.VoluntaryExcess = (discountExtensionList?.Select(d => d.DiscountExtension).FirstOrDefault());
        }
        if (paCoverExtensionList.Any())
        {
            quoteQuery.PACover.UnnamedPassengerValue = Convert.ToInt32((paCoverExtensionList?.Select(d => d.PACoverExtension).FirstOrDefault()));
        }
        if (addOnsExtensionList.Any())
        {
            quoteQuery.GeogExtension = String.Join(",", addOnsExtensionList.Select(x => $"{x.AddOnsExtension} ")).Trim();
        }
        string previousInsurerCode = string.Empty;
        string previousPolicyNumber = string.Empty;
        string ncbValue = string.Empty;
        string ncbValueId = string.Empty;
        quoteQuery.IsSAODMandatry = false;
        quoteQuery.IsSATPMandatory = false;
        quoteQuery.LeadId = query.LeadId;
        if (codeList.Any())
        {
            var codeData = codeList.FirstOrDefault();
            previousInsurerCode = codeData.PreviousInsurerCode;
            ncbValue = codeData.NCBValue;
            ncbValueId = codeData.NCBValueId;
            quoteQuery.VehicleDetails.VehicleMaincode = codeData.VehicleCode;
            quoteQuery.VehicleDetails.LicensePlateNumber = codeData.RTOCode;
            quoteQuery.CityName = codeData.CityName;

            quoteQuery.VehicleDetails.VehicleType = codeData.VehicleType;
            quoteQuery.VehicleDetails.VehicleMakeCode = codeData.VehicleMakeCode;
            quoteQuery.VehicleDetails.VehicleMake = codeData.VehicleMake;
            quoteQuery.VehicleDetails.VehicleModelCode = codeData.VehicleModelCode;
            quoteQuery.VehicleDetails.VehicleModel = codeData.VehicleModel;
            quoteQuery.VehicleDetails.VehicleSubTypeCode = codeData.VehicleSubTypeCode;
            quoteQuery.VehicleDetails.VehicleSubType = codeData.VehicleSubType;
            quoteQuery.VehicleDetails.VehicleVariant = codeData.VehicleVariant;
            quoteQuery.VehicleDetails.Fuel = codeData.Fuel;
            quoteQuery.VehicleDetails.FuelId = codeData.FuelId;
            quoteQuery.VehicleDetails.NoOfWheels = codeData.NoOfWheels;
            quoteQuery.VehicleDetails.Zone = (codeData.Zone);
            quoteQuery.VehicleDetails.VehicleClass = codeData.vehicleclass;
            quoteQuery.VehicleDetails.Chassis = codeData.chassis;
            quoteQuery.VehicleDetails.EngineNumber = codeData.engine;
            quoteQuery.VehicleDetails.VehicleColour = codeData.vehicleColour;
            quoteQuery.VehicleDetails.RegDate = codeData.regDate;
            quoteQuery.VehicleDetails.VehicleCubicCapacity = codeData.vehicleCubicCapacity;
            quoteQuery.VehicleDetails.VehicleSeatCapacity = codeData.vehicleSeatCapacity;
            quoteQuery.VehicleDetails.RegNo = codeData.vehicleNumber;
            quoteQuery.VehicleNumber = query.VehicleNumber;
            quoteQuery.VehicleDetails.ManufactureDate = codeData.ManufactureDate;
            quoteQuery.PlanType = codeData.PlanType;
            quoteQuery.RTOLocationCode = codeData.RTOLocationCode;
            quoteQuery.State_Id = codeData.State_Id;
            quoteQuery.CurrentPolicyType = codeData.CurrentPolicyType;
            previousPolicyNumber = codeData.PreviousInsurancePolicyNumber;
            quoteQuery.SelectedIDV = query.IDV;
            quoteQuery.IDVValue = codeData.IDVValue;
            quoteQuery.MinIDV = codeData.MinIdv;
            quoteQuery.MaxIDV = codeData.MaxIdv;
            quoteQuery.RecommendedIDV = codeData.RecommendedIdv;
            quoteQuery.PolicyTypeId = query.PolicyDates.PreviousPolicyTypeId;
            quoteQuery.ExShowRoomPrice = (decimal)codeData.ExShowRoomPrice;
            quoteQuery.ChassisPrice = (decimal)codeData.ChassisPrice;
        }
        if (!query.IsBrandNewVehicle)
        {
            if (codeList.FirstOrDefault().CurrentPolicyType.Equals("SAOD"))
            {
                quoteQuery.IsSAODMandatry = true;
                quoteQuery.IsSATPMandatory = true;
            }
            if (codeList.FirstOrDefault().CurrentPolicyType.Equals("SATP"))
            {
                quoteQuery.IsSATPMandatory = true;
            }
            else
            {
                quoteQuery.IsSAODMandatry = true;
            }
            quoteQuery.PreviousPolicyDetails.IsClaimInLastYear = true;
            if (query.PreviousPolicy != null && query.PreviousPolicy.IsPreviousPolicy)
            {
                quoteQuery.PreviousPolicyDetails = new PreviousPolicy
                {
                    IsPreviousInsurerKnown = query.PreviousPolicy.IsPreviousPolicy,
                    PreviousPolicyNumber = string.IsNullOrEmpty(previousPolicyNumber) ? query.PreviousPolicy.PreviousPolicyNumber : previousPolicyNumber,
                    PreviousPolicyNumberTP = string.IsNullOrEmpty(previousPolicyNumber) ? query.PreviousPolicy.PreviousPolicyNumberSATP : previousPolicyNumber,
                    PreviousInsurerCode = previousInsurerCode,
                    PreviousNoClaimBonus = ncbValue,
                    PreviousNoClaimBonusValue = ncbValueId,
                    IsClaimInLastYear = query.PreviousPolicy.IsPreviousYearClaim,
                };
            }

            quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = query.PolicyDates.ODPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyStartDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP = query.PolicyDates.TPPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyStartDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = query.PolicyDates.ODPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP = query.PolicyDates.TPPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) : null;
        }

        quoteQuery.PolicyStartDate = Convert.ToDateTime(query.PolicyDates.PolicyStartDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        quoteQuery.PolicyEndDate = Convert.ToDateTime(query.PolicyDates.PolicyEndDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        quoteQuery.RegistrationDate = Convert.ToDateTime(query.PolicyDates.RegistrationDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

        quoteQuery.VehicleODTenure = query.PolicyDates.VehicleODTenure;
        quoteQuery.VehicleTPTenure = query.PolicyDates.VehicleTPTenure;
        quoteQuery.IsBrandNewVehicle = query.IsBrandNewVehicle;
        quoteQuery.RegistrationYear = query.RegistrationYear;
        quoteQuery.ConfigNameValueModels = configNameValueList;
        quoteQuery.VehicleTypeId = query.VehicleTypeId;
        quoteQuery.VehicleDetails.IsTwoWheeler = query.PolicyDates.IsTwoWheeler;
        quoteQuery.VehicleDetails.IsFourWheeler = query.PolicyDates.IsFourWheeler;
        quoteQuery.VehicleDetails.IsCommercialVehicle = query.PolicyDates.IsCommercial;
        quoteQuery.CategoryId = query.CategoryId;
        return quoteQuery;
    }
    public Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        var res = _relianceService.QuoteConfirmDetails(quoteTransactionDbModel, quoteConfirmCommand, cancellationToken);
        return res;
    }

    public async Task<SaveCKYCResponse> GetCKYCDetails(RelianceCKYCCommand relianceCKYCCommand, CancellationToken cancellationToken)
    {
        var cKycResponse = await _relianceService.GetCKYCResponse(relianceCKYCCommand, cancellationToken);
        CreateLeadModel createLeadModelObject = cKycResponse.Item4;
        var response = await _quoteRepository.SaveLeadDetails(relianceCKYCCommand.InsurerId, relianceCKYCCommand.QuoteTransactionId, cKycResponse.Item1, cKycResponse.Item2, "POI", createLeadModelObject, cancellationToken);

        cKycResponse.Item3.LeadID = response.LeadID;
        cKycResponse.Item3.CKYCNumber = response.CKYCNumber;
        cKycResponse.Item3.KYCId = response.KYCId;

        if (cKycResponse != null)
            return cKycResponse.Item3;
        return default;
    }

    public async Task<SaveQuoteTransactionModel> CreateProposal(RelianceRequestDto proposalQuery, RelianceProposalRequest proposalRequest, CreateLeadModel createLeadModel, RelianceCKYCRequestModel ckycRequestModel, CancellationToken cancellationToken)
    {
        SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel();
        var response = await GetStateCityDistrictId(proposalRequest.AddressDetails.state, proposalRequest.AddressDetails.city, proposalRequest.AddressDetails.pincode);
        if (response != null)
        {
            proposalRequest.AddressDetails.state = response.StateId; // set as state id
            proposalRequest.AddressDetails.city = response.CityId; // set as city id
            proposalRequest.AddressDetails.communication_pincode = response.DistrictId; // set as district id
        }
        if (!string.IsNullOrEmpty(proposalQuery.PolicyDetails.PreviousInsuranceDetails.PrevInsuranceID))
        {
            var prevInsurer = await GetPreviousInsurer(proposalQuery.PolicyDetails.PreviousInsuranceDetails.PrevInsuranceID);
            proposalQuery.PolicyDetails.PreviousInsuranceDetails.PrevYearInsurer = prevInsurer.InsurerName;
        }
        var proposalResponse = await _relianceService.CreateProposal(proposalQuery, proposalRequest, createLeadModel, cancellationToken);
        decimal RecommendedIDV = 0;
        decimal MinIDV = 0;
        decimal MaxIDV = 0;
        if (proposalResponse != null)
        {
            saveQuoteTransactionModel = new SaveQuoteTransactionModel()
            {
                quoteResponseModel = proposalResponse.quoteResponseModel,
                RequestBody = proposalResponse.RequestBody,
                ResponseBody = proposalResponse.ResponseBody,
                Stage = (proposalResponse.quoteResponseModel.IsBreakIn || proposalResponse.quoteResponseModel.IsSelfInspection) ? "BreakIn" : "Proposal",
                InsurerId = _relianceConfig.InsurerId,
                LeadId = createLeadModel.LeadID,
                MaxIDV = MaxIDV,
                MinIDV = MinIDV,
                RecommendedIDV = RecommendedIDV,
                PolicyNumber = proposalResponse.quoteResponseModel.PolicyNumber,
                TransactionId = proposalResponse.quoteResponseModel.TransactionID
            };
        }
        return saveQuoteTransactionModel;
    }
    public async Task<RelianceUploadDocumentResponseModel> GetCKYCPOAResponse(string quoteTransactionId, string kycId, CancellationToken cancellationToken)
    {
        var ckycResponse = await _relianceService.GetCKYCPOAResponse(quoteTransactionId, kycId, cancellationToken);
        return ckycResponse;
    }

    public string PaymentURLGeneration(string paymentId, string amount, string quoteTransactionId, string panNumber, string proposalNumber, string ckycNumber, string productCode)
    {
        string Responseurl = $"{_relianceConfig.PGStatusRedirectionURL}{quoteTransactionId}/{_applicationClaims.GetUserId()}";
        string paymentLink = $"{_relianceConfig.PGSubmitPayment}{quoteTransactionId}/{_applicationClaims.GetUserId()}?ProposalNumber={proposalNumber}&Amt={amount}&Url={Responseurl}&Pan={panNumber}&ckyc={ckycNumber}&productCode={productCode}";
        return paymentLink;
    }

    public async Task<ReliancePaymentWrapperModel> GetPaymentDetails(string leadId, string policyNumber, string productCode, CancellationToken cancellationToken)
    {
        var policyDocumentDetails = await _relianceService.GetDocumentPDFBase64(leadId, policyNumber, _relianceConfig.PolicyDownloadURL, cancellationToken);
        return policyDocumentDetails;
    }

    public async Task<RelianceStateCityMasterModel> GetStateCityDistrictId(string state, string city, string pinCode)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("State", state, DbType.String, ParameterDirection.Input);
        parameters.Add("City", city, DbType.String, ParameterDirection.Input);
        parameters.Add("PinCode", pinCode, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<RelianceStateCityMasterModel>("[dbo].[Insurance_GetRelianceStateCityId]", parameters,
                     commandType: CommandType.StoredProcedure);
        return result.FirstOrDefault();
    }

    public async Task<ReliancePreviousInsurer> GetPreviousInsurer(string insurerId)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("InsurerId", insurerId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ReliancePreviousInsurer>("[dbo].[Insurance_GetReliancePreviousInsurer]", parameters,
                     commandType: CommandType.StoredProcedure);
        return result.FirstOrDefault();
    }
    public async Task<VariantAndRTOIdCheckModel> DoesRelianceVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, string vehicleTypeId, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("VariantId", variantId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", rtoId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", vehicleTypeId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_DoesRelianceVariantAndRTOExists]",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = result.Read<VariantAndRTOIdCheckModel>();
        return response.FirstOrDefault();
    }
}
