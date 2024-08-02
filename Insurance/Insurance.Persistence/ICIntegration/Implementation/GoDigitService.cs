using Insurance.Core.Contracts.Common;
using Insurance.Core.Features.GoDigit.Command.CKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Request;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Globalization;
using System.Net;
using System.Text;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;
using GoDigitRequest = Insurance.Domain.GoDigit.Request;
using GoDigitResponse = Insurance.Domain.GoDigit.Response;
using VehicleIDV = Insurance.Domain.GoDigit.Request.VehicleIDV;

namespace Insurance.Persistence.ICIntegration.Implementation;
public class GoDigitService : IGodigitService
{
    private readonly ILogger<GoDigitService> _logger;
    private readonly HttpClient _client;
    private readonly GoDigitConfig _goDigitConfig;
    private readonly IApplicationClaims _applicationClaims;
    private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";
    private readonly ICommonService _commonService;

    public GoDigitService(ILogger<GoDigitService> logger,
                         HttpClient client,
                         IOptions<GoDigitConfig> options,
                         IApplicationClaims applicationClaims,
                         ICommonService commonService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _goDigitConfig = options.Value;
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        _commonService = commonService;
    }

    public async Task<Tuple<QuoteResponseModel, string, string>> GetQuote(QuoteQueryModel quoteQuery, CancellationToken cancellationToken)
    {
        var quoteVm = new QuoteResponseModel();
        string requestBody = string.Empty;
        var responseBody = string.Empty;
        try
        {
            quoteVm.InsurerName = _goDigitConfig.InsurerName;
            GoDigitRequest.CurrentThirdPartyPolicy currentThirdPartyPolicy = null;

            if (!quoteQuery.IsBrandNewVehicle)
            {
                if (quoteQuery.PreviousPolicyDetails != null && !string.IsNullOrEmpty(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP))
                {
                    currentThirdPartyPolicy = new GoDigitRequest.CurrentThirdPartyPolicy
                    {
                        isCurrentThirdPartyPolicyActive = !(Convert.ToDateTime(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP) < DateTime.Now),
                        currentThirdPartyPolicyInsurerCode = quoteQuery.PreviousPolicyDetails.PreviousInsurerCode,
                        currentThirdPartyPolicyNumber = quoteQuery.PreviousPolicyDetails.PreviousPolicyNumber,
                        currentThirdPartyPolicyStartDateTime = quoteQuery.PreviousPolicyDetails.PreviousPolicyStartDateSATP,
                        currentThirdPartyPolicyExpiryDateTime = quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP,
                    };
                }
            }

            var goDigitRequest = new GoDigitRequestDto
            {
                enquiryId = $"PVT_CAR_PACKAGE_{DateTime.Now:yyyyMMddss}",
                contract = new GoDigitRequest.Contract
                {
                    insuranceProductCode = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "insuranceProductCode").Select(x => x.ConfigValue).FirstOrDefault(),
                    subInsuranceProductCode = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "subInsuranceProductCode").Select(x => x.ConfigValue).FirstOrDefault(),
                    startDate = quoteQuery.PolicyStartDate,
                    endDate = quoteQuery.PolicyEndDate,
                    policyHolderType = quoteQuery.ConfigNameValueModels.Where(x => x.ConfigName == "policyHolderType").Select(x => x.ConfigValue).FirstOrDefault(),
                    coverages = new GoDigitRequest.Coverages
                    {
                        voluntaryDeductible = quoteQuery.VehicleDetails.IsTwoWheeler ? "ZERO" : string.IsNullOrWhiteSpace(quoteQuery.VoluntaryExcess) ? "ZERO" : quoteQuery.VoluntaryExcess,
                        personalAccident = new GoDigitRequest.PersonalAccident
                        {
                            selection = quoteQuery.PACover.IsUnnamedOWNERDRIVER,
                            coverTerm = quoteQuery.PACover.IsUnnamedOWNERDRIVER ? quoteQuery.IsBrandNewVehicle ? quoteQuery.VehicleDetails.IsFourWheeler ? 3 : 5 : 1 : 0,
                        },
                        addons = new GoDigitRequest.Addons
                        {
                            partsDepreciation = new GoDigitRequest.PartsDepreciation { selection = quoteQuery.AddOns.IsZeroDebt },
                            engineProtection = new GoDigitRequest.EngineProtection { selection = quoteQuery.AddOns.IsEngineProtectionRequired },
                            roadSideAssistance = new GoDigitRequest.RoadSideAssistance { selection = quoteQuery.AddOns.IsRoadSideAssistanceRequired },
                            tyreProtection = new GoDigitRequest.TyreProtection { selection = quoteQuery.AddOns.IsTyreProtectionRequired },
                            rimProtection = new GoDigitRequest.RimProtection { selection = quoteQuery.AddOns.IsRimProtectionRequired },
                            consumables = new GoDigitRequest.Consumables { selection = quoteQuery.AddOns.IsConsumableRequired },
                            returnToInvoice = new GoDigitRequest.ReturnToInvoice { selection = quoteQuery.AddOns.IsInvoiceCoverRequired },
                        },
                        unnamedPA = new GoDigitRequest.UnnamedPA
                        {
                            unnamedPax = new GoDigitRequest.UnnamedPax
                            {
                                selection = quoteQuery.PACover.IsUnnamedPassenger,
                                insuredAmount = quoteQuery.PACover.UnnamedPassengerValue
                            },
                            unnamedPaidDriver = new GoDigitRequest.UnnamedPaidDriver
                            {
                                selection = quoteQuery.PACover.IsPaidDriver,
                                insuredAmount = quoteQuery.PACover.UnnamedPaidDriverValue
                            },
                            unnamedHirer = new GoDigitRequest.UnnamedHirer
                            {
                                selection = quoteQuery.PACover.IsUnnamedHirer,
                                insuredAmount = quoteQuery.PACover.UnnamedHirerValue
                            },
                            unnamedPillionRider = new GoDigitRequest.UnnamedPillionRider
                            {
                                selection = quoteQuery.PACover.IsUnnamedPillionRider,
                                insuredAmount = quoteQuery.PACover.UnnamedPassengerValue
                            },
                            unnamedCleaner = new GoDigitRequest.UnnamedCleaner
                            {
                                selection = quoteQuery.PACover.IsUnnamedCleaner,
                                insuredAmount = quoteQuery.PACover.UnnamedCleanerValue
                            },
                            unnamedConductor = new GoDigitRequest.UnnamedConductor
                            {
                                selection = quoteQuery.PACover.IsUnnamedConductor,
                                insuredAmount = quoteQuery.PACover.UnnamedConductorValue
                            }
                        },
                        accessories = new Domain.GoDigit.Request.Accessories
                        {
                            cng = new GoDigitRequest.Cng
                            {
                                selection = quoteQuery.Accessories.IsCNG,
                                insuredAmount = quoteQuery.Accessories.CNGValue
                            },
                            electrical = new GoDigitRequest.Electrical
                            {
                                selection = quoteQuery.Accessories.IsElectrical,
                                insuredAmount = quoteQuery.Accessories.ElectricalValue
                            },
                            nonElectrical = new GoDigitRequest.NonElectrical
                            {
                                selection = quoteQuery.Accessories.IsNonElectrical,
                                insuredAmount = quoteQuery.Accessories.NonElectricalValue
                            }
                        },
                        isGeoExt = quoteQuery.AddOns.IsGeoAreaExtension
                    }
                },
                vehicle = new GoDigitRequest.Vehicle
                {
                    isVehicleNew = quoteQuery.IsBrandNewVehicle,
                    vehicleMaincode = quoteQuery.VehicleDetails.VehicleMaincode,
                    licensePlateNumber = quoteQuery.VehicleDetails.LicensePlateNumber,
                    vehicleIdentificationNumber = quoteQuery.VehicleDetails.Chassis,
                    engineNumber = quoteQuery.VehicleDetails.EngineNumber,
                    manufactureDate = quoteQuery.RegistrationDate,
                    registrationDate = quoteQuery.RegistrationDate,
                    vehicleIDV = new VehicleIDV
                    {
                        idv = quoteQuery.IDVValue
                    }
                },
                previousInsurer = new GoDigitRequest.PreviousInsurer
                {
                    isPreviousInsurerKnown = quoteQuery.PreviousPolicyDetails.IsPreviousInsurerKnown,
                    previousInsurerCode = string.IsNullOrEmpty(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD) ? null : quoteQuery.PreviousPolicyDetails.PreviousInsurerCode,
                    previousPolicyNumber = string.IsNullOrEmpty(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD) ? null : quoteQuery.PreviousPolicyDetails.PreviousPolicyNumber,
                    previousPolicyExpiryDate = string.IsNullOrEmpty(quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD) ? null : quoteQuery.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD,
                    isClaimInLastYear = quoteQuery.PreviousPolicyDetails.IsClaimInLastYear,
                    originalPreviousPolicyType = quoteQuery.PreviousPolicyDetails.OriginalPreviousPolicyType,
                    previousPolicyType = quoteQuery.PreviousPolicyDetails.PreviousPolicyType,
                    previousNoClaimBonus = string.IsNullOrWhiteSpace(quoteQuery.PreviousPolicyDetails.PreviousNoClaimBonus) ? "ZERO" : quoteQuery.PreviousPolicyDetails.PreviousNoClaimBonus,
                    previousNoClaimBonusValue = quoteQuery.PreviousPolicyDetails.PreviousNoClaimBonusValue,
                    currentThirdPartyPolicy = currentThirdPartyPolicy
                },
                pospInfo = new GoDigitRequest.PospInfo { isPOSP = false }
            };
            var result = await QuoteResponseFraming(goDigitRequest, quoteQuery, quoteVm, cancellationToken);
            return Tuple.Create(result.Item1, result.Item2, result.Item3);
        }
        catch (Exception ex)
        {
            _logger.LogError("GoDigit Error {exception}", ex.Message);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return Tuple.Create(quoteVm, requestBody, responseBody);
        }
    }

    private async Task<Tuple<HttpResponseMessage, int>> GetQuoteResponse(GoDigitRequestDto goDigitRequest, string leadId, string stage, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
        var id = 0;
        try
        {
            var requestBody = JsonConvert.SerializeObject(goDigitRequest);

            id = await InsertICLogs(requestBody, leadId, _goDigitConfig.BaseURL + _goDigitConfig.QuoteURL, string.Empty, string.Empty, stage);
            try
            {
                httpResponseMessage = await _client.PostAsync(_goDigitConfig.QuoteURL, new StringContent(requestBody, Encoding.UTF8,
                    "application/json"), cancellationToken);
                return Tuple.Create(httpResponseMessage, id);
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, string.Empty, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("GoDigit GetQuoteResponse {exception}", ex.Message);
            await UpdateICLogs(id, string.Empty, ex.Message);
            return Tuple.Create(httpResponseMessage, id);
        }
    }
    private async Task<Tuple<QuoteResponseModel, string, string>> QuoteResponseFraming(GoDigitRequestDto goDigitRequest, QuoteQueryModel quoteQuery, QuoteResponseModel quoteVm, CancellationToken cancellationToken)
    {
        var quoteResponse = await GetQuoteResponse(goDigitRequest, quoteQuery.LeadId, "Quote", cancellationToken);
        var responseBody = string.Empty;
        var applicationId = string.Empty;
        var requestBody = JsonConvert.SerializeObject(goDigitRequest);

        if (!quoteResponse.Item1.IsSuccessStatusCode)
        {
            var stream = await quoteResponse.Item1.Content.ReadAsStringAsync(cancellationToken);
            responseBody = JsonConvert.SerializeObject(stream);
            quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            _logger.LogError("Unable to fetch quote {responseBody}", responseBody);
        }
        else
        {
            var stream = await quoteResponse.Item1.Content.ReadAsStreamAsync(cancellationToken);
            var result = stream.DeserializeFromJson<GoDigitResponse.GoDigitResponseDto>();
            if (result is not null)
            {
                applicationId = result.enquiryId;
                responseBody = JsonConvert.SerializeObject(result);
                _logger.LogInformation("Fetch quote {responseBody}", responseBody);
                if (result.error.httpCode == 200)
                {
                    var tax = result.serviceTax;
                    var addons = result.contract.coverages.addons;
                    _ = result.discounts.otherDiscounts;
                    var accessories = result.contract.coverages.accessories;
                    var ncbDiscountList = result.discounts?.otherDiscounts;
                    string ncbPercentage = "0";
                    if (ncbDiscountList != null && ncbDiscountList.Any())
                    {
                        var ncbDiscount = ncbDiscountList.SingleOrDefault(x => x.discountType.Equals("NCB_DISCOUNT"));
                        if (ncbDiscount != null)
                        {
                            ncbPercentage = ncbDiscount.discountPercent.ToString();
                        }
                    }

                    List<NameValueModel> addOnsList = SetAddOnsResponse(quoteQuery, addons);
                    List<NameValueModel> accessoryList = SetAccessoriesResponse(quoteQuery, accessories);
                    List<NameValueModel> paCoverList = SetPAResponse(quoteQuery, result);
                    List<NameValueModel> discountList = SetDiscountResponse(quoteQuery, result);

                    decimal idv = quoteQuery.RecommendedIDV == 0 ? result.vehicle.vehicleIDV.idv : quoteQuery.RecommendedIDV;
                    decimal minIdv = quoteQuery.MinIDV == 0 ? result.vehicle.vehicleIDV.minimumIdv : quoteQuery.MinIDV;
                    decimal maxIdv = quoteQuery.MaxIDV == 0 ? result.vehicle.vehicleIDV.maximumIdv : quoteQuery.MaxIDV;

                    quoteVm = new QuoteResponseModel
                    {
                        InsurerName = "GoDigit",
                        InsurerStatusCode = (int)HttpStatusCode.OK,
                        SelectedIDV = (quoteQuery.IsBrandNewVehicle && quoteQuery.IDVValue == 0) ? 1 : quoteQuery.SelectedIDV,
                        IDV = Math.Round(idv),
                        MinIDV = Math.Round(minIdv),
                        MaxIDV = Math.Round(maxIdv),
                        Tax = new GoDigitResponse.ServiceTax
                        {
                            cgst = RoundOffValue(tax.cgst.Split(" ")[1]),
                            sgst = RoundOffValue(tax.sgst.Split(" ")[1]),
                            igst = RoundOffValue(tax.igst.Split(" ")[1]),
                            utgst = RoundOffValue(tax.utgst.Split(" ")[1]),
                            totalTax = RoundOffValue(tax.totalTax.Split(" ")[1]),
                        },
                        BasicCover = new BasicCover
                        {
                            CoverList = SetBaseCover(quoteQuery, result)
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
                        TotalPremium = RoundOffValue(result.netPremium.Split(" ")[1]),
                        GrossPremium = RoundOffValue(result.grossPremium.Split(" ")[1]),
                        RTOCode = quoteQuery.VehicleDetails.LicensePlateNumber,
                        NCB = ncbPercentage,
                        PolicyStartDate = Convert.ToDateTime(quoteQuery.PolicyStartDate).ToString("dd-MMM-yyyy"),
                        Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
                        PlanType = quoteQuery.PlanType,
                        IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
                        IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
                        RegistrationDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                        ManufacturingDate = Convert.ToDateTime(quoteQuery.RegistrationDate).ToString("dd-MM-yyyy"),
                        VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.VehicleDetails.LicensePlateNumber : quoteQuery.VehicleNumber,
                        ApplicationId = result.enquiryId
                    };
                }
                else
                {
                    quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                }
            }
            else
            {
                quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
        await UpdateICLogs(quoteResponse.Item2, applicationId, responseBody);
        return Tuple.Create(quoteVm, requestBody, responseBody);
    }

    private static List<NameValueModel> SetBaseCover(QuoteQueryModel quoteQuery, GoDigitResponse.GoDigitResponseDto result)
    {
        List<NameValueModel> baseCoverList = new List<NameValueModel>();
        if (quoteQuery.CurrentPolicyType.Equals("Package Comprehensive") || quoteQuery.CurrentPolicyType.Equals("Comprehensive Bundle"))
        {
            baseCoverList = new List<NameValueModel>
            {
                new NameValueModel
                {
                    Name = "Basic Own Damage Premium",
                    Value = RoundOffValue(result.contract.coverages.ownDamage.netPremium?.Split(" ")[1]),
                    IsApplicable = IsApplicable(result.contract.coverages.ownDamage.netPremium?.Split(" ")[1])
                },
                new NameValueModel
                {
                    Name = "Third Party Cover Premium",
                    Value = RoundOffValue(result.contract.coverages.thirdPartyLiability.netPremium?.Split(" ")[1]),
                    IsApplicable = IsApplicable(result.contract.coverages.thirdPartyLiability.netPremium?.Split(" ")[1])
                }
            };
        }
        if (quoteQuery.CurrentPolicyType.Equals("SAOD"))
        {
            baseCoverList = new List<NameValueModel>
            {
                new NameValueModel
                {
                    Name = "Basic Own Damage Premium",
                    Value = RoundOffValue(result.contract.coverages.ownDamage.netPremium?.Split(" ")[1]),
                    IsApplicable = IsApplicable(result.contract.coverages.ownDamage.netPremium?.Split(" ")[1])
                }
            };
        }
        if (quoteQuery.CurrentPolicyType.Equals("SATP"))
        {
            baseCoverList = new List<NameValueModel>
            {
                new NameValueModel
                {
                    Name = "Third Party Cover Premium",
                    Value = RoundOffValue(result.contract.coverages.thirdPartyLiability.netPremium?.Split(" ")[1]),
                    IsApplicable = IsApplicable(result.contract.coverages.thirdPartyLiability.netPremium?.Split(" ")[1])
                }
            };
        }
        return baseCoverList;
    }

    private static List<NameValueModel> SetDiscountResponse(QuoteQueryModel quoteQuery, GoDigitResponse.GoDigitResponseDto result)
    {
        var ncb = result.discounts?.otherDiscounts.SingleOrDefault(x => x.discountType.Equals("NCB_DISCOUNT"));

        List<NameValueModel> discountList = new List<NameValueModel>();
        if (quoteQuery.Discounts != null && quoteQuery.Discounts.IsAntiTheft)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AntiTheftId,
                Name = "ARAI Approved Anti-Theft Device",
                Value = "0",
                IsApplicable = false
            });
        }
        if (quoteQuery.Discounts != null && quoteQuery.Discounts.IsAAMemberShip)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.AAMemberShipId,
                Name = "AA Membership",
                Value = "0",
                IsApplicable = false
            });
        }
        if (quoteQuery.Discounts != null && quoteQuery.Discounts.IsVoluntarilyDeductible)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
                Name = "Voluntary Deductible",
                Value = quoteQuery.VehicleDetails.IsFourWheeler ? "Included" : "NA",
                IsApplicable = true
            });
        }
        if (quoteQuery.Discounts != null && quoteQuery.Discounts.IsLimitedTPCoverage)
        {
            discountList.Add(new NameValueModel
            {
                Id = quoteQuery.Discounts.LimitedTPCoverageId,
                Name = "Limited Third Party Coverage",
                Value = "0",
                IsApplicable = false
            });
        }
        if (!quoteQuery.IsBrandNewVehicle)
        {
            discountList.Add(new NameValueModel
            {
                Name = $"No Claim Bonus ({ncb?.discountPercent}%)",
                Value = RoundOffValue(ncb?.discountAmount?.Split(" ")[1]),
                IsApplicable = IsApplicable(ncb?.discountAmount?.Split(" ")[1]),
            });
        }
        return discountList;
    }

    private static List<NameValueModel> SetPAResponse(QuoteQueryModel quoteQuery, GoDigitResponse.GoDigitResponseDto result)
    {
        List<NameValueModel> paCoverList = new List<NameValueModel>();
        if (quoteQuery.PACover.IsUnnamedPassenger)
        {
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedPassengerId,
                Name = "PA Cover for Unnamed Passengers",
                Value = RoundOffValue(result.contract.coverages.unnamedPA.unnamedPax.netPremium?.Split(" ")[1]),
                IsApplicable = IsApplicable(result.contract.coverages.unnamedPA.unnamedPax.netPremium?.Split(" ")[1])
            }
            );
        }
        if (quoteQuery.PACover.IsPaidDriver)
        {
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.PaidDriverId,
                Name = "PA Cover for Paid Driver",
                Value = RoundOffValue(result.contract.coverages.unnamedPA.unnamedPaidDriver.netPremium?.Split(" ")[1]),
                IsApplicable = IsApplicable(result.contract.coverages.unnamedPA.unnamedPaidDriver.netPremium?.Split(" ")[1])
            }
            );
        }
        if (quoteQuery.PACover.IsUnnamedPillionRider)
        {
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedPillionRiderId,
                Name = "PA Cover For Unnamed Pillion Rider",
                Value = "0",
                IsApplicable = false,
            });
        }
        if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
        {
            paCoverList.Add(new NameValueModel
            {
                Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
                Name = "PA Cover for Owner Driver",
                Value = RoundOffValue(result.contract.coverages.personalAccident.netPremium?.Split(" ")[1]),
                IsApplicable = IsApplicable(result.contract.coverages.personalAccident.netPremium?.Split(" ")[1]),
            }
            );
        }
        return paCoverList;
    }

    private static List<NameValueModel> SetAccessoriesResponse(QuoteQueryModel quoteQuery, GoDigitResponse.Accessories accessories)
    {
        var accessoryList = new List<NameValueModel>();
        if (quoteQuery.Accessories.IsCNG)
        {
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.CNGId,
                Name = "CNG/LPG Accessory Cover",
                Value = accessories.cng.selection ? "Included" : "NA",
                IsApplicable = accessories.cng.selection
            }
            );
        }
        if (quoteQuery.Accessories.IsElectrical)
        {
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.ElectricalId,
                Name = "Electrical Accessory Cover",
                Value = accessories.electrical.selection ? "Included" : "NA",
                IsApplicable = accessories.electrical.selection
            }
            );
        }
        if (quoteQuery.Accessories.IsNonElectrical)
        {
            accessoryList.Add(new NameValueModel
            {
                Id = quoteQuery.Accessories.NonElectricalId,
                Name = "Non-Electrical Accessory Cover",
                Value = accessories.nonElectrical.selection ? "Included" : "NA",
                IsApplicable = accessories.nonElectrical.selection
            });
        }
        return accessoryList;
    }

    private static List<NameValueModel> SetAddOnsResponse(QuoteQueryModel quoteQuery, GoDigitResponse.Addons addons)
    {
        List<NameValueModel> addOnsList = new List<NameValueModel>();
        if (quoteQuery.AddOns.IsZeroDebt || addons.partsDepreciation.selection)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ZeroDebtId,
                Name = "Zero Dep",
                Value = addons.partsDepreciation.selection ? RoundOffValue(addons.partsDepreciation.netPremium?.Split(" ")[1]) : "0",
                IsApplicable = IsApplicable(addons.partsDepreciation.netPremium?.Split(" ")[1])
            }
            );
        }
        if (quoteQuery.AddOns.IsEngineProtectionRequired || addons.engineProtection.selection)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.EngineProtectionId,
                Name = "Engine Gearbox Protection",
                Value = addons.engineProtection.selection ? RoundOffValue(addons.engineProtection.netPremium?.Split(" ")[1]) : "0",
                IsApplicable = IsApplicable(addons.engineProtection.netPremium?.Split(" ")[1])
            }
            );
        }
        if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired || addons.keyAndLockProtect.selection)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.KeyAndLockProtectionId,
                Name = "Key And Lock Protection",
                Value = addons.keyAndLockProtect.selection ? RoundOffValue(addons.keyAndLockProtect.netPremium?.Split(" ")[1]) : "0",
                IsApplicable = IsApplicable(addons.keyAndLockProtect.netPremium?.Split(" ")[1])
            }
            );
        }
        if (quoteQuery.AddOns.IsRoadSideAssistanceRequired || addons.roadSideAssistance.selection)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RoadSideAssistanceId,
                Name = "Road Side Assistance",
                Value = addons.roadSideAssistance.selection ? RoundOffValue(addons.roadSideAssistance.netPremium?.Split(" ")[1]) : "0",
                IsApplicable = IsApplicable(addons.roadSideAssistance.netPremium?.Split(" ")[1])
            }
            );
        }
        if (quoteQuery.AddOns.IsTyreProtectionRequired || addons.tyreProtection.selection)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.TyreProtectionId,
                Name = "Tyre Protection",
                Value = addons.tyreProtection.selection ? RoundOffValue(addons.tyreProtection.netPremium?.Split(" ")[1]) : "0",
                IsApplicable = IsApplicable(addons.tyreProtection.netPremium?.Split(" ")[1])
            }
            );
        }
        if (quoteQuery.AddOns.IsRimProtectionRequired || addons.rimProtection.selection)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RimProtectionId,
                Name = "RIM Protection",
                Value = addons.rimProtection.selection ? RoundOffValue(addons.rimProtection.netPremium?.Split(" ")[1]) : "0",
                IsApplicable = IsApplicable(addons.rimProtection.netPremium?.Split(" ")[1])
            }
            );
        }
        if (quoteQuery.AddOns.IsConsumableRequired || addons.consumables.selection)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ConsumableId,
                Name = "Consumables",
                Value = addons.consumables.selection ? RoundOffValue(addons.consumables.netPremium?.Split(" ")[1]) : "0",
                IsApplicable = IsApplicable(addons.consumables.netPremium?.Split(" ")[1])
            }
            );
        }
        if (quoteQuery.AddOns.IsInvoiceCoverRequired || addons.returnToInvoice.selection)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.ReturnToInvoiceIdId,
                Name = "RTI",
                Value = addons.returnToInvoice.selection ? RoundOffValue(addons.returnToInvoice.netPremium?.Split(" ")[1]) : "0",
                IsApplicable = IsApplicable(addons.returnToInvoice.netPremium?.Split(" ")[1])
            }
            );
        }
        if (quoteQuery.AddOns.IsGeoAreaExtension)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.GeoAreaExtensionId,
                Name = "Geo Area Extension",
                Value = "0",
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
                Value = "0",
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsPersonalBelongingRequired || addons.personalBelonging.selection)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.PersonalBelongingId,
                Name = "Personal Belongings",
                Value = addons.personalBelonging.selection ? RoundOffValue(addons.personalBelonging.netPremium?.Split(" ")[1]) : "0",
                IsApplicable = IsApplicable(addons.personalBelonging.netPremium?.Split(" ")[1])
            }
            );
        }
        if (quoteQuery.AddOns.IsNCBRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.GeoAreaExtensionId,
                Name = "No Claim Bonus Protection",
                Value = "0",
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsEMIProtectorRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.EMIProtectorId,
                Name = "EMI Protection",
                Value = "0",
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsLimitedOwnPremisesRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.LimitedOwnPremisesId,
                Name = "Limited to Own Premises",
                Value = "0",
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
                Value = "0",
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RoadSideAssistanceAdvanceId,
                Name = "Road Side Assistance Advance",
                Value = "0",
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsRoadSideAssistanceWiderRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.RoadSideAssistanceWiderId,
                Name = "Road Side Assistance Wider",
                Value = "0",
                IsApplicable = false
            }
            );
        }
        if (quoteQuery.AddOns.IsTowingRequired)
        {
            addOnsList.Add(new NameValueModel
            {
                Id = quoteQuery.AddOns.TowingId,
                Name = "Towing Protection",
                Value = "0",
                IsApplicable = false
            }
            );
        }
        return addOnsList;
    }
    private static bool IsApplicable(object _val)
    {
        string val = Convert.ToString(_val);
        return !(string.IsNullOrEmpty(val) || val == "0" || Convert.ToDecimal(val) == 0);
    }
    private static string RoundOffValue(string _val)
    {
        decimal val = Math.Round(Convert.ToDecimal(_val));
        return val.ToString();
    }
    public async Task<Tuple<QuoteResponseModel, string, string>> CreateProposal(GoDigitProposalDto proposalQuery, ProposalRequest proposalRequest, GodigitCKYCRequest godigitCKYCRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken)
    {
        var proposalVm = new QuoteResponseModel();
        string requestBody = string.Empty;
        var responseBody = string.Empty;
        var id = 0;
        try
        {
            GetPersonDetailsByCustomerType(proposalQuery, proposalRequest, godigitCKYCRequest, createLeadModel);

            proposalQuery.vehicle.vehicleIdentificationNumber = proposalRequest.VehicleDetails?.chassisNumber;
            proposalQuery.vehicle.engineNumber = proposalRequest.VehicleDetails?.engineNumber;

            proposalQuery.nominee = new Domain.GoDigit.Nominee
            {
                firstName = proposalRequest.NomineeDetails.nomineeFirstName,
                middleName = proposalRequest.NomineeDetails.middleName,
                lastName = proposalRequest.NomineeDetails.nomineeLastName,
                dateOfBirth = Convert.ToDateTime(proposalRequest.NomineeDetails.nomineeDateOfBirth).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                relation = proposalRequest.NomineeDetails.nomineeRelation,
                personType = "INDIVIDUAL"
            };

            if (createLeadModel.VehicleTypeId.Equals("6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056"))
            {
                bool idv = proposalQuery.vehicle.vehicleIDV.idv < 60000;
                bool cupicCapacity = !string.IsNullOrEmpty(createLeadModel.VehicleCubicCapacity) && Convert.ToDecimal(createLeadModel.VehicleCubicCapacity) < 150;
                bool partsDepreciation = proposalQuery.contract.coverages.addons.partsDepreciation.selection;
                bool roadSideAssistance = proposalQuery.contract.coverages.addons.roadSideAssistance.selection;
                bool engineProtection = proposalQuery.contract.coverages.addons.engineProtection.selection;
                bool tyreProtection = proposalQuery.contract.coverages.addons.tyreProtection.selection;
                bool rimProtection = proposalQuery.contract.coverages.addons.rimProtection.selection;
                bool returnToInvoice = proposalQuery.contract.coverages.addons.returnToInvoice.selection;
                bool consumables = proposalQuery.contract.coverages.addons.consumables.selection;

                if (idv && cupicCapacity && !partsDepreciation && !roadSideAssistance && !engineProtection && !tyreProtection && !rimProtection && !returnToInvoice && !consumables)
                {
                    proposalQuery.motorQuestions = new Motorquestions
                    {
                        financer = proposalRequest.VehicleDetails?.financer,
                        furtherAgreement = string.Empty,
                        selfInspection = !createLeadModel.IsBrandNew && createLeadModel.IsBreakin ? createLeadModel.IsBreakin : createLeadModel.IsSelfInspection
                    };
                    proposalQuery.motorBreakin = new Motorbreakin
                    {
                        isBreakin = createLeadModel.IsBreakin,
                        isPreInspectionWaived = true,
                        isPreInspectionCompleted = true,
                        isDocumentUploaded = false
                    };
                }
                else
                {
                    proposalQuery.motorQuestions = new Motorquestions
                    {
                        financer = proposalRequest.VehicleDetails?.financer,
                        furtherAgreement = string.Empty,
                        selfInspection = !createLeadModel.IsBrandNew && (createLeadModel.IsBreakin ? createLeadModel.IsBreakin : createLeadModel.IsSelfInspection)
                    };
                    proposalQuery.motorBreakin = new Motorbreakin
                    {
                        isBreakin = createLeadModel.IsBreakin,
                        isPreInspectionWaived = false,
                        isPreInspectionCompleted = false,
                        isDocumentUploaded = false
                    };
                }
            }
            else
            {
                proposalQuery.motorQuestions = new Motorquestions
                {
                    financer = proposalRequest.VehicleDetails?.financer,
                    furtherAgreement = string.Empty,
                    selfInspection = !createLeadModel.IsBrandNew && (createLeadModel.IsBreakin ? createLeadModel.IsBreakin : createLeadModel.IsSelfInspection)
                };
                proposalQuery.motorBreakin = new Motorbreakin
                {
                    isBreakin = createLeadModel.IsBreakin,
                    isPreInspectionWaived = false,
                    isPreInspectionCompleted = false,
                    isDocumentUploaded = false
                };

            }
            proposalQuery.kyc = new kyc
            {
                isKYCDone = false,
                ckycReferenceDocId = "D07",
                ckycReferenceNumber = godigitCKYCRequest.panNumber,
                dateOfBirth = (createLeadModel.CarOwnedBy.Equals("INDIVIDUAL") ? godigitCKYCRequest.dateOfBirth : godigitCKYCRequest.dateOfInsertion),
                photo = string.Empty
            };
            if (!string.IsNullOrWhiteSpace(_applicationClaims.GetPOSPId()))
            {
                proposalQuery.pospInfo = new Pospinfo
                {
                    isPOSP = true,
                    pospUniqueNumber = _applicationClaims.GetPOSPId(),
                    pospAadhaarNumber = _applicationClaims.GetAadhaarNumber(),
                    pospContactNumber = _applicationClaims.GetMobileNo(),
                    pospLocation = _applicationClaims.GetLocation(),
                    pospName = _applicationClaims.GetUserName(),
                    pospPanNumber = _applicationClaims.GetPAN()
                };
            }
            proposalVm.InsurerName = _goDigitConfig.InsurerName;
            requestBody = JsonConvert.SerializeObject(proposalQuery);

            id = await InsertICLogs(requestBody, createLeadModel.LeadID, _goDigitConfig.BaseURL + _goDigitConfig.ProposalURL, string.Empty, string.Empty, "Proposal");
            try
            {
                var quoteResponse = await _client.PostAsync(_goDigitConfig.ProposalURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                cancellationToken);

                var stream = await quoteResponse.Content.ReadAsStreamAsync(cancellationToken);
                var result = stream.DeserializeFromJson<GoDigitProposalResponseDto>();

                if (!quoteResponse.IsSuccessStatusCode)
                {

                    if (result.error.httpCode == 400 && result.error.validationMessages.Any())
                    {
                        proposalVm.ValidationMessage = string.Join(',', result.error.validationMessages);
                    }
                    else
                    {
                        proposalVm.ValidationMessage = ValidationMessage;
                    }
                    responseBody = JsonConvert.SerializeObject(result);
                    proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogError("Unable to create proposal {responseBody}", responseBody);
                }
                else
                {
                    if (result != null)
                    {
                        responseBody = JsonConvert.SerializeObject(result);
                        _logger.LogInformation("create proposal {responseBody}", responseBody);
                        if (result.error.httpCode == 200)
                        {
                            var tax = result.serviceTax;

                            proposalVm = new QuoteResponseModel
                            {
                                InsurerName = _goDigitConfig.InsurerName,
                                InsurerStatusCode = (int)HttpStatusCode.OK,
                                Tax = new ServiceTax()
                                {
                                    totalTax = tax.totalTax.Split(" ")[1]
                                },
                                TotalPremium = result.netPremium.Split(" ")[1],
                                GrossPremium = result.grossPremium.Split(" ")[1],
                                ApplicationId = result.applicationId,
                                ProposalNumber = result.policyNumber,
                                PolicyNumber = result.policyNumber,
                                IsBreakIn = result.motorBreakIn.isPreInspectionWaived ? false : result.motorBreakIn.isBreakin,
                                BreakinStatus = result.policyStatus,
                                IsSelfInspection = result.motorBreakIn.isPreInspectionWaived ? false : result.motorQuestions.selfInspection
                            };
                        }
                    }
                    else
                    {
                        proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }

                await UpdateICLogs(id, proposalVm?.ApplicationId, responseBody);
                return Tuple.Create(proposalVm, requestBody, responseBody);
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, proposalVm?.ApplicationId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("GoDigit Proposal Error {exception}", ex.Message);
            proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await UpdateICLogs(id, proposalVm?.ApplicationId, ex.Message);
            return Tuple.Create(proposalVm, requestBody, responseBody);
        }
    }
    private static void GetPersonDetailsByCustomerType(GoDigitProposalDto proposalQuery, ProposalRequest proposalRequest, GodigitCKYCRequest godigitCKYCRequest, CreateLeadModel createLeadModel)
    {
        if (createLeadModel.CarOwnedBy.ToUpper().Equals("INDIVIDUAL"))
        {
            proposalQuery.persons = new IndividualPerson[1];
            IndividualPerson person = new IndividualPerson()
            {
                personType = createLeadModel.CarOwnedBy,
                partyId = string.Empty,
                addresses = new Address[]
                {
                    new Address()
                    {
                        addressType= "PRIMARY_RESIDENCE",
                        flatNumber=proposalRequest.AddressDetails.flatNumber,
                        streetNumber = proposalRequest.AddressDetails.streetNumber,
                        pincode = proposalRequest.AddressDetails.pincode,
                        city = proposalRequest.AddressDetails.city,
                        street = proposalRequest.AddressDetails.street,
                        district = proposalRequest.AddressDetails.district,
                        state = proposalRequest.AddressDetails.state,
                        country = "IN"
                    }
                },
                dateOfBirth = godigitCKYCRequest.dateOfBirth,
                firstName = proposalRequest.PersonalDetails?.firstName,
                gender = proposalRequest.PersonalDetails?.gender,
                middleName = proposalRequest.PersonalDetails?.middleName,
                lastName = proposalRequest.PersonalDetails?.lastName,
                communications = new Communication[]
                 {
                         new Communication()
                         {
                             communicationType = "MOBILE",
                             communicationId = proposalRequest.PersonalDetails?.mobile,
                             isPrefferedCommunication = true
                         },
                         new Communication()
                         {
                             communicationType = "EMAIL",
                             communicationId = proposalRequest.PersonalDetails?.emailId,
                             isPrefferedCommunication = true
                         }
                 },
                identificationDocuments = new Identificationdocument[]
                {
                        new Identificationdocument()
                        {
                            documentType = "PAN_CARD",
                            documentId=godigitCKYCRequest.panNumber
                        }
                },
                isDriver = true,
                isInsuredPerson = true,
                isPolicyHolder = true,
                isVehicleOwner = true,
            };
            proposalQuery.persons[0] = person;
        }
        else if (createLeadModel.CarOwnedBy.ToUpper().Equals("COMPANY"))
        {
            proposalQuery.persons = new CompanyPerson[1];
            CompanyPerson person = new CompanyPerson()
            {
                personType = createLeadModel.CarOwnedBy,
                partyId = string.Empty,
                addresses = new Address[]
                {
                    new Address()
                    {
                        addressType= "HEAD_QUARTER",
                        flatNumber=proposalRequest.AddressDetails.flatNumber,
                        streetNumber = proposalRequest.AddressDetails.streetNumber,
                        pincode = proposalRequest.AddressDetails.pincode,
                        city = proposalRequest.AddressDetails.city,
                        street = proposalRequest.AddressDetails.street,
                        district = proposalRequest.AddressDetails.district,
                        state = proposalRequest.AddressDetails.state,
                        country = "IN"
                    }
                },
                communications = new Communication[]
                 {
                         new Communication()
                         {
                             communicationType = "MOBILE",
                             communicationId = proposalRequest.PersonalDetails?.mobile,
                             isPrefferedCommunication = true
                         },
                         new Communication()
                         {
                             communicationType = "EMAIL",
                             communicationId = proposalRequest.PersonalDetails?.emailId,
                             isPrefferedCommunication = true
                         }
                 },
                identificationDocuments = new Identificationdocument[]
                {
                        new Identificationdocument()
                        {
                            documentType = "GST",
                            documentId = createLeadModel.GSTNo
                        }
                },
                isPolicyHolder = true,
                isVehicleOwner = true,
                isPayer = null,
                companyName = proposalRequest.PersonalDetails.companyName
            };
            proposalQuery.persons[0] = person;
        }
    }
    public async Task<Tuple<QuoteConfirmDetailsResponseModel, QuoteResponseModel, string, string, string, string>> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
    {
        var applicationId = string.Empty;
        GoDigitRequestDto requestBody = JsonConvert.DeserializeObject<GoDigitRequestDto>(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody);
        bool isCurrPolicyEngineCover = requestBody.contract.coverages.addons.engineProtection.selection;
        bool isCurrPolicyPartDept = requestBody.contract.coverages.addons.partsDepreciation.selection;
        bool isSelfInspection = false;

        requestBody.contract.startDate = quoteConfirmCommand.PolicyDates.PolicyStartDate;
        requestBody.contract.endDate = quoteConfirmCommand.PolicyDates.PolicyEndDate;
        requestBody.contract.policyHolderType = quoteConfirmCommand.Customertype;
        requestBody.contract.coverages.personalAccident.selection = !quoteConfirmCommand.IsPACover;
        requestBody.contract.coverages.personalAccident.coverTerm = !quoteConfirmCommand.IsPACover ? Convert.ToInt32(quoteConfirmCommand.PACoverTenure) : 0;
        requestBody.vehicle.licensePlateNumber = quoteConfirmCommand.VehicleNumber;
        requestBody.vehicle.engineNumber = quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine != null ? quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine : requestBody.vehicle.engineNumber;
        requestBody.vehicle.vehicleIdentificationNumber = quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis != null ? quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis : requestBody.vehicle.vehicleIdentificationNumber;
        requestBody.vehicle.manufactureDate = quoteConfirmCommand.ManufacturingMonthYear;
        requestBody.vehicle.registrationDate = quoteConfirmCommand.RegistrationDate;

        if (!quoteConfirmCommand.IsBrandNewVehicle && quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy)
        {
            requestBody.previousInsurer.isPreviousInsurerKnown = true;
            requestBody.previousInsurer.previousInsurerCode = quoteTransactionDbModel.QuoteConfirmDetailsModel.SAODInsurerCode;
            requestBody.previousInsurer.previousPolicyNumber = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
            requestBody.previousInsurer.previousPolicyExpiryDate = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd");
            requestBody.previousInsurer.isClaimInLastYear = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim;
            requestBody.previousInsurer.previousNoClaimBonus = string.IsNullOrWhiteSpace(quoteConfirmCommand.PreviousPolicy.NCBId) ? "ZERO" : quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBName;

            if (requestBody.previousInsurer.currentThirdPartyPolicy != null && !string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.TPPolicyEndDate))
            {
                requestBody.previousInsurer.currentThirdPartyPolicy.isCurrentThirdPartyPolicyActive = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate) < DateTime.Now;
                requestBody.previousInsurer.currentThirdPartyPolicy.currentThirdPartyPolicyInsurerCode = quoteTransactionDbModel.QuoteConfirmDetailsModel.SATPInsurerCode;
                requestBody.previousInsurer.currentThirdPartyPolicy.currentThirdPartyPolicyNumber = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP;
                requestBody.previousInsurer.currentThirdPartyPolicy.currentThirdPartyPolicyStartDateTime = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd");
                requestBody.previousInsurer.currentThirdPartyPolicy.currentThirdPartyPolicyExpiryDateTime = Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd");
            }
        }

        var confirmQuote = await GetQuoteResponse(requestBody, quoteTransactionDbModel.LeadDetail.LeadID, "QuoteConfirm", cancellationToken);
        QuoteConfirmDetailsResponseModel quoteConfirm = new QuoteConfirmDetailsResponseModel();
        string requestBodyFraming = string.Empty;
        string responseBody = string.Empty;
        string commonResponse = quoteTransactionDbModel.QuoteTransactionRequest.CommonResponse;
        QuoteResponseModel updatedResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(commonResponse);

        var leadId = quoteTransactionDbModel.LeadDetail.LeadID;
        string transactionId = null;
        GoDigitResponseDto confirmQuoteResult = new();
        requestBodyFraming = JsonConvert.SerializeObject(requestBody);

        if (confirmQuote.Item1.IsSuccessStatusCode)
        {
            var stream = await confirmQuote.Item1.Content.ReadAsStreamAsync();
            confirmQuoteResult = stream.DeserializeFromJson<GoDigitResponseDto>();
            if (confirmQuoteResult.error.httpCode == 200)
            {
                applicationId = confirmQuoteResult.enquiryId;
                updatedResponse.GrossPremium = RoundOffValue(confirmQuoteResult.grossPremium.Split(" ")[1]);
                responseBody = JsonConvert.SerializeObject(confirmQuoteResult);

                var ncbDiscountList = confirmQuoteResult.discounts?.otherDiscounts;
                string ncbPercentage = "0";
                if (ncbDiscountList != null && ncbDiscountList.Any())
                {
                    var ncbDiscount = ncbDiscountList.SingleOrDefault(x => x.discountType.Equals("NCB_DISCOUNT"));
                    if (ncbDiscount != null)
                    {
                        ncbPercentage = ncbDiscount.discountPercent.ToString();
                    }
                }

                bool policyTypeSelfInspection = false;
                if (!quoteTransactionDbModel.LeadDetail.IsBrandNew)
                {
                    if (!string.IsNullOrEmpty(quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId))
                    {
                        if (quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId.Equals("2AA7FDCA-9E36-4A8D-9583-15ADA737574B") && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals("517D8F9C-F532-4D45-8034-ABECE46693E3"))
                        {
                            policyTypeSelfInspection = true;
                        }
                    }

                    if ((!quoteConfirmCommand.isPrevPolicyEngineCover && isCurrPolicyEngineCover) || (!quoteConfirmCommand.isPrevPolicyNilDeptCover && isCurrPolicyPartDept) || policyTypeSelfInspection)
                    {
                        isSelfInspection = true;
                    }
                }

                quoteConfirm = new QuoteConfirmDetailsResponseModel
                {
                    InsurerStatusCode = (int)HttpStatusCode.OK,
                    InsurerName = "GoDigit",
                    NewPremium = RoundOffValue(confirmQuoteResult.grossPremium.Split(" ")[1]),
                    InsurerId = _goDigitConfig.InsurerId,
                    IDV = (requestBody.vehicle.vehicleIDV.idv == 0) ? Convert.ToInt32(Math.Round(confirmQuoteResult.vehicle.vehicleIDV.idv)) : Convert.ToInt32(Math.Round(requestBody.vehicle.vehicleIDV.idv)),
                    NCB = ncbPercentage,
                    Tax = new ServiceTaxModel
                    {
                        totalTax = RoundOffValue(confirmQuoteResult.serviceTax.totalTax.Split(" ")[1])
                    },
                    TotalPremium = RoundOffValue(confirmQuoteResult.netPremium.Split(" ")[1]),
                    GrossPremium = RoundOffValue(confirmQuoteResult.grossPremium.Split(" ")[1]),
                    IsBreakin = confirmQuoteResult.motorBreakIn.isBreakin,
                    IsSelfInspection = isSelfInspection
                };
            }
        }
        else
        {
            var stream = await confirmQuote.Item1.Content.ReadAsStreamAsync();
            confirmQuoteResult = stream.DeserializeFromJson<GoDigitResponseDto>();
            if (confirmQuoteResult.error.httpCode == 400 && confirmQuoteResult.error.validationMessages.Any())
            {
                quoteConfirm.ValidationMessage = string.Join(',', confirmQuoteResult.error.validationMessages);
            }
            else
            {
                quoteConfirm.ValidationMessage = ValidationMessage;
            }
            responseBody = JsonConvert.SerializeObject(confirmQuoteResult);
            quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
        }
        await UpdateICLogs(confirmQuote.Item2, applicationId, responseBody);
        return Tuple.Create(quoteConfirm, updatedResponse, requestBodyFraming, responseBody, leadId, transactionId);
    }
    public Tuple<string, string, SaveCKYCResponse, CreateLeadModel> GetCKYCResponse(GoDigitCKYCCommand goDigitCKYCCommand, CancellationToken cancellationToken)
    {
        string responseBody = string.Empty;
        string requestBody = string.Empty;
        GodigitCKYCRequest ckycRequest = new GodigitCKYCRequest();
        SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();
        CreateLeadModel createLeadModel = new CreateLeadModel();
        createLeadModel.PermanentAddress = new LeadAddressModel();

        string dob = string.IsNullOrEmpty(goDigitCKYCCommand.DateOfBirth) ? null : Convert.ToDateTime(goDigitCKYCCommand.DateOfBirth).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        string doi = string.IsNullOrEmpty(goDigitCKYCCommand.DateOfInsertion) ? null : Convert.ToDateTime(goDigitCKYCCommand.DateOfInsertion).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        if (goDigitCKYCCommand.DocumentType.Equals("Pan"))
        {
            ckycRequest.panNumber = goDigitCKYCCommand.DocumentId;
            ckycRequest.dateOfBirth = dob;
            ckycRequest.dateOfInsertion = doi;
            createLeadModel.PANNumber = goDigitCKYCCommand.DocumentId;
            if (goDigitCKYCCommand.CustomerType.ToLower().Equals("o"))
            {
                createLeadModel.CarOwnedBy = "COMPANY";
                ckycRequest.customerType = "COMPANY";
                createLeadModel.DateOfIncorporation = doi;
            }
            else
            {
                createLeadModel.DOB = dob;
                createLeadModel.CarOwnedBy = "INDIVIDUAL";
                ckycRequest.customerType = "INDIVIDUAL";
            }
            requestBody = JsonConvert.SerializeObject(ckycRequest);

            saveCKYCResponse.KYC_Status = "KYC_SUCCESS";
            saveCKYCResponse.Message = "KYC Success";
            saveCKYCResponse.IsKYCRequired = false;
            responseBody = "Data Stored Success";
            return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
        }
        else
        {
            saveCKYCResponse.KYC_Status = "FAILED";
            saveCKYCResponse.Message = "Please Enter Correct PAN Number or Proceed With Other Insurer";
            saveCKYCResponse.IsKYCRequired = false;
            responseBody = "Please select Pan as Id";
            return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
        }
    }
    public async Task<GoDigitPaymentURLResponseDto> GetPaymentLink(string leadId, string applicationId, string cancelReturnUrl, string successReturnUrl, CancellationToken cancellationToken)
    {
        string requestBody = string.Empty;
        string responseBody = string.Empty;
        var paymentURLVm = new GoDigitPaymentURLResponseDto();
        var id = 0;
        try
        {
            var request = new GoDigitPaymentURLRequestDto
            {
                applicationId = applicationId,
                cancelReturnUrl = cancelReturnUrl,
                successReturnUrl = successReturnUrl
            };
            requestBody = JsonConvert.SerializeObject(request);
            _logger.LogInformation("Godigit GetPaymentLink {requestBody}", requestBody);

            id = await InsertICLogs(requestBody, leadId, _goDigitConfig.PaymentLinkURL, _goDigitConfig.Authorization, string.Empty, "Payment");
            try
            {
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.Add("Authorization", _goDigitConfig.Authorization);

                var paymentURLLinkResponse = await _client.PostAsync(_goDigitConfig.PaymentLinkURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
                    cancellationToken);

                if (!paymentURLLinkResponse.IsSuccessStatusCode)
                {
                    var stream = await paymentURLLinkResponse.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<GoDigitPaymentURLResponseDto>();

                    if (result != null)
                    {
                        paymentURLVm.ValidationMessage = result.message;
                        responseBody = JsonConvert.SerializeObject(result);
                    }
                    else
                    {
                        paymentURLVm.ValidationMessage = ValidationMessage;
                        responseBody = paymentURLLinkResponse.ReasonPhrase;
                    }
                    paymentURLVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogError("Godigit GetPaymentLink Exception {responseBody}", responseBody);
                }
                else
                {
                    var result = await paymentURLLinkResponse.Content.ReadAsStringAsync(cancellationToken);
                    if (!string.IsNullOrEmpty(result))
                    {
                        responseBody = result;
                        _logger.LogInformation("Godigit GetPaymentLink {responseBody}", responseBody);
                        paymentURLVm.InsurerStatusCode = (int)HttpStatusCode.OK;
                        paymentURLVm.PaymentURL = result;
                    }
                    else
                    {
                        paymentURLVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                await UpdateICLogs(id, applicationId, responseBody);

                return paymentURLVm;
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, applicationId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("GoDigit Proposal Error {exception}", ex.Message);
            paymentURLVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await UpdateICLogs(id, applicationId, ex.Message);
            return paymentURLVm;
        }
    }
    public async Task<PaymentCKCYResponseModel> GetPaymentDetails(GodigitPaymentCKYCReqModel godigitPaymentCKYCReqModel, CancellationToken cancellationToken)
    {
        var responseBody = string.Empty;
        var id = 0;
        try
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", _goDigitConfig.Authorization);

            string url = _goDigitConfig.PaymentStatusCheckURL + godigitPaymentCKYCReqModel.ApplicationId;
            _logger.LogInformation("Payment URL {requestBody}", url);

            id = await InsertICLogs(string.Empty, godigitPaymentCKYCReqModel.LeadId, url, _goDigitConfig.Authorization, string.Empty, "Payment");
            try
            {
                var res = await _client.GetAsync(url, cancellationToken);

                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogInformation("GoDigit Payment status {result}", json);
                    var result = JsonConvert.DeserializeObject<List<GodigitPaymentCheckResponseDto>>(json);
                    responseBody = JsonConvert.SerializeObject(result);
                    if (result != null && result.Any())
                    {
                        var paymentDetails = result.LastOrDefault();
                        var paymentDetailsVm = new PaymentCKCYResponseModel
                        {
                            ApplicationId = paymentDetails?.applicationId,
                            Amount = paymentDetails?.premiumAmount,
                            PaymentStatus = paymentDetails?.status,
                            PaymentTransactionNumber = paymentDetails?.transactionNumber,
                            PolicyNumber = paymentDetails?.applicationNumber
                        };

                        await UpdateICLogs(id, godigitPaymentCKYCReqModel.ApplicationId, responseBody);
                        return paymentDetailsVm;
                    }
                }
                else
                {
                    responseBody = await res.Content.ReadAsStringAsync(cancellationToken);
                    await UpdateICLogs(id, godigitPaymentCKYCReqModel.ApplicationId, responseBody);
                    return default;
                }
                return default;
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, godigitPaymentCKYCReqModel.ApplicationId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("GoDigit Payment Error {exception}", ex.Message);
            await UpdateICLogs(id, godigitPaymentCKYCReqModel.ApplicationId, ex.Message);
            return default;
        }
    }
    public async Task<PaymentCKCYResponseModel> GetCKYCDetails(GodigitPaymentCKYCReqModel godigitPaymentCKYCReqModel, CancellationToken cancellationToken)
    {
        var responseBody = string.Empty;
        var id = 0;
        try
        {
            _client.DefaultRequestHeaders.Clear();
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(_goDigitConfig.AuthCode));
            _client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64EncodedAuthenticationString}");

            string url = _goDigitConfig.CKYCStatusCheckURL + "policyNumber=" + godigitPaymentCKYCReqModel.PolicyNumber;
            _logger.LogInformation("Payment URL {requestBody}", url);

            id = await InsertICLogs(string.Empty, godigitPaymentCKYCReqModel.LeadId, url, base64EncodedAuthenticationString, string.Empty, "KYC");
            try
            {
                var res = await _client.GetAsync(url);

                if (res.IsSuccessStatusCode)
                {
                    var stream = await res.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<GodigitCKYCCheckResponseDto>();
                    responseBody = JsonConvert.SerializeObject(result);
                    if (result != null)
                    {
                        _logger.LogInformation("CKYC Details {responseBody}", responseBody);
                        var paymentDetailsVm = new PaymentCKCYResponseModel
                        {
                            InsurerStatusCode = (int)HttpStatusCode.OK,
                            ProposalNumber = result.policyNumber,
                            CKYCStatus = result.kycVerificationStatus,
                            CKYCLink = result.link,
                            CKYCFailedReason = result.mismatchType
                        };
                        await UpdateICLogs(id, godigitPaymentCKYCReqModel?.ApplicationId, responseBody);
                        return paymentDetailsVm;
                    }
                }
                else
                {
                    await UpdateICLogs(id, godigitPaymentCKYCReqModel?.ApplicationId, responseBody);
                    return default;
                }
                return default;
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, godigitPaymentCKYCReqModel?.ApplicationId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("GoDigit CKYC Error {exception}", ex.Message);
            await UpdateICLogs(id, godigitPaymentCKYCReqModel?.ApplicationId, ex.Message);

            return default;
        }
    }
    public async Task<GodigitPolicyDocumentResponseDto> GetPolicyDocumentPDF(string leadId, string applicationId, CancellationToken cancellationToken)
    {
        var responseBody = string.Empty;
        var id = 0;
        try
        {
            var godigitPolicyDocumentRequestDto = new GodigitPolicyDocumentRequestDto
            {
                policyId = applicationId
            };
            var requestBody = JsonConvert.SerializeObject(godigitPolicyDocumentRequestDto);
            _logger.LogInformation("Policy Document Generation {requestBody}", requestBody);
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("Authorization", _goDigitConfig.Authorization);

            id = await InsertICLogs(string.Empty, leadId, _goDigitConfig.PolicyDocumentGenerationURL, _goDigitConfig.Authorization, string.Empty, "Payment");
            try
            {
                var res = await _client.PostAsync(_goDigitConfig.PolicyDocumentGenerationURL, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken);

                if (res.IsSuccessStatusCode)
                {
                    var stream = await res.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<GodigitPolicyDocumentResponseDto>();
                    if (result != null)
                    {
                        responseBody = JsonConvert.SerializeObject(result);
                        _logger.LogInformation("Policy Document Generation {responseBody}", responseBody);
                        await UpdateICLogs(id, applicationId, responseBody);
                        return result;
                    }
                }
                else
                {
                    responseBody = await res.Content.ReadAsStringAsync(cancellationToken);
                    await UpdateICLogs(id, applicationId, responseBody);
                    return default;
                }
                return default;
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, applicationId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("Policy Document Generation {exception}", ex.Message);
            await UpdateICLogs(id, applicationId, ex.Message);
            return default;
        }
    }
    public async Task<QuoteResponseModel> GetPolicyStatus(string leadId, string policyNumber, CancellationToken cancellationToken)
    {
        var proposalVm = new QuoteResponseModel();
        var responseBody = string.Empty;
        var id = 0;
        try
        {
            string statusURL = _goDigitConfig.GetPolicyStatusURL + policyNumber;

            id = await InsertICLogs(string.Empty, leadId, statusURL, string.Empty, string.Empty, "Payment");
            try
            {
                var response = await _client.GetAsync(statusURL, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<GoDigitProposalResponseDto>();

                    if (result.error.httpCode == 400 && result.error.validationMessages.Any())
                    {
                        proposalVm.ValidationMessage = string.Join(',', result.error.validationMessages);
                        responseBody = string.Join(',', result.error.validationMessages);
                    }
                    else
                    {
                        proposalVm.ValidationMessage = ValidationMessage;
                        responseBody = response.ReasonPhrase;
                    }

                    proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    _logger.LogError("Unable to fetch policy status {responseBody}", responseBody);
                }
                else
                {
                    var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    var result = stream.DeserializeFromJson<GoDigitProposalResponseDto>();
                    if (result != null)
                    {
                        responseBody = JsonConvert.SerializeObject(result);
                        _logger.LogInformation("create proposal {responseBody}", responseBody);
                        if (result.error.httpCode == 200)
                        {
                            var tax = result.serviceTax;

                            proposalVm = new QuoteResponseModel
                            {
                                InsurerId = _goDigitConfig.InsurerId,
                                InsurerName = _goDigitConfig.InsurerName,
                                InsurerStatusCode = (int)HttpStatusCode.OK,
                                Tax = new ServiceTax()
                                {
                                    totalTax = tax.totalTax.Split(" ")[1]
                                },
                                TotalPremium = result.netPremium.Split(" ")[1],
                                GrossPremium = result.grossPremium.Split(" ")[1],
                                ApplicationId = result.applicationId,
                                ProposalNumber = result.policyNumber,
                                PolicyNumber = result.policyNumber,
                                IsBreakIn = result.motorBreakIn.isBreakin,
                                BreakinStatus = result.policyStatus,
                            };
                        }
                    }
                    else
                    {
                        proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    }
                }
                await UpdateICLogs(id, proposalVm?.ApplicationId, responseBody);
                return proposalVm;
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, proposalVm?.ApplicationId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("GoDigit Proposal Error {exception}", ex.Message);
            proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await UpdateICLogs(id, proposalVm?.ApplicationId, ex.Message);
            return proposalVm;
        }
    }
    public async Task<byte[]> GetDocumentPDFBase64(string documentLink, CancellationToken cancellationToken)
    {
        var id = 0;
        var applicationId = string.Empty;
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, documentLink);
            applicationId = documentLink?.Split("=").LastOrDefault();

            id = await InsertICLogs(string.Empty, string.Empty, documentLink, string.Empty, string.Empty, "Payment");
            try
            {
                var response = await _client.SendAsync(request, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                    await UpdateICLogs(id, applicationId, Convert.ToBase64String(data));
                    return data;
                }
                else
                {
                    var data = await response.Content.ReadAsStringAsync(cancellationToken);
                    await UpdateICLogs(id, applicationId, data);
                }
            }
            catch (Exception ex)
            {
                await UpdateICLogs(id, applicationId, ex.Message);
                return default;
            }

        }
        catch (Exception ex)
        {
            _logger.LogError("GoDigit Policy Document Base64 Error {exception}", ex.Message);
            await UpdateICLogs(id, applicationId, ex.Message);
            return default;
        }
        return default;
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
            InsurerId = _goDigitConfig.InsurerId,
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
}
