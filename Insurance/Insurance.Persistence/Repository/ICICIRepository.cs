using AutoMapper;
using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.ICICI.Command.CKYC;
using Insurance.Core.Features.ICICI.Command.UploadICICICKYCDocument;
using Insurance.Core.Features.ICICI.Queries.GetQuote;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
using Insurance.Domain.ICICI.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Insurance.Persistence.ICIntegration.Implementation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Data;
using System.Globalization;
using System.Net;

namespace Insurance.Persistence.Repository
{
    public class ICICIRepository : IICICIRepository
    {
        private readonly IICICIService _iCICIService;
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly ICICIConfig _iCICIConfig;
        private readonly IGoDigitRepository _goDigitRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IdentityApplicationDBContext _identityApplicationDBContext;
        private const string BREAKINSUCCESS = "Recommended";
        private const string BREAKINFAILED = "Rejected";
        private const string BREAKINPENDING = "";
        private readonly IApplicationClaims _applicationClaims;
        private readonly ILogger<ICICIService> _logger;
        private readonly VehicleTypeConfig _vehicleTypeConfig;
        private readonly IMapper _mapper;


        public ICICIRepository(IICICIService iCICIService,
            ApplicationDBContext applicationDBContext,
            IOptions<ICICIConfig> options,
            IGoDigitRepository goDigitRepository,
            IQuoteRepository quoteRepository,
            IdentityApplicationDBContext identityApplicationDBContext,
            IApplicationClaims applicationClaims,
            ILogger<ICICIService> logger,
            IOptions<VehicleTypeConfig> vehicleTypeConfig
            , IMapper mapper
            )
        {
            _iCICIService = iCICIService ?? throw new ArgumentNullException(nameof(iCICIService));
            _applicationDBContext = applicationDBContext ?? throw new ArgumentException(nameof(applicationDBContext));
            _iCICIConfig = options?.Value;
            _goDigitRepository = goDigitRepository ?? throw new ArgumentNullException(nameof(goDigitRepository));
            _quoteRepository = quoteRepository;
            _identityApplicationDBContext = identityApplicationDBContext ?? throw new ArgumentException(nameof(identityApplicationDBContext));
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _vehicleTypeConfig = vehicleTypeConfig.Value;
            _mapper = mapper;
        }
        public async Task<QuoteResponseModel> GetQuote(GetIciciQuoteQuery query, CancellationToken cancellationToken)
        {
            var quoteQuery = await QuoteMasterMapping(query, cancellationToken);

            //calling IDV
            quoteQuery.Token = await CheckToken(query.LeadId, false, true, false, false, false);

            if (quoteQuery.Token != null)
            {
                var idvDetails = await _iCICIService.GetIDV(quoteQuery, cancellationToken);
                if (idvDetails.StatusCode == 200)
                {
                    decimal MaxIDV = Convert.ToDecimal(idvDetails.maximumprice);
                    decimal MinIDV = Convert.ToDecimal(idvDetails.minimumprice);
                    decimal RecommendedIDV = query.IsBrandNewVehicle ? Convert.ToDecimal(idvDetails.maximumprice) : Convert.ToDecimal(idvDetails.minimumprice);

                    quoteQuery.ExShowRoomPrice = query.IDV switch
                    {
                        1 => RecommendedIDV,
                        2 => MinIDV,
                        3 => MaxIDV,
                        > 3 => query.IDV / Convert.ToDecimal(1 - idvDetails.idvdepreciationpercent),
                        _ => MaxIDV,
                    };
                    //Calling Quote
                    quoteQuery.Token = await CheckToken(query.LeadId, true, false, false, false, false);

                    var quoteResponse = await _iCICIService.GetQuote(quoteQuery, cancellationToken);

                    SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
                    {
                        quoteResponseModel = quoteResponse.Item1,
                        RequestBody = quoteResponse.Item2,
                        ResponseBody = quoteResponse.Item3,
                        Stage = "Quote",
                        InsurerId = _iCICIConfig.InsurerId,
                        LeadId = query.LeadId,
                        MaxIDV = MaxIDV,
                        MinIDV = MinIDV,
                        RecommendedIDV = RecommendedIDV,
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
                    //await _quoteRepository.SaveQuoteTransaction(quoteResponse.Item1, quoteResponse.Item2, quoteResponse.Item3, "PreQuote", _iCICIConfig.InsurerId, query.LeadId, MaxIDV, MinIDV, RecommendedIDV, null, cancellationToken);

                    if (quoteResponse.Item1.InsurerName != null)
                    {
                        quoteResponse.Item1.InsurerId = _iCICIConfig.InsurerId;
                        return quoteResponse.Item1;
                    }
                }
            }

            return new QuoteResponseModel
            {
                InsurerId = _iCICIConfig.InsurerId,
                InsurerName = _iCICIConfig.InsurerName,
                InsurerLogo = _iCICIConfig.InsurerLogo,
                InsurerStatusCode = (int)HttpStatusCode.BadRequest
            };
        }
        private async Task<QuoteQueryModel> QuoteMasterMapping(GetIciciQuoteQuery query, CancellationToken cancellationToken)
        {
            var addOnId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnId} ")) : String.Empty;
            var discountId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList?.Select(x => $"{x.DiscountId} ")) : String.Empty;
            var paCoverId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverId} ")) : String.Empty;
            var accessoryId = (query.AccessoryList != null && query.AccessoryList.Any()) ? String.Join(",", query.AccessoryList?.Select(x => $"{x.AccessoryId} ")) : String.Empty;
            var discountExtensionId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList.Select(x => $"{x.DiscountExtensionId}")) : string.Empty;
            var paCoverExtensionId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverExtensionId} ")) : String.Empty;
            var addOnsExtensionId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnsExtensionId} ")) : String.Empty;

            var dateTime = DateTime.Now;
            var previousInsurer = query.PreviousPolicy == null && !string.IsNullOrEmpty(query.PreviousPolicy?.SAODInsurer) && query.PreviousPolicy?.SAODInsurer != "" ? query.PreviousPolicy.SAODInsurer : _iCICIConfig.InsurerId;

            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("PACoverId", paCoverId, DbType.String, ParameterDirection.Input);
            parameters.Add("AccessoryId", accessoryId, DbType.String, ParameterDirection.Input);
            parameters.Add("AddonId", addOnId, DbType.String, ParameterDirection.Input);
            parameters.Add("DiscountId", discountId, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", _iCICIConfig.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("RTOId", query.RTOId, DbType.String, ParameterDirection.Input);
            parameters.Add("VariantId", query.VariantId, DbType.String, ParameterDirection.Input);
            parameters.Add("NCBId", query.PreviousPolicy?.NCBId, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyTypeId", query.PolicyDates?.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleTypeId", query.VehicleTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleNumber", query.VehicleNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("DiscountExtensionId", discountExtensionId, DbType.String, ParameterDirection.Input);
            parameters.Add("PACoverExtensionId", paCoverExtensionId, DbType.String, ParameterDirection.Input);
            parameters.Add("AddOnsExtensionId", addOnsExtensionId, DbType.String, ParameterDirection.Input);
            parameters.Add("SAODInsurerId", previousInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("SATPInsurerId", previousInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("IsBrandNew", query.IsBrandNewVehicle, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetICICIQuoteMasterMapping]", parameters,
                         commandType: CommandType.StoredProcedure);

            var paCoverList = result.Read<PACoverModel>();
            var accessoryList = result.Read<AccessoryModel>();
            var addOnList = result.Read<AddonsModel>();
            var discountList = result.Read<DiscountModel>();
            var discountExtensionList = result.Read<DiscountExtensionModel>();
            var paCoverExtensionList = result.Read<PACoverExtensionModel>();
            var addOnsExtensionList = result.Read<AddOnsExtensionModel>();
            var codeList = result.Read<RTOVehiclePreviousInsurerModel>();
            var configNameValueList = result.Read<ConfigNameValueModel>();

            var quoteQuery = new QuoteQueryModel
            {
                AddOns = new Domain.GoDigit.AddOns(),
                PACover = new PACover(),
                Accessories = new Domain.GoDigit.Accessories(),
                Discounts = new Discounts(),
                VehicleDetails = new VehicleDetails(),
                PreviousPolicyDetails = new PreviousPolicy(),
            };
            if (discountExtensionList.Any())
            {
                quoteQuery.VoluntaryExcess = discountExtensionList.FirstOrDefault().DiscountExtension;
            }
            if (paCoverExtensionList.Any())
            {
                quoteQuery.UnnamedPassangerAndPillonRider = paCoverExtensionList.FirstOrDefault().PACoverExtension;
            }
            if (addOnsExtensionList.Any())
            {
                quoteQuery.GeogExtension = addOnsExtensionList.FirstOrDefault().AddOnsExtension;
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
                    else if (item.AddOns.Equals("PartsDepreciation") && item.AddOns.Equals("PersonalBelongings")) //if zerodept true then personal belongings are applicable 
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
                    if (item.CoverName.Equals("UnnamedPax"))
                    {
                        quoteQuery.PACover.UnnamedPassengerId = (paCoverList?.Where(x => x.CoverName == "UnnamedPax").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsUnnamedPassenger = true;

                    }
                    if (item.CoverName.Equals("UnnamedPillionRider"))
                    {
                        quoteQuery.PACover.UnnamedPassengerId = (paCoverList?.Where(x => x.CoverName == "UnnamedPax").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsUnnamedPassenger = true;

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

            string previousInsurerCode = string.Empty;
            string ncbValue = string.Empty;
            quoteQuery.LeadId = query.LeadId;
            if (codeList.Any())
            {
                var codeData = codeList.FirstOrDefault();

                quoteQuery.CorrelationId = Guid.NewGuid();
                quoteQuery.VehicleDetails.VehicleMakeCode = codeData.VehicleMakeCode;
                quoteQuery.VehicleDetails.VehicleModelCode = codeData.VehicleModelCode;
                quoteQuery.VehicleDetails.LicensePlateNumber = codeData.LicensePlateNumber;
                quoteQuery.RTOLocationCode = codeData.RTOLocationCode;
                quoteQuery.CountryCode = codeData.CountryCode;
                quoteQuery.StateCode = codeData.RegistrationStateCode;
                quoteQuery.CityCode = codeData.CityCode;
                quoteQuery.VehicleDetails.ManufactureDate = codeData.ManufactureDate;
                quoteQuery.GSTToState = codeData.GSTToState;
                quoteQuery.VehicleDetails.RegDate = codeData.regDate;
                quoteQuery.VehicleDetails.VehicleClass = codeData.vehicleclass;
                quoteQuery.VehicleDetails.VehicleType = codeData.VehicleType;
                quoteQuery.VehicleDetails.Chassis = codeData.chassis != null ? codeData.chassis : "12345CHS";
                quoteQuery.VehicleDetails.EngineNumber = codeData.engine != null ? codeData.engine : "12345ENG";
                quoteQuery.VehicleNumber = query.VehicleNumber;
                //quoteQuery.PACover.IsUnnamedOWNERDRIVER = codeData.PreviousPolicyType == "SAOD" ? false : true;
                quoteQuery.CurrentPolicyType = codeData.CurrentPolicyType;
                quoteQuery.PlanType = codeData.PlanType;
                ncbValue = codeData.NCBValue;
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
                        IsClaimInLastYear = query.PreviousPolicy.IsPreviousYearClaim,
                        PreviousNoClaimBonus = ncbValue
                    };
                }
                quoteQuery.PreviousPolicyDetails.PreviousPolicyType = codeList.FirstOrDefault().PreviousPolicyType;
                quoteQuery.PreviousPolicyDetails.PreviousSAODInsurer = codeList.FirstOrDefault().SAODInsurer;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = query.PolicyDates.ODPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = query.PolicyDates.ODPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd") : null;
                quoteQuery.PreviousPolicyDetails.PreviousPolicyNumber = !string.IsNullOrEmpty(codeList.FirstOrDefault().PreviousInsurancePolicyNumber) ? codeList.FirstOrDefault().PreviousInsurancePolicyNumber : "ICICI12345";

                if (quoteQuery.PreviousPolicyDetails.PreviousPolicyType.Equals("SAOD"))
                {
                    quoteQuery.PreviousPolicyDetails.PreviousSATPInsurer = codeList.FirstOrDefault().SATPInsurer;
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP = query.PolicyDates.TPPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd") : null;
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP = query.PolicyDates.TPPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd") : null;
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyNumberTP = codeList.FirstOrDefault().PreviousInsurancePolicyNumber != null ? codeList.FirstOrDefault().PreviousInsurancePolicyNumber : "ICICI12345";
                    quoteQuery.IsSATPMandatory = true;
                    quoteQuery.IsSAODMandatry = true;
                }
                if (codeList.FirstOrDefault().CurrentPolicyType.Equals("SATP"))
                {
                    quoteQuery.IsSATPMandatory = true;
                }
                else
                {
                    quoteQuery.IsSAODMandatry = true;
                }
            }

            quoteQuery.PolicyStartDate = Convert.ToDateTime(query.PolicyDates.PolicyStartDate).ToString("yyyy-MM-dd");
            quoteQuery.PolicyEndDate = Convert.ToDateTime(query.PolicyDates.PolicyEndDate).ToString("yyyy-MM-dd");
            quoteQuery.RegistrationDate = query.PolicyDates.RegistrationDate;
            quoteQuery.VehicleODTenure = query.PolicyDates.VehicleODTenure;
            quoteQuery.VehicleTPTenure = query.PolicyDates.VehicleTPTenure;
            quoteQuery.IsBrandNewVehicle = query.IsBrandNewVehicle;
            quoteQuery.SelectedIDV = query.IDV;
            quoteQuery.RegistrationYear = query.RegistrationYear;
            quoteQuery.ConfigNameValueModels = configNameValueList;
            quoteQuery.VehicleDetails.IsTwoWheeler = query.PolicyDates.IsTwoWheeler;
            quoteQuery.VehicleDetails.IsFourWheeler = query.PolicyDates.IsFourWheeler;
            quoteQuery.VehicleDetails.IsCommercial = query.PolicyDates.IsCommercial;
            //quoteQuery.CategoryId = query.CategoryId;

            return quoteQuery;
        }
        public async Task<SaveQuoteTransactionModel> CreateProposal(ProposalRequestModel proposalRequestModel, CancellationToken cancellationToken)
        {

            var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(proposalRequestModel.QuoteTransactionID, cancellationToken);
            if (_quotedetails != null)
            {
                ICICIProposalRequestDto _iciciProposal = JsonConvert.DeserializeObject<ICICIProposalRequestDto>(_quotedetails.QuoteTransactionRequest?.RequestBody);
                CreateLeadModel _leadDetails = (_quotedetails.LeadDetail);
                ICICIProposalRequest _proposalRequest = JsonConvert.DeserializeObject<ICICIProposalRequest>(_quotedetails.ProposalRequestBody);
                ICICICKYCRequest iCICICKYCRequest = JsonConvert.DeserializeObject<ICICICKYCRequest>(_quotedetails.CKYCRequestBody);

                if (_iciciProposal != null && _proposalRequest != null)
                {
                    var proposalResponse = await _iCICIService.CreateProposal(_iciciProposal, _proposalRequest, iCICICKYCRequest, proposalRequestModel.VehicleTypeId, _leadDetails, cancellationToken);
                    decimal RecommendedIDV = 0;
                    decimal MinIDV = 0;
                    decimal MaxIDV = 0;
                    string transactionId = _iciciProposal.CorrelationId;

                    if (proposalResponse.Item1.InsurerName != null)
                    {
                        proposalResponse.Item1.InsurerId = _iCICIConfig.InsurerId;

                        string stage = string.Empty;
                        if (proposalResponse.Item1.isApprovalRequired || proposalResponse.Item1.isQuoteDeviation || proposalResponse.Item1.IsBreakIn || _leadDetails.IsSelfInspection)
                        {
                            stage = "BreakIn";
                        }
                        else
                        {
                            stage = "Proposal";
                        }

                        SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
                        {
                            quoteResponseModel = proposalResponse.Item1,
                            RequestBody = proposalResponse.Item2,
                            ResponseBody = proposalResponse.Item3,
                            Stage = stage,
                            InsurerId = _iCICIConfig.InsurerId,
                            LeadId = _leadDetails.LeadID,
                            MaxIDV = MaxIDV,
                            MinIDV = MinIDV,
                            RecommendedIDV = RecommendedIDV,
                            TransactionId = transactionId,
                            PolicyNumber = proposalResponse.Item1.ProposalNumber,
                            QuoteTransactionId = proposalRequestModel.QuoteTransactionID
                        };
                        return saveQuoteTransactionModel;
                    }
                }
            }
            return default;
        }
        public async Task<Tuple<string, string>> CreatePaymentLink(string leadId, string proposalRequest, string proposalResponse, CancellationToken cancellationToken)
        {
            var paymentURL = await _iCICIService.CreatePaymentURL(leadId, proposalRequest, proposalResponse, cancellationToken);
            if (paymentURL != null)
            {
                return Tuple.Create(paymentURL.Item1, paymentURL.Item2);
            }
            return default;
        }
        public async Task<ICICIPaymentEnquiryResponse> GetPaymentEnquiry(string transactionId, string leadId, CancellationToken cancellationToken)
        {
            ICICIPaymentEnquiryResponse result = await _iCICIService.TransactionEnquiry(transactionId, leadId, cancellationToken);
            return result;
        }
        public async Task<Tuple<ICICIPaymentTaggingResponseDto, ICICIPOSPDetails>> GetPaymentTagging(ICICIPaymentEnquiryResponse paymentEnquiryResponse, string transactionId, string correlationId)
        {
            ICICIResponseDto proposalResponse = new ICICIResponseDto();
            ICICIProposalRequestDto iciciProposalRequest = new ICICIProposalRequestDto();
            string dealId = string.Empty;
            var response = string.Empty;
            var proposalDetails = await GetProposalDetails(correlationId);
            var pospDetails = await _quoteRepository.GetPOPSDetais(_applicationClaims.GetUserId());
            if (proposalDetails != null)
            {
                iciciProposalRequest = JsonConvert.DeserializeObject<ICICIProposalRequestDto>(proposalDetails.RequestBody);
                dealId = iciciProposalRequest.DealId;

                if (proposalDetails.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                {
                    ICICICVResponseDto mICICICVResponseDto = JsonConvert.DeserializeObject<ICICICVResponseDto>(proposalDetails.ResponseBody);

                    proposalResponse = _mapper.Map<ICICIResponseDto>(mICICICVResponseDto);// proposalResponse.ResponseBody
                }
                else
                {
                    proposalResponse = JsonConvert.DeserializeObject<ICICIResponseDto>(proposalDetails.ResponseBody);
                }
                var result = await _iCICIService.PaymentTagging(paymentEnquiryResponse, proposalResponse, dealId, proposalDetails, pospDetails, iciciProposalRequest.ProductCode);
                if (result != null)
                {
                    ICICIPaymentTaggingResponseDto iCICIPaymentTaggingResponseDto = new ICICIPaymentTaggingResponseDto()
                    {
                        iCICIPaymentTaggingResponse = result,
                        InsurerId = _iCICIConfig.InsurerId,
                        Amount = proposalResponse.finalPremium,
                        LeadId = proposalDetails.LeadId,
                        QuoteTransactionId = proposalDetails.QuoteTransactionId,
                        CustomerId = proposalResponse.generalInformation.customerId.ToString(),
                        DealId = iciciProposalRequest.DealId,
                        VehicleTypeId = proposalDetails.VehicleTypeId,
                        PolicyTypeId = proposalDetails.PolicyTypeId,
                        ProductCode = iciciProposalRequest?.ProductCode
                    };
                    return Tuple.Create(iCICIPaymentTaggingResponseDto, pospDetails);
                }
            }
            return default;
        }
        public async Task<byte[]> GetPloicy(ICICIPaymentTaggingResponseDto iCICIPaymentTaggingResponseDto, ICICIPOSPDetails iCICIPOSPDetails)
        {
            var result = await _iCICIService.GeneratePolicy(iCICIPaymentTaggingResponseDto, iCICIPOSPDetails);
            return result;
        }
        public async Task<string> CreateIMBroker(ICICICreateIMBrokerModel iCICICreateIMBrokerModel, CancellationToken cancellationToken)
        {
            var iMID = string.Empty;
            var submitPOSPResponse = await _iCICIService.SubmitPOSPCertificate(iCICICreateIMBrokerModel, cancellationToken);
            if ((submitPOSPResponse.status.ToLower().Equals("success") || submitPOSPResponse.status.ToLower().Equals("failure")) && !string.IsNullOrEmpty(submitPOSPResponse.correlationID))
            {
                iCICICreateIMBrokerModel.CorrelationId = submitPOSPResponse.correlationID;
                var createImBroker = await _iCICIService.CreateBroker(iCICICreateIMBrokerModel, cancellationToken);
                if (createImBroker != null && createImBroker.status.ToLower().Equals("success"))
                {
                    iMID = await InsertIMBrokerId(iCICICreateIMBrokerModel.POSPId, createImBroker.imid, cancellationToken);
                    if (iMID != null)
                    {
                        return createImBroker.imid;
                    }
                    return "Insert IMID Failed";
                }
            }
            return default;
        }
        public async Task<Tuple<string, string, string>> CreateBreakinId(string leadId, string proposalRequest, SaveQuoteTransactionModel proposalResponse, string vehicleTypeId, CancellationToken cancellationToken)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var request = JsonConvert.DeserializeObject<ICICIProposalRequestDto>(proposalRequest);
            ICICIResponseDto response = new();
            if (vehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
            {
                ICICICVResponseDto mICICICVResponseDto = JsonConvert.DeserializeObject<ICICICVResponseDto>(proposalResponse.ResponseBody);
                response = _mapper.Map<ICICIResponseDto>(mICICICVResponseDto);// proposalResponse.ResponseBody
            }
            else
            {
                response = JsonConvert.DeserializeObject<ICICIResponseDto>(proposalResponse.ResponseBody);
            }
            var parameters = new DynamicParameters();
            parameters.Add("CityCode", request.CustomerDetails.CityCode.ToString(), DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync("[dbo].[Insurance_GetICICIBreakinMaster]", parameters, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                request.LeadId = leadId;
                var breakinIdCreation = await _iCICIService.CreateBreakinId(request, response, result.FirstOrDefault().City, result.FirstOrDefault().State, vehicleTypeId, proposalResponse.CategoryId, cancellationToken);
                if (breakinIdCreation != null)
                {
                    return breakinIdCreation;
                }
            }
            return default;
        }
        public async Task<ICICIGetBreakinStatusResponseModel> BreakinInspectionStatus(CancellationToken cancellationToken)
        {
            var breakinStatus = await _iCICIService.GetBreakinStatus(cancellationToken);
            return breakinStatus;
        }
        public async Task<string> ClearBreakinInspectionStatus(string breakinId, bool isBreakInApproved, string correlationId, CancellationToken cancellationToken)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("BreakinId", breakinId, DbType.String, ParameterDirection.Input);
            parameters.Add("IsBreakinApproved", isBreakInApproved, DbType.Boolean, ParameterDirection.Input);

            var result = await connection.QueryAsync<ICICIGetBreakinDetails>("[dbo].[Insurance_GetICICIBreakinStatus]",
                parameters,
                commandType: CommandType.StoredProcedure);
            var breakinDetails = result.FirstOrDefault();
            var pospDetails = await _quoteRepository.GetPOPSDetais(result.FirstOrDefault().UserId);
            if (result != null && result.Any() && pospDetails != null)
            {
                breakinDetails.BreakinId = breakinId;
                breakinDetails.TransactionId = correlationId;
                var clearBreakinStatus = await _iCICIService.ClearInspectionStatus(breakinDetails, pospDetails, cancellationToken);
                if (clearBreakinStatus != null && clearBreakinStatus.status)
                {
                    return breakinDetails.BreakinId;
                }
            }
            return default;
        }
        public async Task UpdateBreakinStatus(CreateLeadModel createLeadModel, CancellationToken cancellationToken)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("IsBreakinApproved", createLeadModel.IsBreakinApproved, DbType.String, ParameterDirection.Input);
            parameters.Add("PaymentLink", createLeadModel.PaymentLink, DbType.String, ParameterDirection.Input);
            parameters.Add("Stage", createLeadModel.Stage, DbType.String, ParameterDirection.Input);
            parameters.Add("BreakinId", createLeadModel.BreakinId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync("[dbo].[Insurance_UpdateICICIBreakinStatus]",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        public async Task<SaveCKYCResponse> SaveCKYC(ICICICKYCCommand iCICICKYCCommand, CancellationToken cancellationToken)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", _iCICIConfig.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("QuotetransactionId", iCICICKYCCommand.QuoteTransactionId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<QuoteResponseBody>("[dbo].[Insurance_GetICICIQuoteResponse]", parameters,
                      commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            ICICIResponseDto quoteResponse = JsonConvert.DeserializeObject<ICICIResponseDto>(result.FirstOrDefault().ResponseBody);
            iCICICKYCCommand.TransactionId = quoteResponse.correlationId;
            iCICICKYCCommand.LeadId = result?.FirstOrDefault()?.LeadId;
            var ckycResponse = await _iCICIService.GetCKYCResponse(iCICICKYCCommand, cancellationToken);

            CreateLeadModel createLeadModelObject = ckycResponse.Item4;

            var response = await _quoteRepository.SaveLeadDetails(iCICICKYCCommand.InsurerId, iCICICKYCCommand.QuoteTransactionId, ckycResponse.Item1, ckycResponse.Item2, "POI", createLeadModelObject, cancellationToken);

            ckycResponse.Item3.LeadID = response.LeadID;
            ckycResponse.Item3.CKYCNumber = response.CKYCNumber;
            ckycResponse.Item3.KYCId = response.KYCId;

            if (ckycResponse != null)
            {
                return ckycResponse.Item3;
            }
            return default;
        }
        public async Task<UploadCKYCDocumentResponse> UploadCKYCDocument(UploadICICICKYCDocumentCommand iCICICKYCCommand, CancellationToken cancellationToken)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", _iCICIConfig.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("QuotetransactionId", iCICICKYCCommand.QuoteTransactionId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<QuoteResponseBody>("[dbo].[Insurance_GetICICIQuoteResponse]", parameters,
                      commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            ICICIResponseDto quoteResponse = JsonConvert.DeserializeObject<ICICIResponseDto>(result.FirstOrDefault().ResponseBody);
            iCICICKYCCommand.TransactionId = quoteResponse.correlationId;
            iCICICKYCCommand.LeadId = result?.FirstOrDefault()?.LeadId;
            var ckycResponse = await _iCICIService.UploadCKYCDocument(iCICICKYCCommand, cancellationToken);

            CreateLeadModel createLeadModelObject = ckycResponse.Item4;
            var response = await _quoteRepository.SaveLeadDetails(iCICICKYCCommand.InsurerId, iCICICKYCCommand.QuoteTransactionId, ckycResponse.Item1, ckycResponse.Item2, "POA", createLeadModelObject, cancellationToken);

            ckycResponse.Item3.LeadID = response.LeadID;
            ckycResponse.Item3.CKYCNumber = response.CKYCNumber;
            ckycResponse.Item3.KYCId = response.KYCId;

            if (ckycResponse != null)
            {
                return ckycResponse.Item3;
            }
            return default;
        }
        private async Task<string> CheckToken(string leadId, bool quoteToken, bool idvToken, bool ckycToken, bool paymentToken, bool policyGeneration)
        {
            var token = await _iCICIService.GetToken(leadId, quoteToken, idvToken, ckycToken, paymentToken, policyGeneration, "Quote");
            return token;
        }
        public async Task<QuoteTransactionRequest> GetProposalDetails(string transactionID)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("TransactionID", transactionID, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<QuoteTransactionRequest>("[dbo].[Insurance_GetProposalDetails]",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }
        public async Task<QuoteTransactionRequest> GetBreakInPaymentDetails(string transactionId, string breakinId, CancellationToken cancellationToken)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("TransactionId", transactionId, DbType.String, ParameterDirection.Input);
            parameters.Add("BreakInId", breakinId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<QuoteTransactionRequest>("[dbo].[Insurance_GetBreakInPaymentDetails]",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result.FirstOrDefault();
        }
        private async void InsertUpdateConfig(string insurerid, string polictytypeid, string vehicletypeid, string namevalue)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", insurerid, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyTypeId", polictytypeid, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleTypeId", vehicletypeid, DbType.String, ParameterDirection.Input);
            parameters.Add("NameValue", namevalue, DbType.String, ParameterDirection.Input);

            await connection.ExecuteAsync("[dbo].[Insurance_InsertInsuranceICConfig]", parameters,
                         commandType: CommandType.StoredProcedure);
        }
        public async Task<string> InsertIMBrokerId(string POSPId, string iMID, CancellationToken cancellationToken)
        {
            var iMDI = string.Empty;
            using var connection = _identityApplicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("POSPId", POSPId, DbType.String, ParameterDirection.Input);
            parameters.Add("IMID", iMID, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<string>("[dbo].[Identity_InsertIMID]",
                parameters,
                commandType: CommandType.StoredProcedure)
                ;
            if (result != null)
            {
                iMID = result.FirstOrDefault();
            }
            return iMID;
        }
        public Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
        {
            var res = _iCICIService.QuoteConfirmDetails(quoteTransactionDbModel, quoteConfirmCommand, cancellationToken);
            return res;
        }
        public async Task SavePaymentDetails(string requestBody, string responseBody, string transactionId, string stage)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", _iCICIConfig.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("ResponseBody", responseBody, DbType.String, ParameterDirection.Input);
            parameters.Add("RequestBody", requestBody, DbType.String, ParameterDirection.Input);
            parameters.Add("Stage", stage, DbType.String, ParameterDirection.Input);
            parameters.Add("TransactionId", transactionId, DbType.String, ParameterDirection.Input);

            var result = await connection.ExecuteAsync("[dbo].[Insurance_InsertICICIPaymentTransaction]",
                parameters,
                commandType: CommandType.StoredProcedure);
        }
        public async Task<Tuple<string, string>> GetPaymentLink(BreakInPaymentDetailsDBModel breakInPaymentDetailsDBModel, CancellationToken cancellationToken)
        {
            if (breakInPaymentDetailsDBModel.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
            {
                ICICICVResponseDto mICICICVResponseDto = JsonConvert.DeserializeObject<ICICICVResponseDto>(breakInPaymentDetailsDBModel.ProposalResponse);
                var mResponse = _mapper.Map<ICICIResponseDto>(mICICICVResponseDto);
                breakInPaymentDetailsDBModel.ProposalResponse = JsonConvert.SerializeObject(mResponse);
            }
            else
            {
                breakInPaymentDetailsDBModel.ProposalResponse = breakInPaymentDetailsDBModel.ProposalResponse;
            }
            var paymentLink = await _iCICIService.CreatePaymentURL(breakInPaymentDetailsDBModel.LeadId, breakInPaymentDetailsDBModel.ProposalRequest, breakInPaymentDetailsDBModel.ProposalResponse, cancellationToken);
            if (paymentLink != null)
            {
                return Tuple.Create(paymentLink.Item1, paymentLink.Item2);
            }
            return default;
        }
        public async Task<VariantAndRTOIdCheckModel> DoesICICIVariantAndRTOExists(string variantId, string rtoId, string vehicleNumber, CancellationToken cancellationToken)
        {
            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("VariantId", variantId, DbType.String, ParameterDirection.Input);
            parameters.Add("RTOId", rtoId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleNumber", vehicleNumber, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_DoesICICIVariantAndRTOExists]",
                parameters,
                commandType: CommandType.StoredProcedure);

            var response = result.Read<VariantAndRTOIdCheckModel>();
            return response.FirstOrDefault();
        }

        public async Task<QuoteResponseModel> GetCVQuote(GetCommercialICICIQuoteQuery query, CancellationToken cancellationToken)
        {
            //var iciciQuery = _mapper.Map<GetIciciQuoteQuery>(query);

            var quoteQuery = await CVQuoteMasterMapping(query, cancellationToken);

            quoteQuery.CategoryId = query.CategoryId;
            if (quoteQuery.CurrentPolicyType.Equals("SATP"))
            {
                quoteQuery.DealID = query.CategoryId == "1" ? _iCICIConfig.GCVDealIdTP : query.CategoryId == "2" ? _iCICIConfig.PCVDealIdTP : query.CategoryId == "3" ? _iCICIConfig.MISCDealIdTP : "";
                quoteQuery.ProductCode = quoteQuery.CategoryId == "1" ? _iCICIConfig.GCVTPProdectCode : quoteQuery.CategoryId == "2" ? _iCICIConfig.PCVTPProdectCode : quoteQuery.CategoryId == "3" ? _iCICIConfig.MISCTPProdectCode : "0";
            }
            else
            {
                quoteQuery.DealID = query.CategoryId == "1" ? _iCICIConfig.GCVDealId : query.CategoryId == "2" ? _iCICIConfig.PCVDealId : query.CategoryId == "3" ? _iCICIConfig.MISCDealId : "";
                quoteQuery.ProductCode = quoteQuery.CategoryId == "1" ? _iCICIConfig.GCVProdectCode : quoteQuery.CategoryId == "2" ? _iCICIConfig.PCVProdectCode : quoteQuery.CategoryId == "3" ? _iCICIConfig.MISCProdectCode : "0";
            }

            //calling IDV
            quoteQuery.Token = await CheckToken(query.LeadId, false, true, false, false, false);

            if (quoteQuery.Token != null)
            {
                var idvDetails = await _iCICIService.GetIDV(quoteQuery, cancellationToken);
                if (idvDetails.StatusCode == 200)
                {
                    decimal MaxIDV = Convert.ToDecimal(idvDetails.maximumprice);
                    decimal MinIDV = Convert.ToDecimal(idvDetails.minimumprice);
                    decimal RecommendedIDV = query.IsBrandNewVehicle ? Convert.ToDecimal(idvDetails.maximumprice) : Convert.ToDecimal(idvDetails.minimumprice);

                    quoteQuery.ExShowRoomPrice = query.IDV switch
                    {
                        1 => RecommendedIDV,
                        2 => MinIDV,
                        3 => MaxIDV,
                        > 3 => query.IDV / Convert.ToDecimal(1 - idvDetails.idvdepreciationpercent),
                        _ => MaxIDV,
                    };
                    //Calling Quote
                    quoteQuery.Token = await CheckToken(query.LeadId, true, false, false, false, false);

                    var quoteResponse = await _iCICIService.GetCVQuote(quoteQuery, cancellationToken);

                    SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
                    {
                        quoteResponseModel = quoteResponse.Item1,
                        RequestBody = quoteResponse.Item2,
                        ResponseBody = quoteResponse.Item3,
                        Stage = "Quote",
                        InsurerId = _iCICIConfig.InsurerId,
                        LeadId = query.LeadId,
                        MaxIDV = MaxIDV,
                        MinIDV = MinIDV,
                        RecommendedIDV = RecommendedIDV,
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
                        IsBrandNew = query.IsBrandNewVehicle
                    };
                    await _quoteRepository.SaveQuoteTransaction(saveQuoteTransactionModel, cancellationToken);
                    //await _quoteRepository.SaveQuoteTransaction(quoteResponse.Item1, quoteResponse.Item2, quoteResponse.Item3, "PreQuote", _iCICIConfig.InsurerId, query.LeadId, MaxIDV, MinIDV, RecommendedIDV, null, cancellationToken);

                    if (quoteResponse.Item1.InsurerName != null)
                    {
                        quoteResponse.Item1.InsurerId = _iCICIConfig.InsurerId;
                        return quoteResponse.Item1;
                    }
                }
            }

            return new QuoteResponseModel
            {
                InsurerId = _iCICIConfig.InsurerId,
                InsurerName = _iCICIConfig.InsurerName,
                InsurerLogo = _iCICIConfig.InsurerLogo,
                InsurerStatusCode = (int)HttpStatusCode.BadRequest
            };
        }

        public async Task<SaveQuoteTransactionModel> CreateCvProposal(ProposalRequestModel proposalRequestModel, CancellationToken cancellationToken)
        {

            var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(proposalRequestModel.QuoteTransactionID, cancellationToken);
            if (_quotedetails is not null)
            {
                ICICICVProposalRequestDto _iciciProposal = JsonConvert.DeserializeObject<ICICICVProposalRequestDto>(_quotedetails.QuoteTransactionRequest?.RequestBody);
                CreateLeadModel _leadDetails = (_quotedetails.LeadDetail);
                //string ddd = JsonConvert.SerializeObject(_iciciProposal);
                ICICIProposalRequest _proposalRequest = JsonConvert.DeserializeObject<ICICIProposalRequest>(_quotedetails.ProposalRequestBody);
                ICICICKYCRequest iCICICKYCRequest = JsonConvert.DeserializeObject<ICICICKYCRequest>(_quotedetails.CKYCRequestBody);
                _leadDetails.CategoryId = _iciciProposal.CategoryId;
                if (_iciciProposal is not null && _proposalRequest is not null)
                {
                    var proposalResponse = await _iCICIService.CreateCvProposal(_iciciProposal, _proposalRequest, iCICICKYCRequest, proposalRequestModel.VehicleTypeId, _leadDetails, cancellationToken);

                    decimal RecommendedIDV = 0;
                    decimal MinIDV = 0;
                    decimal MaxIDV = 0;
                    string transactionId = _iciciProposal.CorrelationId;

                    if (proposalResponse.Item1.InsurerName != null)
                    {
                        proposalResponse.Item1.InsurerId = _iCICIConfig.InsurerId;

                        string stage = string.Empty;
                        if (proposalResponse.Item1.isApprovalRequired || proposalResponse.Item1.isQuoteDeviation || proposalResponse.Item1.IsBreakIn || _leadDetails.IsSelfInspection)
                        {
                            stage = "BreakIn";
                        }
                        else
                        {
                            stage = "Proposal";
                        }

                        SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel()
                        {
                            quoteResponseModel = proposalResponse.Item1,
                            RequestBody = proposalResponse.Item2,
                            ResponseBody = proposalResponse.Item3,
                            Stage = stage,
                            InsurerId = _iCICIConfig.InsurerId,
                            LeadId = _leadDetails.LeadID,
                            MaxIDV = MaxIDV,
                            MinIDV = MinIDV,
                            RecommendedIDV = RecommendedIDV,
                            TransactionId = transactionId,
                            PolicyNumber = proposalResponse.Item1.ProposalNumber,
                            QuoteTransactionId = proposalRequestModel.QuoteTransactionID,
                            CategoryId = _leadDetails.CategoryId
                        };
                        return saveQuoteTransactionModel;
                    }
                }
            }
            return default;
        }
        private async Task<QuoteQueryModel> CVQuoteMasterMapping(GetCommercialICICIQuoteQuery query, CancellationToken cancellationToken)
        {
            var addOnId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnId} ")) : String.Empty;
            var discountId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList?.Select(x => $"{x.DiscountId} ")) : String.Empty;
            var paCoverId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverId} ")) : String.Empty;
            var accessoryId = (query.AccessoryList != null && query.AccessoryList.Any()) ? String.Join(",", query.AccessoryList?.Select(x => $"{x.AccessoryId} ")) : String.Empty;
            var discountExtensionId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList.Select(x => $"{x.DiscountExtensionId}")) : string.Empty;
            var paCoverExtensionId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList?.Select(x => $"{x.PACoverExtensionId} ")) : String.Empty;
            var addOnsExtensionId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList?.Select(x => $"{x.AddOnsExtensionId} ")) : String.Empty;

            var dateTime = DateTime.Now;
            var previousInsurer = query.PreviousPolicy == null && !string.IsNullOrEmpty(query.PreviousPolicy?.SAODInsurer) && query.PreviousPolicy?.SAODInsurer != "" ? query.PreviousPolicy.SAODInsurer : _iCICIConfig.InsurerId;

            using var connection = _applicationDBContext.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("PACoverId", paCoverId, DbType.String, ParameterDirection.Input);
            parameters.Add("AccessoryId", accessoryId, DbType.String, ParameterDirection.Input);
            parameters.Add("AddonId", addOnId, DbType.String, ParameterDirection.Input);
            parameters.Add("DiscountId", discountId, DbType.String, ParameterDirection.Input);
            parameters.Add("InsurerId", _iCICIConfig.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("RTOId", query.RTOId, DbType.String, ParameterDirection.Input);
            parameters.Add("VariantId", query.VariantId, DbType.String, ParameterDirection.Input);
            parameters.Add("NCBId", query.PreviousPolicy?.NCBId, DbType.String, ParameterDirection.Input);
            parameters.Add("PolicyTypeId", query.PreviousPolicy?.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleTypeId", query.VehicleTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("VehicleNumber", query.VehicleNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("DiscountExtensionId", discountExtensionId, DbType.String, ParameterDirection.Input);
            parameters.Add("PACoverExtensionId", paCoverExtensionId, DbType.String, ParameterDirection.Input);
            parameters.Add("AddOnsExtensionId", addOnsExtensionId, DbType.String, ParameterDirection.Input);
            parameters.Add("SAODInsurerId", previousInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("SATPInsurerId", previousInsurer, DbType.String, ParameterDirection.Input);
            parameters.Add("IsBrandNew", query.IsBrandNewVehicle, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetICICIQuoteMasterMapping]", parameters,
                         commandType: CommandType.StoredProcedure);

            var paCoverList = result.Read<PACoverModel>();
            var accessoryList = result.Read<AccessoryModel>();
            var addOnList = result.Read<AddonsModel>();
            var discountList = result.Read<DiscountModel>();
            var discountExtensionList = result.Read<DiscountExtensionModel>();
            var paCoverExtensionList = result.Read<PACoverExtensionModel>();
            var addOnsExtensionList = result.Read<AddOnsExtensionModel>();
            var codeList = result.Read<RTOVehiclePreviousInsurerModel>();
            var configNameValueList = result.Read<ConfigNameValueModel>();

            var quoteQuery = new QuoteQueryModel
            {
                AddOns = new Domain.GoDigit.AddOns(),
                PACover = new PACover(),
                Accessories = new Domain.GoDigit.Accessories(),
                Discounts = new Discounts(),
                VehicleDetails = new VehicleDetails(),
                PreviousPolicyDetails = new PreviousPolicy(),
            };
            if (discountExtensionList.Any())
            {
                quoteQuery.VoluntaryExcess = discountExtensionList.FirstOrDefault().DiscountExtension;
            }
            if (paCoverExtensionList.Any())
            {
                quoteQuery.UnnamedPassangerAndPillonRider = paCoverExtensionList.FirstOrDefault().PACoverExtension;
            }
            if (addOnsExtensionList.Any())
            {
                quoteQuery.GeogExtension = addOnsExtensionList.FirstOrDefault().AddOnsExtension;
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
                    else if (item.AddOns.Equals("PartsDepreciation") && item.AddOns.Equals("PersonalBelongings")) //if zerodept true then personal belongings are applicable 
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
                    if (item.CoverName.Equals("UnnamedPax"))
                    {
                        quoteQuery.PACover.UnnamedPassengerId = (paCoverList?.Where(x => x.CoverName == "UnnamedPax").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsUnnamedPassenger = true;

                    }
                    if (item.CoverName.Equals("UnnamedPillionRider"))
                    {
                        quoteQuery.PACover.UnnamedPassengerId = (paCoverList?.Where(x => x.CoverName == "UnnamedPax").Select(d => d.PACoverId).FirstOrDefault());
                        quoteQuery.PACover.IsUnnamedPassenger = true;

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

            string previousInsurerCode = string.Empty;
            string ncbValue = string.Empty;
            quoteQuery.LeadId = query.LeadId;
            if (codeList.Any())
            {
                var codeData = codeList.FirstOrDefault();

                quoteQuery.CorrelationId = Guid.NewGuid();
                quoteQuery.VehicleDetails.VehicleMakeCode = codeData.VehicleMakeCode;
                quoteQuery.VehicleDetails.VehicleModelCode = codeData.VehicleModelCode;
                quoteQuery.VehicleDetails.LicensePlateNumber = codeData.LicensePlateNumber;
                quoteQuery.RTOLocationCode = codeData.RTOLocationCode;
                quoteQuery.CountryCode = codeData.CountryCode;
                quoteQuery.StateCode = codeData.RegistrationStateCode;
                quoteQuery.CityCode = codeData.CityCode;
                quoteQuery.VehicleDetails.ManufactureDate = codeData.ManufactureDate;
                quoteQuery.GSTToState = codeData.GSTToState;
                quoteQuery.VehicleDetails.RegDate = codeData.regDate;
                quoteQuery.VehicleDetails.VehicleClass = codeData.vehicleclass;
                quoteQuery.VehicleDetails.VehicleType = codeData.VehicleType;
                quoteQuery.VehicleDetails.Chassis = codeData.chassis != null ? codeData.chassis : "12345CHS";
                quoteQuery.VehicleDetails.EngineNumber = codeData.engine != null ? codeData.engine : "12345ENG";
                quoteQuery.VehicleNumber = query.VehicleNumber;
                //quoteQuery.PACover.IsUnnamedOWNERDRIVER = codeData.PreviousPolicyType == "SAOD" ? false : true;
                quoteQuery.CurrentPolicyType = codeData.CurrentPolicyType;
                quoteQuery.PlanType = codeData.PlanType;
                ncbValue = codeData.NCBValue;
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
                        IsClaimInLastYear = query.PreviousPolicy.IsPreviousYearClaim,
                        PreviousNoClaimBonus = ncbValue
                    };
                }
                quoteQuery.PreviousPolicyDetails.PreviousPolicyType = codeList.FirstOrDefault().PreviousPolicyType;
                quoteQuery.PreviousPolicyDetails.PreviousSAODInsurer = codeList.FirstOrDefault().SAODInsurer;
                if (quoteQuery.PreviousPolicyDetails.PreviousPolicyType.Equals("SATP"))
                {
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = query.PolicyDates.TPPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd") : null;
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = query.PolicyDates.TPPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd") : null;
                }
                else
                {
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSAOD = query.PolicyDates.ODPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd") : null;
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD = query.PolicyDates.ODPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd") : null;
                }
                quoteQuery.PreviousPolicyDetails.PreviousPolicyNumber = !string.IsNullOrEmpty(codeList.FirstOrDefault().PreviousInsurancePolicyNumber) ? codeList.FirstOrDefault().PreviousInsurancePolicyNumber : "ICICI12345";

                if (quoteQuery.PreviousPolicyDetails.PreviousPolicyType.Equals("SAOD"))
                {
                    quoteQuery.PreviousPolicyDetails.PreviousSATPInsurer = codeList.FirstOrDefault().SATPInsurer;
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP = query.PolicyDates.TPPolicyStartDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd") : null;
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP = query.PolicyDates.TPPolicyEndDate != null ? Convert.ToDateTime(query.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd") : null;
                    quoteQuery.PreviousPolicyDetails.PreviousPolicyNumberTP = codeList.FirstOrDefault().PreviousInsurancePolicyNumber != null ? codeList.FirstOrDefault().PreviousInsurancePolicyNumber : "ICICI12345";
                    quoteQuery.IsSATPMandatory = true;
                    quoteQuery.IsSAODMandatry = true;
                }
                if (codeList.FirstOrDefault().CurrentPolicyType.Equals("SATP"))
                {
                    quoteQuery.IsSATPMandatory = true;
                }
                else
                {
                    quoteQuery.IsSAODMandatry = true;
                }
            }

            quoteQuery.PolicyStartDate = Convert.ToDateTime(query.PolicyDates?.PolicyStartDate).ToString("yyyy-MM-dd");
            quoteQuery.PolicyEndDate = Convert.ToDateTime(query.PolicyDates?.PolicyEndDate).ToString("yyyy-MM-dd");
            quoteQuery.RegistrationDate = query.PolicyDates?.RegistrationDate;
            quoteQuery.VehicleODTenure = query.PolicyDates != null ? query.PolicyDates.VehicleODTenure : 0;
            quoteQuery.VehicleTPTenure = query.PolicyDates != null ? query.PolicyDates.VehicleTPTenure : 0;
            quoteQuery.IsBrandNewVehicle = query.IsBrandNewVehicle;
            quoteQuery.SelectedIDV = query.IDV;
            quoteQuery.RegistrationYear = query.RegistrationYear;
            quoteQuery.ConfigNameValueModels = configNameValueList;
            quoteQuery.VehicleDetails.IsTwoWheeler = query.PolicyDates != null ? query.PolicyDates.IsTwoWheeler : false;
            quoteQuery.VehicleDetails.IsFourWheeler = query.PolicyDates != null ? query.PolicyDates.IsFourWheeler : false;

            quoteQuery.VehicleDetails.IsCommercial = query.PolicyDates != null ? query.PolicyDates.IsCommercial : false;

            return quoteQuery;
        }
    }
}
