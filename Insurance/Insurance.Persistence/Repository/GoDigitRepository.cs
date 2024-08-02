using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Command.CKYC;
using Insurance.Core.Features.GoDigit.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data;
using System.Globalization;
using CashlessGarage = Insurance.Domain.GoDigit.CashlessGarage;

namespace Insurance.Persistence.Repository;
public class GoDigitRepository : IGoDigitRepository
{
    private readonly IGodigitService _godigitService;
    private readonly ApplicationDBContext _applicationDBContext;
    private readonly GoDigitConfig _goDigitConfig;
    private readonly IQuoteRepository _quoteRepository;
    private readonly LogoConfig _logoConfig;
    private readonly IApplicationClaims _applicationClaims;

    public GoDigitRepository(IGodigitService godigitService, ApplicationDBContext applicationDBContext, IOptions<GoDigitConfig> options, IQuoteRepository quoteRepository, IOptions<LogoConfig> logoConfig, IApplicationClaims applicationClaims)
    {
        _godigitService = godigitService ?? throw new ArgumentNullException(nameof(godigitService));
        _applicationDBContext = applicationDBContext ?? throw new ArgumentNullException(nameof(applicationDBContext));
        _goDigitConfig = options?.Value;
        _quoteRepository = quoteRepository;
        _logoConfig = logoConfig.Value;
        _applicationClaims = applicationClaims;
    }

    public async Task<InputValidation> ValidationCheck(GetGoDigitQuery query, CancellationToken cancellationToken)
    {
        var addOnId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnId} ")) : String.Empty;
        var discountId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.DiscountList?.Select(x => $"{x.DiscountId} ")) : String.Empty;
        var paCoverId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverId} ")) : String.Empty;
        var accessoryId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AccessoryList?.Select(x => $"{x.AccessoryId} ")) : String.Empty;

        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("AddonId", addOnId, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountId", discountId, DbType.String, ParameterDirection.Input);
        parameters.Add("PAcoverId", paCoverId, DbType.String, ParameterDirection.Input);
        parameters.Add("AccessoryId", accessoryId, DbType.String, ParameterDirection.Input);
        parameters.Add("VarientId", query.VariantId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", query.VehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("PolicyTypeId", query.PreviousPolicy?.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", query.RTOId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", query.VehicleNumber, DbType.String, ParameterDirection.Input);


        var result = await connection.QueryAsync<InputValidation>("[dbo].[Insurance_GetQuoteValidation]", parameters,
                     commandType: CommandType.StoredProcedure);

        var validationResult = result.FirstOrDefault();
        return validationResult;
    }

    public async Task<QuoteResponseModel> GetQuote(GetGoDigitQuery query, CancellationToken cancellationToken)
    {
        var quoteQuery = await QuoteMasterMapping(query, cancellationToken);
        if (quoteQuery is not null)
        {
            var quoteResponse = await _godigitService.GetQuote(quoteQuery, cancellationToken);

            SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
            {
                quoteResponseModel = quoteResponse.Item1,
                RequestBody = quoteResponse.Item2,
                ResponseBody = quoteResponse.Item3,
                Stage = "Quote",
                InsurerId = _goDigitConfig.InsurerId,
                LeadId = query.LeadId,
                MaxIDV = Convert.ToDecimal(quoteResponse.Item1.MaxIDV),
                MinIDV = Convert.ToDecimal(quoteResponse.Item1.MinIDV),
                RecommendedIDV = Convert.ToDecimal(quoteResponse.Item1.IDV),
                TransactionId = quoteResponse.Item1.ApplicationId,
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

            if (quoteResponse.Item1.InsurerName != null)
            {
                quoteResponse.Item1.InsurerId = _goDigitConfig.InsurerId;
                return quoteResponse.Item1;
            }
        }
        return default;
    }

    public async Task QuoteTransaction(QuoteResponseModel quoteResponseModel, string requestBody, string responseModel, string stage, string insurerId, string leadId, decimal MaxIDV, decimal MinIDV, decimal RecommendedIDV, string transactionId)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("InsurerId", insurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("ResponseBody", responseModel, DbType.String, ParameterDirection.Input);
        parameters.Add("RequestBody", requestBody, DbType.String, ParameterDirection.Input);
        parameters.Add("CommonResponse", JsonConvert.SerializeObject(quoteResponseModel), DbType.String, ParameterDirection.Input);
        parameters.Add("Stage", stage, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadId", leadId, DbType.String, ParameterDirection.Input);
        parameters.Add("MaxIDV", MaxIDV, DbType.Decimal, ParameterDirection.Input);
        parameters.Add("MinIDV", MinIDV, DbType.Decimal, ParameterDirection.Input);
        parameters.Add("RecommendedIDV", RecommendedIDV, DbType.Decimal, ParameterDirection.Input);
        parameters.Add("TransactionId", transactionId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_InsertQuoteTransaction]", parameters,
                     commandType: CommandType.StoredProcedure);

        var insurerInfo = result.Read<InsurerInfo>();
        var cashlessList = result.Read<CashlessGarage>();
        var premiumBasicDetailList = result.Read<PremiumBasicDetails>();
        var PremiumBasicSubtitleDetailList = result.Read<PremiumBasicSubDetails>();
        var quoteransactionId = result.Read<string>();

        quoteResponseModel.InsurerLogo = _logoConfig.InsurerLogoURL + insurerInfo.FirstOrDefault().Logo;
        quoteResponseModel.SelfVideoClaim = insurerInfo.FirstOrDefault().SelfVideoClaims;
        quoteResponseModel.InsurerDescription = insurerInfo.FirstOrDefault().SelfDescription;
        quoteResponseModel.IsRecommended = insurerInfo.FirstOrDefault().IsRecommended;
        quoteResponseModel.RecommendedDescription = insurerInfo.FirstOrDefault().RecommendedDescription;
        quoteResponseModel.TransactionID = quoteransactionId.FirstOrDefault();

        foreach (var item in premiumBasicDetailList)
        {
            var subtitleModel = PremiumBasicSubtitleDetailList
                .Where(x => x.PremiumBasicDetailId.Equals(item.PremiumBasicDetailsId))
                .Select(d => new PremiumBasicSubDetails
                {
                    PremiumBasicDetailId = d.PremiumBasicDetailId,
                    SubtitleId = d.SubtitleId,
                    Subtitle = d.Subtitle,
                    Description = d.Description,
                    Icon = d.Icon
                }).ToList();
            item.SubDetailsList = subtitleModel;
        }
        quoteResponseModel.CashlessGarageList = cashlessList;
        quoteResponseModel.PremiumBasicDetailsList = premiumBasicDetailList;
        quoteResponseModel.CachlessGarageCount = cashlessList.Count();
    }

    private async Task<QuoteQueryModel> QuoteMasterMapping(GetGoDigitQuery query, CancellationToken cancellationToken)
    {
        var addOnId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnId} ")) : String.Empty;
        var discountId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList?.Select(x => $"{x.DiscountId} ")) : String.Empty;
        var paCoverId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverId} ")) : String.Empty;
        var accessoryId = (query.AccessoryList != null && query.AccessoryList.Any()) ? String.Join(",", query.AccessoryList?.Select(x => $"{x.AccessoryId} ")) : String.Empty;
        var paCoverExtensionId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverExtensionId} ")) : String.Empty;
        var discountExtensionId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList?.Select(x => $"{x.DiscountExtensionId} ")) : String.Empty;
        var addOnsExtensionId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnsExtensionId} ")) : String.Empty;

        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("PACoverId", paCoverId, DbType.String, ParameterDirection.Input);
        parameters.Add("AccessoryId", accessoryId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddonId", addOnId, DbType.String, ParameterDirection.Input);
        parameters.Add("InsurerId", _goDigitConfig.InsurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", query.RTOId, DbType.String, ParameterDirection.Input);
        parameters.Add("VariantId", query.VariantId, DbType.String, ParameterDirection.Input);
        parameters.Add("NCBId", query.PreviousPolicy?.NCBId, DbType.String, ParameterDirection.Input);
        parameters.Add("PolicyTypeId", query.PolicyDates.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", query.VehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", query.VehicleNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountId", discountId, DbType.String, ParameterDirection.Input);
        parameters.Add("PACoverExtensionId", paCoverExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountExtensionId", discountExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddOnsExtensionId", addOnsExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadId", query.LeadId, DbType.String, ParameterDirection.Input);
        parameters.Add("IDV", query.IDV, DbType.Int32, ParameterDirection.Input);
        parameters.Add("IsBrandNew", query.IsBrandNewVehicle, DbType.String, ParameterDirection.Input);
        parameters.Add("PrevSAODInsurer", query.PreviousPolicy.SAODInsurer, DbType.String, ParameterDirection.Input);
        parameters.Add("PrevTPInsurer", query.PreviousPolicy.TPInsurer, DbType.String, ParameterDirection.Input);
        parameters.Add("RegistrationYear", query.RegistrationYear, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetQuoteMasterMapping]", parameters,
                     commandType: CommandType.StoredProcedure);

        var paCoverList = result.Read<PACoverModel>();
        var accessoryList = result.Read<AccessoryModel>();
        var addOnList = result.Read<AddonsModel>();
        var discountList = result.Read<DiscountModel>();
        var codeList = result.Read<RTOVehiclePreviousInsurerModel>();
        var configNameValueList = result.Read<ConfigNameValueModel>();
        var paCoverExtensionList = result.Read<PACoverExtensionModel>();
        var discountExtensionList = result.Read<DiscountExtensionModel>();
        var addOnsExtensionList = result.Read<AddOnsExtensionModel>();

        var quoteQuery = new QuoteQueryModel
        {
            AddOns = new Domain.GoDigit.AddOns(),
            PACover = new PACover(),
            Accessories = new Domain.GoDigit.Accessories(),
            Discounts = new Discounts(),
            VehicleDetails = new VehicleDetails(),
            PreviousPolicyDetails = new PreviousPolicy()
        };
        if (addOnList.Any())
        {
            foreach (var item in addOnList)
            {
                if (item.AddOns.Equals("PartsDepreciation"))
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
                else if (item.AddOns.Equals("ReturnToInvoice"))
                {
                    quoteQuery.AddOns.ReturnToInvoiceIdId = (addOnList?.Where(x => x.AddOns == "ReturnToInvoice").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsInvoiceCoverRequired = true;
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
                    quoteQuery.PACover.IsUnnamedPassenger = true;
                }
            }
        }
        if (discountList.Any())
        {
            foreach (var item in discountList)
            {
                if (item.DiscountName.Equals("AAMembership"))
                {
                    quoteQuery.Discounts.AAMemberShipId = (paCoverList?.Where(x => x.CoverName == "AAMembership").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.Discounts.IsAAMemberShip = true;
                }
                else if (item.DiscountName.Equals("ARAIApprovedAntiTheftDevice"))
                {
                    quoteQuery.Discounts.AntiTheftId = (paCoverList?.Where(x => x.CoverName == "ARAIApprovedAntiTheftDevice").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.Discounts.IsAntiTheft = true;
                }
                else if (item.DiscountName.Equals("LimitedThirdPartyCoverage"))
                {
                    quoteQuery.Discounts.LimitedTPCoverageId = (paCoverList?.Where(x => x.CoverName == "LimitedThirdPartyCoverage").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.Discounts.IsLimitedTPCoverage = true;
                }
                else if (item.DiscountName.Equals("VoluntaryDeductible"))
                {
                    quoteQuery.Discounts.VoluntarilyDeductibleId = (paCoverList?.Where(x => x.CoverName == "VoluntaryDeductible").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.Discounts.IsVoluntarilyDeductible = true;
                }
            }
        }
        if (discountExtensionList.Any())
        {
            foreach (var item in discountExtensionList)
            {
                quoteQuery.VoluntaryExcess = item.DiscountValueInWords;
            }
        }
        if (addOnsExtensionList.Any())
        {
            quoteQuery.GeogExtension = String.Join(",", addOnsExtensionList.Select(x => $"{x.AddOnsExtension} "));
        }
        if (paCoverExtensionList.Any())
        {
            quoteQuery.PACover.UnnamedPassengerValue = Convert.ToInt32((paCoverExtensionList?.Select(d => d.PACoverExtension).FirstOrDefault()));
        }

        string previousInsurerCode = string.Empty;
        string originalPreviousPolicyType = string.Empty;
        string previousInsurancePolicyNumber = string.Empty;
        string ncbValue = string.Empty;
        quoteQuery.IsSAODMandatry = false;
        quoteQuery.IsSATPMandatory = false;
        quoteQuery.LeadId = query.LeadId;
        if (codeList.Any())
        {
            var codeData = codeList.FirstOrDefault();
            previousInsurerCode = codeData.PreviousInsurerCode;
            originalPreviousPolicyType = codeData.OriginalPreviousPolicyType;
            previousInsurancePolicyNumber = codeData.PreviousInsurancePolicyNumber;
            quoteQuery.VehicleDetails.VehicleMaincode = codeData.VehicleCode;
            quoteQuery.VehicleDetails.LicensePlateNumber = !string.IsNullOrEmpty(query.VehicleNumber) ? query.VehicleNumber : codeData.RTOCode;
            quoteQuery.VehicleDetails.Chassis = codeData.chassis;
            quoteQuery.VehicleDetails.EngineNumber = codeData.engine;
            quoteQuery.CurrentPolicyType = codeData.CurrentPolicyType;
            quoteQuery.PlanType = codeData.PlanType;
            quoteQuery.SelectedIDV = query.IDV;
            quoteQuery.IDVValue = codeData.IDVValue;
            quoteQuery.MinIDV = codeData.MinIdv;
            quoteQuery.MaxIDV = codeData.MaxIdv;
            quoteQuery.RecommendedIDV = codeData.RecommendedIdv;
            ncbValue = codeData.NCBValue;
        }
        if (!query.IsBrandNewVehicle)
        {
            quoteQuery.PreviousPolicyDetails.IsClaimInLastYear = true;
            if (query.PreviousPolicy != null && query.PreviousPolicy.IsPreviousPolicy)
            {
                quoteQuery.PreviousPolicyDetails = new PreviousPolicy
                {
                    IsPreviousInsurerKnown = query.PreviousPolicy.IsPreviousPolicy,
                    PreviousInsurerCode = previousInsurerCode,
                    PreviousPolicyNumber = previousInsurancePolicyNumber,
                    IsClaimInLastYear = query.PreviousPolicy.IsPreviousYearClaim,
                    PreviousNoClaimBonus = ncbValue,
                };
            }
            if (codeList.FirstOrDefault().CurrentPolicyType.Equals("SAOD"))
            {
                quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = query.PolicyDates.ODPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = query.PolicyDates.ODPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP = query.PolicyDates.TPPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP = query.PolicyDates.TPPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.IsSAODMandatry = true;
                quoteQuery.IsSATPMandatory = true;
            }
            else if (codeList.FirstOrDefault().CurrentPolicyType.Equals("SATP"))
            {
                quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP = query.PolicyDates.TPPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP = query.PolicyDates.TPPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.IsSATPMandatory = true;
            }
            else if (codeList.FirstOrDefault().CurrentPolicyType.Equals("Package Comprehensive"))
            {
                quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = query.PolicyDates.ODPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = query.PolicyDates.ODPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.IsSAODMandatry = true;
            }
            quoteQuery.PreviousPolicyDetails.OriginalPreviousPolicyType = originalPreviousPolicyType;
        }
        quoteQuery.PolicyStartDate = Convert.ToDateTime(query.PolicyDates.PolicyStartDate).ToString("yyyy-MM-dd");
        quoteQuery.PolicyEndDate = Convert.ToDateTime(query.PolicyDates.PolicyEndDate).ToString("yyyy-MM-dd");
        quoteQuery.RegistrationDate = Convert.ToDateTime(query.PolicyDates.RegistrationDate).ToString("yyyy-MM-dd");
        quoteQuery.VehicleTPTenure = query.PolicyDates.VehicleTPTenure;
        quoteQuery.VehicleODTenure = query.PolicyDates.VehicleODTenure;
        quoteQuery.IsBrandNewVehicle = query.IsBrandNewVehicle;
        quoteQuery.RegistrationYear = query.RegistrationYear;
        quoteQuery.VehicleDetails.IsFourWheeler = query.PolicyDates.IsFourWheeler;
        quoteQuery.VehicleDetails.IsTwoWheeler = query.PolicyDates.IsTwoWheeler;
        quoteQuery.ConfigNameValueModels = configNameValueList;
        quoteQuery.VehicleNumber = query.VehicleNumber;

        return quoteQuery;
    }

    public async Task<SaveQuoteTransactionModel> CreateProposal(ProposalRequestModel proposalRequestModel, CancellationToken cancellationToken)
    {

        var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(proposalRequestModel.QuoteTransactionID, cancellationToken);
        if (_quotedetails != null)
        {
            GoDigitProposalDto _goDigitProposal = JsonConvert.DeserializeObject<GoDigitProposalDto>(_quotedetails.QuoteTransactionRequest?.RequestBody);
            CreateLeadModel _leadDetails = (_quotedetails.LeadDetail);
            ProposalRequest _proposalRequest = JsonConvert.DeserializeObject<ProposalRequest>(_quotedetails.ProposalRequestBody);
            GodigitCKYCRequest _godigitCKYCRequest = JsonConvert.DeserializeObject<GodigitCKYCRequest>(_quotedetails.CKYCRequestBody);

            if (_goDigitProposal != null && _proposalRequest != null && _godigitCKYCRequest != null)
            {
                var proposalResponse = await _godigitService.CreateProposal(_goDigitProposal, _proposalRequest, _godigitCKYCRequest, _leadDetails, cancellationToken);

                decimal RecommendedIDV = 0;
                decimal MinIDV = 0;
                decimal MaxIDV = 0;

                SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
                {
                    quoteResponseModel = proposalResponse.Item1,
                    RequestBody = proposalResponse.Item2,
                    ResponseBody = proposalResponse.Item3,
                    Stage = (proposalResponse.Item1.IsBreakIn || proposalResponse.Item1.IsSelfInspection) ? "BreakIn" : "Proposal",
                    InsurerId = _goDigitConfig.InsurerId,
                    LeadId = _leadDetails.LeadID,
                    MaxIDV = MaxIDV,
                    MinIDV = MinIDV,
                    RecommendedIDV = RecommendedIDV,
                    QuoteTransactionId = proposalRequestModel.QuoteTransactionID,
                    TransactionId = proposalRequestModel.TransactionId,
                    PolicyNumber = proposalResponse.Item1.ProposalNumber
                };

                if (proposalResponse.Item1.InsurerName != null)
                {
                    proposalResponse.Item1.InsurerId = _goDigitConfig.InsurerId;
                    return saveQuoteTransactionModel;
                }
            }
        }
        return default;
    }

    public async Task<GoDigitPaymentURLResponseDto> CreatePaymentLink(string leadId, string applicationId, string cancelReturnUrl, string successReturnUrl, CancellationToken cancellationToken)
    {
        var paymentURLResponse = await _godigitService.GetPaymentLink(leadId, applicationId, cancelReturnUrl, successReturnUrl, cancellationToken);

        if (paymentURLResponse != null)
        {
            return paymentURLResponse;
        }
        return default;
    }

    public async Task<PaymentCKCYResponseModel> GetPaymentDetails(GodigitPaymentCKYCReqModel godigitPaymentCKYCReqModel, CancellationToken cancellationToken)
    {
        var paymentResponse = await _godigitService.GetPaymentDetails(godigitPaymentCKYCReqModel, cancellationToken);
        return paymentResponse;
    }

    public async Task<PaymentCKCYResponseModel> GetCKYCDetails(GodigitPaymentCKYCReqModel godigitPaymentCKYCReqModel, CancellationToken cancellationToken)
    {
        var ckycResponse = await _godigitService.GetCKYCDetails(godigitPaymentCKYCReqModel, cancellationToken);
        return ckycResponse;
    }
    public async Task<SaveCKYCResponse> SaveCKYC(GoDigitCKYCCommand goDigitCKYCCommand, CancellationToken cancellationToken)
    {
        var ckycResponse = _godigitService.GetCKYCResponse(goDigitCKYCCommand, cancellationToken);

        CreateLeadModel createLeadModelObject = ckycResponse.Item4;
        var response = await _quoteRepository.SaveLeadDetails(goDigitCKYCCommand.InsurerId, goDigitCKYCCommand.QuoteTransactionId, ckycResponse.Item1, ckycResponse.Item2, "POI", createLeadModelObject, cancellationToken);

        ckycResponse.Item3.LeadID = response.LeadID;
        ckycResponse.Item3.CKYCNumber = response.CKYCNumber;
        ckycResponse.Item3.KYCId = response.KYCId;

        return ckycResponse.Item3;
    }

    public async Task<GodigitPolicyDocumentResponseDto> GetPolicyDocumentPDF(string leadId, string applicationId, CancellationToken cancellationToken)
    {
        var policyDocumentResponse = await _godigitService.GetPolicyDocumentPDF(leadId, applicationId, cancellationToken);
        return policyDocumentResponse;
    }

    public async Task<byte[]> GetDocumentPDFBase64(string documentLink, CancellationToken cancellationToken)
    {
        var policyDocumentBase64 = await _godigitService.GetDocumentPDFBase64(documentLink, cancellationToken);
        return policyDocumentBase64;
    }

    public Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        var res = _godigitService.QuoteConfirmDetails(quoteTransactionDbModel, quoteConfirmCommand, cancellationToken);
        return res;
    }

    public async Task<QuoteResponseModel> GetPolicyStatus(string leadId, string policyNumber, CancellationToken cancellationToken)
    {
        var response = await _godigitService.GetPolicyStatus(leadId, policyNumber, cancellationToken);
        return response;
    }
    public async Task<GoDigitPaymentURLResponseDto> GetPaymentLink(string leadId, string applicationId, CancellationToken cancellationToken)
    {
        var getPaymentLinkResponse = await _godigitService.GetPaymentLink(leadId, applicationId, $"{_goDigitConfig.PGRedirectionURL}{applicationId}/{_applicationClaims.GetUserId()}", $"{_goDigitConfig.PGRedirectionURL}{applicationId}/{_applicationClaims.GetUserId()}", cancellationToken);
        return getPaymentLinkResponse;
    }
    public async Task<VariantAndRTOIdCheckModel> DoesGoDigitVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("VariantId", variantId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", rtoId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_DoesGoDigitVariantAndRTOExists]",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = result.Read<VariantAndRTOIdCheckModel>();
        return response.FirstOrDefault();
    }
}

