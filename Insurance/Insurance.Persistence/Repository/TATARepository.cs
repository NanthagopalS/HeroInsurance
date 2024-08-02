using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.TATA.Command.CKYC;
using Insurance.Core.Features.TATA.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Options;
using System.Data;
using System.Globalization;

namespace Insurance.Persistence.Repository;

public class TATARepository : ITATARepository
{
    private readonly ITATAService _tataService;
    private readonly ApplicationDBContext _applicationDBContext;
    private readonly TATAConfig _tataConfig;
    private readonly IdentityApplicationDBContext _identityApplicationDBContext;
    private readonly IApplicationClaims _applicationClaims;
    private readonly IQuoteRepository _quoteRepository;
    public TATARepository(ITATAService hdfcService,
                          ApplicationDBContext applicationDBContext,
                          IOptions<TATAConfig> options,
                          IdentityApplicationDBContext identityApplicationDBContext,
                          IApplicationClaims applicationClaims,
                          IQuoteRepository quoteRepository)
    {
        _tataService = hdfcService ?? throw new ArgumentNullException(nameof(hdfcService));
        _applicationDBContext = applicationDBContext ?? throw new ArgumentNullException(nameof(applicationDBContext));
        _tataConfig = options?.Value;
        _identityApplicationDBContext = identityApplicationDBContext ?? throw new ArgumentException(nameof(identityApplicationDBContext));
        _applicationClaims = applicationClaims;
        _quoteRepository = quoteRepository;
    }

    public async Task<QuoteResponseModel> GetQuote(GetTATAQuoteQuery request, CancellationToken cancellationToken)
    {
        var quoteQuery = await QuoteMasterMapping(request, cancellationToken);
        if (quoteQuery is not null)
        {
            var quoteResponse = await _tataService.GetQuote(quoteQuery, cancellationToken);

            SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
            {
                quoteResponseModel = quoteResponse.QuoteResponseModel,
                RequestBody = quoteResponse.RequestBody,
                ResponseBody = quoteResponse.ResponseBody,
                Stage = "Quote",
                InsurerId = _tataConfig.InsurerId,
                LeadId = request.LeadId,
                MaxIDV = Convert.ToDecimal(quoteResponse.QuoteResponseModel?.MaxIDV),
                MinIDV = Convert.ToDecimal(quoteResponse.QuoteResponseModel?.MinIDV),
                RecommendedIDV = Convert.ToDecimal(quoteResponse.QuoteResponseModel?.IDV),
                TransactionId = quoteResponse.QuoteResponseModel?.TransactionID,
                PolicyNumber = quoteResponse.QuoteResponseModel?.PolicyNumber,
                PolicyId = quoteResponse.QuoteResponseModel?.PolicyId,
                SelectedIDV = Convert.ToString(quoteResponse?.QuoteResponseModel?.SelectedIDV),
                PolicyTypeId = request.PreviousPolicy?.PreviousPolicyTypeId,
                IsPreviousPolicy = request.PreviousPolicy.IsPreviousPolicy,
                SAODInsurerId = request.PreviousPolicy?.SAODInsurer,
                SATPInsurerId = request.PreviousPolicy?.TPInsurer,
                SAODPolicyStartDate = Convert.ToDateTime(request.PolicyDates?.ODPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                SAODPolicyExpiryDate = Convert.ToDateTime(request.PolicyDates?.ODPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                SATPPolicyStartDate = Convert.ToDateTime(request.PolicyDates?.TPPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                SATPPolicyExpiryDate = Convert.ToDateTime(request.PolicyDates?.TPPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                RegistrationDate = Convert.ToDateTime(request.PolicyDates?.RegistrationDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                VehicleNumber = request.IsVehicleNumberPresent ? request.VehicleNumber : quoteQuery.VehicleDetails?.LicensePlateNumber,
                IsPreviousYearClaim = request.PreviousPolicy.IsPreviousYearClaim,
                PreviousYearNCB = request.PreviousPolicy.NCBId,
                IsBrandNew = request.IsBrandNewVehicle,
                RTOId = request.RTOId,
            };
            await _quoteRepository.SaveQuoteTransaction(saveQuoteTransactionModel, cancellationToken);

            if (quoteResponse.QuoteResponseModel.InsurerName != null)
            {
                quoteResponse.QuoteResponseModel.InsurerId = _tataConfig.InsurerId;
                return quoteResponse.QuoteResponseModel;
            }
        }
        return default;
    }
    private async Task<QuoteQueryModel> QuoteMasterMapping(GetTATAQuoteQuery query, CancellationToken cancellationToken)
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
        parameters.Add("InsurerId", _tataConfig.InsurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", query.RTOId, DbType.String, ParameterDirection.Input);
        parameters.Add("VariantId", query.VariantId, DbType.String, ParameterDirection.Input);
        parameters.Add("NCBId", query.PreviousPolicy?.NCBId, DbType.String, ParameterDirection.Input);
        parameters.Add("PolicyTypeId", query.PolicyDates.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", query.VehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("PACoverExtensionId", paCoverExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountExtensionId", discountExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddOnsExtensionId", addOnExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", query.VehicleNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("IsBrandNew", query.IsBrandNewVehicle, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("RegistrationYear", query.RegistrationYear, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadId", query.LeadId, DbType.String, ParameterDirection.Input);
        parameters.Add("IDV", query.IDV, DbType.String, ParameterDirection.Input);
        parameters.Add("RegistrationDate", Convert.ToDateTime(query.PolicyDates.RegistrationDate), DbType.Date, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetTATAQuoteMasterMapping]", parameters,
                     commandType: CommandType.StoredProcedure);

        var paCoverList = result.Read<PACoverModel>();
        var accessoryList = result.Read<AccessoryModel>();
        var discountList = result.Read<DiscountModel>();
        var paCoverExtensionList = result.Read<PACoverExtensionModel>();
        var discountExtensionList = result.Read<DiscountExtensionModel>();
        var addOnsExtensionList = result.Read<AddOnsExtensionModel>();
        var addOnList = result.Read<AddonsModel>();
        var codeList = result.Read<RTOVehiclePreviousInsurerModel>();
        var configNameValueList = result.Read<ConfigNameValueModel>();
        var packageModelResponse = result.Read<PackageModel>()?.FirstOrDefault();

        var quoteQuery = new QuoteQueryModel
        {
            AddOns = new Domain.GoDigit.AddOns(),
            Discounts = new Discounts(),
            PACover = new PACover(),
            Accessories = new Accessories(),
            GeoAreaCountries = new GeoAreaCountries(),
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
                    quoteQuery.PACover.UnnamedPassengerValue = Convert.ToInt32((paCoverExtensionList?.Select(d => d.PACoverExtension).FirstOrDefault()));
                }
                else if (item.CoverName.Equals("UnnamedPillionRider"))
                {
                    quoteQuery.PACover.UnnamedPillionRiderId = (paCoverList?.Where(x => x.CoverName == "UnnamedPillionRider").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedPillionRider = true;
                    quoteQuery.PACover.UnnamedPillonRiderValue = Convert.ToInt32((paCoverExtensionList?.Select(d => d.PACoverExtension).FirstOrDefault()));
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
                    quoteQuery.VoluntaryExcess = (discountExtensionList?.Select(d => d.DiscountExtension).FirstOrDefault());
                }
            }
        }
        if (addOnsExtensionList.Any())
        {
            quoteQuery.GeogExtension = String.Join(",", addOnsExtensionList.Select(x => $"{x.AddOnsExtension} ")).Trim();

            if (quoteQuery.GeogExtension.Equals("Bangladesh"))
            {
                quoteQuery.GeoAreaCountries.IsBangladesh = true;
            }
            else if (quoteQuery.GeogExtension.Equals("Bhutan"))
            {
                quoteQuery.GeoAreaCountries.IsBhutan = true;
            }
            else if (quoteQuery.GeogExtension.Equals("Maldives"))
            {
                quoteQuery.GeoAreaCountries.IsMaldives = true;
            }
            else if (quoteQuery.GeogExtension.Equals("Nepal"))
            {
                quoteQuery.GeoAreaCountries.IsNepal = true;
            }
            else if (quoteQuery.GeogExtension.Equals("Pakistan"))
            {
                quoteQuery.GeoAreaCountries.IsPakistan = true;
            }
            else if (quoteQuery.GeogExtension.Equals("Srilanka"))
            {
                quoteQuery.GeoAreaCountries.IsSrilanka = true;
            }
        }

        string previousInsurerCode = string.Empty;
        string previousPolicyNumber = string.Empty;
        string ncbValue = string.Empty;
        string previousPolicyType = string.Empty;
        string originalPreviousPolicyType = string.Empty;
        DateTime regDate = Convert.ToDateTime(query.PolicyDates.RegistrationDate);
        int vehicleAgeInDays = (DateTime.Now.Date - regDate.Date).Days;
        quoteQuery.IsSAODMandatry = false;
        quoteQuery.IsSATPMandatory = false;
        quoteQuery.AddOns.PackageName = string.Empty;
        quoteQuery.AddOns.PackageFlag = string.Empty;

        //Package Implementation
        if (query.PolicyDates.IsFourWheeler && (query.IsBrandNewVehicle || (!codeList.FirstOrDefault().CurrentPolicyTypeId.Equals("01"))))
        {
            quoteQuery.AddOns.PackageName = packageModelResponse?.PackageName.Trim();
            quoteQuery.AddOns.PackageFlag = packageModelResponse?.PackageFlag.Trim();
        }

        if (codeList.Any())
        {
            var codeData = codeList.FirstOrDefault();
            ncbValue = codeData.NCBValue;
            quoteQuery.RTOLocationCode = codeData.RTOLocationCode;
            quoteQuery.RTOLocationName = codeData.RTOLocationName;
            quoteQuery.VehicleDetails.VehicleMake = codeData.VehicleMake;
            quoteQuery.VehicleDetails.VehicleMakeCode = codeData.VehicleMakeCode;
            quoteQuery.VehicleDetails.VehicleModel = codeData.VehicleModel;
            quoteQuery.VehicleDetails.VehicleModelCode = codeData.VehicleModelCode;
            quoteQuery.VehicleDetails.VehicleVariant = codeData.VehicleVariant;
            quoteQuery.VehicleDetails.VehicleVariantCode = codeData.VehicleVariantCode;
            quoteQuery.VehicleDetails.Chassis = codeData.chassis;
            quoteQuery.VehicleDetails.EngineNumber = codeData.engine;
            quoteQuery.VehicleDetails.VehicleSeatCapacity = codeData.vehicleSeatCapacity;
            quoteQuery.VehicleDetails.VehicleSegment = codeData.VehicleSegment;
            quoteQuery.State_Id = codeData.State_Id;
            quoteQuery.VehicleDetails.Fuel = codeData.Fuel;
            quoteQuery.CurrentPolicyType = codeData.CurrentPolicyType;
            quoteQuery.CurrentPolicyTypeId = codeData.CurrentPolicyTypeId;
            quoteQuery.BusinessType = codeData.BusinessType;
            quoteQuery.BusinessTypeId = codeData.BusinessTypeId;
            previousPolicyType = codeData.PreviousPolicyType;
            quoteQuery.PlanType = codeData.PlanType;
            quoteQuery.GeogExtensionCode = codeData.ExtensionCountryCode;
            quoteQuery.GeogExtension = codeData.GeogExtension;
            originalPreviousPolicyType = codeData.OriginalPreviousPolicyType;
            previousPolicyNumber = codeData.PreviousInsurancePolicyNumber;
            previousInsurerCode = codeData.PreviousInsurerCode;
            quoteQuery.VehicleDetails.LicensePlateNumber = codeData.RTOCode;
            quoteQuery.SelectedIDV = query.IDV;
            quoteQuery.IDVValue = codeData.IDVValue;
            quoteQuery.MinIDV = codeData.MinIdv;
            quoteQuery.MaxIDV = codeData.MaxIdv;
            quoteQuery.RecommendedIDV = codeData.RecommendedIdv;
        }

        if (!query.IsBrandNewVehicle)
        {
            if (codeList.FirstOrDefault().CurrentPolicyTypeId.Equals("05"))
            {
                quoteQuery.IsSAODMandatry = true;
                quoteQuery.IsSATPMandatory = true;
            }
            else if (codeList.FirstOrDefault().CurrentPolicyTypeId.Equals("02"))
            {
                quoteQuery.IsSAODMandatry = true;
            }
            else
            {
                quoteQuery.IsSATPMandatory = true;
            }
            quoteQuery.PreviousPolicyDetails.IsClaimInLastYear = true;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyType = previousPolicyType;
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
            quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = query.PolicyDates.ODPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP = query.PolicyDates.TPPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = query.PolicyDates.ODPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP = query.PolicyDates.TPPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null;
        }

        quoteQuery.PolicyStartDate = Convert.ToDateTime(query.PolicyDates.PolicyStartDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        quoteQuery.PolicyEndDate = Convert.ToDateTime(query.PolicyDates.PolicyEndDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        quoteQuery.RegistrationDate = Convert.ToDateTime(query.PolicyDates.RegistrationDate).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

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
        quoteQuery.LeadId = query.LeadId;
        quoteQuery.VehicleDetails.IsFourWheeler = query.PolicyDates.IsFourWheeler;
        quoteQuery.VehicleDetails.IsTwoWheeler = query.PolicyDates.IsTwoWheeler;
        return quoteQuery;
    }
    public Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        var res = _tataService.QuoteConfirmDetails(quoteTransactionDbModel, quoteConfirmCommand, cancellationToken);
        return res;
    }
    public async Task<SaveQuoteTransactionModel> CreateProposal(QuoteTransactionRequest quoteDetails, QuoteConfirmDetailsModel quoteConfirmDetails, TATAProposalRequest proposalRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken)
    {
        SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel();
        var proposalResponse = await _tataService.CreateProposal(quoteDetails, quoteConfirmDetails, proposalRequest, createLeadModel, cancellationToken);

        decimal RecommendedIDV = 0;
        decimal MinIDV = 0;
        decimal MaxIDV = 0;

        if (proposalResponse != null && proposalResponse.quoteResponseModel != null)
        {
            saveQuoteTransactionModel = new SaveQuoteTransactionModel()
            {
                quoteResponseModel = proposalResponse.quoteResponseModel,
                RequestBody = proposalResponse.RequestBody,
                ResponseBody = proposalResponse.ResponseBody,
                Stage = (proposalResponse.quoteResponseModel.IsBreakIn || proposalResponse.quoteResponseModel.IsSelfInspection) ? "BreakIn" : "Proposal",
                InsurerId = _tataConfig.InsurerId,
                LeadId = createLeadModel.LeadID,
                MaxIDV = MaxIDV,
                MinIDV = MinIDV,
                RecommendedIDV = RecommendedIDV,
                PolicyNumber = proposalResponse.quoteResponseModel?.ProposalNumber,
                TransactionId = proposalResponse.quoteResponseModel?.TransactionID,
                PolicyId = proposalResponse.quoteResponseModel?.PolicyId,
            };
        }
        return saveQuoteTransactionModel;
    }
    public async Task<TATACKYCStatusResponseModel> PanCKYCVerification(TATACKYCRequestModel tataCKYCRequestModel, CancellationToken cancellationToken)
    {
        var verifyCKYC = await _tataService.PanCKYCVerification(tataCKYCRequestModel, cancellationToken);
        return verifyCKYC;
    }
    public async Task<TATACKYCStatusResponseModel> POACKYCVerification(TATACKYCRequestModel tataCKYCRequestModel, CancellationToken cancellationToken)
    {
        var verifyCKYC = await _tataService.POACKYCVerification(tataCKYCRequestModel, cancellationToken);
        return verifyCKYC;
    }
    public async Task<TATACKYCStatusResponseModel> POAAadharOTPSubmit(POAAadharOTPSubmitRequestModel poaAadharOTPRequest, CancellationToken cancellationToken)
	{
		var poaAadharOTPSubit = await _tataService.POAAadharOTPSubmit(poaAadharOTPRequest, cancellationToken);
		return poaAadharOTPSubit;
	}
	public async Task<TATACKYCStatusResponseModel> POADocumentUpdload(POADocumentUploadRequestModel poaDocumentUploadRequest, CancellationToken cancellationToken)
	{
		var res = await _tataService.POADocumentUpdload(poaDocumentUploadRequest, cancellationToken);
		return res;
	}
	public async Task<TATAPaymentResponseDataDto> GetPaymentLink(TATAPaymentRequestModel tATAPaymentRequestModel, CancellationToken cancellationToken)
    {
        var response = await _tataService.GetPaymentLink(tATAPaymentRequestModel, cancellationToken);
        return response;
    }
    public async Task<TATABreakInResponseModel> VerifyBreakIn(TATABreakInPaymentRequestModel tataBreakInRequestModel, CancellationToken cancellationToken)
    {
        var response = await _tataService.VerifyBreakIn(tataBreakInRequestModel, cancellationToken);
        return response;
	}
    public async Task<TATAVerifyPaymentStatusResponseDto> VerifyPaymentDetails(string vehicleTypeId, string paymentId, string leadId, CancellationToken cancellationToken)
    {
        var result = await _tataService.VerifyPaymentDetails(vehicleTypeId, paymentId, leadId, cancellationToken);
        return result;
    }
    public async Task<TATAPolicyDocumentResponseModel> GetPolicyDocument(string encriptedPolicyId, string vehicleTypeId, string leadId, CancellationToken cancellationToken)
    {
        var res = await _tataService.GetPolicyDocument(encriptedPolicyId, vehicleTypeId, leadId, cancellationToken);
        return res;
    }
    public async Task<VariantAndRTOIdCheckModel> DoesTATAVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("VariantId", variantId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", rtoId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_DoesTATAVariantAndRTOExists]",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = result.Read<VariantAndRTOIdCheckModel>();
        return response.FirstOrDefault();
    }
    public async Task<TATARequestModelForPaymentVerify> GetLeadIdAndPaymentId(string QuoteTransactionId, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("QuoteTransactionId", QuoteTransactionId, DbType.String, ParameterDirection.Input);
        parameters.Add("InsurerId", _tataConfig.InsurerId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetTATALeadIdAndPaymentId]",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = result.Read<TATARequestModelForPaymentVerify>();
        return response.FirstOrDefault();
    }
    public async Task<CreateLeadModel> GetDetailsForKYCAfterProposal(string quoteTransactionId, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("QuoteTransactionId", quoteTransactionId, DbType.String, ParameterDirection.Input);
        parameters.Add("InsurerId", _tataConfig.InsurerId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetDetailsForTATAKYCAfterProposal]",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = result.Read<CreateLeadModel>();
        return response.FirstOrDefault();
    }
    public async Task<TATAQuoteResponseDuringProposal> GetQuoteToAppendPincode(string vehicleTypeId, string requestBody, string pincode, string leadId, CancellationToken cancellationToken)
    {
        var res = await _tataService.GetQuoteToAppendPincode(vehicleTypeId, requestBody, pincode, leadId, cancellationToken);
        return res;
    }
}
