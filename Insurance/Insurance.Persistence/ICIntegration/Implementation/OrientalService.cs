using DnsClient.Internal;
using DocumentFormat.OpenXml.Wordprocessing;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Features.Oriental.Command.CKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Oriental;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using ThirdPartyUtilities.Helpers;
using static System.Net.Mime.MediaTypeNames;

namespace Insurance.Persistence.ICIntegration.Implementation;

public class OrientalService : IOrientalService
{
    private readonly ILogger<OrientalService> _logger;
    private readonly HttpClient _client;
    private readonly OrientalConfig _orientalConfig;
    private readonly IApplicationClaims _applicationClaims;
    private readonly ICommonService _commonService;
    private readonly PolicyTypeConfig _policyTypeConfig;
    private const string BreakInMessage = "Insurer is currently not providing break-in services. Please retry with any other insurance partner. Please contact support for any assistance.";
    private const string KYC_SUCCESS = "KYC_SUCCESS";
    private const string POA_SUCCESS = "POA_SUCCESS";
    private const string KYC_FAILED = "KYC_FAILED";
    private const string POA_REQUIRED = "POA_REQUIRED";
    private const string MESSAGE = "Please enter correct document number or proceed with other insurer";
    private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";
    private const string FAILED = "FAILED";
    public OrientalService(ILogger<OrientalService> logger,
        HttpClient httpClient,
        IOptions<OrientalConfig> option,
        IApplicationClaims applicationClaims,
        ICommonService commonService,
        IOptions<PolicyTypeConfig> policyType)
    {
        _logger = logger;
        _client = httpClient;
        _orientalConfig = option.Value;
        _applicationClaims = applicationClaims;
        _commonService = commonService;
        _policyTypeConfig = policyType.Value;
    }
    #region Quotation
    public async Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
    {
        var quoteVm = new QuoteResponseModel();
        string requestBody = string.Empty;
        var responseBody = string.Empty;
        var engineNumber = string.Empty;
        var chassisNumber = string.Empty;
        var regNo = string.Empty;
        try
        {
            var productCode = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "productCode").Select(x => x.ConfigValue).FirstOrDefault();

            if (quoteQuery.VehicleDetails.IsTwoWheeler)
            {
                engineNumber = quoteQuery.IsBrandNewVehicle ? "BE4DH2045095" : "HA11EJGGG18133";
                chassisNumber = quoteQuery.IsBrandNewVehicle ? "MD634BE41H2D44201" : "MBLHA11ATGG07814";
                regNo = "MH-01-CL-3283";
            }
            else
            {
                engineNumber = quoteQuery.IsBrandNewVehicle ? "D4FCFM406470" : "D27030069";
                chassisNumber = quoteQuery.IsBrandNewVehicle ? "MALBM51RLFM156405" : "T57062366A09";
                regNo = "KA-29-M-4454";
            }

            var keyAndLockProtectAndConsumables = "N~N";
            if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired && quoteQuery.AddOns.IsConsumableRequired)
            {
                keyAndLockProtectAndConsumables = "Y~Y";
            }
            else if (!quoteQuery.AddOns.IsKeyAndLockProtectionRequired && quoteQuery.AddOns.IsConsumableRequired)
            {
                keyAndLockProtectAndConsumables = "N~Y";
            }
            else if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired && !quoteQuery.AddOns.IsConsumableRequired)
            {
                keyAndLockProtectAndConsumables = "Y~N";
            }

            if (_applicationClaims.GetRole().Equals("POSP"))
            {
                productCode = productCode?.Replace("PRD", "POS");
            }
            var quoteRequest = new OrientalEnvelope()
            {
                Body = new OrientalBody()
                {
                    GetQuoteMotor = new GetQuoteMotor()
                    {
                        ObjGetQuoteMotorETT = new ObjGetQuoteMotorEtt()
                        {
                            LOGINID = _orientalConfig.LoginId,
                            DLRINVNO = _orientalConfig.InvoiceNo,
                            DLRINVDT = DateTime.Now.ToString("dd-MMM-yyyy").ToUpper(),
                            PRODUCTCODE = productCode,
                            POLICYTYPE = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "policyType").Select(x => x.ConfigValue).FirstOrDefault(),
                            STARTDATE = Convert.ToDateTime(quoteQuery.PolicyStartDate).ToString("dd-MMM-yyyy").ToUpper(),
                            ENDDATE = Convert.ToDateTime(quoteQuery.PolicyEndDate).ToString("dd-MMM-yyyy").ToUpper(),
                            INSUREDNAME = "Test",
                            ADDR01 = "Test Address",
                            ADDR02 = null,
                            ADDR03 = null,
                            CITY = "AMBIKAPUR",
                            STATE = "MP",
                            PINCODE = "497001",
                            COUNTRY = _orientalConfig.Country,
                            EMAILID = "test@gmail.com",
                            MOBILENO = "9898989898",
                            TELNO = null,
                            FAXNO = null,
                            INSUREDKYCVERIFIED = 1,
                            MOUORGMEMID = null,
                            MOUORGMEMVALI = null,
                            MANUFVEHICLECODE = quoteQuery.VehicleDetails.VehicleMakeCode,//"VEH_MANF_008",
                            FUELTYPECODE = quoteQuery.VehicleDetails.FuelId,//"MFT1",
                            VEHICLECODE = quoteQuery.VehicleDetails.VehicleModelCode,//"VEH_MAK_2874",
                            VEHICLETYPECODE = quoteQuery.IsBrandNewVehicle ? "W" : "N",//"W",
                            VEHICLECLASSCODE = quoteQuery.VehicleDetails.IsTwoWheeler ? _orientalConfig.TwoWheelerVehicleClass : _orientalConfig.FourWheelerVehicleClass,
                            MANUFCODE = quoteQuery.VehicleDetails.VehicleMakeCode,//"VEH_MANF_008",
                            VEHICLEMODELCODE = quoteQuery.VehicleDetails.VehicleModelCode,//"VEH_MAK_2874",
                            TYPEOFBODYCODE = quoteQuery.VehicleDetails.VehicleSubType,//"BD4",
                            VEHICLECOLOR = "BLACK",
                            VEHICLEREGNUMBER = quoteQuery.IsBrandNewVehicle ? "NEW-1234" : regNo,
                            FIRSTREGDATE = quoteQuery.RegistrationDate.ToUpper(),//"01-SEP-2023",
                            ENGINENUMBER = engineNumber,//quoteQuery.IsBrandNewVehicle ? "D4FCFM406470" : "D27030069",
                            CHASSISNUMBER = chassisNumber,//quoteQuery.IsBrandNewVehicle ? "MALBM51RLFM156405" : "T57062366A09",
                            VEHIDV = (quoteQuery.IDVValue).ToString(), //1597895,
                            CUBICCAPACITY = quoteQuery.VehicleDetails.VehicleCubicCapacity,//1086,
                            THREEWHEELERYN = 0,
                            SEATINGCAPACITY = quoteQuery.VehicleDetails.VehicleSeatCapacity,//4,
                            VEHICLEGVW = null,
                            NOOFDRIVERS = quoteQuery.PACover.IsPaidDriver ? 1 : 0,//1,
                            RTOCODE = quoteQuery.RTOLocationCode,// "HR-26",
                            ZONECODE = 36,
                            GEOEXTCODE = quoteQuery.AddOns.IsGeoAreaExtension ? quoteQuery.GeogExtension : "",//"GEO-EXT-COD1",
                            VOLUNTARYEXCESS = quoteQuery.Discounts.IsVoluntarilyDeductible ? quoteQuery.VoluntaryExcess : "",//"PCVE1",
                            MEMBEROFAAI = quoteQuery.Discounts.IsAAMemberShip ? 1 : 0,
                            ANTITHEFTDEVICEDESC = quoteQuery.Discounts.IsAntiTheft ? 1 : 0,
                            NONELECACCESSDESC = quoteQuery.Accessories.IsNonElectrical ? "NONELEC" : "",//"NONELEC",
                            NONELECACCESSVALUE = quoteQuery.Accessories.IsNonElectrical ? quoteQuery.Accessories.NonElectricalValue : 0,//5000,
                            ELECACCESSDESC = quoteQuery.Accessories.IsElectrical ? "ELECACC" : "",//"ELECACC",
                            ELECACCESSVALUE = quoteQuery.Accessories.IsElectrical ? quoteQuery.Accessories.ElectricalValue : 0,//5000,
                            SIDECARACCESSDESC = null,
                            SIDECARSVALUE = null,
                            TRAILERDESC = null,
                            ARTITRAILERDESC = null,
                            ARTITRAILERVALUE = null,
                            PREVYRICR = null,
                            NCBDECLSUBMITYN = quoteQuery.PreviousPolicyDetails.IsClaimInLastYear ? 0 : 1,
                            NCBPERCENTAGE = !quoteQuery.IsBrandNewVehicle ? quoteQuery.PreviousPolicyDetails.PreviousNoClaimBonus : null,
                            LIMITEDTPPDYN = quoteQuery.Discounts.IsLimitedTPCoverage ? 1 : 0,
                            RALLYCOVERYN = 0,
                            NILDEPYN = quoteQuery.AddOns.IsZeroDebt ? 1 : 0,
                            CNGKITVALUE = quoteQuery.Accessories.IsCNG ? quoteQuery.Accessories.CNGValue.ToString() : null, //10000,
                            FIBRETANKVALUE = 0,
                            ALTCARBENEFIT = null,
                            PERSEFFCOVER = null,
                            NOOFPAOWNERDRIVER = quoteQuery.PACover.IsUnnamedOWNERDRIVER ? 1 : 0,
                            NOOFPANAMEDPERSONS = 0,
                            PANAMEDPERSONSSI = 0,
                            NOOFPAUNNAMEDPERSONS = quoteQuery.PACover.IsUnnamedPassenger ? 1 : 0,
                            PAUNNAMEDPERSONSSI = quoteQuery.PACover.IsUnnamedPassenger ? quoteQuery.PACover.UnnamedPassengerValue : 0,// 100000,
                            NOOFPAUNNAMEDHIRER = null,
                            NOOFLLEMPLOYEES = 0,
                            NOOFLLPAIDDRIVER = quoteQuery.PACover.IsPaidDriver ? 1 : 0,//1,
                            NOOFLLSOLDIERS = null,
                            OTHSINGLEFUELCVR = 0,
                            IMPCARWOCUSTOMSCVR = 0,
                            DRIVINGTUITIONEXTCVR = 0,
                            NOOFCOOLIES = 0,
                            NOOFCONDUCTORS = 0,
                            NOOFCLEANERS = 0,
                            TOWINGTYPE = null,
                            NOOFTRAILERSTOWED = null,
                            NOOFNFPPEMPL = null,
                            NOOFNFPPOTHTHANEMPL = null,
                            DLRPANOMINEENAME = null,
                            DLRPANOMINEEDOB = null,
                            DLRPANOMINEERELATION = null,
                            RETNTOINVOICE = quoteQuery.AddOns.IsInvoiceCoverRequired ? 1 : 0,
                            HYPOTYPE = null,
                            HYPOCOMPNAME = null,
                            HYPOCOMPADDR01 = null,
                            HYPOCOMPADDR02 = null,
                            HYPOCOMPADDR03 = null,
                            HYPOCOMPCITY = null,
                            HYPOCOMPPINCODE = null,
                            HYPOCOMPSTATE = null,
                            PAYMENTTYPE = _orientalConfig.PaymentOption,
                            EXISPOLFMOTHERINSR = 0,
                            IPADDRESS = null,
                            MACADDRESS = null,
                            WINUSERID = null,
                            WINMACHINEID = null,
                            DISCOUNTPERC = quoteQuery.DiscountPercentage,
                            FLEX03 = quoteQuery.RegistrationYear,//2023,
                            FLEX04 = quoteQuery.Accessories.IsCNG ? "Y" : null,
                            FLEX12 = "0",//quoteQuery.AddOns.IsZeroDebt ? "20" : null,
                            FLEX19 = _applicationClaims.GetRole().Equals("POSP") ? "PS0000000002" : null,
                            FLEX20 = quoteQuery.PACover.IsUnnamedOWNERDRIVER ? "N" : "Y",//"Y",
                            FLEX21 = quoteQuery.AddOns.IsEngineProtectionRequired ? "Y" : "N",
                            FLEX22 = quoteQuery.PACover.IsUnnamedOWNERDRIVER && quoteQuery.IsBrandNewVehicle ? 1 : 0,
                            FLEX24 = quoteQuery.CurrentPolicyType.Equals("SAOD") ? $"{quoteQuery.PreviousPolicyDetails.PreviousSAODInsurer}~{quoteQuery.PreviousPolicyDetails.PreviousPolicyNumberTP}~{Convert.ToDateTime(quoteQuery.PreviousPolicyDetails?.PreviousPolicyStartDateSAOD).ToString("dd-MMM-yyyy").ToUpper()}~{Convert.ToDateTime(quoteQuery.PreviousPolicyDetails?.PreviousPolicyExpiryDateSAOD).ToString("dd-MMM-yyyy").ToUpper()}" : string.Empty,
                            FLEX25 = keyAndLockProtectAndConsumables //"Y~Y"
                        }
                    }
                }
            };

            if (!quoteQuery.IsBrandNewVehicle)
            {
                quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.PREVINSUCOMPANY = quoteQuery.PreviousPolicyDetails.PreviousSAODInsurer;
                quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.PREVPOLNUMBER = quoteQuery.PolicyTypeId.Equals(_policyTypeConfig.SAOD) ? quoteQuery.PreviousPolicyDetails.PreviousPolicyNumberTP : quoteQuery.PreviousPolicyDetails.PreviousPolicyNumber;
                quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.PREVPOLSTARTDATE = !string.IsNullOrEmpty(quoteQuery.PreviousPolicyDetails?.PreviousPolicyStartDateSAOD) ?
                Convert.ToDateTime(quoteQuery.PreviousPolicyDetails?.PreviousPolicyStartDateSAOD).ToString("dd-MMM-yyyy").ToUpper() : Convert.ToDateTime(quoteQuery.PreviousPolicyDetails?.PreviousPolicyStartDateSATP).ToString("dd-MMM-yyyy").ToUpper();
                quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.PREVPOLENDDATE = !string.IsNullOrEmpty(quoteQuery.PreviousPolicyDetails?.PreviousPolicyExpiryDateSAOD) ? Convert.ToDateTime(quoteQuery.PreviousPolicyDetails?.PreviousPolicyExpiryDateSAOD).ToString("dd-MMM-yyyy").ToUpper() : Convert.ToDateTime(quoteQuery.PreviousPolicyDetails?.PreviousPolicyExpiryDateSATP).ToString("dd-MMM-yyyy").ToUpper();
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(OrientalEnvelope));
            StringBuilder requestBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(requestBuilder);
            xmlSerializer.Serialize(stringWriter, quoteRequest);
            requestBody = requestBuilder.ToString();
            _logger.LogInformation("Oriental Quote request {request}", requestBody);
            return await QuoteResponseFraming(requestBody, quoteQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("Oriental Quote Error {exception}", ex.Message);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return Tuple.Create(quoteVm, requestBody, responseBody);
        }
    }
    public async Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        QuoteConfirmDetailsResponseModel quoteConfirm = new QuoteConfirmDetailsResponseModel();
        var quoteResponseVM = new QuoteConfirmResponseModel();
        var responseBody = string.Empty;
        var id = 0;
        ServiceTaxModel tax = new ServiceTaxModel();
        string commonResponse = quoteTransactionDbModel.QuoteTransactionRequest.CommonResponse;
        QuoteResponseModel updatedResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(commonResponse);
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(OrientalEnvelope));
        XmlSerializer responsexmlSerializer = new XmlSerializer(typeof(QuoteResponseEnvelope));
        StringReader reader = new StringReader(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody.ToString());
        var requestframing = (OrientalEnvelope)(xmlSerializer.Deserialize(reader));
        string requestBody = string.Empty;
        bool isBreakin = false;
        bool isSelfInspection = false;
        bool isPolicyExpired = false;
        bool policyTypeSelfInspection = false;
        var todayDate = DateTime.Now.ToString("yyyy-MM-dd");
        var responseReferance = string.Empty;
        bool isCurrPolicyEngineCover = requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.FLEX21.Equals("Y") ? true : false;
        bool isCurrPolicyPartDept = requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.NILDEPYN == 1 ? true : false;

        if (!quoteConfirmCommand.IsBrandNewVehicle)
        {
            if (quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId != null && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId != null && quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId.Equals(_policyTypeConfig.SATP) && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive))
            {
                policyTypeSelfInspection = true;
            }

            //if ((!quoteConfirmCommand.isPrevPolicyEngineCover && isCurrPolicyEngineCover) || (!quoteConfirmCommand.isPrevPolicyNilDeptCover && isCurrPolicyPartDept) || policyTypeSelfInspection)
            //{
            //    isSelfInspection = true;
            //}

            if (!quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SATP))
            {
                if (!quoteConfirmCommand.IsBrandNewVehicle && quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy)
                {
                    if (Convert.ToDateTime(quoteConfirmCommand.PreviousPolicy.SAODPolicyExpiryDate) < Convert.ToDateTime(todayDate))
                    {
                        isPolicyExpired = true;
                    }
                }
                else if (!quoteConfirmCommand.IsBrandNewVehicle && !quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy)
                {
                    isPolicyExpired = true;
                }
            }
        }

        if (!isPolicyExpired)//&& !isSelfInspection)
        {
            string manufactureYear = (quoteConfirmCommand.PolicyDates.ManufacturingDate).Substring(Math.Max(0, quoteConfirmCommand.PolicyDates.ManufacturingDate.Length - 4));
            requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.DLRINVDT = DateTime.Now.ToString("dd-MMM-yyyy").ToUpper();
            requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.FLEX03 = manufactureYear;
            requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.FIRSTREGDATE = Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("dd-MMM-yyyy").ToUpper();
            requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.STARTDATE = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("dd-MMM-yyyy").ToUpper();
            requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.ENDDATE = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyEndDate).ToString("dd-MMM-yyyy").ToUpper();

            //PACover
            requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.NOOFPAOWNERDRIVER = !quoteConfirmCommand.IsPACover ? 1 : 0;
            requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.FLEX20 = !quoteConfirmCommand.IsPACover ? "N" : "Y";
            requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.FLEX22 = quoteConfirmCommand.PACoverTenure.Equals("1") ? 0 : 1;

            if (!quoteConfirmCommand.IsBrandNewVehicle)
            {
                requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.ENGINENUMBER = quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine.Replace(" ", "");
                requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.CHASSISNUMBER = quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis.Replace(" ", "");
                requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.VEHICLEREGNUMBER = string.Join("-", VehicleNumberSplit(quoteConfirmCommand.VehicleNumber));
                requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.NCBDECLSUBMITYN = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? 0 : 1;
                requestframing.Body.GetQuoteMotor.ObjGetQuoteMotorETT.NCBPERCENTAGE = quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue;
            }

            StringBuilder requestBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(requestBuilder);
            xmlSerializer.Serialize(stringWriter, requestframing);
            requestBody = requestBuilder.ToString();
            _logger.LogError("Oriental QuoteConfirm RequestBody {responseBody}", requestBody);

            var getQuoteResponse = await GetQuoteResponse(quoteTransactionDbModel.LeadDetail.LeadID, requestBody, "QuoteConfirm", cancellationToken);

            if (!getQuoteResponse.Item1.IsSuccessStatusCode)
            {
                responseBody = getQuoteResponse.Item1.ReasonPhrase;
                id = getQuoteResponse.Item2;
                quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogError("Oriental QuoteConfirm Data not found {responseBody}", responseBody);
            }
            else
            {
                id = getQuoteResponse.Item2;
                responseBody = getQuoteResponse.Item1.Content.ReadAsStringAsync().Result.ToString();
                reader = new StringReader(responseBody);
                var response = (QuoteResponseEnvelope)(responsexmlSerializer.Deserialize(reader));

                if (response != null && response.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.ERRORCODE == "0 0 ")
                {
                    var coversList = response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult.FLEX01OUT
                        + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX02OUT
                        + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX03OUT
                        + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX04OUT
                        + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX05OUT;
                    var coversSplit = coversList.Split(",");
                    tax.totalTax = response?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.SERVICETAX;
                    updatedResponse.PolicyNumber = response?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.PROPOSALNOOUT;

                    quoteConfirm = new QuoteConfirmDetailsResponseModel()
                    {
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        InsurerName = _orientalConfig.InsurerName,
                        NewPremium = RoundOffValue(response?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.ANNUALPREMIUM),
                        InsurerId = _orientalConfig.InsurerId,
                        NCB = response?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.NCBPERCENTAGEOUT.Replace("%", ""),
                        Tax = tax,
                        TotalPremium = GetTotalPremium(coversSplit).ToString(),
                        GrossPremium = RoundOffValue(response?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.ANNUALPREMIUM),
                        IsBreakin = isBreakin,
                        IsSelfInspection = policyTypeSelfInspection,
                        IDV = Convert.ToInt32(updatedResponse.IDV),
                        MinIDV = Convert.ToInt32(updatedResponse.MinIDV),
                        MaxIDV = Convert.ToInt32(updatedResponse.MaxIDV)
                    };
                }
                else
                {
                    quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.OK;
                    quoteConfirm.ValidationMessage = BreakInMessage;
                    quoteConfirm.IsBreakin = true;
                    quoteConfirm.IsSelfInspection = true;
                    quoteConfirm.isNavigateToQuote = true;
                }
            }
        }
        else
        {
            quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.OK;
            quoteConfirm.ValidationMessage = BreakInMessage;
            quoteConfirm.IsBreakin = true;
            quoteConfirm.IsSelfInspection = true;
            quoteConfirm.isNavigateToQuote = true;
        }
        quoteResponseVM = new QuoteConfirmResponseModel()
        {
            quoteConfirmResponse = quoteConfirm,
            quoteResponse = updatedResponse,
            RequestBody = requestBody,
            ResponseBody = responseBody,
            LeadId = quoteTransactionDbModel.LeadDetail.LeadID,
            ResponseReferanceFlag = responseReferance
        };
        await UpdateICLogs(id, string.Empty, responseBody);
        return quoteResponseVM;
    }

    private async Task<Tuple<QuoteResponseModel, string, string>> QuoteResponseFraming(string requestBody, QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
    {
        QuoteResponseModel quoteVm = new QuoteResponseModel();
        XmlSerializer xmlSerializer = new XmlSerializer(typeof(QuoteResponseEnvelope));
        var responseBody = string.Empty;

        var getQuoteResponse = await GetQuoteResponse(quoteQuery.LeadId, requestBody, "Quote", cancellationToken);

        if (!getQuoteResponse.Item1.IsSuccessStatusCode)
        {
            responseBody = getQuoteResponse.Item1.ReasonPhrase;
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            _logger.LogError("Oriental Quotation Not Found {responseBody}", responseBody);
        }
        else
        {
            responseBody = getQuoteResponse.Item1.Content.ReadAsStringAsync().Result.ToString();
            StringReader reader = new StringReader(responseBody);
            var response = (QuoteResponseEnvelope)(xmlSerializer.Deserialize(reader));

            if (response != null && response.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.ERRORCODE == "0 0 ")
            {
                var coversList = response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult.FLEX01OUT
                    + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX02OUT
                    + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX03OUT
                    + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX04OUT
                    + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX05OUT;
                var coversSplit = coversList.Split(",");
                List<NameValueModel> paCoverList = SetPACoverResponse(quoteQuery, coversSplit);
                List<NameValueModel> addOnsList = SetAddOnsResponse(quoteQuery, coversSplit);
                List<NameValueModel> accessoryList = SetAccessoryResponse(quoteQuery, coversSplit);
                List<NameValueModel> discountList = SetDiscountResponse(quoteQuery, coversSplit, response);

                var tax = new ServiceTax
                {
                    totalTax = response.Body.GetQuoteMotorResponse.GetQuoteMotorResult.SERVICETAX
                };

                quoteVm = new QuoteResponseModel
                {
                    InsurerName = _orientalConfig.InsurerName,
                    InsurerStatusCode = (int)HttpStatusCode.OK,
                    SelectedIDV = quoteQuery.SelectedIDV,
                    IDV = quoteQuery.IDVValue,
                    MinIDV = quoteQuery.MinIDV,
                    MaxIDV = quoteQuery.MaxIDV,
                    Tax = tax,
                    BasicCover = new BasicCover
                    {
                        CoverList = SetBaseCover(quoteQuery.CurrentPolicyType, coversSplit)
                    },
                    PACovers = new PACovers
                    {
                        PACoverList = paCoverList
                    },
                    AddonCover = new AddonCover
                    {
                        AddonList = addOnsList
                    },
                    Discount = new Domain.GoDigit.Discount
                    {
                        DiscountList = discountList
                    },
                    AccessoriesCover = new AccessoriesCover
                    {
                        AccessoryList = accessoryList
                    },
                    NCB = response?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.NCBPERCENTAGEOUT.Replace("%", ""),
                    TotalPremium = GetTotalPremium(coversSplit).ToString(),
                    GrossPremium = RoundOffValue(response?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.ANNUALPREMIUM),
                    RTOCode = quoteQuery.RTOLocationCode,
                    PolicyStartDate = Convert.ToDateTime(quoteQuery.PolicyStartDate).ToString("dd-MMM-yyyy"),
                    Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                    PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
                    IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                    IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                    RegistrationDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                    ManufacturingDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                    VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.RTOLocationCode.Replace("-", "") : quoteQuery.VehicleNumber,
                    PolicyNumber = response?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.PROPOSALNOOUT
                };
            }
            else
            {
                quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogError("Oriental Quotation Not Found {responseBody}", responseBody);
            }
        }
        await UpdateICLogs(getQuoteResponse.Item2, string.Empty, responseBody);
        return Tuple.Create(quoteVm, getQuoteResponse.Item3, responseBody);
    }
    private async Task<Tuple<HttpResponseMessage, int, string>> GetQuoteResponse(string leadId, string requestBody, string stage, CancellationToken cancellationToken)
    {
        HttpResponseMessage quoteResponse;

        string url = _orientalConfig.BaseURL + _orientalConfig.QuoteURL;
        int id = 0;

        _client.DefaultRequestHeaders.Add("SOAPAction", _orientalConfig.SOAPAction);
        id = await InsertICLogs(requestBody, leadId, url, string.Empty, _client.DefaultRequestHeaders.ToString(), stage);
        try
        {
            quoteResponse = await _client.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "text/xml"), cancellationToken);
            return Tuple.Create(quoteResponse, id, requestBody);
        }
        catch (Exception ex)
        {
            _logger.LogError("Oriental GetQuoteResponse {exception}", ex.Message);
            await UpdateICLogs(id, string.Empty, ex.Message);
            return default;
        }
    }
    #endregion Quotation

    private async Task UpdateICLogs(int id, string applicationId, string data)
    {
        var logsModel = new LogsModel
        {
            Id = id,
            ResponseBody = data,
            ResponseTime = DateTime.Now,
            ApplicationId = applicationId
        };
        await _commonService.UpdateLogs(logsModel);
    }
    private async Task<int> InsertICLogs(string requestBody, string leadId, string api, string token, string header, string stage)
    {
        var logsModel = new LogsModel
        {
            InsurerId = _orientalConfig.InsurerId,
            RequestBody = requestBody,
            API = api,
            UserId = _applicationClaims.GetUserId(),
            Token = token,
            Headers = header,
            LeadId = leadId,
            Stage = stage
        };

        var id = await _commonService.InsertLogs(logsModel);
        return id;
    }

    private static List<NameValueModel> SetDiscountResponse(QuoteQueryModel quoteQuery, string[] coverList, QuoteResponseEnvelope quoteResponse)
    {
        var item = string.Empty;
        var premium = "0";
        List<NameValueModel> discountList = new List<NameValueModel>();
        if (quoteQuery.Discounts.IsAntiTheft)
        {
            item = FindCover(coverList, "MOT-DIS-002");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AntiTheftId,
                Name = "ARAI Approved Anti-Theft Device",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        if (quoteQuery.Discounts.IsAAMemberShip)
        {
            item = FindCover(coverList, "MOT-DIS-005");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AAMemberShipId,
                Name = "AA Membership",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        if (quoteQuery.Discounts.IsVoluntarilyDeductible)
        {
            item = FindCover(coverList, "MOT-DIS-004");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                Name = "Voluntary Deductible",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        if (!quoteQuery.CurrentPolicyType.Equals("SAOD"))
        {
            item = FindCover(coverList, "MOT-CVR-019");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.LimitedTPCoverageId,
                Name = "Limited Third Party Coverage",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        discountList.Add(new NameValueModel
        {
            Name = $"No Claim Bonus ({quoteResponse?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.NCBPERCENTAGEOUT})",
            Value = RoundOffValue(quoteResponse?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.NCBAMOUNT),
            IsApplicable = IsApplicable(quoteResponse?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.NCBAMOUNT),
        });
        return discountList;
    }
    private static List<NameValueModel> SetAccessoryResponse(QuoteQueryModel quoteQuery, string[] coverList)
    {
        var item = string.Empty;
        var premium = "0";
        List<NameValueModel> accessoryList = new List<NameValueModel>();
        if (quoteQuery.Accessories.IsCNG)
        {
            item = FindCover(coverList, "MOT-CVR-053");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover OD",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
            item = FindCover(coverList, "MOT-CVR-058");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover TP",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        if (quoteQuery.Accessories.IsElectrical)
        {
            item = FindCover(coverList, "MOT-CVR-002");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.ElectricalId,
                Name = "Electrical Accessory Cover",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        if (quoteQuery.Accessories.IsNonElectrical)
        {
            item = FindCover(coverList, "");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.NonElectricalId,
                Name = "Non-Electrical Accessory Cover",
                Value = "Included",
                IsApplicable = true
            });
        }
        return accessoryList;
    }
    private static List<NameValueModel> SetPACoverResponse(QuoteQueryModel quoteQuery, string[] coverList)
    {
        var item = string.Empty;
        var premium = "0";
        List<NameValueModel> paCoverList = new List<NameValueModel>();
        if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
        {
            item = FindCover(coverList, "MOT-CVR-010");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                Name = "PA Cover for Owner Driver",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        if (quoteQuery.PACover.IsPaidDriver)
        {
            item = FindCover(coverList, "MOT-CVR-015");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.PaidDriverId,
                Name = "PA Cover for Paid Driver",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        if (quoteQuery.PACover.IsUnnamedPassenger)
        {
            item = FindCover(coverList, "MOT-CVR-012");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedPassengerId,
                Name = "PA Cover for Unnamed Passengers",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            });
        }
        return paCoverList;
    }
    private static List<NameValueModel> SetAddOnsResponse(QuoteQueryModel quoteQuery, string[] coverList)
    {
        var item = string.Empty;
        var premium = "0";
        List<NameValueModel> addOnsList = new List<NameValueModel>();
        if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired)
        {
            item = FindCover(coverList, "MOT-CVR-154");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                Name = "Key And Lock Protection",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            }
            );
        }
        if (quoteQuery.AddOns.IsTyreProtectionRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.TyreProtectionId,
                Name = "Tyre Protection",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RoadSideAssistanceId,
                Name = "Road Side Assistance Advance",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsNCBRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.NCBId,
                Name = "No Claim Bonus Protection Protect",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsInvoiceCoverRequired)
        {
            item = FindCover(coverList, "MOT-CVR-070");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                Name = "RTI",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            }
            );
        }
        if (quoteQuery.AddOns.IsTowingRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.TowingId,
                Name = "Towing Protection",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsLossOfDownTimeRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.LossOfDownTimeId,
                Name = "Loss of Down Time Protection",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsDailyAllowance)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.DailyAllowanceId,
                Name = "Daily Allowance",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsZeroDebt)
        {
            item = FindCover(coverList, "MOT-CVR-149");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            var discountItem = FindCover(coverList, "MOT-DIS-ACN");
            var discountAmount = !string.IsNullOrEmpty(discountItem) ? discountItem?.Split("~")[1] : "0";
            premium = (Convert.ToInt32(premium) - Convert.ToInt32(discountAmount)).ToString();
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ZeroDebtId,
                Name = "Zero Dep",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            }
            );
        }
        if (quoteQuery.AddOns.IsEMIProtectorRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.EMIProtectorId,
                Name = "EMI Protection",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsConsumableRequired)
        {
            item = FindCover(coverList, "MOT-CVR-155");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ConsumableId,
                Name = "Consumables",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            }
            );
        }
        if (quoteQuery.AddOns.IsLimitedOwnPremisesRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.LimitedOwnPremisesId,
                Name = "Limited to Own Premises",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsRoadSideAssistanceRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RoadSideAssistanceId,
                Name = "Road Side Assistance",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsRimProtectionRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RimProtectionId,
                Name = "RIM Protection",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsPersonalBelongingRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.PersonalBelongingId,
                Name = "Personal Belongings",
                Value = null,
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsEngineProtectionRequired)
        {
            item = FindCover(coverList, "MOT-CVR-EPC");
            premium = !string.IsNullOrEmpty(item) ? item?.Split("~")[1] : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.EngineProtectionId,
                Name = "Engine Gearbox Protection",
                Value = RoundOffValue(premium),
                IsApplicable = IsApplicable(premium)
            }
            );
        }
        if (quoteQuery.AddOns.IsGeoAreaExtension)
        {
            var odItem = FindCover(coverList, "MOT-CVR-006");
            var odPremium = !string.IsNullOrEmpty(odItem) ? odItem?.Split("~")[1] : "0";
            var tpItem = FindCover(coverList, "MOT-CVR-051");
            var tpPremium = !string.IsNullOrEmpty(tpItem) ? tpItem?.Split("~")[1] : "0";
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.GeoAreaExtensionId,
                Name = "Geo Area Extension OD",
                Value = RoundOffValue(odPremium),
                IsApplicable = IsApplicable(odPremium)
            });
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.GeoAreaExtensionId,
                Name = "Geo Area Extension TP",
                Value = RoundOffValue(tpPremium),
                IsApplicable = IsApplicable(tpPremium)
            });
        }
        if (quoteQuery.AddOns.IsRoadSideAssistanceWiderRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RoadSideAssistanceWiderId,
                Name = "Road Side Assistance Wider",
                Value = null,
                IsApplicable = false,
            });
        }
        return addOnsList;
    }
    private static List<NameValueModel> SetBaseCover(string previousPolicy, string[] coverList)
    {
        var odItem = string.Empty;
        var odRoundOffItem = string.Empty;
        var odRoundOffPremium = string.Empty;
        var tpItem = string.Empty;
        var discountItem = string.Empty;
        var odPremium = "0";
        var tpPremium = "0";
        var discountPremium = "0";
        List<NameValueModel> baseCoverList = new List<NameValueModel>();
        if (previousPolicy.Equals("Package Comprehensive") || previousPolicy.Equals("Comprehensive Bundle"))
        {
            odItem = FindCover(coverList, "MOT-CVR-001");
            odRoundOffItem = FindCover(coverList, "MOT-OD-LOD");
            tpItem = FindCover(coverList, "MOT-CVR-007");
            discountItem = FindCover(coverList, "MOT-DLR-IMT");
            odPremium = !string.IsNullOrEmpty(odItem) ? RoundOffValue(odItem?.Split("~")[1]) : "0";
            tpPremium = !string.IsNullOrEmpty(tpItem) ? RoundOffValue(tpItem?.Split("~")[1]) : "0";
            odRoundOffPremium = !string.IsNullOrEmpty(odRoundOffItem) ? RoundOffValue(odRoundOffItem?.Split("~")[1]) : "0";
            discountPremium = !string.IsNullOrEmpty(discountItem) ? RoundOffValue(discountItem?.Split("~")[1]) : "0";
            odPremium = ((Convert.ToInt32(odPremium) + Convert.ToInt32(odRoundOffPremium)) - Convert.ToInt32(discountPremium)).ToString();
            baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value = RoundOffValue(odPremium),
                        IsApplicable = IsApplicable(odPremium)
                    },
                    new NameValueModel
                    {
                        Name = "Third Party Cover Premium",
                        Value = RoundOffValue(tpPremium),
                        IsApplicable = IsApplicable(tpPremium)
                    },
                };
        }
        if (previousPolicy.Equals("SAOD"))
        {
            odItem = FindCover(coverList, "MOT-CVR-001");
            odRoundOffItem = FindCover(coverList, "MOT-OD-LOD");
            discountItem = FindCover(coverList, "MOT-DLR-IMT");
            odPremium = !string.IsNullOrEmpty(odItem) ? RoundOffValue(odItem?.Split("~")[1]) : "0";
            odRoundOffPremium = !string.IsNullOrEmpty(odRoundOffItem) ? RoundOffValue(odRoundOffItem?.Split("~")[1]) : "0";
            discountPremium = !string.IsNullOrEmpty(discountItem) ? RoundOffValue(discountItem?.Split("~")[1]) : "0";
            odPremium = ((Convert.ToInt32(odPremium) + Convert.ToInt32(odRoundOffPremium)) - Convert.ToInt32(discountPremium)).ToString();
            baseCoverList = new List<NameValueModel>
                {
                    new NameValueModel
                    {
                        Name = "Basic Own Damage Premium",
                        Value = RoundOffValue(odPremium),
                        IsApplicable = IsApplicable(odPremium)
                    }
                };
        }
        if (previousPolicy.Equals("SATP"))
        {
            tpItem = FindCover(coverList, "MOT-CVR-007");
            tpPremium = !string.IsNullOrEmpty(tpItem) ? RoundOffValue(tpItem?.Split("~")[1]) : "0";
            baseCoverList = new List<NameValueModel>
                {
                   new NameValueModel
                   {
                        Name = "Third Party Cover Premium",
                        Value = RoundOffValue(tpPremium),
                        IsApplicable = IsApplicable(tpPremium)
                   },
                };
        }
        return baseCoverList;
    }
    private static string RoundOffValue(string value)
    {
        if (!string.IsNullOrEmpty(value) && value != "-")
        {
            decimal val = Math.Round(Convert.ToDecimal(value));
            return val.ToString();
        }
        return "0";
    }
    private static bool IsApplicable(object _val)
    {
        string val = Convert.ToString(_val);
        return !(string.IsNullOrEmpty(val) || val == "0" || Convert.ToDecimal(val) == 0);
    }
    private static string FindCover(string[] coverList, string itemCode)
    {
        foreach (var item in coverList)
        {
            if (item.Contains(itemCode))
            {
                return item;
            }
        }
        return default;
    }
    private static int GetTotalPremium(string[] coverList)
    {
        coverList = coverList.SkipLast(2).ToArray();
        var premium = 0;
        var individualPremium = 0;
        foreach (var item in coverList)
        {
            if (item.Contains("MOT-CVR-019") || item.Contains("MOT-DIS-002") || item.Contains("MOT-DIS-004") || item.Contains("MOT-DIS-005") || item.Contains("MOT-DIS-ACN") || item.Contains("MOT-DLR-IMT") || item.Contains("MOT-DIS-310"))
            {
                individualPremium = Convert.ToInt32(RoundOffValue(item.Split("~")[1]));
                premium = premium - individualPremium;
            }
            else
            {
                individualPremium = Convert.ToInt32(RoundOffValue(item.Split("~")[1]));
                premium = premium + individualPremium;
            }
        }
        return premium;
    }
    public static IEnumerable<string> VehicleNumberSplit(string input)
    {
        var words = new List<StringBuilder> { new StringBuilder() };
        for (var i = 0; i < input.Length; i++)
        {
            words[words.Count - 1].Append(input[i]);
            if (i + 1 < input.Length && char.IsLetter(input[i]) != char.IsLetter(input[i + 1]))
            {
                words.Add(new StringBuilder());
            }
        }
        return words.Select(x => x.ToString());
    }
    public async Task<Tuple<QuoteResponseModel, string, string>> GetProposal(OrientalEnvelope quoteRequest,
CreateLeadModel createLeadModel, OrientalProposalDynamicDetail proposalDynamicDetails, QuoteResponseModel commonResponse, CancellationToken cancellationToken)
    {
        var requestBody = string.Empty;
        var responseBody = string.Empty;
        var proposalVm = new QuoteResponseModel();
        var id = 0;
        var tax = new ServiceTax();
        var coversSplit = (dynamic)null;

        XmlSerializer responsexmlSerializer = new XmlSerializer(typeof(QuoteResponseEnvelope));
        XmlSerializer requestxmlSerializer = new XmlSerializer(typeof(OrientalEnvelope));
        try
        {
            //Personal Details Mapping
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.INSUREDNAME = createLeadModel.CarOwnedBy.Equals("INDIVIDUAL") ? proposalDynamicDetails.PersonalDetails.customerName : proposalDynamicDetails.PersonalDetails.companyName;
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.EMAILID = proposalDynamicDetails.PersonalDetails.emailId;
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.MOBILENO = proposalDynamicDetails.PersonalDetails.mobile;

            //Address Details Mapping
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.ADDR01 = proposalDynamicDetails.AddressDetails.addressLine1;
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.PINCODE = proposalDynamicDetails.AddressDetails.pincode;
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.CITY = proposalDynamicDetails.AddressDetails.city;
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.STATE = proposalDynamicDetails.AddressDetails.state;

            //NomineeDetails Mapping
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.DLRPANOMINEENAME = proposalDynamicDetails.NomineeDetails.nomineeName;
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.DLRPANOMINEEDOB = Convert.ToDateTime(proposalDynamicDetails.NomineeDetails.nomineeDateOfBirth).ToString("dd-MMM-yyyy").ToUpper();
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.DLRPANOMINEERELATION = proposalDynamicDetails.NomineeDetails.nomineeRelation;

            //Vehicle Details Mapping
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.ENGINENUMBER = proposalDynamicDetails.VehicleDetails.engineNumber;
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.CHASSISNUMBER = proposalDynamicDetails.VehicleDetails.chassisNumber;
            quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.VEHICLECOLOR = proposalDynamicDetails.VehicleDetails.vehicleColour;
            if (proposalDynamicDetails.VehicleDetails.isFinancier.Equals("Yes"))
            {
                quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.HYPOTYPE = "2";
                quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.HYPOCOMPNAME = proposalDynamicDetails.VehicleDetails.financer;
                quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.HYPOCOMPADDR01 = proposalDynamicDetails.VehicleDetails.financierAddressLine1;
                quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.HYPOCOMPCITY = proposalDynamicDetails.VehicleDetails.financiercity;
                quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.HYPOCOMPSTATE = proposalDynamicDetails.VehicleDetails.financierstate;
                quoteRequest.Body.GetQuoteMotor.ObjGetQuoteMotorETT.HYPOCOMPPINCODE = proposalDynamicDetails.VehicleDetails.financierpincode;
            }

            StringBuilder requestBuilder = new StringBuilder();
            StringWriter stringWriter = new StringWriter(requestBuilder);
            requestxmlSerializer.Serialize(stringWriter, quoteRequest);
            requestBody = requestBuilder.ToString();
            _logger.LogError("Oriental Proposal RequestBody {responseBody}", requestBody);

            id = await InsertICLogs(requestBody, createLeadModel.LeadID, _orientalConfig.BaseURL + _orientalConfig.QuoteURL, string.Empty, string.Empty, "Proposal");

            var getQuoteResponse = await GetQuoteResponse(createLeadModel.LeadID, requestBody, "Proposal", cancellationToken);

            if (!getQuoteResponse.Item1.IsSuccessStatusCode)
            {
                responseBody = getQuoteResponse.Item1.ReasonPhrase;
                proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogError("Oriental Proposal Not Found {responseBody}", responseBody);
            }
            else
            {
                responseBody = getQuoteResponse.Item1.Content.ReadAsStringAsync().Result.ToString();
                StringReader reader = new StringReader(responseBody);
                var response = (QuoteResponseEnvelope)(responsexmlSerializer.Deserialize(reader));

                if (response != null && response.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.ERRORCODE == "0 0 ")
                {

                    tax.totalTax = response.Body.GetQuoteMotorResponse.GetQuoteMotorResult.SERVICETAX;

                    var coversList = response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult.FLEX01OUT
                        + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX02OUT
                        + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX03OUT
                        + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX04OUT
                        + response.Body.GetQuoteMotorResponse?.GetQuoteMotorResult?.FLEX05OUT;
                    coversSplit = coversList.Split(",");

                    proposalVm = new QuoteResponseModel
                    {
                        InsurerName = _orientalConfig.InsurerName,
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        IDV = commonResponse.IDV,
                        MinIDV = commonResponse.MinIDV,
                        MaxIDV = commonResponse.MaxIDV,
                        Tax = tax,
                        NCB = commonResponse.NCB,
                        TotalPremium = GetTotalPremium(coversSplit).ToString(),
                        GrossPremium = RoundOffValue(response?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.ANNUALPREMIUM),
                        RTOCode = commonResponse.RTOCode,
                        PolicyNumber = response?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.PROPOSALNOOUT,
                        ApplicationId = response?.Body?.GetQuoteMotorResponse?.GetQuoteMotorResult?.PROPOSALNOOUT
                    };
                }
            }
            await UpdateICLogs(id, proposalVm.ApplicationId, requestBody);
            return Tuple.Create(proposalVm, requestBody, responseBody);
        }
        catch (Exception ex)
        {
            _logger.LogError("Oriental Proposal {exception}", ex.Message);
            return default;
        }
    }
    public async Task<OrientalCKYCStatusResponseModel> GetCKYCResponse(OrientalCKYCDetailsModel orientalCKYCDetailsModel, CancellationToken cancellationToken)
    {
        string responseBody = string.Empty;
        string requestBody = string.Empty;
        OrientalCKYCFetchResponseModel response = new OrientalCKYCFetchResponseModel();
        OrientalCKYCStatusResponseModel orientalCKYCStatusResponseModel = new OrientalCKYCStatusResponseModel();
        CreateLeadModel createLeadModel = new CreateLeadModel();
        var id = 0;
        SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();
        OrientalCKYCFetchRequestModel orientalCKYCFetchRequestModel = new OrientalCKYCFetchRequestModel()
        {
            idNo = orientalCKYCDetailsModel.orientalProposalDynamicDetail?.PersonalDetails.documentId,
            idType = orientalCKYCDetailsModel.orientalProposalDynamicDetail?.PersonalDetails.documentType,
            mobileNo = orientalCKYCDetailsModel.orientalProposalDynamicDetail?.PersonalDetails.mobile,
            dob = orientalCKYCDetailsModel.CustomerType.Equals("INDIVIDUAL") ? Convert.ToDateTime(orientalCKYCDetailsModel.orientalProposalDynamicDetail?.PersonalDetails.dateOfBirth).ToString("dd-MM-yyyy") : Convert.ToDateTime(orientalCKYCDetailsModel.orientalProposalDynamicDetail?.PersonalDetails.dateOfIncorporation).ToString("dd-MM-yyyy"),
            pincode = orientalCKYCDetailsModel.orientalProposalDynamicDetail?.AddressDetails?.pincode,
           entityType= orientalCKYCDetailsModel.CustomerType.Equals("INDIVIDUAL") ? "individual" : "legalEntity",
           returnOnlySearchResponse ="no"
        };
        _client.DefaultRequestHeaders.Clear();
        _client.DefaultRequestHeaders.Add("appId", _orientalConfig.CKYCAppId);
        _client.DefaultRequestHeaders.Add("appKey", _orientalConfig.CKYCAppKey);
        _client.DefaultRequestHeaders.Add("transactionId", orientalCKYCDetailsModel?.ProposalNumber);
        requestBody = JsonConvert.SerializeObject(orientalCKYCFetchRequestModel);
        _logger.LogInformation("GetCKYCResponse requestBody {requestBody}", requestBody);

        id = await InsertICLogs(requestBody, orientalCKYCDetailsModel.LeadId, _orientalConfig.CKYCsearchAndDownload, string.Empty, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "KYC");
        try
        {
            var ckycResponse = await _client.PostAsync(_orientalConfig.CKYCsearchAndDownload, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken);
            if (!ckycResponse.IsSuccessStatusCode)
            {
                responseBody = await ckycResponse.Content.ReadAsStringAsync();
                saveCKYCResponse.KYC_Status = KYC_FAILED;
                saveCKYCResponse.Message = MESSAGE;
                _logger.LogError("Oriental CKYC Not Found {responseBody}", responseBody);
            }
            else
            {
                var stream = await ckycResponse.Content.ReadAsStreamAsync(cancellationToken);
                response = stream.DeserializeFromJson<OrientalCKYCFetchResponseModel>();
                responseBody = JsonConvert.SerializeObject(response);
                _logger.LogInformation("Oriental CKYCsearchAndDownload responseBody {responseBody}", responseBody);
                if (response != null && response.statusCode.Equals("200") && response.status.Equals("success"))
                {
                    createLeadModel.CKYCstatus = KYC_SUCCESS;
                    createLeadModel.LeadName = response.result?.fullName;
                    createLeadModel.ckycNumber = response.result.ckycNo;

                    createLeadModel.PermanentAddress = new LeadAddressModel()
                    {
                        AddressType = "PRIMARY",
                        Address1 = response.result ?.address1 + response.result?.address2,
                        Address2 = response.result?.address3,
                        Address3 = response.result?.city + " " + response.result?.state,
                        Pincode = response.result?.pincode,
                    };
                    createLeadModel.CommunicationAddress = new LeadAddressModel()
                    {
                        AddressType = "SECONDARY",
                        Address1 = response.result?.corresAddress1 + response.result?.corresAddress2,
                        Address2 = response.result?.corresAddress3,
                        Address3 = response.result?.corresCity + " " + response.result?.corresState,
                        Pincode = response.result?.corresPin,
                    };
                    saveCKYCResponse.Name = response.result?.fullName;
                    saveCKYCResponse.CKYCNumber = response.result?.ckycNo;
                    saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                    saveCKYCResponse.Gender = response.result?.gender == "M" ? "Male" : "Female";
                    saveCKYCResponse.DOB = response.result?.dob;
                    saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                    saveCKYCResponse.Message = KYC_SUCCESS;
                    saveCKYCResponse.InsurerName = _orientalConfig.InsurerName;
                    saveCKYCResponse.IsDocumentUpload = false;
                }
                else
                {
                    saveCKYCResponse.KYC_Status = POA_REQUIRED;
                    saveCKYCResponse.Message = MESSAGE;
                    saveCKYCResponse.IsDocumentUpload = true;   
                }
            }
            orientalCKYCStatusResponseModel.RequestBody = requestBody;
            orientalCKYCStatusResponseModel.ResponseBody = responseBody;
            orientalCKYCStatusResponseModel.OrientalCKYCFetchResponseModel = response;
            orientalCKYCStatusResponseModel.CreateLeadModel = createLeadModel;
            orientalCKYCStatusResponseModel.SaveCKYCResponse = saveCKYCResponse;

            await UpdateICLogs(id, orientalCKYCDetailsModel.ProposalNumber, responseBody);
            return orientalCKYCStatusResponseModel;
        }
        catch(Exception ex)
        {
            _logger.LogError("Oriental CKYC {exception}", ex.Message);
            return default;
        }
    }
    public async Task<OrientalUploadCKYCStatusResponseModel> UploadCKYCDocument(OrientalCKYCCommand orientalCKYCCommand, CreateLeadModel createLeadModel,CancellationToken cancellationToken)
    {
        string responseBody = string.Empty;
        OrientalUploadCKYCStatusResponseModel orientalUploadCKYCStatusResponseModel = new OrientalUploadCKYCStatusResponseModel();
        SaveCKYCResponse cKYCResponse = new SaveCKYCResponse();
        var id = 0;

        byte[] documentFront = Convert.FromBase64String(orientalCKYCCommand.POADocumentUploadFront);
        byte[] documentBack = Convert.FromBase64String(orientalCKYCCommand.POADocumentUploadBack);

        var client = new RestClient(_orientalConfig.CKYCUploadDocument);
        var request = new RestRequest(string.Empty, Method.Post);
        request.AlwaysMultipartFormData = true;
        request.AddHeader("appid", _orientalConfig.CKYCAppId);
        request.AddHeader("appkey", _orientalConfig.CKYCAppKey);
        request.AddHeader("transactionId", createLeadModel.PolicyNumber);
        request.AddParameter("documentId", orientalCKYCCommand.ProofOfAddress);
        request.AddFile("front", documentFront, orientalCKYCCommand.POADocumentUploadFrontExtension);
        request.AddFile("back", documentBack, orientalCKYCCommand.POADocumentUploadBackExtension);

        orientalUploadCKYCStatusResponseModel.RequestBody = JsonConvert.SerializeObject(orientalCKYCCommand);

        id = await InsertICLogs(orientalUploadCKYCStatusResponseModel.RequestBody, createLeadModel.LeadID, _orientalConfig.CKYCUploadDocument, string.Empty, JsonConvert.SerializeObject(request.Parameters), "KYC");
        try
        {
            var response = await client.ExecuteAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                orientalUploadCKYCStatusResponseModel.ResponseBody = response.Content;
                cKYCResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                cKYCResponse.KYC_Status = KYC_FAILED;
                cKYCResponse.Message = ValidationMessage;
                _logger.LogError("Oriental DocumentUpdload error responseBody {responseBody}", responseBody);
            }
            else
            {
                var result = JsonConvert.DeserializeObject<OrientalUploadCKYCResponse>(response.Content);
                responseBody = JsonConvert.SerializeObject(result);
                orientalUploadCKYCStatusResponseModel.ResponseBody = responseBody;
                _logger.LogError("Oriental DocumentUpdload responseBody {responseBody}", responseBody);
                if (result.statusCode == 200 && result.result.details.front.status.Equals("success") && result.result.details.front.statusCode == 200 && result.result.details.back.status.Equals("success") && result.result.details.back.statusCode == 200)
                {
                    cKYCResponse.KYC_Status = KYC_SUCCESS;
                    cKYCResponse.Name = result?.result?.details?.front?.result?.details?.FirstOrDefault()?.fieldsExtracted?.fullName?.value;
                    cKYCResponse.InsurerStatusCode = 200;
                    cKYCResponse.ProposalId = result?.metaData.transactionId;
                }
                else
                {
                    cKYCResponse.InsurerStatusCode = result.statusCode;
                    cKYCResponse.KYC_Status = KYC_FAILED;
                    cKYCResponse.Message = MESSAGE;
                }
            }
            orientalUploadCKYCStatusResponseModel.saveCKYCResponse = cKYCResponse;
            await UpdateICLogs(id, createLeadModel.PolicyNumber, responseBody);
            return orientalUploadCKYCStatusResponseModel;
        }
        catch (Exception ex)
        {
            cKYCResponse.KYC_Status = KYC_FAILED;
            cKYCResponse.Message = MESSAGE;
            orientalUploadCKYCStatusResponseModel.saveCKYCResponse = cKYCResponse;
            _logger.LogError("Oriental CKYC Upload Document Error {exception}", ex.Message);
            return orientalUploadCKYCStatusResponseModel;
        }
    }
}