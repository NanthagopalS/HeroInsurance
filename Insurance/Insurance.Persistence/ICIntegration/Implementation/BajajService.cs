using Insurance.Core.Contracts.Common;
using Insurance.Core.Features.Bajaj.Command.CKYC;
using Insurance.Core.Features.Bajaj.Command.UploadCKYCDocument;
using Insurance.Domain.Bajaj;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using RestSharp;
using System.Data;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Persistence.ICIntegration.Implementation;
public class BajajService : IBajajService
{
    private readonly HttpClient _client;
    private readonly ILogger<BajajService> _logger;
    private readonly IOptions<BajajConfig> _config;
    private readonly BajajConfig _bajajConfig;
    private readonly ICommonService _commonService;
    private const string KYC_SUCCESS = "KYC_SUCCESS";
    private const string POA_SUCCESS = "POA_SUCCESS";
    private const string FAILED = "FAILED";
    private const string POA_REQUIRED = "POA_REQUIRED";
    private const string SUCCESS = "SUCCESS";
    private const string NOT_FOUND = "NOT_FOUND";
    private const string POI_SUCCESS = "POI SUCCESS";
    private const string POI_failed = "POI failed";
    private const string MESSAGE = "Please enter correct document number or proceed with other insurer";
    private const string SATPTwoWHeelerCode = "1806";
    private const string SATPFourWHeelerCode = "1805";
    private const string SAODTwoWHeelerCode = "1871";
    private const string SAODFourWHeelerCode = "1870";
    private readonly IApplicationClaims _applicationClaims;
    private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";
    private readonly PolicyTypeConfig _policyTypeConfig;

    public BajajService(ILogger<BajajService> logger, HttpClient client, IOptions<BajajConfig> options,
        IApplicationClaims applicationClaims, IOptions<PolicyTypeConfig> policyTypeConfig, ICommonService commonService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _config = options ?? throw new ArgumentNullException(nameof(options));
        _bajajConfig = _config.Value;
        _applicationClaims = applicationClaims;
        _policyTypeConfig = policyTypeConfig.Value;
        _commonService = commonService;
    }
    public async Task<Tuple<QuoteResponseModel, string, string, string>> GetQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
    {
        var quoteVm = new QuoteResponseModel();
        string requestBody = string.Empty;
        var responseBody = string.Empty;
        bool isVehicleAgeLessThan5Years = IsYearGreaterThanValue(Convert.ToDateTime(quoteQuery.RegistrationDate), 5);

        try
        {
            quoteVm.InsurerName = "Bajaj";
            string userId = string.Empty;
            string password = string.Empty;

            List<Paddoncoverlist> paAddonCoverList = GetPAAddonCoverRequest(quoteQuery);
            List<Accessorieslist> accessorieslists = GetAccessoriesListDynamic(quoteQuery);
            if (quoteQuery.CurrentPolicyType.Equals("SATP"))
            {
                userId = _bajajConfig.TPUserID;
                password = _bajajConfig.TPPassword;
            }
            else
            {
                userId = _bajajConfig.UserID;
                password = _bajajConfig.Password;
            }

            var bajajrequest = new BajajServiceRequestModel()
            {
                userid = userId,
                password = password,
                vehiclecode = quoteQuery.VehicleDetails.VehicleMaincode,
                city = quoteQuery.CityName,
                weomotpolicyin = new Weomotpolicyin()
                {
                    contractid = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "contractid").Select(x => x.ConfigValue).FirstOrDefault(),
                    poltype = (quoteQuery.IsBrandNewVehicle) ? "1" : (quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "poltype").Select(x => x.ConfigValue).FirstOrDefault()),
                    product4digitcode = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "product4digitcode").Select(x => x.ConfigValue).FirstOrDefault(),
                    deptcode = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "deptcode").Select(x => x.ConfigValue).FirstOrDefault(),
                    branchcode = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "branchcode").Select(x => x.ConfigValue).FirstOrDefault(),
                    termstartdate = quoteQuery.PolicyStartDate,
                    termenddate = quoteQuery.PolicyEndDate,
                    tpfintype = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "tpfintype").Select(x => x.ConfigValue).FirstOrDefault(),
                    hypo = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "hypo").Select(x => x.ConfigValue).FirstOrDefault(),
                    vehicletypecode = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "vehicletypecode").Select(x => x.ConfigValue).FirstOrDefault(),
                    vehicletype = quoteQuery.VehicleDetails.VehicleType,
                    miscvehtype = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "miscvehtype").Select(x => x.ConfigValue).FirstOrDefault(),
                    vehiclemakecode = quoteQuery.VehicleDetails.VehicleMakeCode,
                    vehiclemake = quoteQuery.VehicleDetails.VehicleMake,
                    vehiclemodelcode = quoteQuery.VehicleDetails.VehicleModelCode,
                    vehiclemodel = quoteQuery.VehicleDetails.VehicleModel,
                    vehiclesubtypecode = quoteQuery.VehicleDetails.VehicleSubTypeCode,
                    vehiclesubtype = quoteQuery.VehicleDetails.VehicleSubType,
                    fuel = quoteQuery.VehicleDetails.Fuel,
                    zone = quoteQuery.VehicleDetails.Zone,
                    engineno = quoteQuery.VehicleDetails.EngineNumber,
                    chassisno = quoteQuery.VehicleDetails.Chassis,
                    registrationno = quoteQuery.IsBrandNewVehicle ? "NEW" : quoteQuery.VehicleDetails.RegNo,
                    registrationdate = quoteQuery.RegistrationDate,
                    registrationlocation = quoteQuery.CityName,
                    regilocother = quoteQuery.CityName,
                    carryingcapacity = quoteQuery.VehicleDetails.VehicleSeatCapacity,
                    cubiccapacity = quoteQuery.VehicleDetails.VehicleCubicCapacity,
                    yearmanf = quoteQuery.RegistrationYear,
                    color = quoteQuery.VehicleDetails?.VehicleColour,
                    vehicleidv = Convert.ToDouble(quoteQuery.IDVValue),
                    ncb = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "ncb").Select(x => x.ConfigValue).FirstOrDefault(),
                    addloading = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "addloading").Select(x => x.ConfigValue).FirstOrDefault(),
                    addloadingon = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "addloadingon").Select(x => x.ConfigValue).FirstOrDefault(),
                    spdiscrate = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "spdiscrate").Select(x => x.ConfigValue).FirstOrDefault(),
                    elecacctotal = quoteQuery.Accessories.IsElectrical ? (quoteQuery.Accessories.ElectricalValue).ToString() : null,
                    nonelecacctotal = quoteQuery.Accessories.IsNonElectrical ? (quoteQuery.Accessories.NonElectricalValue).ToString() : null,
                    prvpolicyref = quoteQuery.IsBrandNewVehicle ? null : quoteQuery.PreviousPolicyDetails.PreviousPolicyNumber,
                    prvexpirydate = quoteQuery.IsBrandNewVehicle ? null : Convert.ToDateTime(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD).ToString("dd-MMM-yyyy"),
                    prvinscompany = quoteQuery.IsBrandNewVehicle ? null : quoteQuery.PreviousPolicyDetails.PreviousInsurerCode,
                    prvncb = quoteQuery.IsBrandNewVehicle ? null : quoteQuery.PreviousPolicyDetails.PreviousNoClaimBonus,
                    prvclaimstatus = (quoteQuery.PreviousPolicyDetails.IsClaimInLastYear) ? "1" : "0",
                    automembership = quoteQuery.Discounts.IsAAMemberShip ? "1" : "0",
                    partnertype = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "partnertype").Select(x => x.ConfigValue).FirstOrDefault(),
                },
                accessorieslist = accessorieslists,
                paddoncoverlist = paAddonCoverList,
                motextracover = new Motextracover()
                {
                    geogextn = string.IsNullOrEmpty(quoteQuery.GeogExtension) ? null : (quoteQuery.GeogExtension).Trim(),
                    noofpersonspa = (quoteQuery.PACover.IsUnnamedPassenger || quoteQuery.PACover.IsUnnamedPillionRider) ? Convert.ToInt32(quoteQuery.VehicleDetails.VehicleSeatCapacity) : 0,
                    suminsuredpa = (quoteQuery.PACover.IsUnnamedPassenger || quoteQuery.PACover.IsUnnamedPillionRider) ? (quoteQuery.PACover.UnnamedPassengerValue).ToString() : "0",
                    suminsuredtotalnamedpa = null,
                    cngvalue = (quoteQuery.Accessories.CNGValue).ToString(),
                    noofemployeeslle = "0",
                    noofpersonsllo = quoteQuery.PACover.IsPaidDriver ? "1" : "0",
                    fibreglassvalue = "0",
                    sidecarvalue = "0",
                    nooftrailers = "0",
                    totaltrailervalue = "0",
                    voluntaryexcess = quoteQuery.VoluntaryExcess,
                    covernoteno = "",
                    covernotedate = "",
                    subimdcode = "",
                    extrafield1 = "",
                    extrafield2 = "",
                    extrafield3 = ""
                },
                questlist = new List<Questlist>() {
                new Questlist(){  contractid="", questionref="", questionval="" }
            },
                detariffobj = new Detariffobj()
                {
                    vehpurchasetype = "",
                    vehpurchasedate = quoteQuery.RegistrationDate,
                    monthofmfg = "",
                    registrationauth = "",
                    bodytype = "",
                    goodstranstype = "",
                    natureofgoods = "",
                    othergoodsfrequency = "",
                    permittype = "",
                    roadtype = "",
                    vehdrivenby = "",
                    driverexperience = "",
                    clmhistcode = "",
                    incurredclmexpcode = "",
                    driverqualificationcode = "",
                    tacmakecode = "",
                    extcol1 = "",
                    extcol2 = "",
                    extcol3 = "",
                    extcol4 = "",
                    extcol5 = "",
                    extcol6 = "",
                    extcol7 = "",
                    extcol8 = quoteQuery.PACover.IsUnnamedOWNERDRIVER ? "MCPA" : "ACPA",
                    extcol9 = "",
                    extcol10 = isVehicleAgeLessThan5Years ? quoteQuery.AddOns.PackageName : "",
                    extcol11 = "",
                    extcol12 = "",
                    extcol13 = "",
                    extcol14 = "",
                    extcol15 = "",
                    extcol16 = "",
                    extcol17 = "",
                    extcol18 = "",
                    extcol19 = "",
                    extcol20 = "",
                    extcol21 = "",
                    extcol22 = "",
                    extcol23 = "",
                    extcol24 = quoteQuery.PACover.IsUnnamedOWNERDRIVER ? quoteQuery.IsBrandNewVehicle ? quoteQuery.VehicleDetails.IsFourWheeler ? "3" : "5" : "1" : "",
                    extcol25 = "",
                    extcol26 = "",
                    extcol27 = "",
                    extcol28 = "",
                    extcol29 = "",
                    extcol30 = "",
                    extcol31 = "",
                    extcol32 = "",
                    extcol33 = "",
                    extcol34 = "",
                    extcol35 = "",
                    extcol36 = "",
                    extcol37 = "",
                    extcol38 = "",
                    extcol39 = "",
                    extcol40 = ""

                },
                transactionid = "0",
                transactiontype = "MOTOR_WEBSERVICE",
                contactno = null
            };

            return await QuoteResponseFraming(bajajrequest, quoteQuery, quoteVm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("GoDigit Error {exception}", ex.Message);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return Tuple.Create(quoteVm, requestBody, responseBody, string.Empty);
        }
    }
    private async Task<Tuple<HttpResponseMessage, int>> GetQuoteResponse(string policyType, string leadId, string stage, BajajServiceRequestModel requestBody, CancellationToken cancellationToken)
    {
        HttpResponseMessage quoteResponse = new HttpResponseMessage();
        var id = 0;
        try
        {
            var request = JsonConvert.SerializeObject(requestBody);
            string url = string.Empty;
            if (policyType.Equals("SATP"))
            {
                requestBody.paddoncoverlist.Clear();
                url = _bajajConfig.TPQuoteURL;
            }
            else
            {
                url = _bajajConfig.QuoteURL;
            }
            id = await InsertICLogs(JsonConvert.SerializeObject(requestBody), leadId, _bajajConfig.BaseURL + url, string.Empty, string.Empty, stage);
            try
            {
                _logger.LogInformation("Bajaj GetQuoteResponse {request}", requestBody);
                quoteResponse = await _client.PostAsJsonAsync(url, requestBody, cancellationToken);
                return Tuple.Create(quoteResponse, id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Bajaj GetQuoteResponse {exception}", ex.Message);
                await UpdateICLogs(id, string.Empty, ex.Message);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Bajaj GetQuoteResponse {exception}", ex.Message);
            return default;
        }
    }
    private async Task<Tuple<QuoteResponseModel, string, string, string>> QuoteResponseFraming(BajajServiceRequestModel bajajrequest, QuoteQueryModel quoteQuery, QuoteResponseModel quoteVm, CancellationToken cancellationToken)
    {
        var responseBody = string.Empty;
        string transactionId = string.Empty;
        var responseMessage = await GetQuoteResponse(quoteQuery.CurrentPolicyType, quoteQuery.LeadId, "Quote", bajajrequest, cancellationToken);
        var appliactionId = string.Empty;
        try
        {
            var requestBody = JsonConvert.SerializeObject(bajajrequest);
            _logger.LogInformation("Bajaj QuoteResponseFraming {requestBody}", requestBody);

            if (!responseMessage.Item1.IsSuccessStatusCode)
            {
                responseBody = responseMessage.Item1.ReasonPhrase;
                quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                _logger.LogError("Data not found {responseBody}", responseBody);
            }
            else
            {
                var stream = await responseMessage.Item1.Content.ReadAsStreamAsync(cancellationToken);
                var result = stream.DeserializeFromJson<BajajServiceResponseModel>();
                responseBody = JsonConvert.SerializeObject(result);
                _logger.LogInformation("Bajaj API Quote Response {responseBody}", responseBody);
                if (result != null && result.errorcode == 0 && result.premiumdetails != null && !result.premiumdetails.netpremium.Equals("null"))
                {
                    transactionId = result.transactionid;
                    double ncbPercentage = 0;
                    appliactionId = result.transactionid;
                    quoteVm.InsurerStatusCode = (int)HttpStatusCode.OK;
                    var basicCover = SetBaseCover(quoteQuery.CurrentPolicyType, result);
                    var paCover = SetPACoverResponse(quoteQuery, result);
                    var addonCover = SetAddOnsResponse(quoteQuery, result);
                    var accessoriesCover = SetAccessoriesResponse(quoteQuery, result);
                    var discount = SetDiscountResponse(quoteQuery, result);


                    decimal idv = quoteQuery.RecommendedIDV == 0 ? Math.Round((result.premiumdetails.totaliev == "null") ? 0 : Convert.ToDecimal(result.premiumdetails.totaliev)) : quoteQuery.RecommendedIDV;
                    double maxIdv = quoteQuery.MaxIDV == 0 ? Math.Round(Convert.ToDouble(idv) * 1.2) : Convert.ToDouble(quoteQuery.MaxIDV); // 20% more from recommended

                    if (result.premiumdetails?.ncbamt != "null")
                    {
                        double odAmount = result.premiumsummerylist.Where(x => x.paramref == "OD")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                            Convert.ToDouble(result.premiumsummerylist.Where(x => x.paramref == "OD")?.Select(x => x.od)?.FirstOrDefault());

                        double elecAccAmount = result.premiumsummerylist.Where(x => x.paramref == "ELECACC")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                            Convert.ToDouble(result.premiumsummerylist.Where(x => x.paramref == "ELECACC")?.Select(x => x.od)?.FirstOrDefault());

                        double nonAccAmount = result.premiumsummerylist.Where(x => x.paramref == "NELECACC")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                            Convert.ToDouble(result.premiumsummerylist.Where(x => x.paramref == "NELECACC")?.Select(x => x.od)?.FirstOrDefault());

                        double cngAccAmount = result.premiumsummerylist.Where(x => x.paramref == "CNG")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                            Convert.ToDouble(result.premiumsummerylist.Where(x => x.paramref == "CNG")?.Select(x => x.od)?.FirstOrDefault());

                        double geogAmount = result.premiumsummerylist.Where(x => x.paramref == "GEOG")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                            Convert.ToDouble(result.premiumsummerylist.Where(x => x.paramref == "GEOG")?.Select(x => x.od)?.FirstOrDefault());

                        double aaMemberAmount = result.premiumsummerylist.Where(x => x.paramref == "AAM")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                            Convert.ToDouble(result.premiumsummerylist.Where(x => x.paramref == "AAM")?.Select(x => x.od)?.FirstOrDefault(), System.Globalization.CultureInfo.GetCultureInfo("en-US"));

                        double volexAmount = result.premiumsummerylist.Where(x => x.paramref == "VOLEX")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                            Convert.ToDouble(result.premiumsummerylist.Where(x => x.paramref == "VOLEX")?.Select(x => x.od)?.FirstOrDefault(), System.Globalization.CultureInfo.GetCultureInfo("en-US"));


                        double ncbAmount = -(Convert.ToDouble(result.premiumdetails.ncbamt, System.Globalization.CultureInfo.GetCultureInfo("en-US")));

                        ncbPercentage = Math.Round(ncbAmount / (odAmount + elecAccAmount + nonAccAmount + cngAccAmount + geogAmount + aaMemberAmount + volexAmount) * 100);
                    }

                    quoteVm = new QuoteResponseModel
                    {
                        InsurerName = "Bajaj",
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        TotalPremium = result.premiumdetails.totalpremium,
                        GrossPremium = result.premiumdetails.finalpremium,
                        SelectedIDV = (quoteQuery.IsBrandNewVehicle && quoteQuery.IDVValue == 0) ? 1 : quoteQuery.SelectedIDV,
                        IDV = idv,
                        MinIDV = idv,//same as rec idv
                        MaxIDV = Convert.ToDecimal(maxIdv),
                        NCB = ncbPercentage.ToString(),
                        Tax = new ServiceTax
                        {
                            totalTax = result.premiumdetails.servicetax
                        },
                        BasicCover = new BasicCover
                        {
                            CoverList = basicCover
                        },
                        PACovers = new PACovers
                        {
                            PACoverList = paCover
                        },
                        AddonCover = new AddonCover
                        {
                            AddonList = addonCover
                        },
                        AccessoriesCover = new AccessoriesCover
                        {
                            AccessoryList = accessoriesCover
                        },
                        Discount = new Domain.GoDigit.Discount
                        {
                            DiscountList = discount
                        },
                        RTOCode = quoteQuery.VehicleDetails.RegNo,
                        PolicyStartDate = Convert.ToDateTime(quoteQuery.PolicyStartDate).ToString("dd-MMM-yyyy"),
                        Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                        PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
                        IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                        IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                        RegistrationDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                        ManufacturingDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                        VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.VehicleDetails.RegNo : quoteQuery.VehicleNumber,
                        ApplicationId = result.transactionid
                    };
                }
                else
                {
                    quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            await UpdateICLogs(responseMessage.Item2, appliactionId, responseBody);
            return Tuple.Create(quoteVm, requestBody, responseBody, transactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError("Bajaj GetQuoteResponse {exception}", ex.Message);
            await UpdateICLogs(responseMessage.Item2, appliactionId, ex.Message);
            return default;
        }
    }
    private static List<NameValueModel> SetBaseCover(string previousPolicy, BajajServiceResponseModel result)
    {
        List<NameValueModel> baseCoverList = new List<NameValueModel>();
        decimal odPremium = (Convert.ToDecimal(result.premiumsummerylist.Where(x => x.paramref == "OD").Select(x => x.od).FirstOrDefault())) + Convert.ToDecimal((result.premiumsummerylist.Where(x => x.paramref == "COMMDISC").Select(x => x.od).FirstOrDefault()) + Convert.ToDecimal(result.premiumdetails.addloadprem));
        if (previousPolicy.Equals("Package Comprehensive") || previousPolicy.Equals("Comprehensive Bundle"))
        {
            baseCoverList = new List<NameValueModel>
            {
                new NameValueModel
                {
                    Name = "Basic Own Damage Premium",
                    Value = Convert.ToString(Math.Round(odPremium)),
                    IsApplicable=IsApplicable(Convert.ToString(Math.Round(odPremium)))
                },
                new NameValueModel
                {
                    Name="Third Party Cover Premium",
                    Value=RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "ACT").Select(x => x.act).FirstOrDefault()),
                    IsApplicable=IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "ACT").Select(x => x.act)?.FirstOrDefault())
                }
            };
        };
        if (previousPolicy.Equals("SAOD"))
        {
            baseCoverList = new List<NameValueModel>
            {
                new NameValueModel
                {
                    Name = "Basic Own Damage Premium",
                    Value = Convert.ToString(Math.Round(odPremium)),
                    IsApplicable=IsApplicable(Convert.ToString(Math.Round(odPremium)))
                },
            };
        };
        if (previousPolicy.Equals("SATP"))
        {
            baseCoverList = new List<NameValueModel>
            {
                new NameValueModel
                {
                    Name="Third Party Cover Premium",
                    Value=RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "ACT").Select(x => x.act).FirstOrDefault()),
                    IsApplicable=IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "ACT").Select(x => x.act)?.FirstOrDefault())
                }
            };
        };
        return baseCoverList;
    }
    private static List<Accessorieslist> GetAccessoriesListDynamic(QuoteQueryModel quoteQuery)
    {
        List<Accessorieslist> accessorieslists = new List<Accessorieslist>();
        Accessorieslist elecAccessory = new Accessorieslist();
        Accessorieslist nonElecAccessory = new Accessorieslist();
        if (quoteQuery.Accessories.IsElectrical)
        {
            elecAccessory.contractid = "0";
            elecAccessory.acccategorycode = "1";
            elecAccessory.acctypecode = "6";
            elecAccessory.accmake = "1";
            elecAccessory.accmodel = "0";
            elecAccessory.acciev = (quoteQuery.Accessories.ElectricalValue).ToString();
            elecAccessory.acccount = "1";
            accessorieslists.Add(elecAccessory);
        }
        if (quoteQuery.Accessories.IsNonElectrical)
        {
            nonElecAccessory.contractid = "0";
            nonElecAccessory.acccategorycode = "2";
            nonElecAccessory.acctypecode = "6";
            nonElecAccessory.accmake = "1";
            nonElecAccessory.accmodel = "0";
            nonElecAccessory.acciev = (quoteQuery.Accessories.NonElectricalValue).ToString();
            nonElecAccessory.acccount = "1";
            accessorieslists.Add(nonElecAccessory);
        }

        return accessorieslists;
    }
    private static List<NameValueModel> SetDiscountResponse(QuoteQueryModel quoteQuery, BajajServiceResponseModel result)
    {
        var discount = new List<NameValueModel>();
        if (quoteQuery.CurrentPolicyType.Equals("Package Comprehensive") || quoteQuery.CurrentPolicyType.Equals("Comprehensive Bundle"))
        {
            if (quoteQuery.Discounts.IsVoluntarilyDeductible)
            {
                discount.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                    Name = "Voluntary Deductible OD",
                    Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "VOLEX").Select(x => x.od)?.FirstOrDefault()),
                    IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "VOLEX")?.Select(x => x.od)?.FirstOrDefault())
                }
                );
                discount.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                    Name = "Voluntary Deductible TP",
                    Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "VOLEX").Select(x => x.act)?.FirstOrDefault()),
                    IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "VOLEX")?.Select(x => x.act)?.FirstOrDefault())
                }
                );
            };
        }
        else if (quoteQuery.CurrentPolicyType.Equals("SAOD"))
        {
            if (quoteQuery.Discounts.IsVoluntarilyDeductible)
            {
                discount.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                    Name = "Voluntary Deductible",
                    Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "VOLEX").Select(x => x.od)?.FirstOrDefault()),
                    IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "VOLEX")?.Select(x => x.od)?.FirstOrDefault())
                }
                );
            };
        }
        else if (quoteQuery.CurrentPolicyType.Equals("SATP"))
        {
            if (quoteQuery.Discounts.IsVoluntarilyDeductible)
            {
                discount.Add(new NameValueModel
                {
                    Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                    Name = "Voluntary Deductible",
                    Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "VOLEX").Select(x => x.od)?.FirstOrDefault()),
                    IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "VOLEX")?.Select(x => x.act)?.FirstOrDefault())
                }
                );
            };
        }

        if (quoteQuery.Discounts.IsAAMemberShip)
        {
            discount.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AAMemberShipId,
                Name = "AA Membership",
                Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "AAM").Select(x => x.od)?.FirstOrDefault()),
                IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "AAM").Select(x => x.od)?.FirstOrDefault())
            }
            );
        };
        if (quoteQuery.Discounts.IsAntiTheft)
        {
            discount.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AntiTheftId,
                Name = "ARAI Approved Anti-Theft Device",
                Value = "0",
                IsApplicable = false
            }
            );
        };
        if (quoteQuery.Discounts.IsLimitedTPCoverage)
        {
            discount.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.LimitedTPCoverageId,
                Name = "Limited Third Party Coverage",
                Value = "0",
                IsApplicable = false
            }
            );
        };
        string ncbValue = (result.premiumdetails.ncbamt == "null") ? "0" : Convert.ToString(result.premiumdetails.ncbamt);
        discount.Add(new NameValueModel
        {
            Name = "No Claim Bonus",
            Value = RoundOffValue(ncbValue),
            IsApplicable = IsApplicable(ncbValue)
        });
        return discount;
    }
    private static List<NameValueModel> SetAccessoriesResponse(QuoteQueryModel quoteQuery, BajajServiceResponseModel result)
    {
        var accessoriesCover = new List<NameValueModel>();
        if (quoteQuery.Accessories.IsCNG || quoteQuery.VehicleDetails.Fuel.Equals("C"))
        {
            accessoriesCover.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover OD",
                Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "CNG").Select(x => x.od).FirstOrDefault()),
                IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "CNG").Select(x => x.od)?.FirstOrDefault())
            }
            );
            accessoriesCover.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover TP",
                Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "CNG").Select(x => x.act).FirstOrDefault()),
                IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "CNG").Select(x => x.act)?.FirstOrDefault())
            }
            );
        };
        if (quoteQuery.Accessories.IsElectrical)
        {
            accessoriesCover.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.ElectricalId,
                Name = "Electrical Accessory Cover",
                Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "ELECACC").Select(x => x.od).FirstOrDefault()),
                IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "ELECACC").Select(x => x.od)?.FirstOrDefault())
            });
        };
        if (quoteQuery.Accessories.IsNonElectrical)
        {
            accessoriesCover.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.NonElectricalId,
                Name = "Non-Electrical Accessory Cover",
                Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "NELECACC").Select(x => x.od).FirstOrDefault()),
                IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "NELECACC").Select(x => x.od)?.FirstOrDefault())
            });
        };
        return accessoriesCover;
    }
    private static List<NameValueModel> SetAddOnsResponse(QuoteQueryModel quoteQuery, BajajServiceResponseModel result)
    {
        var addonCover = new List<NameValueModel>();
        if (!quoteQuery.CurrentPolicyType.Equals("SATP"))
        {
            if (quoteQuery.VehicleTypeId == "2d566966-5525-4ed7-bd90-bb39e8418f39")
            {
                if (quoteQuery.AddOns.PackageName == "DRIVE_ASSURE_PACK")
                {
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.RoadSideAssistanceId,
                        Name = "Road Side Assistance",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S1").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S1").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.ZeroDebtId,
                        Name = "Zero Dep",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S3").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S3").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.EngineProtectionId,
                        Name = "Engine Gearbox Protection",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S4").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S4").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                }
                else if (quoteQuery.AddOns.PackageName == "DRIVE_ASSURE_PACK_PLUS")
                {
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.RoadSideAssistanceId,
                        Name = "Road Side Assistance",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S1").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S1").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                        Name = "Key And Lock Protection",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S13").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S13").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.PersonalBelongingId,
                        Name = "Personal Belongings",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S14").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S14").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.ZeroDebtId,
                        Name = "Zero Dep",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S3").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S3").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.EngineProtectionId,
                        Name = "Engine Gearbox Protection",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S4").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S4").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                }
                else
                {
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.RoadSideAssistanceId,
                        Name = "Road Side Assistance",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S1").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S1").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                        Name = "Key And Lock Protection",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S13").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S13").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                }
                if (quoteQuery.AddOns.IsGeoAreaExtension)
                {
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.GeoAreaExtensionId,
                        Name = "Geo Area Extension OD",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "GEOG").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "GEOG").Select(x => x.od)?.FirstOrDefault())
                    });
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.GeoAreaExtensionId,
                        Name = "Geo Area Extension TP",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "GEOG").Select(x => x.act)?.FirstOrDefault()),
                        IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "GEOG").Select(x => x.act)?.FirstOrDefault())
                    });
                };
                if (quoteQuery.AddOns.IsConsumableRequired)
                {
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.TyreProtectionId,
                        Name = "Consumables",
                        Value = "0",
                        IsApplicable = false
                    });
                };
            }
            else if (quoteQuery.VehicleTypeId == "6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056")
            {
                if (quoteQuery.AddOns.PackageName == "DRIVE_ASSURE_SPOT")
                {
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.RoadSideAssistanceId,
                        Name = "Road Side Assistance",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S1").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S1").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                }
                else if (quoteQuery.AddOns.PackageName == "DRIVE_ASSURE_BASIC")
                {
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.ZeroDebtId,
                        Name = "Zero Dep",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S3").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S3").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                }
                else if (quoteQuery.AddOns.PackageName == "DRIVE_ASSURE_SILVER")
                {
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.ZeroDebtId,
                        Name = "Zero Dep",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S3").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S3").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.EngineProtectionId,
                        Name = "Engine Gearbox Protection",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S4").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S4").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.ConsumableId,
                        Name = "Consumables",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "S17").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = result.premiumsummerylist.Where(x => x.paramref == "S17").Select(x => x.od)?.FirstOrDefault() == null ? false : true
                    });
                }
                if (quoteQuery.AddOns.IsGeoAreaExtension)
                {
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.GeoAreaExtensionId,
                        Name = "Geo Area Extension OD",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "GEOG").Select(x => x.od)?.FirstOrDefault()),
                        IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "GEOG").Select(x => x.od)?.FirstOrDefault())
                    });
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.GeoAreaExtensionId,
                        Name = "Geo Area Extension TP",
                        Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "GEOG").Select(x => x.act)?.FirstOrDefault()),
                        IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "GEOG").Select(x => x.act)?.FirstOrDefault())
                    });
                };
                if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired)
                {
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                        Name = "Key And Lock Protection",
                        Value = "0",
                        IsApplicable = false
                    }
                    );
                };
                if (quoteQuery.AddOns.IsPersonalBelongingRequired)
                {
                    addonCover.Add(new NameValueModel
                    {
                        Id = quoteQuery.AddOns.PersonalBelongingId,
                        Name = "Personal Belongings",
                        Value = "0",
                        IsApplicable = false
                    }
                    );
                };
            }
            if (quoteQuery.AddOns.IsDailyAllowance)
            {
                addonCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.DailyAllowanceId,
                    Name = "Daily Allowance",
                    Value = "0",
                    IsApplicable = false
                }
                );
            };
            if (quoteQuery.AddOns.IsEMIProtectorRequired)
            {
                addonCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.EMIProtectorId,
                    Name = "EMI Protection",
                    Value = "0",
                    IsApplicable = false
                }
                );
            };
            if (quoteQuery.AddOns.IsLimitedOwnPremisesRequired)
            {
                addonCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.LimitedOwnPremisesId,
                    Name = "Limited to Own Premises",
                    Value = "0",
                    IsApplicable = false
                }
                );
            };
            if (quoteQuery.AddOns.IsLossOfDownTimeRequired)
            {
                addonCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.LossOfDownTimeId,
                    Name = "Loss of Down Time Protection",
                    Value = "0",
                    IsApplicable = false
                }
                );
            };
            if (quoteQuery.AddOns.IsNCBRequired)
            {
                addonCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.NCBId,
                    Name = "No Claim Bonus Protection",
                    Value = "0",
                    IsApplicable = false
                }
                );
            };
            if (quoteQuery.AddOns.IsInvoiceCoverRequired)
            {
                addonCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.ReturnToInvoiceIdId,
                    Name = "RTI",
                    Value = "0",
                    IsApplicable = false
                }
                );
            };
            if (quoteQuery.AddOns.IsRimProtectionRequired)
            {
                addonCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RimProtectionId,
                    Name = "RIM Protection",
                    Value = "0",
                    IsApplicable = false
                }
                );
            };
            if (quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired)
            {
                addonCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RoadSideAssistanceAdvanceId,
                    Name = "Road Side Assitance Advance",
                    Value = "0",
                    IsApplicable = false
                }
                );
            };
            if (quoteQuery.AddOns.IsRoadSideAssistanceWiderRequired)
            {
                addonCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.RoadSideAssistanceWiderId,
                    Name = "Road Side Assitance Wider",
                    Value = "0",
                    IsApplicable = false
                }
                );
            };
            if (quoteQuery.AddOns.IsTowingRequired)
            {
                addonCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.TowingId,
                    Name = "Towing Protection",
                    Value = "0",
                    IsApplicable = false
                }
                );
            };
            if (quoteQuery.AddOns.IsTyreProtectionRequired)
            {
                addonCover.Add(new NameValueModel
                {
                    Id = quoteQuery.AddOns.TyreProtectionId,
                    Name = "Tyre Protection",
                    Value = "0",
                    IsApplicable = false
                }
                );
            };
        }

        return addonCover;
    }
    private static List<NameValueModel> SetPACoverResponse(QuoteQueryModel quoteQuery, BajajServiceResponseModel result)
    {
        var paCover = new List<NameValueModel>();
        if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
        {
            paCover.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                Name = "PA Cover for Owner Driver",
                Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "PA_DFT").Select(x => x.act)?.FirstOrDefault()),
                IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "PA_DFT").Select(x => x.act)?.FirstOrDefault())
            }
            );
        };
        if (quoteQuery.PACover.IsUnnamedPassenger)
        {
            paCover.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedPassengerId,
                Name = "PA Cover for Unnamed Passengers",
                Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "PA").Select(x => x.act)?.FirstOrDefault()),
                IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "PA").Select(x => x.act)?.FirstOrDefault())
            }
            );
        };
        if (quoteQuery.PACover.IsUnnamedPillionRider)
        {
            paCover.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedPillionRiderId,
                Name = "PA Cover For Unnamed Pillion Rider",
                Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "PA").Select(x => x.act)?.FirstOrDefault()),
                IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "PA").Select(x => x.act)?.FirstOrDefault())
            }
            );
        };
        if (quoteQuery.PACover.IsPaidDriver)
        {
            paCover.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.PaidDriverId,
                Name = "PA Cover for Paid Driver",
                Value = RoundOffValue(result.premiumsummerylist.Where(x => x.paramref == "LLO").Select(x => x.act)?.FirstOrDefault()),
                IsApplicable = IsApplicable(result.premiumsummerylist.Where(x => x.paramref == "LLO").Select(x => x.act)?.FirstOrDefault())
            });
        };

        return paCover;
    }
    private static List<Paddoncoverlist> GetPAAddonCoverRequest(QuoteQueryModel quoteQuery)
    {
        var paAddonCoverList = new List<Paddoncoverlist>();
        if (quoteQuery.Accessories.IsElectrical)
        {
            paAddonCoverList.Add(new Paddoncoverlist()
            {
                paramdesc = "Electrical Accessories",
                paramref = "ELECACC"
            });
        }
        if (quoteQuery.Accessories.IsNonElectrical)
        {
            paAddonCoverList.Add(new Paddoncoverlist()
            {
                paramdesc = "Non-Electrical Accessories",
                paramref = "NELECACC"
            });
        }
        if (quoteQuery.Accessories.IsCNG)
        {
            paAddonCoverList.Add(new Paddoncoverlist()
            {
                paramdesc = "CNG",
                paramref = "CNG"
            });
        }
        if (quoteQuery.Discounts.IsVoluntarilyDeductible)
        {
            paAddonCoverList.Add(new Paddoncoverlist()
            {
                paramdesc = "VOLEX",
                paramref = "VOLEX"
            });
        }
        if (quoteQuery.PACover.IsUnnamedPassenger || quoteQuery.PACover.IsUnnamedPillionRider)
        {
            paAddonCoverList.Add(new Paddoncoverlist()
            {
                paramdesc = "PA",
                paramref = "PA"
            });
        }
        if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
        {
            paAddonCoverList.Add(new Paddoncoverlist()
            {
                paramdesc = "PA Cover For Owner-Driver",
                paramref = "PA_DFT"
            });
        }
        if (quoteQuery.PACover.IsPaidDriver)
        {
            paAddonCoverList.Add(new Paddoncoverlist()
            {
                paramdesc = "LLO",
                paramref = "LLO"
            });
        }
        if (quoteQuery.AddOns.IsGeoAreaExtension)
        {
            paAddonCoverList.Add(new Paddoncoverlist()
            {
                paramdesc = "GEOG",
                paramref = "GEOG"
            });
        }

        return paAddonCoverList;
    }
    private static bool IsApplicable(object _val)
    {
        string val = Convert.ToString(_val);
        return !(string.IsNullOrEmpty(val) || val == "0" || Convert.ToDecimal(val) == 0);
    }
    public async Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        var responseBody = string.Empty;
        string requestBody = string.Empty;
        string applicationId = string.Empty;
        try
        {
            BajajServiceRequestModel request = JsonConvert.DeserializeObject<BajajServiceRequestModel>(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody);

            request.weomotpolicyin.termstartdate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("dd-MMM-yyyy");
            request.weomotpolicyin.termenddate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyEndDate).ToString("dd-MMM-yyyy");
            request.weomotpolicyin.engineno = quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine;
            request.weomotpolicyin.chassisno = quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis;
            request.weomotpolicyin.registrationno = quoteConfirmCommand.IsBrandNewVehicle ? "NEW" : quoteConfirmCommand.VehicleNumber;
            request.weomotpolicyin.registrationdate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.RegistrationDate).ToString("dd-MMM-yyyy");
            request.detariffobj.extcol8 = !quoteConfirmCommand.IsPACover ? "MCPA" : "ACPA";
            request.detariffobj.extcol24 = !quoteConfirmCommand.IsPACover ? quoteConfirmCommand.PACoverTenure : "0";
            request.weomotpolicyin.partnertype = quoteConfirmCommand.Customertype.Equals("INDIVIDUAL") ? "P" : "I";
            if (!quoteConfirmCommand.IsBrandNewVehicle && quoteConfirmCommand.PreviousPolicy != null)
            {
                if (quoteTransactionDbModel.QuoteConfirmDetailsModel.CurrentPolicyType.Equals("Package Comprehensive"))
                {
                    request.weomotpolicyin.prvpolicyref = !string.IsNullOrEmpty(quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP) ?
                                            quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP : quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
                    request.weomotpolicyin.prvexpirydate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("dd-MMM-yyyy");
                    request.weomotpolicyin.prvinscompany = !string.IsNullOrEmpty(quoteTransactionDbModel.QuoteConfirmDetailsModel.SATPInsurerCode) ?
                        quoteTransactionDbModel.QuoteConfirmDetailsModel.SATPInsurerCode : quoteTransactionDbModel.QuoteConfirmDetailsModel.SAODInsurerCode;
                }
                else if (quoteTransactionDbModel.QuoteConfirmDetailsModel.CurrentPolicyType.Equals("SAOD"))
                {
                    request.weomotpolicyin.prvpolicyref = !string.IsNullOrEmpty(quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber) ?
                        quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber : quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
                    request.weomotpolicyin.prvexpirydate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("dd-MMM-yyyy");
                    request.weomotpolicyin.prvinscompany = quoteTransactionDbModel.QuoteConfirmDetailsModel.SAODInsurerCode;
                }
                else if (quoteTransactionDbModel.QuoteConfirmDetailsModel.CurrentPolicyType.Equals("SATP"))
                {
                    request.weomotpolicyin.prvpolicyref = !string.IsNullOrEmpty(quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber) ?
                        quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber : quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP;
                    request.weomotpolicyin.prvexpirydate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("dd-MMM-yyyy");
                    request.weomotpolicyin.prvinscompany = !string.IsNullOrEmpty(quoteTransactionDbModel.QuoteConfirmDetailsModel.SATPInsurerCode) ?
                        quoteTransactionDbModel.QuoteConfirmDetailsModel.SATPInsurerCode : "2";
                }
                request.weomotpolicyin.prvncb = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? null : quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue;
                request.weomotpolicyin.prvclaimstatus = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? "1" : "0";
            }
            requestBody = JsonConvert.SerializeObject(request);
            _logger.LogInformation("Bajaj QuoteConfirm {requestBody}", requestBody);


            var responseMessage = await GetQuoteResponse(quoteTransactionDbModel.QuoteConfirmDetailsModel.CurrentPolicyType, quoteTransactionDbModel.LeadDetail.LeadID, "QuoteConfirm", request, cancellationToken);
            QuoteConfirmDetailsResponseModel quoteConfirm = new QuoteConfirmDetailsResponseModel();
            string commonResponse = quoteTransactionDbModel.QuoteTransactionRequest.CommonResponse;
            QuoteResponseModel updatedResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(commonResponse);

            string transactionId = string.Empty;
            var leadId = quoteTransactionDbModel.LeadDetail.LeadID;
            BajajServiceResponseModel confirmQuoteResult = new BajajServiceResponseModel();

            if (responseMessage.Item1.IsSuccessStatusCode)
            {
                var stream = await responseMessage.Item1.Content.ReadAsStreamAsync(cancellationToken);
                confirmQuoteResult = stream.DeserializeFromJson<BajajServiceResponseModel>();
                responseBody = JsonConvert.SerializeObject(confirmQuoteResult);
                _logger.LogInformation("Bajaj QuoteConfirm {responseBody}", responseBody);


                if (confirmQuoteResult != null && confirmQuoteResult.errorcode == 0 && !confirmQuoteResult.premiumdetails.Equals("null"))
                {
                    if (!confirmQuoteResult.premiumdetails.netpremium.Equals("null"))
                    {
                        applicationId = confirmQuoteResult.transactionid;
                        transactionId = confirmQuoteResult.transactionid.ToString();
                        updatedResponse.GrossPremium = confirmQuoteResult.premiumdetails.finalpremium;
                        updatedResponse.ApplicationId = confirmQuoteResult.transactionid;
                        bool policyTypeSelfInspection = false;
                        bool isSelfInspection = false;
                        bool isPrevPolicyExpired = false;
                        double ncbPercentage = 0;
                        DateTime prevPolicyExpiryDate = new DateTime();
                        DateTime currentDateTime = DateTime.UtcNow.Date;

                        if (!quoteConfirmCommand.IsBrandNewVehicle)
                        {
                            if (quoteTransactionDbModel.LeadDetail.PolicyTypeId != null && quoteTransactionDbModel.LeadDetail.PolicyTypeId.Equals("2AA7FDCA-9E36-4A8D-9583-15ADA737574B"))
                            {
                                prevPolicyExpiryDate = Convert.ToDateTime(quoteConfirmCommand.PreviousPolicy.TPPolicyExpiryDate);
                            }
                            else
                            {
                                prevPolicyExpiryDate = Convert.ToDateTime(quoteConfirmCommand.PreviousPolicy.SAODPolicyExpiryDate);
                            }
                            if (DateTime.Compare(prevPolicyExpiryDate, currentDateTime) < 0 && DateTime.Compare(prevPolicyExpiryDate, currentDateTime) != 0)
                            {
                                isPrevPolicyExpired = true;
                            }

                            string packageName = request.detariffobj.extcol10;
                            bool isPackageHavingEngPartCover = string.IsNullOrWhiteSpace(packageName) ? false :
                                ((packageName.Equals("DRIVE_ASSURE_PACK") || packageName.Equals("DRIVE_ASSURE_PACK_PLUS")
                                || packageName.Equals("DRIVE_ASSURE_SILVER") || packageName.Equals("DRIVE_ASSURE_BASIC")) ? true : false);


                            if (quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId != null && quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId.Equals("2AA7FDCA-9E36-4A8D-9583-15ADA737574B") && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals("517D8F9C-F532-4D45-8034-ABECE46693E3"))
                            {
                                policyTypeSelfInspection = true;
                            }

                            if ((!quoteConfirmCommand.isPrevPolicyEngineCover && isPackageHavingEngPartCover) || (!quoteConfirmCommand.isPrevPolicyNilDeptCover && isPackageHavingEngPartCover) || policyTypeSelfInspection || isPrevPolicyExpired)
                            {
                                isSelfInspection = true;
                            }
                        }
                        if (confirmQuoteResult.premiumdetails?.ncbamt != "null")
                        {
                            double odAmount = confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "OD")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                                    Convert.ToDouble(confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "OD")?.Select(x => x.od)?.FirstOrDefault());

                            double elecAccAmount = confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "ELECACC")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                                Convert.ToDouble(confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "ELECACC")?.Select(x => x.od)?.FirstOrDefault());

                            double nonAccAmount = confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "NELECACC")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                                Convert.ToDouble(confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "NELECACC")?.Select(x => x.od)?.FirstOrDefault());

                            double cngAccAmount = confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "CNG")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                                Convert.ToDouble(confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "CNG")?.Select(x => x.od)?.FirstOrDefault());

                            double geogAmount = confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "GEOG")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                                Convert.ToDouble(confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "GEOG")?.Select(x => x.od)?.FirstOrDefault());

                            double aaMemberAmount = confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "AAM")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                                Convert.ToDouble(confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "AAM")?.Select(x => x.od)?.FirstOrDefault(), System.Globalization.CultureInfo.GetCultureInfo("en-US"));

                            double volexAmount = confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "VOLEX")?.Select(x => x.od)?.FirstOrDefault() == null ? 0 :
                                Convert.ToDouble(confirmQuoteResult.premiumsummerylist.Where(x => x.paramref == "VOLEX")?.Select(x => x.od)?.FirstOrDefault(), System.Globalization.CultureInfo.GetCultureInfo("en-US"));

                            double ncbAmount = -(Convert.ToDouble(confirmQuoteResult.premiumdetails.ncbamt, System.Globalization.CultureInfo.GetCultureInfo("en-US")));

                            ncbPercentage = Math.Round(ncbAmount / (odAmount + elecAccAmount + nonAccAmount + cngAccAmount + geogAmount + aaMemberAmount + volexAmount) * 100);
                        }
                        quoteConfirm = new QuoteConfirmDetailsResponseModel
                        {
                            InsurerStatusCode = (int)HttpStatusCode.OK,
                            InsurerName = "Bajaj",
                            NewPremium = Math.Round(Convert.ToDecimal(confirmQuoteResult.premiumdetails.finalpremium)).ToString(),
                            InsurerId = _bajajConfig.InsurerId,
                            IDV = Convert.ToInt32(confirmQuoteResult.premiumdetails.totaliev),
                            NCB = ncbPercentage.ToString(),
                            Tax = new ServiceTaxModel
                            {
                                totalTax = Math.Round(Convert.ToDecimal(confirmQuoteResult.premiumdetails.servicetax)).ToString(),
                            },
                            TotalPremium = Math.Round(Convert.ToDecimal(confirmQuoteResult.premiumdetails.totalpremium)).ToString(),
                            GrossPremium = Math.Round(Convert.ToDecimal(confirmQuoteResult.premiumdetails.finalpremium)).ToString(),
                            IsBreakin = isSelfInspection,
                            IsSelfInspection = isSelfInspection,
                            TransactionId = transactionId
                        };
                    }
                }
                else if (confirmQuoteResult?.errorcode == 1)
                {
                    if (confirmQuoteResult.errorlist.Any())
                    {
                        quoteConfirm.ValidationMessage = confirmQuoteResult.errorlist[0].errtext;
                    }
                    else
                    {
                        quoteConfirm.ValidationMessage = ValidationMessage;
                    }
                    quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    responseBody = ValidationMessage;
                    quoteConfirm.ValidationMessage = ValidationMessage;
                    quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            else
            {
                responseBody = await responseMessage.Item1.Content.ReadAsStringAsync();
                _logger.LogInformation("Bajaj QuoteConfirm {responseBody}", responseBody);
                quoteConfirm.ValidationMessage = ValidationMessage;
                quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            }
            await UpdateICLogs(responseMessage.Item2, applicationId, responseBody);
            return Tuple.Create(quoteConfirm, updatedResponse, requestBody, responseBody, leadId, transactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return default;
        }
    }

    #region CKYC
    public async Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(BajajCKYCCommand cKYCModel, CancellationToken cancellationToken)
    {
        var responseBody = string.Empty;
        var requestJson = string.Empty;
        SaveCKYCResponse saveCKYCResponse = new();
        CreateLeadModel createLeadModel = new();
        createLeadModel.PermanentAddress = new LeadAddressModel();
        var dob = Convert.ToDateTime(cKYCModel.DateOfBirth).ToString("dd-MMM-yyyy");
        var id = 0;
        try
        {
            string token = CreateJWTToken();
            var requestBody = new BajajCKYCRequestDto
            {
                docTypeCode = cKYCModel.DocumentType,
                docNumber = cKYCModel.DocumentId,
                fieldType = _bajajConfig.CKYCData.fieldType,
                fieldValue = cKYCModel.TransactionId,
                dob = dob,
                appType = _bajajConfig.CKYCData.appType,
                productCode = _bajajConfig.CKYCData.productCode,
                sysType = _bajajConfig.CKYCData.sysType,
                locationCode = _bajajConfig.CKYCData.locationCode,
                userId = _bajajConfig.CKYCData.userId,
                kycType = _bajajConfig.CKYCData.kycType,
                customerType = cKYCModel.CustomerType,
                passportFileNumber = string.Empty,
                gender = cKYCModel.Gender,
                field1 = _bajajConfig.CKYCData.field1,
                field2 = string.Empty
            };
            requestJson = JsonConvert.SerializeObject(requestBody, Newtonsoft.Json.Formatting.Indented);

            var encryptRequest = EncryptBajajRequest(requestJson.Replace("'", "\"").ToString());
            var client = new RestClient(_bajajConfig.CKYCURL);
            var request = new RestRequest(string.Empty, Method.Post);
            request.AddHeader("Auth", $"Bearer {token}");
            request.AddHeader("BusinessCorelationId", _bajajConfig.KYCBusinessCorelationId);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{""payload"":""Encrypt_Req""}";
            body = body.Replace("Encrypt_Req", encryptRequest);
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            id = await InsertICLogs(encryptRequest, cKYCModel.LeadId, _bajajConfig.CKYCURL, token, JsonConvert.SerializeObject(request.Parameters), "KYC");
            try
            {
                _logger.LogInformation("Bajaj GetCKYCResponse {Request}{EncryptRequest}", requestJson, encryptRequest);
                var response = await client.ExecuteAsync(request, cancellationToken);

                if (response.Content.Contains("payload"))
                {
                    dynamic resp = JsonConvert.DeserializeObject(response?.Content);
                    var decryptResponse = DecryptBajajResponse(resp?.payload.ToString());
                    var cKYCResponse = JsonConvert.DeserializeObject<BajajCKYCResponseDto>(decryptResponse);
                    responseBody = JsonConvert.SerializeObject(cKYCResponse);
                    _logger.LogInformation("Bajaj GetCKYCResponse {responseBody}", responseBody);
                    await UpdateICLogs(id, cKYCModel?.TransactionId, responseBody);
                    if (cKYCResponse.errMsg.ToUpper().Equals(SUCCESS) && cKYCResponse.kycStatus.ToUpper().Equals(KYC_SUCCESS))
                    {
                        createLeadModel.LeadName = cKYCResponse.firstName + " " + cKYCResponse.middleName.Replace("  ", " ");
                        createLeadModel.DateOfIncorporation = cKYCResponse.doi;
                        createLeadModel.MiddleName = cKYCResponse.middleName;
                        createLeadModel.LastName = cKYCResponse.lastName;
                        createLeadModel.DOB = cKYCResponse.dob != null && cKYCResponse.dob != "" ? Convert.ToDateTime(cKYCResponse.dob).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) :
                            Convert.ToDateTime(dob).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                        createLeadModel.Gender = cKYCResponse.gender == "M" || cKYCResponse.title == "MR" ? "M" : "F";
                        createLeadModel.PANNumber = requestBody.docNumber;
                        createLeadModel.PermanentAddress = new LeadAddressModel()
                        {
                            AddressType = "PRIMARY",
                            Address1 = cKYCResponse.address1,
                            Address2 = cKYCResponse.address2,
                            Address3 = string.Empty,
                            Pincode = cKYCResponse.pincode
                        };
                        string address = string.Empty;

                        var addressArray = new[] { cKYCResponse.address1, cKYCResponse.address2, cKYCResponse.pincode };
                        if (addressArray[0] != null || addressArray[1] != null || addressArray[2] != null)
                        {
                            address = string.Join(",", addressArray.Where(s => !string.IsNullOrEmpty(s)));
                        }

                        saveCKYCResponse.Name = cKYCResponse.firstName + " " + cKYCResponse.middleName + " " + cKYCResponse.lastName.Replace("  ", " ");
                        saveCKYCResponse.Gender = cKYCResponse.gender == "M" || cKYCResponse.title == "MR" ? "Male" : "Female";
                        saveCKYCResponse.DOB = dob;
                        saveCKYCResponse.Address = address;
                        saveCKYCResponse.InsurerName = _bajajConfig.InsurerName;
                        saveCKYCResponse.KYC_Status = KYC_SUCCESS;
                        saveCKYCResponse.Message = KYC_SUCCESS;
                        saveCKYCResponse.IsKYCRequired = true;
                        return Tuple.Create(requestJson, responseBody, saveCKYCResponse, createLeadModel);
                    }
                    else if (cKYCResponse.errMsg.ToUpper().Equals(POI_SUCCESS) && cKYCResponse.kycStatus.ToUpper().Equals("NA"))
                    {
                        saveCKYCResponse.KYC_Status = POA_REQUIRED;
                        saveCKYCResponse.Message = POA_REQUIRED;
                        saveCKYCResponse.IsKYCRequired = true;
                        return Tuple.Create(requestJson, responseBody, saveCKYCResponse, createLeadModel);
                    }
                    else if (cKYCResponse.errMsg.Equals(POI_failed) && cKYCResponse.kycStatus.Equals(NOT_FOUND))
                    {
                        saveCKYCResponse.KYC_Status = FAILED;
                        saveCKYCResponse.Message = MESSAGE;
                        saveCKYCResponse.IsKYCRequired = true;
                        return Tuple.Create(requestJson, responseBody, saveCKYCResponse, createLeadModel);
                    }
                }
                else
                {
                    responseBody = JsonConvert.SerializeObject(response.ErrorException);
                }
                saveCKYCResponse.KYC_Status = FAILED;
                saveCKYCResponse.Message = MESSAGE;
                saveCKYCResponse.IsKYCRequired = true;
                await UpdateICLogs(id, cKYCModel?.TransactionId, responseBody);
                return Tuple.Create(requestJson, responseBody, saveCKYCResponse, createLeadModel);
            }
            catch (Exception ex)
            {
                saveCKYCResponse.KYC_Status = FAILED;
                saveCKYCResponse.Message = MESSAGE;
                await UpdateICLogs(id, cKYCModel?.TransactionId, ex.Message);
                _logger.LogError("Bajaj Ckyc Error {exception}", ex.Message);
                return Tuple.Create(requestJson, responseBody, saveCKYCResponse, createLeadModel);
            }
        }
        catch (Exception ex)
        {
            saveCKYCResponse.KYC_Status = FAILED;
            saveCKYCResponse.Message = MESSAGE;
            _logger.LogError("Bajaj Ckyc Error {exception}", ex.Message);
            return Tuple.Create(requestJson, responseBody, saveCKYCResponse, createLeadModel);
        }
    }
    public async Task<Tuple<string, string, UploadCKYCDocumentResponse, CreateLeadModel>> UploadCKYCDocument(UploadBajajCKYCDocumentCommand uploadBajajCKYCDocument, CancellationToken cancellationToken)
    {
        var responseBody = string.Empty;
        var requestBody = string.Empty;
        UploadCKYCDocumentResponse uploadCKYCDocumentResponse = new UploadCKYCDocumentResponse();
        CreateLeadModel createLeadModel = new CreateLeadModel();
        var id = 0;
        try
        {
            string token = CreateJWTToken();
            var request = new BajajCKYCDocumentUploadRequestDto
            {
                appType = _bajajConfig.CKYCData.appType,
                fieldType = _bajajConfig.CKYCData.fieldType,
                fieldValue = uploadBajajCKYCDocument.TransactionId,
                kycDocumentType = "POA",
                kycDocumentCategory = uploadBajajCKYCDocument.ProofOfAddress,
                documentNumber = uploadBajajCKYCDocument.poaDocumentId,
                documentExtension = uploadBajajCKYCDocument.poaDocumentUploadExtension,
                documentArray = uploadBajajCKYCDocument.POADocumentUpload
            };
            requestBody = JsonConvert.SerializeObject(request);
            _logger.LogInformation("Bajaj UploadCKYCDocument {requestBody}", requestBody);


            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Auth", $"Bearer {token}");
            _client.DefaultRequestHeaders.Add("BusinessCorelationId", Guid.NewGuid().ToString());
            id = await InsertICLogs(requestBody, uploadBajajCKYCDocument.LeadId, _bajajConfig.CKYCDocumentUploadURL, token, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "KYC");
            var responseMessage = await _client.PostAsJsonAsync(_bajajConfig.CKYCDocumentUploadURL, request, cancellationToken);
            try
            {
                if (!responseMessage.IsSuccessStatusCode)
                {
                    responseBody = responseMessage.ReasonPhrase;
                    _logger.LogError("Bajaj UploadCKYCDocument {responseBody}", responseBody);
                }
                else
                {
                    var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<BajajCKYCDocumentUploadResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("Bajaj UploadCKYCDocument {responseBody}", responseBody);

                    await UpdateICLogs(id, uploadBajajCKYCDocument?.TransactionId, responseBody);
                    if (result.errorCode == "0")
                    {
                        uploadCKYCDocumentResponse.CKYCStatus = POA_SUCCESS;
                        uploadCKYCDocumentResponse.Message = POA_SUCCESS;
                        return Tuple.Create(requestBody, responseBody, uploadCKYCDocumentResponse, createLeadModel);
                    }
                    else
                    {
                        uploadCKYCDocumentResponse.CKYCStatus = FAILED;
                        uploadCKYCDocumentResponse.Message = MESSAGE;
                        return Tuple.Create(requestBody, responseBody, uploadCKYCDocumentResponse, createLeadModel);
                    }
                }
                await UpdateICLogs(id, uploadBajajCKYCDocument?.TransactionId, responseBody);
                uploadCKYCDocumentResponse.CKYCStatus = FAILED;
                uploadCKYCDocumentResponse.Message = MESSAGE;
                return Tuple.Create(requestBody, responseBody, uploadCKYCDocumentResponse, createLeadModel);
            }
            catch (Exception ex)
            {
                uploadCKYCDocumentResponse.CKYCStatus = FAILED;
                uploadCKYCDocumentResponse.Message = MESSAGE;
                _logger.LogError("Bajaj Error {exception}", ex.Message);
                await UpdateICLogs(id, uploadBajajCKYCDocument?.TransactionId, ex.Message);
                return Tuple.Create(requestBody, responseBody, uploadCKYCDocumentResponse, createLeadModel);
            }
        }
        catch (Exception ex)
        {
            uploadCKYCDocumentResponse.CKYCStatus = FAILED;
            uploadCKYCDocumentResponse.Message = MESSAGE;
            _logger.LogError("Bajaj Error {exception}", ex.Message);
            return Tuple.Create(requestBody, responseBody, uploadCKYCDocumentResponse, createLeadModel);
        }
    }
    #region Proposal
    public async Task<SaveQuoteTransactionModel> CreateProposal(QuoteTransactionDbModel quoteTransactionDbModel, CancellationToken cancellationToken)
    {

        SaveQuoteTransactionModel saveQuoteTransactionModel = new SaveQuoteTransactionModel();
        string requestBody = string.Empty;
        var responseBody = string.Empty;
        string returnURL = string.Empty;
        var id = 0;
        var quoteResponse = new HttpResponseMessage();
        string url = string.Empty;
        var applicationId = string.Empty;
        try
        {
            BajajServiceResponseModel bajajServiceResponseModel = JsonConvert.DeserializeObject<BajajServiceResponseModel>(quoteTransactionDbModel.QuoteTransactionRequest?.ResponseBody);
            CreateLeadModel leadDetails = (quoteTransactionDbModel?.LeadDetail);
            BajajProposalRequest proposalRequest = JsonConvert.DeserializeObject<BajajProposalRequest>(quoteTransactionDbModel?.ProposalRequestBody);
            QuoteResponseModel commonResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(quoteTransactionDbModel.QuoteTransactionRequest?.CommonResponse);
            BajajProposalRequestDto bajajProposal = JsonConvert.DeserializeObject<BajajProposalRequestDto>(quoteTransactionDbModel.QuoteTransactionRequest?.RequestBody);
            applicationId = bajajServiceResponseModel?.transactionid.ToString();
            if (!bajajProposal.weomotpolicyin.product4digitcode.Equals(SATPTwoWHeelerCode) && !bajajProposal.weomotpolicyin.product4digitcode.Equals(SATPFourWHeelerCode) || !_bajajConfig.IsTPEnable)
            {
                returnURL = $"{_bajajConfig.ReturnURL}/{commonResponse.TransactionID}/{_applicationClaims.GetUserId()}?";
            }
            else
            {
                returnURL = $"{_bajajConfig.TPReturnURL}/{commonResponse.TransactionID}/{_applicationClaims.GetUserId()}?";
            }

            bajajProposal.paymentmode = _bajajConfig.PaymentMode;
            bajajProposal.custdetails = new Custdetails()
            {
                firstname = bajajProposal.weomotpolicyin.partnertype.Equals("P") ? proposalRequest.PersonalDetails.firstName : "",
                middlename = bajajProposal.weomotpolicyin.partnertype.Equals("P") ? proposalRequest.PersonalDetails.middleName : "",
                surname = bajajProposal.weomotpolicyin.partnertype.Equals("P") ? proposalRequest.PersonalDetails.lastName : "",
                dateofbirth = bajajProposal.weomotpolicyin.partnertype.Equals("P") ? Convert.ToDateTime(proposalRequest.PersonalDetails.dateOfBirth).ToString("dd-MMM-yyyy") :
                Convert.ToDateTime(proposalRequest.PersonalDetails.dateOfIncorporation).ToString("dd-MMM-yyyy"),
                email = proposalRequest.PersonalDetails.emailId,
                mobile = proposalRequest.PersonalDetails.mobile,
                addline1 = proposalRequest.AddressDetails.addLine1,
                addline2 = proposalRequest.AddressDetails.addLine2,
                addline3 = proposalRequest.AddressDetails.city,
                addline5 = proposalRequest.AddressDetails.state,
                pincode = proposalRequest.AddressDetails.pincode,
                poladdline1 = proposalRequest.AddressDetails.addLine1,
                poladdline2 = proposalRequest.AddressDetails.addLine2,
                poladdline3 = proposalRequest.AddressDetails.city,
                poladdline5 = proposalRequest.AddressDetails.state,
                polpincode = proposalRequest.AddressDetails.pincode,
                cptype = bajajProposal.weomotpolicyin.partnertype,
                institutionname = bajajProposal.weomotpolicyin.partnertype.Equals("I") ? proposalRequest.PersonalDetails.companyName : ""
            };
            if (leadDetails.IsBrandNew)
            {
                bajajProposal.weomotpolicyin.registrationno = "NEW";
            }
            bajajProposal.potherdetails = new Potherdetails
            {
                extra1 = "NEWPG",
            };
            bajajProposal.weomotpolicyin.tpfintype = proposalRequest.VehicleDetails.isFinancier.Equals("true") ? "1" : "0";
            bajajProposal.weomotpolicyin.hypo = proposalRequest.VehicleDetails.isFinancier.Equals("true") ? proposalRequest.VehicleDetails.financer : null;
            bajajProposal.weomotpolicyin.engineno = proposalRequest.VehicleDetails?.engineNumber;
            bajajProposal.weomotpolicyin.chassisno = proposalRequest.VehicleDetails?.chassisNumber;
            bajajProposal.detariffobj.extcol20 = returnURL;
            bajajProposal.detariffobj.extcol38 = "~" + proposalRequest.NomineeDetails.nomineeFirstName + " " + proposalRequest.NomineeDetails.nomineeLastName + "~" + proposalRequest.NomineeDetails.nomineeRelation;
            bajajProposal.detariffobj.extcol40 = _applicationClaims.GetRole().Equals("POSP") ? "" : _applicationClaims.GetPAN();
            bajajProposal.motextracover.extrafield1 = proposalRequest.AddressDetails.pincode;
            bajajProposal.transactionid = bajajServiceResponseModel.transactionid.ToString();
            bajajProposal.detariffobj.extcol37 = bajajServiceResponseModel.transactionid.ToString();
            bajajProposal.detariffobj.extcol6 = !string.IsNullOrEmpty(leadDetails.BreakinId) ? leadDetails.BreakinId : "";
            bajajProposal.weomotpolicyin.vehicleidv = Convert.ToDouble(bajajServiceResponseModel.premiumdetails.totaliev);
            if (bajajProposal.weomotpolicyin.product4digitcode == "1805" && bajajProposal.weomotpolicyin.product4digitcode == "1806")
            {
                bajajProposal.password = _bajajConfig.TPPassword;
            }
            if (bajajProposal.weomotpolicyin.product4digitcode.Equals(SAODTwoWHeelerCode) || bajajProposal.weomotpolicyin.product4digitcode.Equals(SAODFourWHeelerCode))
            {
                var tenure = Convert.ToDateTime(leadDetails.PreviousSATPPolicyStartDate).Compare(Convert.ToDateTime(leadDetails.PrevPolicyExpiryDate),
                    DateTimeInterval.Years);
                var claim = leadDetails.PrevPolicyClaims.Equals("YES") ? 1 : 0;
                bajajProposal.detariffobj.extcol36 = $"{Convert.ToDateTime(bajajProposal.weomotpolicyin.prvexpirydate).ToString("dd-MMM-yyyy")}~" +
                    $"{bajajProposal.weomotpolicyin.prvinscompany}~{quoteTransactionDbModel.QuoteTransactionRequest.PreviousSATPInsurerName}~" +
                    $"{leadDetails.PrevPolicyNumber}~{Convert.ToDateTime(leadDetails.PrevPolicyExpiryDate).ToString("dd-MMM-yyyy")}~" +
                    $"{claim}~{tenure}~{leadDetails.PreviousSATPPolicyStartDate}~";
            }
            requestBody = JsonConvert.SerializeObject(bajajProposal);
            _logger.LogInformation("Bajaj CreateProposal request {requestBody}", requestBody);

            if (!bajajProposal.weomotpolicyin.product4digitcode.Equals(SATPTwoWHeelerCode) && !bajajProposal.weomotpolicyin.product4digitcode.Equals(SATPFourWHeelerCode))
            {
                url = _bajajConfig.ProposalURL;
            }
            else
            {
                url = _bajajConfig.TPProposalURL;
            }
            id = await InsertICLogs(requestBody, quoteTransactionDbModel.LeadDetail.LeadID, url, string.Empty, string.Empty, "Proposal");

            try
            {
                quoteResponse = await _client.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                            cancellationToken);
                if (!quoteResponse.IsSuccessStatusCode)
                {
                    responseBody = quoteResponse.ReasonPhrase;
                    commonResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogError("Unable to create proposal {responseBody}", responseBody);
                }
                else
                {
                    var stream = await quoteResponse.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<BajajProposalResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("Bajaj CreateProposal response {response}", responseBody);
                    if (result.errorcode.Equals("0"))
                    {
                        commonResponse.InsurerStatusCode = 200;
                        if ((bajajProposal.weomotpolicyin.product4digitcode.Equals("1805") || bajajProposal.weomotpolicyin.product4digitcode.Equals("1806")) && _bajajConfig.IsTPEnable)
                        {
                            commonResponse.PaymentURLLink = result?.motextracover?.extrafield2;
                        }
                        else
                        {
                            commonResponse.PaymentURLLink = _bajajConfig.PaymentURL + bajajProposal.transactionid
                                + "&Username=" + _bajajConfig.UserID + "&sourceName=" + _bajajConfig.SourceName;
                        }
                        responseBody = JsonConvert.SerializeObject(result);
                        commonResponse.ApplicationId = bajajProposal.transactionid;
                        commonResponse.ProposalNumber = bajajProposal.transactionid;
                        commonResponse.IsBreakIn = false;
                        commonResponse.IsSelfInspection = false;
                        commonResponse.InsurerId = _bajajConfig.InsurerId;
                        commonResponse.PaymentStatus = "Proposal Success";
                        commonResponse.InsurerName = _bajajConfig.InsurerName;
                        commonResponse.InsurerLogo = _bajajConfig.InsurerLogo;
                        commonResponse.CustomerId = result?.partid;
                    }
                    else
                    {
                        commonResponse.ValidationMessage = result.errorlist[0].errtext;
                    }
                    saveQuoteTransactionModel.quoteResponseModel = commonResponse;
                    saveQuoteTransactionModel.RequestBody = requestBody;
                    saveQuoteTransactionModel.ResponseBody = responseBody;
                    saveQuoteTransactionModel.Stage = "Proposal";
                    saveQuoteTransactionModel.InsurerId = _bajajConfig.InsurerId;
                    saveQuoteTransactionModel.LeadId = leadDetails.LeadID;
                    saveQuoteTransactionModel.MaxIDV = Convert.ToDecimal(commonResponse.MaxIDV);
                    saveQuoteTransactionModel.MinIDV = Convert.ToDecimal(commonResponse.MinIDV);
                    saveQuoteTransactionModel.RecommendedIDV = Convert.ToDecimal(commonResponse.IDV);
                    saveQuoteTransactionModel.TransactionId = bajajProposal.transactionid;
                    saveQuoteTransactionModel.PolicyNumber = string.Empty;
                    await UpdateICLogs(id, applicationId, responseBody);
                    return saveQuoteTransactionModel;
                }
                await UpdateICLogs(id, applicationId, responseBody);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("Bajaj Create Proposal Error {exception}", ex.Message);
                await UpdateICLogs(id, applicationId, ex.Message);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Bajaj Create Proposal Error {exception}", ex.Message);
            return default;
        }
    }
    #endregion
    #region Breakin
    public async Task<Tuple<string, string, string, string>> BreakInPinGeneration(string leadId, string transactionId, string vehicleNumber, string mobileNumber, CancellationToken cancellationToken)
    {
        string requestBody = string.Empty;
        string responseBody = string.Empty;
        string breakInPin = string.Empty;
        string errorMessage = string.Empty;
        var id = 0;
        try
        {
            var split = SplitValue(vehicleNumber);
            BajajBreakInPinGenerationRequestModel requestModel = new BajajBreakInPinGenerationRequestModel()
            {
                userName = _bajajConfig.UserID,
                transactionId = transactionId,
                regNoPart1 = split[0],
                regNoPart2 = split[1],
                regNoPart3 = split[2],
                regNoPart4 = split[3],
                flag = "Y"
            };
            requestBody = JsonConvert.SerializeObject(requestModel);
            _logger.LogInformation("Bajaj BreakInPinGeneration {request}", requestBody);
            id = await InsertICLogs(requestBody, leadId, _bajajConfig.BreakInPinGenerateURL, string.Empty, string.Empty, "BreakIn");

            try
            {
                var response = await _client.PostAsync(_bajajConfig.BreakInPinGenerateURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                        cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<BreakinPinGeneration>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("Bajaj BreakInPinGeneration Response {responseBody}", responseBody);
                    await UpdateICLogs(id, transactionId, responseBody);
                    return Tuple.Create(result.pinNumber, requestBody, responseBody, result.vehicleDetails?.stringval11);
                }
                else
                {
                    responseBody = response.ReasonPhrase;
                    _logger.LogError("Bajaj BreakInPinGeneration error {responseBody}", responseBody);
                    await UpdateICLogs(id, transactionId, responseBody);
                    return Tuple.Create(breakInPin, requestBody, responseBody, errorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Bajaj BreakInPinGeneration exception {exception}", ex.Message);
                await UpdateICLogs(id, transactionId, ex.Message);
                return Tuple.Create(breakInPin, requestBody, responseBody, errorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Bajaj BreakInPinGeneration exception {exception}", ex.Message);
            return Tuple.Create(breakInPin, requestBody, responseBody, errorMessage);
        }
    }
    public async Task<BajajBreakinStatusCheckResponseModel> GetBreakinPinStatus(string leadId, string vehicleNumber, CancellationToken cancellationToken)
    {
        string requestBody = string.Empty;
        var responseBody = string.Empty;
        var id = 0;
        try
        {
            var split = SplitValue(vehicleNumber);
            BajajBreakinStatusCheckRequestModel request = new BajajBreakinStatusCheckRequestModel()
            {
                regNoPart1 = split[0],
                regNoPart2 = split[1],
                regNoPart3 = split[2],
                regNoPart4 = split[3],
            };
            requestBody = JsonConvert.SerializeObject(request);
            _logger.LogInformation("Bajaj GetBreakinPinStatus {request}", requestBody);
            id = await InsertICLogs(requestBody, leadId, _bajajConfig.BreakinPinStatusUrl, string.Empty, string.Empty, "BreakIn");

            try
            {
                var response = await _client.PostAsync(_bajajConfig.BreakinPinStatusUrl, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                        cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    responseBody = response.ReasonPhrase;
                    _logger.LogError("Get BreakinPin Status Failed {response}", response);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<BajajBreakinStatusCheckResponseModel>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("Bajaj GetBreakinPinStatus Response {responseBody}", responseBody);
                    await UpdateICLogs(id, result?.pinList[result.pinList.Length - 1].stringval1, responseBody);
                    return result;
                }
                await UpdateICLogs(id, vehicleNumber, responseBody);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("Bajaj GetBreakinPinStatus {exception}", ex.Message);
                await UpdateICLogs(id, vehicleNumber, responseBody);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Bajaj GetBreakinPinStatus {exception}", ex.Message);
            return default;
        }
    }
    #endregion
    #region Policy Generation
    public async Task<byte[]> GeneratePolicy(string leadId, string policyNumber, bool isTP)
    {
        var id = 0;
        byte[] policyArray = null;
        try
        {
            var requestBody = string.Empty;
            var responseBody = string.Empty;
            var response = new HttpResponseMessage();
            BajajPolicyGenerationRequest request = new BajajPolicyGenerationRequest()
            {
                userid = isTP ? _bajajConfig.TPUserID : _bajajConfig.UserID,
                password = isTP ? _bajajConfig.TPPassword : _bajajConfig.Password,
                pdfmode = isTP ? _bajajConfig.TPPDFMode : _bajajConfig.PDFMode,
                policynum = policyNumber
            };

            requestBody = JsonConvert.SerializeObject(request);
            _logger.LogInformation("Bajaj GeneratePolicy Request {requestBody}", requestBody);
            string url = isTP ? _bajajConfig.TPGeneratePolicyURL : _bajajConfig.GeneratePolicy;
            id = await InsertICLogs(requestBody, leadId, url, string.Empty, string.Empty, "Payment");

            try
            {
                response = await _client.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    responseBody = response.ReasonPhrase;
                    _logger.LogError("Bajaj GeneratePolicy Failed {response}", responseBody);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync();
                    var result = stream.DeserializeFromJson<BajajPolicyGenerationResponse>();
                    responseBody = JsonConvert.SerializeObject(result);
                    _logger.LogInformation("Bajaj GeneratePolicy Response {responseBody}", responseBody);
                    await UpdateICLogs(id, policyNumber, responseBody);
                    if (!isTP || !_bajajConfig.IsTPEnable)
                    {
                        if (!string.IsNullOrEmpty(result.fileByteObj))
                        {
                            policyArray = System.Convert.FromBase64String(result.fileByteObj);
                        }
                    }
                    else
                    {
                        if (result.errorcode.Equals("0") && result.errormsg.ToLower().Equals("success"))
                        {
                            policyArray = System.Convert.FromBase64String(result.fileByteObj);
                        }
                    }
                    return policyArray;
                }
                await UpdateICLogs(id, policyNumber, responseBody);
                return default;
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, policyNumber, ex.Message);
                _logger.LogError("Bajaj GeneratePolicy Error {exception}", ex.Message);
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Bajaj GeneratePolicy Error {exception}", ex.Message);
            return default;
        }
    }
    #endregion
    private static string CreateJWTToken()
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expires = DateTime.UtcNow.AddDays(1);

        var tokenHandler = new JwtSecurityTokenHandler();

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
        {
                new Claim("sub", "KYC_WS_BROKER")
        });

        tokenHandler.SetDefaultTimesOnTokenCreation = false;

        var token =
                tokenHandler.CreateJwtSecurityToken(subject: claimsIdentity,
                issuedAt: issuedAt, expires: expires);

        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }
    public string EncryptBajajRequest(string plainText)
    {
        string key = _bajajConfig.KYCEncryptionKey;
        string iV = _bajajConfig.KYCEncryptionIV;
        byte[] iv = Encoding.UTF8.GetBytes(iV); ;
        byte[] array;

        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key)
    ;
            aes.IV = iv;

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }
                    array = memoryStream.ToArray();
                }
            }
        }
        return Convert.ToBase64String(array);
    }
    private static string DecryptBajajResponse(string cipherText)
    {
        string key = "kycwsbrkmotr2023";
        string iV = "kycwsbrkmotr2023";
        byte[] iv = Encoding.UTF8.GetBytes(iV);
        byte[] buffer = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256;
            aes.Padding = PaddingMode.ISO10126;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = iv;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                    {
                        return streamReader.ReadToEnd();
                    }
                }
            }
        }
    }
    public static List<string> SplitValue(string input)
    {
        var words = new List<string> { string.Empty };
        for (var i = 0; i < input.Length; i++)
        {
            words[words.Count - 1] += input[i];
            if (i + 1 < input.Length && char.IsLetter(input[i]) != char.IsLetter(input[i + 1]))
            {
                words.Add(string.Empty);
            }
        }
        return words;
    }
    private static string RoundOffValue(string _val)
    {
        decimal val = Math.Round(Convert.ToDecimal(_val));
        return val.ToString();
    }
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
            InsurerId = _bajajConfig.InsurerId,
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
    private static bool IsYearGreaterThanValue(DateTime registrationDate, int yearCheck)
    {
        double year = (DateTime.Now - registrationDate).ToYear();
        return year <= yearCheck;
    }
    #endregion
}

