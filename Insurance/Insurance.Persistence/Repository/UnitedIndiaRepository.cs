using Dapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.UnitedIndia.Command;
using Insurance.Core.Features.UnitedIndia.Queries.GetCKYCPOAStatus;
using Insurance.Core.Features.UnitedIndia.Queries.GetQuote;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Oriental;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
using Insurance.Domain.UnitedIndia;
using Insurance.Domain.UnitedIndia.Payment;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Insurance.Persistence.ICIntegration.Implementation;
using Microsoft.Extensions.Options;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Transactions;

namespace Insurance.Persistence.Repository;

public class UnitedIndiaRepository : IUnitedIndiaRepository
{
    private readonly IUnitedIndiaService _unitedIndiaService;
    private readonly UnitedIndiaConfig _unitedIndiaConfig;
    private readonly IQuoteRepository _quoteRepository;
    private readonly ApplicationDBContext _applicationDBContext;
    private readonly InsurerIdConfig _insurerIdConfig;
    public UnitedIndiaRepository(IUnitedIndiaService unitedIndiaService,
        IOptions<UnitedIndiaConfig> unitedIndiaConfig,
        IQuoteRepository quoteRepository,
        ApplicationDBContext applicationDBContext,
        IOptions<InsurerIdConfig> insurerIdConfig)
    {
        _unitedIndiaService = unitedIndiaService;
        _unitedIndiaConfig = unitedIndiaConfig.Value;
        _quoteRepository = quoteRepository;
        _applicationDBContext = applicationDBContext;
        _insurerIdConfig = insurerIdConfig.Value;
    }
    public async Task<QuoteResponseModel> GetQuote(GetUnitedIndiaQuoteQuery query, CancellationToken cancellationToken)
    {
        var quoteQuery = await QuoteMasterMapping(query);

        var quoteResponse = await _unitedIndiaService.GetQuote(quoteQuery, cancellationToken);

        SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
        {
            quoteResponseModel = quoteResponse.Item1,
            RequestBody = quoteResponse.Item2,
            ResponseBody = quoteResponse.Item3,
            Stage = "Quote",
            InsurerId = _unitedIndiaConfig.InsurerId,
            LeadId = query.LeadId,
            MaxIDV = quoteQuery.MaxIDV,
            MinIDV = quoteQuery.MinIDV,
            RecommendedIDV = quoteQuery.IDVValue,
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
            VehicleNumber = query.IsVehicleNumberPresent ? query.VehicleNumber : quoteQuery.RegistrationRTOCode,
            IsPreviousYearClaim = query.PreviousPolicy.IsPreviousYearClaim,
            PreviousYearNCB = query.PreviousPolicy.NCBId,
            IsBrandNew = query.IsBrandNewVehicle,
            RTOId = query.RTOId,
        };
        await _quoteRepository.SaveQuoteTransaction(saveQuoteTransactionModel, cancellationToken);

        if (quoteResponse.Item1.InsurerName != null)
        {
            quoteResponse.Item1.InsurerId = _unitedIndiaConfig.InsurerId;
            return quoteResponse.Item1;
        }
        else
        {
            return new QuoteResponseModel
            {
                InsurerId = _unitedIndiaConfig.InsurerId,
                InsurerName = _unitedIndiaConfig.InsurerName,
                InsurerLogo = _unitedIndiaConfig.InsurerLogo,
                InsurerStatusCode = quoteResponse.Item1.InsurerStatusCode,
                ValidationMessage = quoteResponse?.Item1?.ValidationMessage
            };
        }
    }
    public Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        var res = _unitedIndiaService.QuoteConfirmDetails(quoteTransactionDbModel, quoteConfirmCommand, cancellationToken);
        return res;
    }
    private async Task<QuoteQueryModel> QuoteMasterMapping(GetUnitedIndiaQuoteQuery query)
    {
        var addOnId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnId} ")) : String.Empty;
        var discountId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList?.Select(x => $"{x.DiscountId} ")) : String.Empty;
        var paCoverId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverId} ")) : String.Empty;
        var accessoryId = (query.AccessoryList != null && query.AccessoryList.Any()) ? String.Join(",", query.AccessoryList?.Select(x => $"{x.AccessoryId} ")) : String.Empty;
        var discountExtensionId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList.Select(x => $"{x.DiscountExtensionId}")) : string.Empty;
        var paCoverExtensionId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverExtensionId} ")) : String.Empty;
        var addOnsExtensionId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnsExtensionId} ")) : String.Empty;

        var previousInsurer = query.PreviousPolicy != null && !string.IsNullOrEmpty(query.PreviousPolicy?.SAODInsurer) && query.PreviousPolicy?.SAODInsurer != "" ? query.PreviousPolicy.SAODInsurer : _insurerIdConfig.Bajaj;

        var age = GetVehicleAge(query.RegistrationYear);

        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("PACoverId", paCoverId, DbType.String, ParameterDirection.Input);
        parameters.Add("AccessoryId", accessoryId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddonId", addOnId, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountId", discountId, DbType.String, ParameterDirection.Input);
        parameters.Add("InsurerId", _unitedIndiaConfig.InsurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", query.RTOId, DbType.String, ParameterDirection.Input);
        parameters.Add("VariantId", query.VariantId, DbType.String, ParameterDirection.Input);
        parameters.Add("NCBId", query.PreviousPolicy?.NCBId, DbType.String, ParameterDirection.Input);
        parameters.Add("PolicyTypeId", query.PolicyDates.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", query.VehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", query.VehicleNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountExtensionId", discountExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("PACoverExtensionId", paCoverExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddOnsExtensionId", addOnsExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("SAODInsurerId", previousInsurer, DbType.String, ParameterDirection.Input);
        parameters.Add("SATPInsurerId", previousInsurer, DbType.String, ParameterDirection.Input);
        parameters.Add("IsBrandNew", query.IsBrandNewVehicle, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleAge", age.ToString(), DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetUnitedIndiaQuoteMasterMapping]", parameters,
                     commandType: CommandType.StoredProcedure);

        var paCoverList = result.Read<PACoverModel>();
        var accessoryList = result.Read<AccessoryModel>();
        var addOnList = result.Read<AddonsModel>();
        var discountList = result.Read<DiscountModel>();
        var discountExtensionList = result.Read<DiscountExtensionModel>();
        var paCoverExtensionList = result.Read<PACoverExtensionModel>();
        var addOnsExtensionList = result.Read<AddOnsExtensionModel>();
        var codeList = result.Read<RTOVehiclePreviousInsurerModel>();
        var quoteQuery = new QuoteQueryModel
        {
            AddOns = new Domain.GoDigit.AddOns(),
            PACover = new PACover(),
            Accessories = new Domain.GoDigit.Accessories(),
            Discounts = new Discounts(),
            VehicleDetails = new VehicleDetails(),
            PreviousPolicyDetails = new PreviousPolicy(),
        };

        string previousInsurerCode = string.Empty;
        string ncbValue = string.Empty;
        quoteQuery.LeadId = query.LeadId;

        if (discountExtensionList.Any())
        {
            quoteQuery.VoluntaryExcess = discountExtensionList.FirstOrDefault()?.DiscountExtension;
        }
        if (paCoverExtensionList.Any())
        {
            quoteQuery.UnnamedPassangerAndPillonRider = paCoverExtensionList.FirstOrDefault()?.PACoverExtension;
        }
        if (addOnsExtensionList.Any())
        {
            quoteQuery.GeogExtension = "NoExtn";
            var addOnsExtensionCheck = addOnsExtensionList?.FirstOrDefault().AddOnsExtension;
            if (addOnsExtensionCheck.ToLower().Equals("bangladesh") || addOnsExtensionCheck.ToLower().Equals("bhutan") || addOnsExtensionCheck.ToLower().Equals("nepal"))
            {
                quoteQuery.GeogExtension = addOnsExtensionCheck;
            }
        }

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
                else if (item.AddOns.Equals("Consumables"))
                {
                    quoteQuery.AddOns.ConsumableId = (addOnList?.Where(x => x.AddOns == "Consumables").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsConsumableRequired = true;
                }
                else if (item.AddOns.Equals("PersonalBelongings"))
                {
                    quoteQuery.AddOns.PersonalBelongingId = (addOnList?.Where(x => x.AddOns == "PersonalBelongings").Select(d => d.AddOnId).FirstOrDefault());
                    quoteQuery.AddOns.IsPersonalBelongingRequired = true;
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
            if (quoteQuery.AddOns.IsConsumableRequired || quoteQuery.AddOns.IsNCBRequired)
            {
                quoteQuery.AddOns.IsZeroDebt = true;
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
                if (item.CoverName.Equals("UnnamedPax"))
                {
                    quoteQuery.PACover.UnnamedPassengerId = (paCoverList?.Where(x => x.CoverName == "UnnamedPax").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedPassenger = true;
                    quoteQuery.PACover.UnnamedPassengerValue = Convert.ToInt32(paCoverExtensionList?.FirstOrDefault().PACoverExtension);
                }
                if (item.CoverName.Equals("UnnamedPillionRider"))
                {
                    quoteQuery.PACover.UnnamedPassengerId = (paCoverList?.Where(x => x.CoverName == "UnnamedPillionRider").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedPassenger = true;
                    quoteQuery.PACover.UnnamedPassengerValue = Convert.ToInt32(paCoverExtensionList?.FirstOrDefault().PACoverExtension);
                }
                else if (item.CoverName.Equals("UnnamedOWNERDRIVER"))
                {
                    quoteQuery.PACover.UnnamedOWNERDRIVERId = (paCoverList?.Where(x => x.CoverName == "UnnamedOWNERDRIVER").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsUnnamedOWNERDRIVER = true;
                }
                else if (item.CoverName.Equals("UnnamedPaidDriver"))
                {
                    quoteQuery.PACover.PaidDriverId = (paCoverList?.Where(x => x.CoverName == "UnnamedPaidDriver").Select(d => d.PACoverId).FirstOrDefault());
                    quoteQuery.PACover.IsPaidDriver = true;
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

        if (codeList.Any())
        {
            var codeData = codeList.FirstOrDefault();
            quoteQuery.VehicleDetails.VehicleModelCode = codeData?.VehicleModelCode;
            quoteQuery.VehicleDetails.VehicleMake = codeData?.VehicleMake;
            quoteQuery.VehicleDetails.VehicleModel = codeData?.VehicleModel;
            quoteQuery.VehicleDetails.VehicleCubicCapacity = codeData?.vehicleCubicCapacity;
            quoteQuery.VehicleDetails.VehicleSeatCapacity = codeData?.vehicleSeatCapacity;
            quoteQuery.VehicleDetails.VehicleVariant = codeData?.VehicleVariant;
            quoteQuery.VehicleDetails.Fuel = codeData?.Fuel;
            quoteQuery.VehicleDetails.Zone = codeData?.Zone;
            quoteQuery.VehicleDetails.VehicleType = codeData?.VehicleType;
            quoteQuery.VehicleDetails.VehicleSegment = codeData?.VehicleBodyType;
            quoteQuery.RTOStateName = codeData?.RTOStateName;
            quoteQuery.RTOLocationName = codeData?.RTOLocationName;
            quoteQuery.CityCode = codeData.CityCode;
            quoteQuery.RTOCityName = codeData.RTOCityName;
            quoteQuery.VehicleDetails.Zone = codeData.Zone;
            quoteQuery.CurrentPolicyType = codeData.CurrentPolicyType;
            quoteQuery.IsSAODMandatry = false;
            quoteQuery.IsSATPMandatory = false;
            quoteQuery.RTOLocationCode = codeData?.RTOCode;
            quoteQuery.VehicleNumber = query.VehicleNumber;
            quoteQuery.RegistrationRTOCode = codeData.RegistrationRTOCode;

            double idv = 0.0;
            if (query.IsBrandNewVehicle)
            {
                idv = codeData.ExShowRoomPrice * .95;
            }
            else
            {
                idv = Math.Round(codeData.ExShowRoomPrice * Convert.ToDouble(codeData.IDVPercentage));
            }
            var getIDVpercentageValue = Math.Round(idv * .05);
            quoteQuery.IDVValue = query.IDV switch
            {
                1 => Convert.ToDecimal(idv),
                2 => Convert.ToDecimal(Math.Round(idv + getIDVpercentageValue)),
                3 => Convert.ToDecimal(Math.Round(idv - getIDVpercentageValue)),
                > 3 => query.IDV,
            };
            quoteQuery.MaxIDV = Convert.ToDecimal(Math.Round(idv + getIDVpercentageValue));
            quoteQuery.MinIDV = Convert.ToDecimal(Math.Round(idv - getIDVpercentageValue));

        }
        if (query.PolicyDates.PreviousPolicyTypeId.Equals("20541BE3-D76E-4E73-9AB1-240CCB33DA5D") || query.PolicyDates.PreviousPolicyTypeId.Equals("517D8F9C-F532-4D45-8034-ABECE46693E3"))
        {
            quoteQuery.CurrentPolicyType = "PackagePolicy";
        }
        else if (query.PolicyDates.PreviousPolicyTypeId.Equals("48B01586-C66A-4A4A-AAFB-3F07F8A31896"))
        {
            quoteQuery.CurrentPolicyType = "StandAloneOD";
        }
        else
        {
            quoteQuery.CurrentPolicyType = "LiabilityOnly";
        }

        if (!query.IsBrandNewVehicle)
        {
            if (query.PreviousPolicy != null && query.PreviousPolicy.IsPreviousPolicy)
            {
                quoteQuery.PreviousPolicyDetails = new PreviousPolicy
                {
                    IsPreviousInsurerKnown = query.PreviousPolicy.IsPreviousPolicy,
                };
            }
            quoteQuery.PreviousPolicyDetails.PreviousInsurerCode = codeList?.FirstOrDefault().PreviousInsurerCode;
            quoteQuery.PreviousPolicyDetails.PreviousNoClaimBonus = codeList?.FirstOrDefault().NCBValue;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyType = codeList.FirstOrDefault().PreviousPolicyType;
            quoteQuery.PreviousPolicyDetails.PreviousSAODInsurer = codeList.FirstOrDefault().SAODInsurer;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = query.PolicyDates.ODPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd") : null;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = query.PolicyDates.ODPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd") : null;
            quoteQuery.PolicyTypeId = query.PolicyDates.PreviousPolicyTypeId;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyNumber = string.IsNullOrEmpty(query.PreviousPolicy.PreviousPolicyNumber) ? "TEST1234567890" : query.PreviousPolicy.PreviousPolicyNumber;

            if (quoteQuery.PreviousPolicyDetails.PreviousPolicyType.Equals("SAOD"))
            {
                quoteQuery.PreviousPolicyDetails.PreviousSATPInsurer = codeList.FirstOrDefault().SATPInsurer;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP = query.PolicyDates.TPPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP = query.PolicyDates.TPPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyNumberTP = string.IsNullOrEmpty(query.PreviousPolicy.PreviousPolicyNumberSATP) ? "TEST1234567890" : query.PreviousPolicy.PreviousPolicyNumberSATP;
                quoteQuery.IsSATPMandatory = true;
                quoteQuery.IsSAODMandatry = true;
            }
            else if (quoteQuery.PreviousPolicyDetails.PreviousPolicyType.Equals("SATP"))
            {
                quoteQuery.IsSATPMandatory = true;
            }
            else
            {
                quoteQuery.IsSAODMandatry = true;
            }
            quoteQuery.PreviousPolicyDetails.IsClaimInLastYear = query.PreviousPolicy.IsPreviousYearClaim;
        }

        quoteQuery.PolicyStartDate = query.PolicyDates.PolicyStartDate;
        quoteQuery.PolicyEndDate = query.PolicyDates.PolicyEndDate;
        quoteQuery.RegistrationDate = query.PolicyDates.RegistrationDate;
        quoteQuery.VehicleODTenure = query.PolicyDates.VehicleODTenure;
        quoteQuery.VehicleTPTenure = query.PolicyDates.VehicleTPTenure;
        quoteQuery.SelectedIDV = query.IDV;
        quoteQuery.RegistrationYear = query.RegistrationYear;
        quoteQuery.VehicleDetails.IsTwoWheeler = query.PolicyDates.IsTwoWheeler;
        quoteQuery.VehicleDetails.IsFourWheeler = query.PolicyDates.IsFourWheeler;
        quoteQuery.IsBrandNewVehicle = query.IsBrandNewVehicle;
        return quoteQuery;
    }
    public async Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> SaveCKYC(CKYCModel cKYCModel, UnitedProposalDynamicDetail unitedProposalDynamicDetail, CancellationToken cancellationToken)
    {
        var ckycResponse = await _unitedIndiaService.GetCKYCResponse(cKYCModel, unitedProposalDynamicDetail, cancellationToken);
        return ckycResponse;
    }
    public async Task<VariantAndRTOIdCheckModel> DoesUnitedIndiaVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("VariantId", variantId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", rtoId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_DoesUnitedIndiaVariantAndRTOExists]",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = result.Read<VariantAndRTOIdCheckModel>();
        return response.FirstOrDefault();
    }
    private static int GetVehicleAge(string registrationYear)
    {
        int year = (Convert.ToInt32(DateTime.Now.Year) - Convert.ToInt32(registrationYear));
        return year;
    }

    public async Task<PaymentstausResponse> GetPaymentStatus(InitiatePaymentRequestDto request, CancellationToken cancellationToken)
    {
        return await _unitedIndiaService.GetPaymentStatus(request, cancellationToken);
    }

    public async Task<string> InitiatePayment(InitiatePaymentRequestDto request, CancellationToken cancellationToken)
    {
        string PaymentURL = string.Empty;
        PaymentURL = await _unitedIndiaService.InitiatePayment(request, cancellationToken);
        return PaymentURL;
    }
    public async Task<TATARequestModelForPaymentVerify> GetLeadIdAndPaymentId(string QuoteTransactionId, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("QuoteTransactionId", QuoteTransactionId, DbType.String, ParameterDirection.Input);
        parameters.Add("InsurerId", _unitedIndiaConfig.InsurerId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetUnitedIndiaLeadIdAndPaymentId]",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = result.Read<TATARequestModelForPaymentVerify>();
        return response.FirstOrDefault();
    }
    public async Task<UnitedIndiaQuoteResponseDuringProposal> GetQuoteUpdate(QuoteTransactionRequest request, CreateLeadModel createLeadModel, UnitedProposalDynamicDetail unitedProposalDynamicDetail, CancellationToken cancellationToken)
    {
        var updatedQuoteResponse = await _unitedIndiaService.UpdateUserDetailsInQuotation(request, createLeadModel, unitedProposalDynamicDetail, cancellationToken);
        return updatedQuoteResponse;
    }
    public async Task<SaveQuoteTransactionModel> CreateProposal(string requestBody, string transactionId, string leadId, CancellationToken cancellationToken)
    {
        SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel();
        var proposalResponse = await _unitedIndiaService.CreateProposal(requestBody, transactionId, leadId, cancellationToken);

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
                Stage = "Proposal",
                InsurerId = _unitedIndiaConfig.InsurerId,
                LeadId = leadId,
                MaxIDV = MaxIDV,
                MinIDV = MinIDV,
                RecommendedIDV = RecommendedIDV,
                PolicyNumber = proposalResponse.quoteResponseModel?.ProposalNumber,
                TransactionId = proposalResponse.quoteResponseModel?.TransactionID,
                PolicyId = proposalResponse.quoteResponseModel?.PolicyId,
                TotalPremium = proposalResponse.quoteResponseModel.TotalPremium,
                GrossPremiume = proposalResponse.quoteResponseModel.GrossPremium,
                Tax = JsonConvert.SerializeObject(proposalResponse.quoteResponseModel.Tax),
                CustomerId = proposalResponse.quoteResponseModel.CustomerId
            };
        }
        return saveQuoteTransactionModel;
    }

    public async Task<UnitedIndiaPaymentInfoResponseEnvelope> GetPaymentInfo(UIICPaymentInfoHEADER requestDto, string LeadId, CancellationToken cancellationToken)
    {
        UnitedIndiaPaymentInfoResponseEnvelope PaymentstausResponseObj = new UnitedIndiaPaymentInfoResponseEnvelope();
        PaymentstausResponseObj = await _unitedIndiaService.GetPaymentInfo(requestDto, LeadId, cancellationToken);
        return PaymentstausResponseObj;
    }
    public async Task<IEnumerable<NameValueModel>> GetFinancierBranchDetails(string financierCode, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
       
        parameters.Add("financierCode", financierCode, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetUnitedIndiaFinancierDetails]", parameters,
                     commandType: CommandType.StoredProcedure);

        var financierDetails = result.Read<NameValueModel>();
        return financierDetails;
    }
    public async Task<CreateLeadModel> UpdateCKYCPOAStatus(GetUnitedIndiaCKYCPOAQuery getUnitedIndiaCKYCPOAQuery, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();

        parameters.Add("QuotetransactionId", getUnitedIndiaCKYCPOAQuery.QuoteTransactionId, DbType.String, ParameterDirection.Input);
        parameters.Add("CKYCStatus", getUnitedIndiaCKYCPOAQuery.CKYCSatus, DbType.String, ParameterDirection.Input);
        parameters.Add("OEMUniqId", getUnitedIndiaCKYCPOAQuery.TrannsactionId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_UpdateUIICCKYCPOAStatus]", parameters,
                     commandType: CommandType.StoredProcedure);

        var leadDetails = result.Read<CreateLeadModel>();
        return leadDetails.FirstOrDefault();
    }

}
