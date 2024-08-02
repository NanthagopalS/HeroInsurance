using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Chola.Command.CKYC;
using Insurance.Core.Features.Chola.Queries.GetBreakinStatus;
using Insurance.Core.Features.Chola.Queries.GetCKYCStatus;
using Insurance.Core.Features.Chola.Queries.GetPOAStatus;
using Insurance.Core.Features.Chola.Queries.GetQuote;
using Insurance.Domain;
using Insurance.Domain.Chola;
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
using System.Security.Cryptography;
using System.Text;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Models.SMS;
using Accessories = Insurance.Domain.GoDigit.Accessories;

namespace Insurance.Persistence.Repository;

public class CholaRepository : ICholaRepository
{
    private readonly ICholaService _cholaService;
    private readonly ApplicationDBContext _applicationDBContext;
    private readonly CholaConfig _cholaConfig;
    private readonly IMongoDBService _mongoDBService;
    private readonly IQuoteRepository _quoteRepository;
    private readonly IApplicationClaims _applicationClaims;
    private readonly ISmsService _smsService;
    public CholaRepository(ICholaService cholaService,
                           ApplicationDBContext applicationDBContext,
                           IOptions<CholaConfig> cholaConfig,
                           IGoDigitRepository goDigitRepository, IQuoteRepository quoteRepository, IMongoDBService mongoDBService, IApplicationClaims applicationClaims, ISmsService smsService)
    {
        _cholaService = cholaService ?? throw new ArgumentNullException(nameof(cholaService));
        _applicationDBContext = applicationDBContext ?? throw new ArgumentNullException(nameof(applicationDBContext));
        _cholaConfig = cholaConfig.Value;
        _quoteRepository = quoteRepository;
        _mongoDBService = mongoDBService ?? throw new ArgumentException(nameof(mongoDBService));
        _applicationClaims = applicationClaims;
        _smsService = smsService;
    }

    public async Task<QuoteResponseModel> GetQuote(GetCholaQuery query, CancellationToken cancellationToken)
    {
        var quoteQuery = await QuoteMasterMapping(query);

        if (quoteQuery != null && quoteQuery.Token != null)
        {
            var idvResponse = await _cholaService.GetIDV(quoteQuery, cancellationToken);
            if (idvResponse != null)
            {
                decimal MaxIDV = 0;
                decimal MinIDV = 0;
                decimal RecommendedIDV = 0;

                MaxIDV = Convert.ToDecimal(idvResponse.Data?.idv_4);
                MinIDV = Convert.ToDecimal(idvResponse.Data?.idv_1);
                RecommendedIDV = Convert.ToDecimal(idvResponse.Data?.idv_3);

                quoteQuery.IDVValue = query.IDV switch
                {
                    1 => RecommendedIDV,
                    2 => MinIDV,
                    3 => MaxIDV,
                    > 3 => query.IDV,
                    _ => MaxIDV,
                };

                var quoteResponse = await _cholaService.GetQuote(quoteQuery, cancellationToken);

                SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
                {
                    quoteResponseModel = quoteResponse.Item1,
                    RequestBody = quoteResponse.Item2,
                    ResponseBody = quoteResponse.Item3,
                    Stage = "Quote",
                    InsurerId = _cholaConfig.InsurerId,
                    LeadId = query.LeadId,
                    MaxIDV = quoteQuery.MaxIDV,
                    MinIDV = quoteQuery.MinIDV,
                    RecommendedIDV = quoteQuery.RecommendedIDV,
                    TransactionId = quoteResponse.Item1.ApplicationId,
                    PolicyNumber = string.Empty,
                    SelectedIDV = Convert.ToString(query.IDV),
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
                    quoteResponse.Item1.InsurerId = _cholaConfig.InsurerId;
                    return quoteResponse.Item1;
                }
            }
        }
        return default;
    }

    private async Task<QuoteQueryModel> QuoteMasterMapping(GetCholaQuery query)
    {
        var addOnId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnId} ")) : String.Empty;
        var discountId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList?.Select(x => $"{x.DiscountId} ")) : String.Empty;
        var paCoverId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverId} ")) : String.Empty;
        var accessoryId = (query.AccessoryList != null && query.AccessoryList.Any()) ? String.Join(",", query.AccessoryList?.Select(x => $"{x.AccessoryId} ")) : String.Empty;
        var paCoverExtensionId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverExtensionId} ")) : String.Empty;
        var discountExtensionId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList?.Select(x => $"{x.DiscountExtensionId} ")) : String.Empty;
        var addOnsExtensionId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnsExtensionId} ")) : String.Empty;

        var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("PACoverId", paCoverId, DbType.String, ParameterDirection.Input);
        parameters.Add("AccessoryId", accessoryId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddonId", addOnId, DbType.String, ParameterDirection.Input);
        parameters.Add("InsurerId", _cholaConfig.InsurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", query.RTOId, DbType.String, ParameterDirection.Input);
        parameters.Add("VariantId", query.VariantId, DbType.String, ParameterDirection.Input);
        parameters.Add("NCBId", query.PreviousPolicy?.NCBId, DbType.String, ParameterDirection.Input);
        parameters.Add("PolicyTypeId", query.PolicyDates?.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", query.VehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", query.VehicleNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountId", discountId, DbType.String, ParameterDirection.Input);
        parameters.Add("PACoverExtensionId", paCoverExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountExtensionId", discountExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddOnsExtensionId", addOnsExtensionId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetCholaQuoteMasterMapping]", parameters,
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
            Accessories = new Accessories(),
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
                else if (item.CoverName.Equals("UnnamedPax") && codeList?.FirstOrDefault()?.vehicleclass == "45")
                {
                    quoteQuery.PACover.UnnamedPassengerId = (paCoverList?.Where(x => x.CoverName == "UnnamedPax").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedPassenger = true;
                }
                else if (item.CoverName.Equals("UnnamedPillionRider") && codeList?.FirstOrDefault()?.vehicleclass == "37")
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
            foreach (var item in discountExtensionList)
            {
                quoteQuery.VoluntaryExcess = item.DiscountExtension;
            }
        }
        if (addOnsExtensionList.Any())
        {
            quoteQuery.GeogExtension = String.Join(",", addOnsExtensionList.Select(x => $"{x.AddOnsExtensionId} "));
        }
        if (paCoverExtensionList.Any())
        {
            quoteQuery.UnnamedPassangerAndPillonRider = paCoverExtensionList.FirstOrDefault()?.PACoverExtension;
        }

        string previousInsurerCode = string.Empty;
        string ncbValue = string.Empty;
        quoteQuery.LeadId = query.LeadId;
        quoteQuery.PolicyTypeId = query?.PolicyDates.PreviousPolicyTypeId;
        if (codeList.Any())
        {
            var codeData = codeList.FirstOrDefault();

            ncbValue = codeData.NCBValue;
            quoteQuery.VehicleDetails.VehicleMake = codeData.VehicleMake;
            quoteQuery.VehicleDetails.VehicleModel = codeData.VehicleModel;
            quoteQuery.VehicleDetails.VehicleModelCode = codeData.VehicleModelCode;
            quoteQuery.VehicleDetails.Fuel = codeData.Fuel;
            quoteQuery.VehicleDetails.VehicleCubicCapacity = codeData.vehicleCubicCapacity;
            quoteQuery.ExShowRoomPrice = Convert.ToDecimal(codeData.ExShowRoomPrice);
            quoteQuery.RTOLocationCode = codeData.RTOLocationCode;
            quoteQuery.RegistrationStateCode = codeData.RegistrationStateCode;
            quoteQuery.RegistrationRTOCode = codeData.RTOCode;//codeData.RegistrationRTOCode;
            quoteQuery.RTOCityName = codeData.RTOCityName;
            quoteQuery.RTOStateName = codeData.RTOStateName;
            quoteQuery.VehicleDetails.LicensePlateNumber = codeData.RTOCode;
            quoteQuery.VehicleDetails.VehicleClass = codeData.vehicleclass;
            quoteQuery.VehicleDetails.VehicleSeatCapacity = codeData.vehicleSeatCapacity;
            quoteQuery.PlanType = codeData.PlanType;
            quoteQuery.CurrentPolicyType = codeData.CurrentPolicyType;
            quoteQuery.PACover.IsUnnamedOWNERDRIVER = (codeData.CurrentPolicyType != "SAOD");
            quoteQuery.IsSAODMandatry = false;
            quoteQuery.IsSATPMandatory = false;
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
                    PreviousPolicyNumber = null,
                    IsClaimInLastYear = query.PreviousPolicy.IsPreviousYearClaim,
                    PreviousNoClaimBonus = ncbValue,
                };
            }
            if (codeList.FirstOrDefault().CurrentPolicyType.Equals("SAOD"))
            {
                quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP = query.PolicyDates.TPPolicyStartDate != null ? query.PolicyDates.TPPolicyStartDate : null;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP = query.PolicyDates.TPPolicyEndDate != null ? query.PolicyDates.TPPolicyEndDate : null;
                quoteQuery.IsSAODMandatry = true;
                quoteQuery.IsSATPMandatory = true;
            }
            else if (codeList.FirstOrDefault().CurrentPolicyType.Equals("SATP"))
            {
                quoteQuery.IsSATPMandatory = true;
            }
            else if (codeList.FirstOrDefault().CurrentPolicyType.Equals("Package Comprehensive"))
            {
                quoteQuery.IsSAODMandatry = true;
            }
            quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = query.PolicyDates.ODPolicyStartDate != null ? query.PolicyDates.ODPolicyStartDate : null;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = query.PolicyDates.ODPolicyEndDate != null ? query.PolicyDates.ODPolicyEndDate : null;
        }


        quoteQuery.PolicyStartDate = query.PolicyDates.PolicyStartDate;
        quoteQuery.PolicyEndDate = query.PolicyDates.PolicyEndDate;
        quoteQuery.RegistrationDate = query.PolicyDates.RegistrationDate;
        quoteQuery.VehicleODTenure = query.PolicyDates.VehicleODTenure;
        quoteQuery.VehicleTPTenure = query.PolicyDates.VehicleTPTenure;
        quoteQuery.VehicleTypeId = query.VehicleTypeId;
        quoteQuery.IsBrandNewVehicle = query.IsBrandNewVehicle;
        quoteQuery.VehicleNumber = query.VehicleNumber;

        var token = await _cholaService.GetToken(query.LeadId, "Quote");
        quoteQuery.Token = token.Token;
        quoteQuery.RegistrationYear = query.RegistrationYear;
        quoteQuery.SelectedIDV = query.IDV;
        quoteQuery.ConfigNameValueModels = configNameValueList;

        return quoteQuery;
    }
    public async Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        var res = await _cholaService.QuoteConfirmDetails(quoteTransactionDbModel, quoteConfirmCommand, cancellationToken);
        return res;
    }

    public async Task<CholaPaymentWrapperModel> GetPaymentDetails(CholaPaymentTaggingRequestModel model, CancellationToken cancellationToken)
    {
        var res = await _cholaService.GetPaymentDetails(model, cancellationToken);
        return res;
    }

    public async Task<string> UploadPolicyDocumentMongoDB(byte[] policyDocumentBase64)
    {
        MemoryStream stream = new MemoryStream(policyDocumentBase64);
        string documentId = await _mongoDBService.MongoUpload("Policy Document PDF", stream, policyDocumentBase64);
        if (documentId != null)
        {
            return documentId;
        }
        return default;
    }

    public async Task<string> DownloadPolicyDocumentMongoDB(string documentId, CancellationToken cancellationToken)
    {
        string policyDocumentPDF = await _mongoDBService.MongoDownload(documentId);
        if (policyDocumentPDF != null)
        {
            return policyDocumentPDF;
        }
        return default;
    }

    public async Task<SaveCKYCResponse> GetCKYCDetails(CholaCKYCCommand cholaCKYCCommand, CancellationToken cancellationToken)
    {
        var dbConnection = _applicationDBContext.CreateConnection();
        var dbParameters = new DynamicParameters();
        dbParameters.Add("QuoteTransactionId", cholaCKYCCommand.QuoteTransactionId);
        dbParameters.Add("InsurerId", cholaCKYCCommand.InsurerId);
        var result = await dbConnection.QueryAsync<CholaKYCRequiredData>("[dbo].[Insurance_GetCholaQuoteResponse]", dbParameters,
             commandType: CommandType.StoredProcedure);
        CholaResponseDto cholaResponseDto = Newtonsoft.Json.JsonConvert.DeserializeObject<CholaResponseDto>(result?.FirstOrDefault()?.ResponseBody);
        cholaCKYCCommand.TransactionId = cholaResponseDto.Data.quote_id;
        cholaCKYCCommand.LeadId = result?.FirstOrDefault()?.LeadId;

        var cKycResponse = await _cholaService.GetCKYCResponse(cholaCKYCCommand, cancellationToken);

        CreateLeadModel createLeadModelObject = cKycResponse.Item4;
        var response = await _quoteRepository.SaveLeadDetails(cholaCKYCCommand.InsurerId, cholaCKYCCommand.QuoteTransactionId, cKycResponse.Item1, cKycResponse.Item2, "POI", createLeadModelObject, cancellationToken);

        cKycResponse.Item3.LeadID = response.LeadID;
        cKycResponse.Item3.CKYCNumber = response.CKYCNumber;
        cKycResponse.Item3.KYCId = response.KYCId;

        if (cKycResponse != null)
            return cKycResponse.Item3;
        return default;
    }

    public async Task<CholaCKYCStatusReponseModel> GetCKYCStatus(GetCholaCKYCStatusQuery cholaCKYCStatusQuery, CancellationToken cancellationToken)
    {
        var cKycStatusResponse = await _cholaService.GetCKYCStatusResponse(cholaCKYCStatusQuery, cancellationToken);
        return cKycStatusResponse;
    }

    public async Task<SaveQuoteTransactionModel> CreateProposal(CholaServiceRequestModel proposalQuery, CholaProposalRequest proposalRequest, CreateLeadModel createLeadModel, CholaCKYCRequestModel ckycRequestModel, CancellationToken cancellationToken)
    {
        SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel();
        var proposalResponse = await _cholaService.CreateProposal(proposalQuery, proposalRequest, createLeadModel, ckycRequestModel, cancellationToken);

        decimal RecommendedIDV = 0;
        decimal MinIDV = 0;
        decimal MaxIDV = 0;

        saveQuoteTransactionModel = new SaveQuoteTransactionModel()
        {
            quoteResponseModel = proposalResponse.quoteResponseModel,
            RequestBody = proposalResponse.RequestBody,
            ResponseBody = proposalResponse.ResponseBody,
            Stage = (proposalResponse.quoteResponseModel.IsBreakIn || proposalResponse.quoteResponseModel.IsSelfInspection) ? "BreakIn" : "Proposal",
            InsurerId = _cholaConfig.InsurerId,
            LeadId = createLeadModel.LeadID,
            MaxIDV = MaxIDV,
            MinIDV = MinIDV,
            RecommendedIDV = RecommendedIDV,
            PolicyNumber = proposalResponse.quoteResponseModel?.ProposalNumber,
            TransactionId = proposalResponse.quoteResponseModel?.TransactionID
        };
        return saveQuoteTransactionModel;
    }

    public async Task<SaveQuoteTransactionModel> CreateBreakIn(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken)
    {
        QuoteResponseModel commonResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(quoteTransactionDbModel.QuoteTransactionRequest?.CommonResponse);
        CholaResponseDto quoteResponse = JsonConvert.DeserializeObject<CholaResponseDto>(quoteTransactionDbModel.QuoteTransactionRequest.ResponseBody);
        CreateLeadModel createLeadModel = (quoteTransactionDbModel?.LeadDetail);

        var breakinPinResponse = await _cholaService.CreateBreakIn(quoteTransactionDbModel, cancellationToken);
        if (breakinPinResponse != null)
        {
            commonResponse.ApplicationId = quoteResponse.Data.quote_id;
            commonResponse.BreakinId = breakinPinResponse.Item1;
            commonResponse.ProposalNumber = quoteResponse.Data.proposal_id;
            commonResponse.IsBreakIn = true;
            commonResponse.IsSelfInspection = true;
            commonResponse.ValidationMessage = breakinPinResponse.Item4;
            SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
            {
                quoteResponseModel = commonResponse,
                RequestBody = breakinPinResponse.Item2,
                ResponseBody = breakinPinResponse.Item3,
                Stage = "BreakIn",
                InsurerId = _cholaConfig.InsurerId,
                LeadId = createLeadModel.LeadID,
                MaxIDV = Convert.ToDecimal(commonResponse.MaxIDV),
                MinIDV = Convert.ToDecimal(commonResponse.MinIDV),
                RecommendedIDV = Convert.ToDecimal(commonResponse.IDV),
                TransactionId = commonResponse.TransactionID,
                PolicyNumber = string.Empty,
                BreakinInspectionURL = breakinPinResponse.Item5
            };
            return saveQuoteTransactionModel;
        }
        return default;
    }
    public string PaymentURLGeneration(string paymentId, string amount, string quoteTransactionId)
    {
        string url = _cholaConfig.PGStatusRedirectionURL + quoteTransactionId + "/" + _applicationClaims.GetUserId();
        string text = _cholaConfig.MerchantId + "|" + paymentId + "|NA|" + amount + "|NA|NA|NA|" +
            _cholaConfig.Currency + "|NA|" + _cholaConfig.Code1 + "|" + _cholaConfig.SecurityId + "|NA|NA|" + _cholaConfig.Code2 +
            "|NA|"+_cholaConfig.AdditionalInformation2+"|NA|NA|NA|NA|NA|" + url;

        byte[] message = Encoding.UTF8.GetBytes(text);
        byte[] hashValue;
        byte[] key = Encoding.UTF8.GetBytes(_cholaConfig.CheckSumKey);
        HMACSHA256 hashString = new HMACSHA256(key);
        string hex = "";
        hashValue = hashString.ComputeHash(message);
        foreach (byte x in hashValue)
        {
            hex += String.Format("{0:x2}", x);
        }
        return hex.ToUpper();
    }

    public async Task<CholaBreakInResponseModel> GetBreakInStatus(GetBreakinStatusQuery getBreakinStatusQuery, CancellationToken cancellationToken)
    {
        var getBreakInStatus = await _cholaService.GetBreakInStatus(getBreakinStatusQuery, cancellationToken);
        return getBreakInStatus;
    } 
    public async Task UpdatedIsBreakinApproved(string breakingId, bool isBreakinApproved, string stage, string inspectionDate, string inspectionAgency, CancellationToken cancellationToken)
    {

        var dbConnection = _applicationDBContext.CreateConnection();
        var dbParameters = new DynamicParameters();
        dbParameters.Add("IsBreakinApproved", isBreakinApproved);
        dbParameters.Add("BreakinId", breakingId);
        dbParameters.Add("Stage", stage);
        dbParameters.Add("InspectionDate", inspectionDate);
        dbParameters.Add("InspectionAgency", inspectionAgency);
        var result = await dbConnection.QueryAsync<string>("[dbo].[Insurance_UpdateCholaBreakinStatus]", dbParameters,
             commandType: CommandType.StoredProcedure);
    }
    public async Task<GetCholaCKYCStatusQuery> GetKycPOA(GetPOAStatusQuery getPOAStatusQuery, CancellationToken cancellationToken)
    {
        var dbConnection = _applicationDBContext.CreateConnection();
        var dbParameters = new DynamicParameters();
        dbParameters.Add("QuotetransactionId", getPOAStatusQuery.QuoteTransactionId);
        dbParameters.Add("InsurerId", _cholaConfig.InsurerId);
        var result = await dbConnection.QueryAsync<CholaPOAStatusRequestModel>("[dbo].[Insurance_GetCholaCkycPOAResponse]", dbParameters,
             commandType: CommandType.StoredProcedure);
        GetCholaCKYCStatusQuery cholaCKYCStatusQuery = new()
        {
            QuoteTransactionId = getPOAStatusQuery.QuoteTransactionId,
            AppRefNo = result.FirstOrDefault()?.AppRefNo,
            TransactionID = result.FirstOrDefault()?.TransactionId,
            LeadId = result.FirstOrDefault()?.LeadId
        };
        return (cholaCKYCStatusQuery);
    }
    public async Task<CreateLeadModel> GetBreakinDetails(string quoteTransactionId, CancellationToken cancellationToken)
    {
        var dbConnection = _applicationDBContext.CreateConnection();
        var dbParameters = new DynamicParameters();
        dbParameters.Add("QuotetransactionId", quoteTransactionId);
        var result = await dbConnection.QueryAsync<CreateLeadModel>("[dbo].[Insurance_GetCholaBreakinDetails]", dbParameters,
             commandType: CommandType.StoredProcedure);

        return result.FirstOrDefault();
    }
    public async Task<string> SendBreakinNotification(string breakinId, string mobileNumber, string inspectionURL, CancellationToken cancellationToken)
    {
        var response = await _smsService.SendCholaBreakinNotification(mobileNumber, inspectionURL, breakinId, _cholaConfig.InsurerName, cancellationToken);
        if (response != null && response.response.status.Equals("success"))
        {
            return "success";
        }
        return default;
    }
    public async Task<VariantAndRTOIdCheckModel> DoesCholaVariantAndRTOExists(string variantId, string rtoId, string policyTypeId, string vehicleNumber, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("VariantId", variantId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", rtoId, DbType.String, ParameterDirection.Input);
        parameters.Add("PolicyTypeId", policyTypeId, DbType.String, ParameterDirection.Input); 
        parameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_DoesCholaVariantAndRTOExists]",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = result.Read<VariantAndRTOIdCheckModel>();
        return response.FirstOrDefault();
    }
}
