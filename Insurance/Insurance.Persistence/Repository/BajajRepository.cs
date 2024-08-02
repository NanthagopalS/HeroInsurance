using Dapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Bajaj.Command.CKYC;
using Insurance.Core.Features.Bajaj.Command.UploadCKYCDocument;
using Insurance.Core.Features.Bajaj.Queries.GetQuote;
using Insurance.Domain.Bajaj;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;

namespace Insurance.Persistence.Repository;
public class BajajRepository : IBajajRepository
{
    private readonly IBajajService _bajajService;
    private readonly ApplicationDBContext _applicationDBContext;
    private readonly BajajConfig _bajajConfig;
    private readonly IQuoteRepository _quoteRepository;

    public BajajRepository(IBajajService bajajService,
                           ApplicationDBContext applicationDBContext,
                           IOptions<BajajConfig> options,
                           IQuoteRepository quoteRepository)
    {
        _bajajService = bajajService ?? throw new ArgumentNullException(nameof(bajajService));
        _applicationDBContext = applicationDBContext ?? throw new ArgumentNullException(nameof(applicationDBContext));
        _bajajConfig = options?.Value;
        _quoteRepository = quoteRepository;
    }

    public async Task<QuoteResponseModel> GetQuote(GetBajajQuoteQuery request, CancellationToken cancellationToken)
    {
        var quoteQuery = await QuoteMasterMapping(request, cancellationToken);

        if (quoteQuery is not null)
        {
            // Call Quote API
            var quoteResponse = await _bajajService.GetQuote(quoteQuery, cancellationToken);

            SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
            {
                quoteResponseModel = quoteResponse.Item1,
                RequestBody = quoteResponse.Item2,
                ResponseBody = quoteResponse.Item3,
                Stage = "Quote",
                InsurerId = _bajajConfig.InsurerId,
                LeadId = request.LeadId,
                MaxIDV = Convert.ToDecimal(quoteResponse.Item1.MaxIDV),
                MinIDV = Convert.ToDecimal(quoteResponse.Item1.MinIDV),
                RecommendedIDV = Convert.ToDecimal(quoteResponse.Item1.IDV),
                TransactionId = quoteResponse.Item1.ApplicationId,
                PolicyNumber = string.Empty,
                SelectedIDV = (quoteQuery.IsBrandNewVehicle && quoteQuery.IDVValue == 0) ? "1" : Convert.ToString(request.IDV),
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
                quoteResponse.Item1.InsurerId = _bajajConfig.InsurerId;
                return quoteResponse.Item1;
            }
        }
        return default;
    }
    private async Task<QuoteQueryModel> QuoteMasterMapping(GetBajajQuoteQuery query, CancellationToken cancellationToken)
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
        parameters.Add("InsurerId", _bajajConfig.InsurerId, DbType.String, ParameterDirection.Input);
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

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetBajajQuoteMasterMapping]", parameters,
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
        var packageName = result.Read<PackageName>();


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
        if (addOnsExtensionList.Any())
        {
            quoteQuery.GeogExtension = String.Join(",", addOnsExtensionList.Select(x => $"{x.AddOnsExtension} "));
        }
        if (paCoverExtensionList.Any())
        {
            quoteQuery.PACover.UnnamedPassengerValue = Convert.ToInt32((paCoverExtensionList?.Select(d => d.PACoverExtension).FirstOrDefault()));
        }
        if (packageName.Any())
        {
            quoteQuery.AddOns.PackageName = (Convert.ToString(packageName.Select(d => d.Package_Name)?.FirstOrDefault()));
        }

        string previousInsurerCode = string.Empty;
        string previousPolicyNumber = string.Empty;
        string ncbValue = string.Empty;
        quoteQuery.LeadId = query.LeadId;
        if (codeList.Any())
        {
            var codeData = codeList.FirstOrDefault();
            previousInsurerCode = codeData.PreviousInsurerCode;
            ncbValue = codeData.NCBValue;
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
            quoteQuery.VehicleDetails.Fuel = codeData.Fuel;
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
            quoteQuery.RTOLocationCode = codeData.RTOCode;
            quoteQuery.CurrentPolicyType = codeData.CurrentPolicyType;
            previousPolicyNumber = codeData.PreviousInsurancePolicyNumber;
            quoteQuery.SelectedIDV = query.IDV;
            quoteQuery.IDVValue = codeData.IDVValue;
            quoteQuery.MinIDV = codeData.MinIdv;
            quoteQuery.MaxIDV = codeData.MaxIdv;
            quoteQuery.RecommendedIDV = codeData.RecommendedIdv;
            quoteQuery.IsSAODMandatry = false;
            quoteQuery.IsSATPMandatory = false;
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
                    PreviousPolicyNumber = previousPolicyNumber,
                    PreviousInsurerCode = previousInsurerCode,
                    PreviousNoClaimBonus = ncbValue,
                    IsClaimInLastYear = query.PreviousPolicy.IsPreviousYearClaim,
                };
            }

            quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = query.PolicyDates.ODPolicyStartDate != null ? query.PolicyDates.ODPolicyStartDate : null;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP = query.PolicyDates.TPPolicyStartDate != null ? query.PolicyDates.TPPolicyStartDate : null;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = query.PolicyDates.ODPolicyEndDate != null ? query.PolicyDates.ODPolicyEndDate : null;
            quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP = query.PolicyDates.TPPolicyEndDate != null ? query.PolicyDates.TPPolicyEndDate : null;
        }

        quoteQuery.PolicyStartDate = query.PolicyDates.PolicyStartDate;
        quoteQuery.PolicyEndDate = query.PolicyDates.PolicyEndDate;
        quoteQuery.RegistrationDate = query.PolicyDates.RegistrationDate;
        quoteQuery.VehicleODTenure = query.PolicyDates.VehicleODTenure;
        quoteQuery.VehicleTPTenure = query.PolicyDates.VehicleTPTenure;
        quoteQuery.IsBrandNewVehicle = query.IsBrandNewVehicle;
        quoteQuery.RegistrationYear = query.RegistrationYear;
        quoteQuery.ConfigNameValueModels = configNameValueList;
        quoteQuery.VehicleTypeId = query.VehicleTypeId;
        quoteQuery.VehicleDetails.IsTwoWheeler = query.PolicyDates.IsTwoWheeler;
        quoteQuery.VehicleDetails.IsFourWheeler = query.PolicyDates.IsFourWheeler;
        return quoteQuery;
    }
    public async Task<SaveQuoteTransactionModel> CreateProposal(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken)
    {
        if (quoteTransactionDbModel != null)
        {
            var proposalResponse = await _bajajService.CreateProposal(quoteTransactionDbModel, cancellationToken);
            return proposalResponse;
        }
        return default;
    }
    public async Task<SaveCKYCResponse> SaveCKYC(BajajCKYCCommand bajajCKYCCommand, CancellationToken cancellationToken)
    {
        var dbConnection = _applicationDBContext.CreateConnection();
        var dbParameters = new DynamicParameters();
        dbParameters.Add("QuoteTransactionId", bajajCKYCCommand.QuoteTransactionId);
        var result = await dbConnection.QueryAsync<BajajCKYCDetailsModel>("[dbo].[Insurance_GetCKYCDataMapping]", dbParameters,
             commandType: CommandType.StoredProcedure);

        bajajCKYCCommand.TransactionId = result?.FirstOrDefault()?.TransactionId;
        bajajCKYCCommand.LeadId = result?.FirstOrDefault()?.LeadId;

        var ckycResponse = await _bajajService.GetCKYCResponse(bajajCKYCCommand, cancellationToken);

        CreateLeadModel createLeadModelObject = ckycResponse.Item4;
        var response = await _quoteRepository.SaveLeadDetails(bajajCKYCCommand.InsurerId, bajajCKYCCommand.QuoteTransactionId, ckycResponse.Item1, ckycResponse.Item2, "POI", createLeadModelObject, cancellationToken);

        ckycResponse.Item3.LeadID = response.LeadID;
        ckycResponse.Item3.CKYCNumber = response.CKYCNumber;
        ckycResponse.Item3.KYCId = response.KYCId;

        if (ckycResponse != null)
        {
            return ckycResponse.Item3;
        }
        return default;
    }

    public async Task<UploadCKYCDocumentResponse> UploadCKYCDocument(UploadBajajCKYCDocumentCommand uploadBajajCKYC, CancellationToken cancellationToken)
    {
        var dbConnection = _applicationDBContext.CreateConnection();
        var dbParameters = new DynamicParameters();
        dbParameters.Add("QuoteTransactionId", uploadBajajCKYC.QuoteTransactionId);
        var result = await dbConnection.QueryAsync<BajajCKYCDetailsModel>("[dbo].[Insurance_GetCKYCDataMapping]", dbParameters,
             commandType: CommandType.StoredProcedure);

        uploadBajajCKYC.TransactionId = result?.FirstOrDefault()?.TransactionId;
        uploadBajajCKYC.LeadId = result?.FirstOrDefault()?.LeadId;


        var uploadCKYCResponse = await _bajajService.UploadCKYCDocument(uploadBajajCKYC, cancellationToken);

        var response = await _quoteRepository.SaveLeadDetails(uploadBajajCKYC.InsurerId, uploadBajajCKYC.QuoteTransactionId, uploadCKYCResponse.Item1, uploadCKYCResponse.Item2, "POA", uploadCKYCResponse.Item4, cancellationToken);

        uploadCKYCResponse.Item3.LeadID = response.LeadID;
        uploadCKYCResponse.Item3.CKYCNumber = response.CKYCNumber;
        uploadCKYCResponse.Item3.KYCId = response.KYCId;

        if (uploadCKYCResponse != null)
        {
            return uploadCKYCResponse.Item3;
        }
        return default;
    }

    public async Task<Tuple<byte[]>> GetPolicy(string leadId, string policyNumber, bool IsTP)
    {
        var policyArray = await _bajajService.GeneratePolicy(leadId, policyNumber, IsTP);
        return Tuple.Create(policyArray);
    }
    public async Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        var resConfirmQuote = await _bajajService.QuoteConfirmDetails(quoteTransactionDbModel, quoteConfirmCommand, cancellationToken);
        return resConfirmQuote;
    }

    public async Task<Tuple<BajajBreakinStatusCheckResponseModel, CreateLeadModel>> GetBreakinPinStatus(string leadId, string vehicleNumber, CancellationToken cancellationToken)
    {
        CreateLeadModel createLeadModel = new CreateLeadModel();
        var response = await _bajajService.GetBreakinPinStatus(leadId, vehicleNumber, cancellationToken);
        if (response != null && response.pinList != null)
        {
            if (response.pinList[response.pinList.Length - 1].stringval2.Equals("PIN_APPRD"))
            {
                createLeadModel.BreakInStatus = true;
                createLeadModel.Stage = "Proposal";
            }
            else
            {
                createLeadModel.BreakInStatus = false;
                createLeadModel.Stage = "BreakIn";
            }
        }
        return Tuple.Create(response, createLeadModel);
    }
    public async Task<string> GetQuoteTransactionId(string vehicleNumber, string quoteTransactionId, CancellationToken cancellationToken)
    {
        var dbConnection = _applicationDBContext.CreateConnection();
        var dbParameters = new DynamicParameters();
        dbParameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);
        dbParameters.Add("QuoteTransactionId", quoteTransactionId, DbType.String, ParameterDirection.Input);

        var result = await dbConnection.QueryAsync<string>("[dbo].[Insurance_GetQuoteTransactionId]", dbParameters,
             commandType: CommandType.StoredProcedure);

        return result.FirstOrDefault();
    }
    public async Task<SaveQuoteTransactionModel> GenerateBreakinPin(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken)
    {
        BajajProposalRequestDto bajajProposalRequestDto = JsonConvert.DeserializeObject<BajajProposalRequestDto>(quoteTransactionDbModel.QuoteTransactionRequest?.RequestBody);
        CreateLeadModel createLeadModel = (quoteTransactionDbModel?.LeadDetail);
        QuoteResponseModel commonResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(quoteTransactionDbModel.QuoteTransactionRequest?.CommonResponse);

        var breakinPinResponse = await _bajajService.BreakInPinGeneration(createLeadModel?.LeadID, commonResponse.TransactionID, createLeadModel.VehicleNumber, createLeadModel.PhoneNumber, cancellationToken);
        if (breakinPinResponse != null & !string.IsNullOrEmpty(breakinPinResponse.Item1))
        {
            commonResponse.ApplicationId = bajajProposalRequestDto.transactionid;
            commonResponse.BreakinId = breakinPinResponse.Item1;
            commonResponse.ProposalNumber = commonResponse.TransactionID;
            commonResponse.IsBreakIn = true;
            commonResponse.IsSelfInspection = true;
            commonResponse.ValidationMessage = breakinPinResponse.Item4;
            SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
            {
                quoteResponseModel = commonResponse,
                RequestBody = breakinPinResponse.Item2,
                ResponseBody = breakinPinResponse.Item3,
                Stage = "BreakIn",
                InsurerId = _bajajConfig.InsurerId,
                LeadId = createLeadModel.LeadID,
                MaxIDV = Convert.ToDecimal(commonResponse.MaxIDV),
                MinIDV = Convert.ToDecimal(commonResponse.MinIDV),
                RecommendedIDV = Convert.ToDecimal(commonResponse.IDV),
                TransactionId = commonResponse.TransactionID,
                PolicyNumber = string.Empty,
            };
            return saveQuoteTransactionModel;
        }
        return default;
    }
    public async Task<string> GetQuoteTransactionId(string quoteTransactionId, CancellationToken cancellationToken)
    {
        var dbConnection = _applicationDBContext.CreateConnection();
        var dbParameters = new DynamicParameters();
        dbParameters.Add("QuoteTransactionId", quoteTransactionId, DbType.String, ParameterDirection.Input);

        var result = await dbConnection.QueryAsync<string>("[dbo].[Insurance_GetQuoteTransactionId]", dbParameters,
             commandType: CommandType.StoredProcedure);

        return result.FirstOrDefault();
    }
    public async Task<SaveQuoteTransactionModel> GetPaymentLink(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken)
    {
        var response = await _bajajService.CreateProposal(quoteTransactionDbModel, cancellationToken);
        return response;
    }
    public async Task<VariantAndRTOIdCheckModel> DoesBajajVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, CancellationToken cancellationToken)
    {
        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("VariantId", variantId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", rtoId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_DoesBajajVariantAndRTOExists]",
            parameters,
            commandType: CommandType.StoredProcedure);

        var response = result.Read<VariantAndRTOIdCheckModel>();
        return response.FirstOrDefault();
    }
}
