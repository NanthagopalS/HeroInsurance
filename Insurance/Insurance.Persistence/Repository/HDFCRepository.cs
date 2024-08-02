using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.HDFC.Command.CKYC;
using Insurance.Core.Features.HDFC.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.ICICI;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using ThirdPartyUtilities.Abstraction;

namespace Insurance.Persistence.Repository;
public class HDFCRepository : IHDFCRepository
{
    private readonly IHDFCService _hdfcService;
    private readonly ApplicationDBContext _applicationDBContext;
    private readonly HDFCConfig _hdfcConfig;
    private readonly IdentityApplicationDBContext _identityApplicationDBContext;
    private readonly IApplicationClaims _applicationClaims;
    private readonly IQuoteRepository _quoteRepository;
    public HDFCRepository(IHDFCService hdfcService,
                          ApplicationDBContext applicationDBContext,
                          IOptions<HDFCConfig> options,
                          IdentityApplicationDBContext identityApplicationDBContext, 
                          IApplicationClaims applicationClaims,
                          IQuoteRepository quoteRepository)
    {
        _hdfcService = hdfcService ?? throw new ArgumentNullException(nameof(hdfcService));
        _applicationDBContext = applicationDBContext ?? throw new ArgumentNullException(nameof(applicationDBContext));
        _hdfcConfig = options?.Value;
        _identityApplicationDBContext = identityApplicationDBContext ?? throw new ArgumentException(nameof(identityApplicationDBContext));
        _applicationClaims = applicationClaims;
        _quoteRepository = quoteRepository;
    }

    public async Task<QuoteResponseModel> GetQuote(GetHdfcQuoteQuery request, CancellationToken cancellationToken)
    {
        var quoteQuery = await QuoteMasterMapping(request, cancellationToken);

        if (quoteQuery != null)
        {

            var idvDetails = await _hdfcService.GetIDV(quoteQuery, cancellationToken);

            decimal MaxIDV = 0;
            decimal MinIDV = 0;
            decimal RecommendedIDV = 0;
            if (idvDetails.CalculatedIDV != null)
            {
                MaxIDV = Convert.ToDecimal(idvDetails.CalculatedIDV.MAX_IDV_AMOUNT);
                MinIDV = Convert.ToDecimal(idvDetails.CalculatedIDV.MIN_IDV_AMOUNT);
                RecommendedIDV = Convert.ToDecimal(idvDetails.CalculatedIDV.IDV_AMOUNT);
            }
            quoteQuery.IDVValue = request.IDV switch
            {
                1 => RecommendedIDV,
                2 => MinIDV,
                3 => MaxIDV,
                > 3 => request.IDV,
                _ => MaxIDV,
            };

            var quoteResponse = await _hdfcService.GetQuote(quoteQuery, cancellationToken);

            SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
            {
                quoteResponseModel = quoteResponse.Item1,
                RequestBody = quoteResponse.Item2,
                ResponseBody = quoteResponse.Item3,
                Stage = "Quote",
                InsurerId = _hdfcConfig.InsurerId,
                LeadId = request.LeadId,
                MaxIDV = MaxIDV,
                MinIDV = MinIDV,
                RecommendedIDV = RecommendedIDV,
                TransactionId = quoteResponse.Item1.ApplicationId,
                PolicyNumber = string.Empty,
                SelectedIDV = Convert.ToString(request.IDV),
                PolicyTypeId = request.PreviousPolicy.PreviousPolicyTypeId,
                IsPreviousPolicy = request.PreviousPolicy.IsPreviousPolicy,
                SAODInsurerId = request.PreviousPolicy.SAODInsurer,
                SATPInsurerId = request.PreviousPolicy.TPInsurer,
                SAODPolicyStartDate = Convert.ToDateTime(request.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                SAODPolicyExpiryDate = Convert.ToDateTime(request.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                SATPPolicyStartDate = Convert.ToDateTime(request.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                SATPPolicyExpiryDate = Convert.ToDateTime(request.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                RegistrationDate = Convert.ToDateTime(request.PolicyDates.RegistrationDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                VehicleNumber = request.IsVehicleNumberPresent ? request.VehicleNumber : quoteQuery.VehicleDetails.LicensePlateNumber,
                IsPreviousYearClaim = request.PreviousPolicy.IsPreviousYearClaim,
                PreviousYearNCB = request.PreviousPolicy.NCBId,
                IsBrandNew = request.IsBrandNewVehicle,
                RTOId = request.RTOId,
            };

            await _quoteRepository.SaveQuoteTransaction(saveQuoteTransactionModel, cancellationToken);

            if (quoteResponse.Item1.InsurerName != null)
            {
                quoteResponse.Item1.InsurerId = _hdfcConfig.InsurerId;
                return quoteResponse.Item1;
            }
        }
        return default;
    }
    private async Task<QuoteQueryModel> QuoteMasterMapping(GetHdfcQuoteQuery query, CancellationToken cancellationToken)
    {
        var addOnId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList.Select(x => $"{x.AddOnId} ")) : string.Empty;
        var paCoverId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList.Select(x => $"{x.PACoverId} ")) : string.Empty;
        var accessoryId = (query.AccessoryList != null && query.AccessoryList.Any()) ? String.Join(",", query.AccessoryList.Select(x => $"{x.AccessoryId} ")) : string.Empty;
        var discountId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList.Select(x => $"{x.DiscountId}")) : string.Empty;
        var paCoverExtensionId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList.Select(x => $"{x.PACoverExtensionId}")) : string.Empty;
        var discountExtensionId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList.Select(x => $"{x.DiscountExtensionId}")) : string.Empty;
        var addOnExtensionId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList.Select(x => $"{x.AddOnsExtensionId}")) : string.Empty;

        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("PACoverId", paCoverId, DbType.String, ParameterDirection.Input);
        parameters.Add("AccessoryId", accessoryId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddonId", addOnId, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountId", discountId, DbType.String, ParameterDirection.Input);
        parameters.Add("InsurerId", _hdfcConfig.InsurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", query.RTOId, DbType.String, ParameterDirection.Input);
        parameters.Add("VariantId", query.VariantId, DbType.String, ParameterDirection.Input);
        parameters.Add("NCBId", query.PreviousPolicy?.NCBId, DbType.String, ParameterDirection.Input);
        parameters.Add("PolicyTypeId", query.PolicyDates.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", query.VehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("PACoverExtensionId", paCoverExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountExtensionId", discountExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddOnsExtensionId", addOnExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", query.VehicleNumber, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetHDFCQuoteMasterMapping]", parameters,
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
                else if (item.AddOns.Equals("IMT23"))
                {
                    quoteQuery.AddOns.IMT23Id = (addOnList?.Where(x => x.AddOns == "IMT23").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsIMT23 = true;
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
                else if (item.CoverName.Equals("UnnamedCleaner"))
                {
                    quoteQuery.PACover.UnnamedCleanerId = (paCoverList?.Where(x => x.CoverName == "UnnamedCleaner").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedCleaner = true;
                }
                else if (item.CoverName.Equals("UnnamedConductor"))
                {
                    quoteQuery.PACover.UnnamedConductorId = (paCoverList?.Where(x => x.CoverName == "UnnamedConductor").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedConductor = true;
                }
                else if (item.CoverName.Equals("UnnamedHelper"))
                {
                    quoteQuery.PACover.UnnamedHelperId = (paCoverList?.Where(x => x.CoverName == "UnnamedHelper").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedHelper = true;
                }
                else if (item.CoverName.Equals("UnnamedHirer"))
                {
                    quoteQuery.PACover.UnnamedHirerId = (paCoverList?.Where(x => x.CoverName == "UnnamedHirer").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedHirer = true;
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

        string previousInsurerCode = string.Empty;
        string previousPolicyNumber = string.Empty;
        string ncbValue = string.Empty;
        string previousPolicyType = string.Empty;
        string originalPreviousPolicyType = string.Empty;
        quoteQuery.IsSAODMandatry = false;
        quoteQuery.IsSATPMandatory = false;

        if (codeList.Any())
        {
            var codeData = codeList.FirstOrDefault();
            ncbValue = codeData.NCBValue;
            quoteQuery.RTOLocationCode = codeData.RTOLocationCode;
            quoteQuery.VehicleDetails.VehicleModelCode = codeData.VehicleModelCode;
            quoteQuery.VehicleDetails.Chassis = codeData.chassis;
            quoteQuery.VehicleDetails.EngineNumber = codeData.engine;
            quoteQuery.VehicleDetails.VehicleSeatCapacity = codeData.vehicleSeatCapacity;
            quoteQuery.State_Id = codeData.State_Id;
            quoteQuery.VehicleDetails.Fuel = codeData.Fuel;
            quoteQuery.VehicleDetails.RegNo = codeData.RTOCode;
            quoteQuery.CurrentPolicyType = codeData.CurrentPolicyType;
            previousPolicyType = codeData.PreviousPolicyType;
            quoteQuery.PlanType = codeData.PlanType;
            quoteQuery.GeogExtensionCode = codeData.ExtensionCountryCode;
            quoteQuery.GeogExtension = codeData.GeogExtension;
            originalPreviousPolicyType = codeData.OriginalPreviousPolicyType;
            previousPolicyNumber = codeData.PreviousInsurancePolicyNumber;
            previousInsurerCode = codeData.PreviousInsurerCode;
            quoteQuery.VehicleDetails.LicensePlateNumber = codeData.RTOCode;
        }

        if (!query.IsBrandNewVehicle)
        {
            if (codeList.FirstOrDefault().CurrentPolicyType.Equals("OD Only"))
            {
                quoteQuery.IsSAODMandatry = true;
                quoteQuery.IsSATPMandatory = true;
            }
            else if (codeList.FirstOrDefault().CurrentPolicyType.Equals("OD Plus TP"))
            {
                quoteQuery.IsSAODMandatry = true;
            }
            else
            {
                quoteQuery.IsSATPMandatory = true;
            }
            quoteQuery.PreviousPolicyDetails.IsClaimInLastYear = true;
            if (query.PreviousPolicy != null && query.PreviousPolicy.IsPreviousPolicy)
            {
                quoteQuery.PreviousPolicyDetails = new PreviousPolicy
                {
                    IsPreviousInsurerKnown = query.PreviousPolicy.IsPreviousPolicy,
                    PreviousPolicyNumber = previousPolicyNumber,
                    PreviousInsurerCode = previousInsurerCode,
                    PreviousNoClaimBonus = ncbValue,
                    IsClaimInLastYear = query.PreviousPolicy.IsPreviousYearClaim,
                    PreviousPolicyType = previousPolicyType
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
        quoteQuery.SelectedIDV = query.IDV;
        quoteQuery.RegistrationYear = query.RegistrationYear;
        quoteQuery.IsBrandNewVehicle = query.IsBrandNewVehicle;
        quoteQuery.ConfigNameValueModels = configNameValueList;
        quoteQuery.VehicleTypeId = query.VehicleTypeId;
        quoteQuery.PolicyTypeId = query.PolicyDates?.PreviousPolicyTypeId;
        quoteQuery.PreviousPolicyDetails.OriginalPreviousPolicyType = originalPreviousPolicyType;
        quoteQuery.VehicleNumber = query.VehicleNumber;

        var tokenAndTransactionId = await _hdfcService.GetToken(quoteQuery.VehicleTypeId, quoteQuery.PolicyTypeId, "Quote", query.CategoryId, query.LeadId, cancellationToken);
        quoteQuery.Token = tokenAndTransactionId.Token;
        quoteQuery.TransactionId = tokenAndTransactionId.TransactionId;
        quoteQuery.ProductCode = tokenAndTransactionId.ProductCode;
        quoteQuery.LeadId = query.LeadId;
        quoteQuery.CategoryId = query.CategoryId;

        return quoteQuery;
    }
    public async Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> SaveCKYC(HDFCCKYCCommand hdfcCKYCCommand, CancellationToken cancellationToken)
    {
        var ckycResponse = await _hdfcService.GetCKYCResponse(hdfcCKYCCommand, cancellationToken);
        return ckycResponse;
    }
    public async Task<HDFCCkycPOAStatusResponseModel> GetCKYCPOAStatus(string kycId, string leadId, CancellationToken cancellationToken)
    {
        var kycRes = await _hdfcService.GetCKYCPOAStatus(kycId,leadId, cancellationToken);
        return kycRes;
    }
    public Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        var res = _hdfcService.QuoteConfirmDetails(quoteTransactionDbModel, quoteConfirmCommand, cancellationToken);
        return res;
    }
    public async Task<HDFCUploadDocumentResponseModel> GetCKYCPOAResponse(string quoteTransactionId, string kycId, string leadId, CancellationToken cancellationToken)
    {
        var ckycResponse = await _hdfcService.GetCKYCPOAResponse(quoteTransactionId, kycId, leadId, cancellationToken);
        return ckycResponse;
    }
    public async Task<SaveQuoteTransactionModel> CreateProposal(HDFCServiceRequestModel proposalQuery, HDFCProposalRequest proposalRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken)
    {
        SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel();
        var proposalResponse = await _hdfcService.CreateProposal(proposalQuery, proposalRequest, createLeadModel, cancellationToken);

        decimal RecommendedIDV = 0;
        decimal MinIDV = 0;
        decimal MaxIDV = 0;

        if(proposalResponse != null)
        {
            saveQuoteTransactionModel = new SaveQuoteTransactionModel()
            {
                quoteResponseModel = proposalResponse.quoteResponseModel,
                RequestBody = proposalResponse.RequestBody,
                ResponseBody = proposalResponse.ResponseBody,
                Stage = (proposalResponse.quoteResponseModel.IsBreakIn || proposalResponse.quoteResponseModel.IsSelfInspection) ? "BreakIn" : "Proposal",
                InsurerId = _hdfcConfig.InsurerId,
                LeadId = createLeadModel.LeadID,
                MaxIDV = MaxIDV,
                MinIDV = MinIDV,
                RecommendedIDV = RecommendedIDV,
                PolicyNumber = proposalResponse.quoteResponseModel.ProposalNumber,
                TransactionId = proposalResponse.quoteResponseModel.TransactionID
            };
        }
        
        return saveQuoteTransactionModel;
    }
    public async Task<bool> CreatePOSP(HDFCCreateIMBrokerRequestDto model, CancellationToken cancellationToken)
    {
        bool pospID = false;
        if (model != null)
        {
            var submitPOSPResponse = await _hdfcService.CreatePOSP(model, cancellationToken);
            if (submitPOSPResponse != null && submitPOSPResponse.Resp_POSP != null && submitPOSPResponse.Resp_POSP.POSP_ID != null)
            {
                pospID = await InsertPOSP(model.PospId, submitPOSPResponse.Resp_POSP.POSP_ID, cancellationToken);
            }
        }
        return pospID;
    }
    public async Task<bool> InsertPOSP(string POSPId, string HDFCPospId, CancellationToken cancellationToken)
    {
        using var connection = _identityApplicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("POSPId", POSPId, DbType.String, ParameterDirection.Input);
        parameters.Add("HDFCPospId", HDFCPospId, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Identity_InsertPOSP]", parameters, commandType: CommandType.StoredProcedure);
        return result > 0;
    }
    public async Task<string> GeneratePaymentCheckSum(string transactionId, string amount, string redirectionURL,string leadId, CancellationToken cancellationToken)
    {
        string checkSumId = await _hdfcService.GeneratePaymentCheckSum(transactionId, amount, redirectionURL,leadId, cancellationToken);
        return checkSumId;
    }
    public async Task<HDFCPolicyRequestModel> GetPaymentFields(string applicationId, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("ApplicationId", applicationId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetHDFCPaymentFields]", parameters,
                     commandType: CommandType.StoredProcedure);

        HDFCPolicyRequestModel dbRes = result.Read<HDFCPolicyRequestModel>()?.FirstOrDefault();
        return dbRes;
    }
    public async Task<HDFCPolicyDocumentResponseModel> GetPolicyDocument(HDFCPolicyRequestModel hdfcPolicyReqModel, CancellationToken cancellationToken)
    {
        var resultFields = await GetPaymentFields(hdfcPolicyReqModel.ApplicationId, cancellationToken);
        if (resultFields != null)
        {
            resultFields.BankName= hdfcPolicyReqModel.BankName;
            resultFields.PaymentDate= hdfcPolicyReqModel.PaymentDate;
            resultFields.GrossPremium = hdfcPolicyReqModel.GrossPremium;
            resultFields.ApplicationId= hdfcPolicyReqModel.ApplicationId;
            var result = await _hdfcService.GetPolicyDocument(resultFields, cancellationToken);
            return result;
        }
        return default;
    }
    public async Task<string> GetPaymentLink(BreakInPaymentDetailsDBModel breakInPaymentDetailsDBModel, string QuoteTransactionId,  CancellationToken cancellationToken)
    {
        var hdfcProposalResponse = JsonConvert.DeserializeObject<HDFCServiceResponseModel>(breakInPaymentDetailsDBModel.ProposalResponse);
        string checkSumId = await _hdfcService.GeneratePaymentCheckSum(breakInPaymentDetailsDBModel.ApplicationId, breakInPaymentDetailsDBModel.GrossPremium, $"{_hdfcConfig.PGStatusRedirectionURL}{QuoteTransactionId}/{_applicationClaims.GetUserId()}", breakInPaymentDetailsDBModel.LeadId, cancellationToken);
        if (checkSumId != null)
        {
            string paymentLink = $"{_hdfcConfig.PGSubmitPayment}{QuoteTransactionId}/{_applicationClaims.GetUserId()}?Trnsno={breakInPaymentDetailsDBModel.ApplicationId}&Amt={breakInPaymentDetailsDBModel.GrossPremium}&Chksum={checkSumId}";
            return paymentLink;
        }
        return default;
    }
    public async Task<VariantAndRTOIdCheckModel> DoesHDFCVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, string vehicleTypeId, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("VariantId", variantId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", rtoId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", vehicleTypeId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_DoesHDFCVariantAndRTOExists]",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = result.Read<VariantAndRTOIdCheckModel>();
        return response.FirstOrDefault();
    }
}