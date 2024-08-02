using Insurance.Core.Contracts.Common;
using Insurance.Core.Features.Reliance.Command.CKYC;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.Reliance;
using Insurance.Domain.Reliance.ProposalTaxListResponse;
using Insurance.Domain.Reliance.TPResponse;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Persistence.ICIntegration.Implementation;

public class RelianceService : IRelianceService
{
	private readonly ILogger<RelianceService> _logger;
	private readonly HttpClient _client;
	private readonly RelianceConfig _relianceConfig;
	private readonly PolicyTypeConfig _policyTypeConfig;
	private readonly VehicleTypeConfig _vehicleTypeConfig;
	private readonly IApplicationClaims _applicationClaims;
	private readonly ICommonService _commonService;
	private const string KYC_SUCCESS = "KYC_SUCCESS";
	private const string FAILED = "FAILED";
	private const string POA_REQUIRED = "POA_REQUIRED";
	private const string MESSAGE = "Please enter correct document number or proceed with other insurer";
	private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";
	private const string POA_SUCCESS = "POA_SUCCESS";
	private const string BreakInMessage = "Insurer is currently not providing break-in services. Please retry with any other insurance partner. Please contact support for any assistance.";
	public RelianceService(ILogger<RelianceService> logger, HttpClient client, IOptions<RelianceConfig> option, IOptions<PolicyTypeConfig> optionPolicyType, IOptions<VehicleTypeConfig> optionVehicleType, IApplicationClaims applicationClaims, ICommonService commonService)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_client = client ?? throw new ArgumentNullException(nameof(client));
		_relianceConfig = option.Value;
		_policyTypeConfig = optionPolicyType.Value;
		_vehicleTypeConfig = optionVehicleType.Value;
		_applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
		_commonService = commonService;
	}
	public async Task<RelianceCoverageResponseDto> GetCoverageDetails(QuoteQueryModel quoteQueryModel, string vehicleNumber, string prevPolicyStartDate, string prevPolicyEndDate, bool manufacturerfullybuild, CancellationToken cancellationToken)
	{
		RelianceCoverageResponseDto relianceCoverageResponseDto = new RelianceCoverageResponseDto();
		string requestBody = string.Empty;
		var responseBody = string.Empty;
		try
		{
			_logger.LogInformation("Get Reliance Coverage Details");
			var relianceRequest = new CoveragePolicyDetails
			{
				CoverDetails = string.Empty,
				TrailerDetails = string.Empty,
				ClientDetails = new CoverageClientDetails
				{
					ClientType = 0 //Individual - 0 and Corporate - 1
				},
				Policy = new CoveragePolicy
				{
					BusinessType = quoteQueryModel.IsBrandNewVehicle ? 1 : 5,
					CoverFrom = quoteQueryModel.PolicyStartDate,
					CoverTo = quoteQueryModel.PolicyEndDate,
					BranchCode = _relianceConfig.BranchCode,
					AgentName = "Direct",
					Productcode = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "ProductCode").Select(x => x.ConfigValue).FirstOrDefault(),
					OtherSystemName = 1,
					IsMotorQuote = true,
					IsMotorQuoteFlow = true,
				},
				Risk = new CoverageRisk
				{
					VehicleMakeID = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleMakeCode),
					VehicleModelID = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleModelCode),
					CubicCapacity = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleCubicCapacity),
					RTOLocationID = Convert.ToInt32(quoteQueryModel.RTOLocationCode),
					ManufactureMonth = Convert.ToDateTime(quoteQueryModel.RegistrationDate).ToString("%M"),
					ManufactureYear = Convert.ToInt32(quoteQueryModel.RegistrationYear),
					VehicleVariant = quoteQueryModel.VehicleDetails.VehicleVariant,
					StateOfRegistrationID = Convert.ToInt32(quoteQueryModel.State_Id),
					DateOfPurchase = quoteQueryModel.RegistrationDate,
					RtoRegionCode = quoteQueryModel.RTOLocationCode,
					IDV = Convert.ToString(quoteQueryModel.IDVValue),
					Zone = quoteQueryModel.VehicleDetails.Zone
				},
				Vehicle = new CoverageVehicle
				{
					RegistrationNumber = quoteQueryModel.IsBrandNewVehicle ? "NEW" : vehicleNumber,
					RegistrationDate = quoteQueryModel.RegistrationDate,
					TypeOfFuel = quoteQueryModel.Accessories.IsCNG ? 5 : Convert.ToInt32(quoteQueryModel.VehicleDetails.FuelId),
					SeatingCapacity = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleSeatCapacity),
					PCVVehicleCategory = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleSeatCapacity) > 6 ? Convert.ToInt32(quoteQueryModel.ConfigNameValueModels.FirstOrDefault().VehicleCategoryId) : 0,
					PCVVehicleUsageType = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleSeatCapacity) > 6 ? Convert.ToInt32(quoteQueryModel.ConfigNameValueModels.FirstOrDefault().UsageTypeId) : 0,
					PCVVehicleSubUsageType = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleSeatCapacity) > 6 ? Convert.ToInt32(quoteQueryModel.ConfigNameValueModels.FirstOrDefault().SubUsageTypeId) : 0,
					BodyPrice = Convert.ToInt64(quoteQueryModel.ExShowRoomPrice),
					ChassisPrice = Convert.ToInt64(quoteQueryModel.ChassisPrice),
					ISmanufacturerfullybuild = manufacturerfullybuild

				},
				Cover = new CoverageCover
				{
					IsBasicODCoverage = true,
					IsBasicLiability = true,

					IsBiFuelKit = quoteQueryModel.Accessories.IsCNG,
					BifuelKit = new CoverageBifuelKit
					{
						BifuelKits = new CoverageBifuelKit
						{
							IsChecked = false,
							IsMandatory = false,
							PolicyCoverDetailsID = string.Empty,
							Fueltype = quoteQueryModel.Accessories.IsCNG ? "CNG" : null,
							ISLpgCng = quoteQueryModel.Accessories.IsCNG,
							SumInsured = quoteQueryModel.Accessories.CNGValue,
						}
					},
				},
				PreviousInsuranceDetails = new CoveragePreviousInsuranceDetails
				{
					PrevYearInsurer = quoteQueryModel.PreviousPolicyDetails.PreviousInsurerCode,
					PrevYearPolicyNo = string.IsNullOrEmpty(quoteQueryModel.PreviousPolicyDetails.PreviousPolicyNumber) ? quoteQueryModel.PreviousPolicyDetails.PreviousPolicyNumber : quoteQueryModel.PreviousPolicyDetails.PreviousPolicyNumberTP,
					PrevYearInsurerAddress = string.Empty,
					PrevYearPolicyType = string.Empty,//doubt
					PrevYearPolicyStartDate = prevPolicyStartDate,
					PrevYearPolicyEndDate = prevPolicyEndDate,
				},
				Productcode = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "ProductCode").Select(x => x.ConfigValue).FirstOrDefault(),
				NCBEligibility = new CoverageNCBEligibility
				{
					NCBEligibilityCriteria = quoteQueryModel.PreviousPolicyDetails.IsClaimInLastYear ? 1 : 2,
					PreviousNCB = Convert.ToInt32(quoteQueryModel.PreviousPolicyDetails.PreviousNoClaimBonusValue),
				},
				UserID = _relianceConfig.UserID,
				SourceSystemID = _relianceConfig.UserID,
				AuthToken = _relianceConfig.AuthToken
			};

			_client.DefaultRequestHeaders.Clear();

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(CoveragePolicyDetails));
			StringBuilder requestBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(requestBuilder);
			xmlSerializer.Serialize(stringWriter, relianceRequest);

			requestBody = RemoveXmlDefinition(requestBuilder.ToString());
			_logger.LogInformation("Reliance Coverage RequestBody {request}", requestBody);

			var id = 0;
			id = await InsertICLogs(requestBody, quoteQueryModel?.LeadId, _relianceConfig.BaseURL + _relianceConfig.CoverageURL, _relianceConfig.AuthToken, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "Quote");

			var quoteResponse = await _client.PostAsync(_relianceConfig.CoverageURL, new StringContent(requestBody, Encoding.UTF8, "application/xml"), cancellationToken);
			var response = await quoteResponse.Content.ReadAsStringAsync(cancellationToken);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(response);
			string result = JsonConvert.SerializeXmlNode(doc);
			relianceCoverageResponseDto = JsonConvert.DeserializeObject<RelianceCoverageResponseDto>(result);
			if (!string.IsNullOrEmpty(relianceCoverageResponseDto.ListCovers.ErrorMessages))
			{
				responseBody = relianceCoverageResponseDto.ListCovers.ErrorMessages;
				_logger.LogError("Reliance Coverage Response {responseBody}", responseBody);
				UpdateICLogs(id, relianceCoverageResponseDto.ListCovers.TraceID, responseBody);
				return relianceCoverageResponseDto;
			}
			else
			{
				responseBody = JsonConvert.SerializeObject(relianceCoverageResponseDto);
				_logger.LogInformation("Reliance Coverage Response {responseBody}", responseBody);
				UpdateICLogs(id, relianceCoverageResponseDto.ListCovers.TraceID, responseBody);
			}
			return relianceCoverageResponseDto;
		}
		catch (Exception ex)
		{
			_logger.LogError("Reliance Coverage Exception {Message}", ex.Message);
		}
		return default;
	}
	public async Task<QuoteResponseModelGeneric> GetQuote(QuoteQueryModel quoteQueryModel, CancellationToken cancellationToken)
	{
		string requestBody = string.Empty;
		string vehicleNumber = string.Empty;
		string prevPolicyStartDate = string.Empty;
		string prevPolicyEndDate = string.Empty;

		if (!quoteQueryModel.IsBrandNewVehicle)
		{
			if (_policyTypeConfig.SATP.Equals(quoteQueryModel.CurrentPolicyType))
			{
				prevPolicyStartDate = quoteQueryModel.PreviousPolicyDetails.PreviousPolicyStartDateSATP;
				prevPolicyEndDate = quoteQueryModel.PreviousPolicyDetails.PreviousPolicyExpiryDateSATP;
			}
			else
			{
				prevPolicyStartDate = quoteQueryModel.PreviousPolicyDetails.PreviousPolicyStartDateSAOD;
				prevPolicyEndDate = quoteQueryModel.PreviousPolicyDetails.PreviousPolicyExpiryDateSAOD;
			}
		}

		if (string.IsNullOrEmpty(quoteQueryModel.VehicleNumber))
		{
			vehicleNumber = string.Join("-", VehicleNumberSplit(quoteQueryModel.VehicleDetails.LicensePlateNumber)) + "-AB-0000";
		}
		else
		{
			if (!quoteQueryModel.IsBrandNewVehicle && quoteQueryModel.VehicleNumber.Length == 4)
				vehicleNumber = string.Join("-", VehicleNumberSplit(quoteQueryModel.VehicleNumber)) + "-AB-0000";
			else
				vehicleNumber = string.Join("-", VehicleNumberSplit(quoteQueryModel.VehicleNumber));
		}
		bool manufacturerfullybuild = quoteQueryModel.VehicleDetails.IsCommercialVehicle ? false : true;

		var getCoverageDetails = await GetCoverageDetails(quoteQueryModel, vehicleNumber, prevPolicyStartDate, prevPolicyEndDate, manufacturerfullybuild, cancellationToken);

		if (!string.IsNullOrEmpty(getCoverageDetails.ListCovers.ErrorMessages))
		{
			var errRes = new QuoteResponseModelGeneric()
			{
				QuoteResponseModel = new QuoteResponseModel()
				{
					ValidationMessage = getCoverageDetails.ListCovers.ErrorMessages
				}
			};
			return errRes;
		}

		CoverageList coverageList = getCoverageDetails?.ListCovers?.CoverageList;
		LstAddonCovers lstAddonCovers = getCoverageDetails?.ListCovers?.LstAddonCovers;
		ReliancePackageAddonResponseModel packageResponse = new();
		if (lstAddonCovers != null)
		{
			packageResponse = ReliancePackageLogic(quoteQueryModel, lstAddonCovers);
		}

		try
		{
			_logger.LogInformation("Reliance Service Start");

			var relianceRequest = new PolicyDetails
			{
				CoverDetails = string.Empty,
				TrailerDetails = string.Empty,
				ClientDetails = new ClientDetails
				{
					ClientType = 0 //Individual - 0 and Corporate - 1
				},
				Policy = new Policy
				{
					BusinessType = quoteQueryModel.IsBrandNewVehicle ? 1 : 5,
					Cover_From = quoteQueryModel.PolicyStartDate,
					Cover_To = quoteQueryModel.PolicyEndDate,
					Branch_Code = _relianceConfig.BranchCode,
					AgentName = "Direct",
					productcode = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "ProductCode").Select(x => x.ConfigValue).FirstOrDefault(),
					OtherSystemName = 1,
					isMotorQuote = true,
					isMotorQuoteFlow = true,
				},
				Risk = new Risk
				{
					VehicleMakeID = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleMakeCode),
					VehicleModelID = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleModelCode),
					CubicCapacity = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleCubicCapacity),
					RTOLocationID = Convert.ToInt32(quoteQueryModel.RTOLocationCode),
					ExShowroomPrice = Convert.ToInt32(quoteQueryModel.ExShowRoomPrice), //doubt
					ManufactureMonth = Convert.ToDateTime(quoteQueryModel.RegistrationDate).ToString("%M"),
					ManufactureYear = Convert.ToInt32(quoteQueryModel.RegistrationYear),
					VehicleVariant = quoteQueryModel.VehicleDetails.VehicleVariant,
					StateOfRegistrationID = Convert.ToInt32(quoteQueryModel.State_Id),
					DateOfPurchase = quoteQueryModel.RegistrationDate,
					IsVehicleHypothicated = false,
					FinanceType = string.Empty,
					FinancierName = string.Empty,
					FinancierAddress = string.Empty,
					FinancierCity = string.Empty,
					Rto_RegionCode = quoteQueryModel.RTOLocationCode,
					IDV = Convert.ToString(quoteQueryModel.IDVValue),
					NoOfWheels = Convert.ToInt32(quoteQueryModel.VehicleDetails.NoOfWheels),
					Colour = string.Empty,
					BodyType = string.Empty,
					OtherColour = string.Empty,
					GrossVehicleWeight = string.Empty,
					Zone = quoteQueryModel.VehicleDetails.Zone,
					LicensedCarryingCapacity = string.Empty,
					PurposeOfUsage = string.Empty,
					EngineNo = string.IsNullOrEmpty(quoteQueryModel.VehicleDetails.EngineNumber) ? string.Empty : quoteQueryModel.VehicleDetails.EngineNumber,
					Chassis = string.IsNullOrEmpty(quoteQueryModel.VehicleDetails.Chassis) ? string.Empty : quoteQueryModel.VehicleDetails.Chassis,
					TrailerIDV = string.Empty,
					BodyIDV = string.Empty,
					ChassisIDV = string.Empty,
					Rto_State_City = string.Empty,
					SalesManagerCode = "Direct",
					SalesManagerName = "Direct",
					IsRegAddressSameasCommAddress = true,
					IsPermanentAddressSameasCommAddress = true,
					IsInspectionAddressSameasCommAddress = true,
					IsRegAddressSameasPermanentAddress = true
				},
				Vehicle = new Vehicle
				{
					Registration_Number = quoteQueryModel.IsBrandNewVehicle ? "NEW" : vehicleNumber,
					Registration_date = quoteQueryModel.RegistrationDate,
					TypeOfFuel = (quoteQueryModel.Accessories.IsCNG && coverageList.CoverDetail.Exists(x => x.CoverageID == "4")) ? 5 : Convert.ToInt32(quoteQueryModel.VehicleDetails.FuelId),
					MiscTypeOfVehicleID = string.Empty,
					SeatingCapacity = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleSeatCapacity),
					IsNewVehicle = quoteQueryModel.IsBrandNewVehicle,
					PCVVehicleCategory = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleSeatCapacity) > 6 ? Convert.ToInt32(quoteQueryModel.ConfigNameValueModels.FirstOrDefault().VehicleCategoryId) : 0,
					PCVVehicleUsageType = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleSeatCapacity) > 6 ? Convert.ToInt32(quoteQueryModel.ConfigNameValueModels.FirstOrDefault().UsageTypeId) : 0,
					PCVVehicleSubUsageType = Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleSeatCapacity) > 6 ? Convert.ToInt32(quoteQueryModel.ConfigNameValueModels.FirstOrDefault().SubUsageTypeId) : 0,
					//
					BodyPrice = Convert.ToInt64(quoteQueryModel.ExShowRoomPrice),
					ChassisPrice = Convert.ToInt64(quoteQueryModel.ChassisPrice),
					ISmanufacturerfullybuild = manufacturerfullybuild,
					//
				},
				Cover = new Cover
				{
					IsBasicODCoverage = !quoteQueryModel.PolicyTypeId.Equals(_policyTypeConfig.SATP),
					BasicODCoverage = new BasicODCoverage
					{
						basicODCoverage = new BasicODCoverage
						{
							IsMandatory = false,
							IsChecked = !quoteQueryModel.PolicyTypeId.Equals(_policyTypeConfig.SATP),
							NoOfItems = string.Empty,
							PackageName = string.Empty
						}
					},
					IsBasicLiability = !quoteQueryModel.PolicyTypeId.Equals(_policyTypeConfig.SAOD),
					BasicLiability = new BasicLiabilitys
					{
						BasicLiability = new BasicLiabilitys
						{
							IsMandatory = false,
							IsChecked = !quoteQueryModel.PolicyTypeId.Equals(_policyTypeConfig.SAOD),
							NoOfItems = string.Empty,
							PackageName = string.Empty
						}
					},
					//Accessories
					IsElectricalItemFitted = quoteQueryModel.Accessories.IsElectrical && coverageList.CoverDetail.Exists(x => x.CoverageID == "1"),
					ElectricalItemsTotalSI = Convert.ToString(quoteQueryModel.Accessories.ElectricalValue),
					ElectricItems = new ElectricItems
					{
						ElectricalItems = new ElectricalItems
						{
							ElectricalItemsID = string.Empty,
							PolicyId = string.Empty,
							SerialNo = string.Empty,
							MakeModel = string.Empty,
							ElectricPremium = string.Empty,
							Description = string.Empty,
							ElectricalAccessorySlNo = string.Empty,
							SumInsured = Convert.ToString(quoteQueryModel.Accessories.ElectricalValue),
						}
					},
					IsNonElectricalItemFitted = quoteQueryModel.Accessories.IsNonElectrical && coverageList.CoverDetail.Exists(x => x.CoverageID == "2"),
					NonElectricalItemsTotalSI = Convert.ToString(quoteQueryModel.Accessories.NonElectricalValue),
					NonElectricItems = new NonElectricItems
					{
						NonElectricalItems = new NonElectricalItems
						{
							NonElectricalItemsID = string.Empty,
							PolicyID = string.Empty,
							SerialNo = string.Empty,
							MakeModel = string.Empty,
							NonElectricPremium = string.Empty,
							Description = string.Empty,
							Category = string.Empty,
							NonElectricalAccessorySlNo = string.Empty,
							SumInsured = Convert.ToString(quoteQueryModel.Accessories.NonElectricalValue),
						}
					},
					IsBiFuelKit = quoteQueryModel.Accessories.IsCNG && coverageList.CoverDetail.Exists(x => x.CoverageID == "4"),
					BiFuelKitSi = Convert.ToString(quoteQueryModel.Accessories.CNGValue),
					BifuelKit = new BifuelKits
					{
						BifuelKit = new BifuelKits
						{
							IsChecked = quoteQueryModel.Accessories.IsCNG && coverageList.CoverDetail.Exists(x => x.CoverageID == "4"),
							IsMandatory = false,
							PolicyCoverDetailsID = string.Empty,
							Fueltype = quoteQueryModel.Accessories.IsCNG ? "CNG" : null,
							ISLpgCng = false,
							PolicyCoverID = string.Empty,
							SumInsured = quoteQueryModel.Accessories.CNGValue,
							NoOfItems = string.Empty,
							PackageName = string.Empty,
						}
					},
					//PACover
					IsPAToOwnerDriverCoverd = quoteQueryModel.PACover.IsUnnamedOWNERDRIVER && coverageList.CoverDetail.Exists(x => x.CoverageID == "24"),
					PACoverToOwner = new PACoverToOwners
					{
						PACoverToOwner = new PACoverToOwners
						{
							IsChecked = quoteQueryModel.PACover.IsUnnamedOWNERDRIVER && coverageList.CoverDetail.Exists(x => x.CoverageID == "24"),
							CPAcovertenure = quoteQueryModel.VehicleDetails.IsCommercialVehicle ? 1 : NestedTernaryOperatorMethod(quoteQueryModel.IsBrandNewVehicle, quoteQueryModel.VehicleDetails.IsFourWheeler),
							NoOfItems = string.Empty,
							PackageName = string.Empty,
							AppointeeName = string.Empty,
							NomineeName = string.Empty,
							NomineeDOB = string.Empty,
							NomineeRelationship = string.Empty,
							NomineeAddress = string.Empty,
							OtherRelation = string.Empty,
						},
					},
					IsLiabilityToPaidDriverCovered = quoteQueryModel.PACover.IsPaidDriver && coverageList.CoverDetail.Exists(x => x.CoverageID == "13"),
					LiabilityToPaidDriver = new LiabilityToPaidDrivers
					{
						LiabilityToPaidDriver = new LiabilityToPaidDrivers
						{
							IsMandatory = false,
							IsChecked = quoteQueryModel.PACover.IsPaidDriver && coverageList.CoverDetail.Exists(x => x.CoverageID == "13"),
							NoOfItems = "1",
							PackageName = string.Empty,
							PolicyCoverID = string.Empty,
						},
					},
					IsPAToUnnamedPassengerCovered = quoteQueryModel.VehicleDetails.IsFourWheeler ? (quoteQueryModel.PACover.IsUnnamedPassenger && coverageList.CoverDetail.Where(x => x.CoverageID == "16").Any()) : (quoteQueryModel.PACover.IsUnnamedPillionRider && coverageList.CoverDetail.Where(x => x.CoverageID == "16").Any()),
					NoOfUnnamedPassenegersCovered = quoteQueryModel.PACover.IsUnnamedPassenger || quoteQueryModel.PACover.IsUnnamedPillionRider ? Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleSeatCapacity) : 0,
					UnnamedPassengersSI = quoteQueryModel.PACover.IsUnnamedPassenger || quoteQueryModel.PACover.IsUnnamedPillionRider ? quoteQueryModel.PACover.UnnamedPassengerValue : 0,
					PAToUnNamedPassenger = new PAToUnNamedPassengers
					{
						PAToUnNamedPassenger = new PAToUnNamedPassengers
						{
							IsMandatory = false,
							IsChecked = quoteQueryModel.VehicleDetails.IsFourWheeler ? (quoteQueryModel.PACover.IsUnnamedPassenger && coverageList.CoverDetail.Where(x => x.CoverageID == "16").Any()) : quoteQueryModel.PACover.IsUnnamedPillionRider,
							NoOfItems = quoteQueryModel.PACover.IsUnnamedPassenger || quoteQueryModel.PACover.IsUnnamedPillionRider ? Convert.ToInt32(quoteQueryModel.VehicleDetails.VehicleSeatCapacity) : 0,
							PackageName = string.Empty,
							PolicyCoverID = string.Empty,
							SumInsured = quoteQueryModel.PACover.IsUnnamedPassenger || quoteQueryModel.PACover.IsUnnamedPillionRider ? quoteQueryModel.PACover.UnnamedPassengerValue : 0
						},
					},
					IsPAToConductorCovered = quoteQueryModel.VehicleDetails.IsCommercial ? (quoteQueryModel.PACover.IsUnnamedConductor && coverageList.CoverDetail.Where(x => x.CoverageID == "16").Any()) : false,
					PAToConductor = new PAToConductor
					{
						pAToConductor = new PAToConductor
						{
							IsMandatory = false,
							IsChecked = quoteQueryModel.VehicleDetails.IsCommercial ? (quoteQueryModel.PACover.IsUnnamedConductor && coverageList.CoverDetail.Where(x => x.CoverageID == "131").Any()) : false,
							NoOfItems = "1",
							SumInsured = string.Empty,
						},
					},
					IsPAToPaidCleanerCovered = quoteQueryModel.VehicleDetails.IsCommercial ? (quoteQueryModel.PACover.IsUnnamedConductor && coverageList.CoverDetail.Where(x => x.CoverageID == "16").Any()) : false,
					PAToPaidCleaner = new PAToPaidCleaners
					{
						PAToPaidCleaner = new PAToPaidCleaners
						{
							IsMandatory = false,
							IsChecked = quoteQueryModel.VehicleDetails.IsCommercial ? (quoteQueryModel.PACover.IsUnnamedConductor && coverageList.CoverDetail.Where(x => x.CoverageID == "123").Any()) : false,
							NoOfItems = "1",
							SumInsured = quoteQueryModel.PACover.UnnamedCleanerValue,
						},
					},


					//Discount
					IsAutomobileAssociationMember = quoteQueryModel.Discounts.IsAAMemberShip && coverageList.CoverDetail.Exists(x => x.CoverageID == "19"),
					AutomobileAssociationName = string.Empty,
					AutomobileAssociationNo = string.Empty,
					AutomobileAssociationExpiryDate = string.Empty,
					AutomobileAssociationMembershipDiscount = new AutomobileAssociationMembershipDiscount
					{
						automobileAssociationMembershipDiscount = new AutomobileAssociationMembershipDiscount
						{
							IsMandatory = false,
							IsChecked = quoteQueryModel.Discounts.IsAAMemberShip && coverageList.CoverDetail.Exists(x => x.CoverageID == "19"),
							NoOfItems = string.Empty,
							PackageName = string.Empty,
						},
					},
					IsAntiTheftDeviceFitted = quoteQueryModel.Discounts.IsAntiTheft && coverageList.CoverDetail.Exists(x => x.CoverageID == "11"),
					AntiTheftDeviceDiscount = new AntiTheftDeviceDiscount
					{
						antiTheftDeviceDiscount = new AntiTheftDeviceDiscount
						{
							IsMandatory = false,
							IsChecked = quoteQueryModel.Discounts.IsAntiTheft && coverageList.CoverDetail.Exists(x => x.CoverageID == "11"),
							NoOfItems = string.Empty,
							PackageName = string.Empty,
						},
					},
					IsVoluntaryDeductableOpted = quoteQueryModel.Discounts.IsVoluntarilyDeductible && coverageList.CoverDetail.Exists(x => x.CoverageID == "3"),
					VoluntaryDeductible = new VoluntaryDeductibles
					{
						VoluntaryDeductible = new VoluntaryDeductibles
						{
							IsMandatory = false,
							SumInsured = quoteQueryModel.Discounts.IsVoluntarilyDeductible ? quoteQueryModel.VoluntaryExcess : "0",
							IsChecked = quoteQueryModel.Discounts.IsVoluntarilyDeductible && coverageList.CoverDetail.Exists(x => x.CoverageID == "3"),
							NoOfItems = string.Empty,
							PolicyCoverID = string.Empty,
							PackageName = string.Empty
						},
					},
					IsTPPDCover = false,
					TPPDCover = new TPPDCovers
					{
						TPPDCover = new TPPDCovers
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
							PackageName = string.Empty,
							PolicyCoverID = string.Empty,
							SumInsured = string.Empty
						}
					},
					//AddOns
					IsGeographicalAreaExtended = quoteQueryModel.AddOns.IsGeoAreaExtension && coverageList.CoverDetail.Exists(x => x.CoverageID == "5"),
					GeographicalExtension = new GeographicalExtension
					{
						IsMandatory = false,
						IsChecked = quoteQueryModel.AddOns.IsGeoAreaExtension && coverageList.CoverDetail.Exists(x => x.CoverageID == "5"),
						Countries = quoteQueryModel.GeogExtension,
					},
					IsNilDepreciation = packageResponse.IsZeroDept,
					NilDepreciationCoverage = new NilDepreciationCoverages
					{
						NilDepreciationCoverage = new NilDepreciationCoverages
						{
							IsMandatory = false,
							IsChecked = packageResponse.IsZeroDept,
							NoOfItems = string.Empty,
							PackageName = string.Empty,
							PolicyCoverID = string.Empty,
							ApplicableRate = lstAddonCovers?.AddonCovers?.Where(x => x.CoverageID == "10").Select(x => x.rate).FirstOrDefault(),
						}
					},
					IsSecurePlus = packageResponse.IsSecurePlus,
					SecurePlus = new SecurePluss
					{
						SecurePlus = new SecurePluss
						{
							IsMandatory = false,
							IsChecked = packageResponse.IsSecurePlus,
							PolicyCoverID = string.Empty,
							PackageName = string.Empty,
							NoOfItems = string.Empty,
							ApplicableRate = lstAddonCovers?.AddonCovers?.Where(x => x.CoverageID == "374").Select(x => x.rate).FirstOrDefault(),
						}
					},
					IsSecurePremium = packageResponse.IsSecurePremium,
					SecurePremium = new SecurePremiums
					{
						SecurePremium = new SecurePremiums
						{
							IsMandatory = false,
							IsChecked = packageResponse.IsSecurePremium,
							PolicyCoverID = string.Empty,
							PackageName = string.Empty,
							NoOfItems = string.Empty,
							ApplicableRate = lstAddonCovers?.AddonCovers?.Where(x => x.CoverageID == "375").Select(x => x.rate).FirstOrDefault(),
						}
					},
					IsAdditionalTowingCover = false,
					AdditionalTowingCoverage = new AdditionalTowingCoverage
					{
						additionalTowingCoverage = new AdditionalTowingCoverage
						{
							IsMandatory = false,
							IsChecked = false,
							PackageName = string.Empty,
							NoOfItems = string.Empty,
							SumInsured = string.Empty
						}
					},
					EMIprotectionCover = string.Empty,
					IsCoverageoFTyreBumps = false,
					IsTotalCover = false,
					TotalCover = new TotalCover
					{
						totalCover = new TotalCover
						{
							IsMandatory = false,
							IsChecked = false,
							PolicyCoverID = string.Empty,
							PackageName = string.Empty,
							NoOfItems = string.Empty,
						}
					},

					IsImt23LampOrTyreTubeOrHeadlightCover = false,

					IsFibreGlassFuelTankFitted = false,
					FibreGlassFuelTank = new FibreGlassFuelTank
					{
						fibreGlassFuelTank = new FibreGlassFuelTank
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
							PackageName = string.Empty,
						}
					},
					IsLiabilityToEmployeeCovered = false,
					LiabilityToEmployee = new LiabilityToEmployee
					{
						liabilityToEmployee = new LiabilityToEmployee
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
							PackageName = string.Empty,
							PolicyCoverID = string.Empty,
						},
					},
					IsPAToDriverCovered = false,
					PAToPaidDriver = new PAToPaidDrivers
					{
						PAToPaidDriver = new PAToPaidDrivers
						{
							IsChecked = false,
							NoOfItems = string.Empty,
							SumInsured = string.Empty,
						},
					},
					IsInsurancePremium = false,

					IsUsedForDrivingTuition = false,

					IsSpeciallyDesignedForHandicapped = false,
					SpeciallyDesignedforChallengedPerson = new SpeciallyDesignedforChallengedPerson
					{
						speciallyDesignedforChallengedPerson = new SpeciallyDesignedforChallengedPerson
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
							PackageName = string.Empty,
						},
					},
					RegistrationCost = new RegistrationCost
					{
						registrationCost = new RegistrationCost
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
							PackageName = string.Empty,
							SumInsured = string.Empty
						}
					},
					RoadTax = new RoadTax
					{
						roadTax = new RoadTax
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
							PackageName = string.Empty,
							SumInsured = string.Empty
						}
					},
					InsurancePremium = new InsurancePremium
					{
						insurancePremium = new InsurancePremium
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
							PackageName = string.Empty,
							SumInsured = string.Empty
						}
					},
					PAToNamedPassenger = new PAToNamedPassengers
					{
						PAToNamedPassenger = new PAToNamedPassengers
						{
							IsMandatory = false,
							IsChecked = false,
							PackageName = string.Empty,
							NoOfItems = string.Empty,
							SumInsured = string.Empty
						}
					},
					DrivingTuitionCoverage = new DrivingTuitionCoverage
					{
						drivingTuitionCoverage = new DrivingTuitionCoverage
						{
							IsMandatory = false,
							IsChecked = false,
							PackageName = string.Empty,
							NoOfItems = string.Empty,
						}
					},
					IsHandicappedDiscount = false,
					NFPPIncludingEmployees = new NFPPIncludingEmployees
					{
						nFPPIncludingEmployees = new NFPPIncludingEmployees
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
						}
					},
					NFPPExcludingEmployees = new NFPPExcludingEmployees
					{
						nFPPExcludingEmployees = new NFPPExcludingEmployees
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
						}
					},
					WorkmenCompensationExcludingDriver = new WorkmenCompensationExcludingDriver
					{
						workmenCompensationExcludingDriver = new WorkmenCompensationExcludingDriver
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
						}
					},
					LiabilityToConductor = new LiabilityToConductor
					{
						liabilityToConductor = new LiabilityToConductor
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
						}
					},
					LiabilitytoCoolie = new LiabilitytoCoolie
					{
						liabilitytoCoolie = new LiabilitytoCoolie
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
						}
					},
					LegalLiabilitytoCleaner = string.Empty,
					IndemnityToHirer = new IndemnityToHirer
					{
						indemnityToHirer = new IndemnityToHirer
						{
							IsMandatory = false,
							IsChecked = false,
							NoOfItems = string.Empty,
						}
					},
					TrailerDetails = new TrailerDetails
					{
						TrailerInfo = new TrailerInfo
						{
							MakeandModel = string.Empty,
							IDV = 0,
							Registration_No = string.Empty,
							ChassisNumber = string.Empty,
							ManufactureYear = string.Empty,
							SerialNumber = string.Empty
						}
					},
				},
				PreviousInsuranceDetails = new PreviousInsuranceDetails
				{
					PrevInsuranceID = quoteQueryModel.PreviousPolicyDetails.PreviousInsurerCode,
					IsVehicleOfPreviousPolicySold = string.Empty,
					IsNCBApplicable = !quoteQueryModel.PreviousPolicyDetails.IsClaimInLastYear,
					PrevYearInsurer = !string.IsNullOrEmpty(quoteQueryModel.PreviousPolicyDetails.PreviousSAODInsurer) ? quoteQueryModel.PreviousPolicyDetails.PreviousSAODInsurer : quoteQueryModel.PreviousPolicyDetails.PreviousSATPInsurer,
					PrevYearPolicyNo = !string.IsNullOrEmpty(quoteQueryModel.PreviousPolicyDetails.PreviousPolicyNumber) ? quoteQueryModel.PreviousPolicyDetails.PreviousPolicyNumber : quoteQueryModel.PreviousPolicyDetails.PreviousPolicyNumberTP,
					PrevYearInsurerAddress = string.Empty,
					DocumentProof = string.Empty,
					PrevYearPolicyType = string.Empty,//doubt
					PrevYearPolicyStartDate = prevPolicyStartDate,
					PrevYearPolicyEndDate = prevPolicyEndDate,
					MTAReason = string.Empty,
					PrevYearNCB = Convert.ToInt32(quoteQueryModel.PreviousPolicyDetails.PreviousNoClaimBonusValue),
					IsInspectionDone = string.Empty,
					InspectionDate = string.Empty,
					Inspectionby = string.Empty,
					InspectorName = string.Empty,
					IsNCBEarnedAbroad = string.Empty,
					ODLoading = string.Empty,
					IsClaimedLastYear = "false",
					ODLoadingReason = string.Empty,
					PreRateCharged = string.Empty,
					PreSpecialTermsAndConditions = string.Empty,
					IsTrailerNCB = string.Empty,
					InspectionID = string.Empty,
				},
				NCBEligibility = new NCBEligibility
				{
					NCBEligibilityCriteria = quoteQueryModel.PreviousPolicyDetails.IsClaimInLastYear ? 1 : 2,
					NCBReservingLetter = string.Empty,
					PreviousNCB = Convert.ToInt32(quoteQueryModel.PreviousPolicyDetails.PreviousNoClaimBonusValue),
				},
				LstCoveragePremium = string.Empty,
				ProductCode = quoteQueryModel.ConfigNameValueModels.Where(x => x.ConfigName == "ProductCode").Select(x => x.ConfigValue).FirstOrDefault(),
				UserID = _relianceConfig.UserID,
				SourceSystemID = _relianceConfig.UserID,
				AuthToken = _relianceConfig.AuthToken
			};

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(PolicyDetails));
			StringBuilder requestBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(requestBuilder);
			xmlSerializer.Serialize(stringWriter, relianceRequest);

			requestBody = RemoveXmlDefinition(requestBuilder.ToString());
			_logger.LogInformation("Reliance Quote RequestBody {request}", requestBody);

			return await QuoteResponseFraming(requestBody, quoteQueryModel, packageResponse, cancellationToken);
		}
		catch (Exception ex)
		{
			_logger.LogError("Reliance Quote RequestBody Exception {exception}", ex.Message);
		}
		return default;
	}
	private async Task<QuoteResponseModelGeneric> QuoteResponseFraming(string requestBody, QuoteQueryModel quoteQuery, ReliancePackageAddonResponseModel packageResponse, CancellationToken cancellationToken)
	{
		var quoteResponseVm = new QuoteResponseModelGeneric();
		var quoteVm = new QuoteResponseModel();
		string responseBody = string.Empty;

		var id = 0;
		id = await InsertICLogs(requestBody, quoteQuery?.LeadId, _relianceConfig.BaseURL + _relianceConfig.QuoteURL, _relianceConfig.AuthToken, null, "Quote");
		var responseMessage = await GetQuoteResponse(quoteQuery?.LeadId, "Quote", requestBody, cancellationToken);

		_logger.LogInformation("Reliance Quote Request {request}", requestBody);

		var response = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
		if (!string.IsNullOrEmpty(XElement.Parse(response).Element("ErrorMessages").Value))
		{
			responseBody = XElement.Parse(response).Element("ErrorMessages").Value;
			quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
			quoteVm.InsurerName = _relianceConfig.InsurerName;
			_logger.LogError("Reliance Quote Response {responseBody}", responseBody);
			UpdateICLogs(id, XElement.Parse(response).Element("TraceID").Value, responseBody);
		}
		else
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(response);
			string result = JsonConvert.SerializeXmlNode(doc);
			var quoteResponseObject = JsonConvert.DeserializeObject<RelianceQuoteResponseDto>(result);
			responseBody = JsonConvert.SerializeObject(quoteResponseObject);
			_logger.LogInformation("Reliance Quote Response {responseBody}", responseBody);
			var lstTaxComponent = new List<TaxComponent>();
			var isBrancket = quoteResponseObject.MotorPolicy.LstTaxComponentDetails.TaxComponent.ToString().Contains("[");
			JToken token;
			// JToken token1;
			var taxComp = quoteResponseObject.MotorPolicy?.LstTaxComponentDetails?.TaxComponent;
			var taxAmount = string.Empty;
			if (isBrancket)
			{

				if (taxComp != null)
				{
					foreach (var taxItem in taxComp)
					{
						var tokenn = JObject.Parse(taxItem?.ToString());
						if (tokenn != null)
						{
							lstTaxComponent.Add(JsonConvert.DeserializeObject<TaxComponent>(tokenn.ToString()));
						}
					}

					var totalTax = lstTaxComponent.Sum(tax => Convert.ToDouble(tax.Amount));
					taxAmount = Math.Round(totalTax).ToString();
				}
			}
			else
			{
				token = JObject.Parse(quoteResponseObject.MotorPolicy.LstTaxComponentDetails?.TaxComponent?.ToString());
				var taxComponent = JsonConvert.DeserializeObject<TaxComponent>(token.ToString());
				lstTaxComponent.Add(taxComponent);
				var totalTax = Math.Round(Convert.ToDouble(taxComponent.Amount));
				taxAmount = Convert.ToString(totalTax);
			}
			if (quoteResponseObject != null && !string.IsNullOrEmpty(quoteResponseObject.MotorPolicy.NetPremium))
			{
				quoteVm.InsurerStatusCode = (int)HttpStatusCode.OK;
				double recIdv, minIdv, maxIdv;
				if (_vehicleTypeConfig.Commerical.Equals(quoteQuery.VehicleTypeId))
				{
					recIdv = string.IsNullOrEmpty(quoteResponseObject.MotorPolicy.MinIDV) ? Convert.ToDouble(quoteQuery.RecommendedIDV) : Convert.ToDouble(quoteResponseObject.MotorPolicy.IDV);

					minIdv = string.IsNullOrEmpty(quoteResponseObject.MotorPolicy.MinIDV) ? Convert.ToDouble(quoteQuery.MinIDV) : Convert.ToDouble(quoteResponseObject.MotorPolicy.MinIDV);

					maxIdv = string.IsNullOrEmpty(quoteResponseObject.MotorPolicy.MaxIDV) ? Convert.ToDouble(quoteQuery.MaxIDV) : Convert.ToDouble(quoteResponseObject.MotorPolicy.MaxIDV);
				}
				else
				{
					recIdv = string.IsNullOrEmpty(quoteResponseObject.MotorPolicy.DerivedVehicleIDV) ? 0 : Convert.ToDouble(quoteResponseObject.MotorPolicy.DerivedVehicleIDV);
					minIdv = string.IsNullOrEmpty(quoteResponseObject.MotorPolicy.MinIDV) ? Math.Round(Convert.ToDouble(recIdv) * 0.9) : Convert.ToDouble(quoteResponseObject.MotorPolicy.MinIDV); // 10% less from recommended
					maxIdv = 0;

					if (quoteQuery.IsBrandNewVehicle)
						maxIdv = string.IsNullOrEmpty(quoteResponseObject.MotorPolicy.MaxIDV) ? Math.Round(Convert.ToDouble(recIdv) * 1.2) : Convert.ToDouble(quoteResponseObject.MotorPolicy.MaxIDV); // 20% more from recommended for brandnew
					else
						maxIdv = string.IsNullOrEmpty(quoteResponseObject.MotorPolicy.MaxIDV) ? Math.Round(Convert.ToDouble(recIdv) * 1.1) : Convert.ToDouble(quoteResponseObject.MotorPolicy.MaxIDV); // 10% more from recommended for renewal
				}

				var accessoriesCover = SetAccessoriesCover(quoteQuery, quoteResponseObject);
				var discountCover = SetDiscountCover(quoteQuery, quoteResponseObject);
				var paCover = SetPACover(quoteQuery, quoteResponseObject);
				var addOnCover = SetAddonsCover(quoteQuery, packageResponse, quoteResponseObject);
				var setBaseCover = SetBaseCover(quoteQuery?.PolicyTypeId, quoteResponseObject);

				quoteVm = new QuoteResponseModel()
				{
					InsurerName = _relianceConfig.InsurerName,
					InsurerStatusCode = (int)HttpStatusCode.OK,
					TotalPremium = Math.Round(Convert.ToDouble(quoteResponseObject.MotorPolicy.NetPremium)).ToString(),
					GrossPremium = Math.Round(Convert.ToDouble(quoteResponseObject.MotorPolicy.FinalPremium)).ToString(),
					SelectedIDV = (quoteQuery.IsBrandNewVehicle && quoteQuery.IDVValue == 0) ? 1 : quoteQuery.SelectedIDV,
					IDV = Convert.ToDecimal(Math.Round(recIdv)),
					MinIDV = Convert.ToDecimal(Math.Round(minIdv)),
					MaxIDV = Convert.ToDecimal(Math.Round(maxIdv)),
					NCB = quoteResponseObject.MotorPolicy.CurrentYearNCB.Split(".")[0],
					Tax = new ServiceTax
					{
						totalTax = taxAmount
					},
					BasicCover = new BasicCover
					{
						CoverList = setBaseCover
					},
					PACovers = new PACovers
					{
						PACoverList = paCover
					},
					AddonCover = new AddonCover
					{
						AddonList = addOnCover
					},
					AccessoriesCover = new AccessoriesCover
					{
						AccessoryList = accessoriesCover
					},
					Discount = new Domain.GoDigit.Discount
					{
						DiscountList = discountCover
					},
					RTOCode = quoteQuery.VehicleDetails.LicensePlateNumber,
					TransactionID = quoteResponseObject.MotorPolicy.TraceID,
					PolicyStartDate = DateTime.ParseExact(quoteQuery.PolicyStartDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MMM-yyyy", CultureInfo.InvariantCulture),
					Tenure = (quoteQuery.VehicleODTenure).ToString() + " OD " + "+ " + (quoteQuery.VehicleTPTenure).ToString() + " TP",
					PlanType = (quoteQuery.VehicleODTenure).ToString() + "OD " + "_" + (quoteQuery.VehicleTPTenure).ToString() + "TP",
					IsSAODDateMandatory = quoteQuery.IsSAODMandatry,
					IsSATPDateMandatory = quoteQuery.IsSATPMandatory,
					RegistrationDate = DateTime.ParseExact(quoteQuery.RegistrationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
					ManufacturingDate = DateTime.ParseExact(quoteQuery.RegistrationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
					VehicleNumber = string.IsNullOrEmpty(quoteQuery.VehicleNumber) || quoteQuery.VehicleNumber == "" ? quoteQuery.VehicleDetails.LicensePlateNumber : quoteQuery.VehicleNumber
				};

			}
			else
			{
				quoteVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
				_logger.LogError("Reliance Quote Response {responseBody}", responseBody);
			}
		}
		quoteResponseVm = new QuoteResponseModelGeneric
		{
			RequestBody = requestBody,
			ResponseBody = responseBody,
			QuoteResponseModel = quoteVm
		};
		return quoteResponseVm;
	}
	private async Task<HttpResponseMessage> GetQuoteResponse(string leadId, string stage, string requestBody, CancellationToken cancellationToken)
	{
		try
		{
			var id = 0;
			id = await InsertICLogs(requestBody, leadId, _relianceConfig.BaseURL + _relianceConfig.QuoteURL, string.Empty, string.Empty, stage);
			_client.DefaultRequestHeaders.Clear();
			var quoteResponse = await _client.PostAsync(_relianceConfig.QuoteURL, new StringContent(requestBody, Encoding.UTF8, "application/xml"), cancellationToken);
			var response = await quoteResponse.Content.ReadAsStringAsync(cancellationToken);
			UpdateICLogs(id, null, response);
			return quoteResponse;
		}
		catch (Exception ex)
		{
			_logger.LogError("Reliance Quote RequestBody Exception {exception}", ex.Message);
			return default;
		}
	}
	private List<NameValueModel> SetBaseCover(string currentPolicyType, RelianceQuoteResponseDto result)
	{
		List<NameValueModel> baseCoverList = new List<NameValueModel>();
		if (currentPolicyType.Equals(_policyTypeConfig.PackageComprehensive) || currentPolicyType.Equals(_policyTypeConfig.ComprehensiveBundle))
		{
			baseCoverList = new List<NameValueModel>
			{
				new NameValueModel
				{
					Name = "Basic Own Damage Premium",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x=>x.CoverID.Equals("21")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x=>x.CoverID.Equals("21")).Select(x => x.Premium).FirstOrDefault()),
				},
				new NameValueModel
				{
					Name="Third Party Cover Premium",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x=>x.CoverID.Equals("22")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x=>x.CoverID.Equals("22")).Select(x => x.Premium).FirstOrDefault()),
				}
			};
		}
		else if (currentPolicyType.Equals(_policyTypeConfig.SAOD))
		{
			baseCoverList = new List<NameValueModel>
			{
				new NameValueModel
				{
					Name = "Basic Own Damage Premium",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x=>x.CoverID.Equals("21")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x=>x.CoverID.Equals("21")).Select(x => x.Premium).FirstOrDefault()),
				}
			};
		}
		else if (currentPolicyType.Equals(_policyTypeConfig.SATP))
		{
			baseCoverList = new List<NameValueModel>
			{
				new NameValueModel
				{
					Name="Third Party Cover Premium",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x=>x.CoverID.Equals("22")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x=>x.CoverID.Equals("22")).Select(x => x.Premium).FirstOrDefault()),
				}
			};
		}
		return baseCoverList;
	}
	private List<NameValueModel> SetPACover(QuoteQueryModel quoteQuery, RelianceQuoteResponseDto result)
	{
		var paCover = new List<NameValueModel>();
		if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.FourWheeler))
		{
			if (quoteQuery.PACover.IsPaidDriver)
			{
				paCover.Add(new NameValueModel
				{
					Id = quoteQuery.PACover.PaidDriverId,
					Name = "PA Cover for Paid Driver",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("13")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("13")).Select(x => x.Premium).FirstOrDefault()),
				}
				);
			}
			if (quoteQuery.PACover.IsUnnamedPassenger)
			{
				paCover.Add(new NameValueModel
				{
					Id = quoteQuery.PACover.UnnamedPassengerId,
					Name = "PA Cover for Unnamed Passengers",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("16")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("16")).Select(x => x.Premium).FirstOrDefault()),
				}
				);
			}
			if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
			{
				paCover.Add(new NameValueModel
				{
					Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
					Name = "PA Cover for Owner Driver",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("24")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("24")).Select(x => x.Premium).FirstOrDefault()),
				});
			}
		}
		else if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.TwoWheeler))
		{
			if (quoteQuery.PACover.IsPaidDriver)
			{
				paCover.Add(new NameValueModel
				{
					Id = quoteQuery.PACover.PaidDriverId,
					Name = "PA Cover for Paid Driver",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("13")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("13")).Select(x => x.Premium).FirstOrDefault()),
				}
				);
			}
			if (quoteQuery.PACover.IsUnnamedPillionRider)
			{
				paCover.Add(new NameValueModel
				{
					Id = quoteQuery.PACover.UnnamedPillionRiderId,
					Name = "PA Cover For Unnamed Pillion Rider",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("16")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("16")).Select(x => x.Premium).FirstOrDefault()),
				}
				);
			}
			if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
			{
				paCover.Add(new NameValueModel
				{
					Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
					Name = "PA Cover for Owner Driver",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("24")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("24")).Select(x => x.Premium).FirstOrDefault()),
				});
			}
		}
		else if (quoteQuery.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
		{
			if (quoteQuery.PACover.IsPaidDriver)
			{
				paCover.Add(new NameValueModel
				{
					Id = quoteQuery.PACover.PaidDriverId,
					Name = "PA Cover for Paid Driver",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("13")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("13")).Select(x => x.Premium).FirstOrDefault()),
				}
				);
			}
			if (quoteQuery.PACover.IsUnnamedConductor)
			{
				paCover.Add(new NameValueModel
				{
					Id = quoteQuery.PACover.UnnamedConductorId,
					Name = "PA to Conductor",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("131")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("131")).Select(x => x.Premium).FirstOrDefault()),
				}
				);
			}
			if (quoteQuery.PACover.IsUnnamedHirer)
			{
				paCover.Add(new NameValueModel
				{
					Id = quoteQuery.PACover.UnnamedHirerId,
					Name = "Hired Vehicles Driven By Hirer",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("416")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("416")).Select(x => x.Premium).FirstOrDefault()),
				}
				);
			}
			if (quoteQuery.PACover.IsUnnamedOWNERDRIVER)
			{
				paCover.Add(new NameValueModel
				{
					Id = quoteQuery.PACover.UnnamedOWNERDRIVERId,
					Name = "PA Cover for Owner Driver",
					Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("24")).Select(x => x.Premium).FirstOrDefault()),
					IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("24")).Select(x => x.Premium).FirstOrDefault()),
				});
			}
		}
		return paCover;
	}
	private static List<NameValueModel> SetDiscountCover(QuoteQueryModel quoteQuery, RelianceQuoteResponseDto result)
	{
		var discountCover = new List<NameValueModel>();
		if (quoteQuery.Discounts.IsLimitedTPCoverage)
		{
			discountCover.Add(new NameValueModel
			{
				Id = quoteQuery.Discounts.LimitedTPCoverageId,
				Name = "Limited Third Party Coverage",
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("20")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("20")).Select(x => x.Premium).FirstOrDefault()),
			});
		}
		if (quoteQuery.Discounts.IsAAMemberShip)
		{
			discountCover.Add(new NameValueModel
			{
				Id = quoteQuery.Discounts.AAMemberShipId,
				Name = "AA Membership",
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("19")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("19")).Select(x => x.Premium).FirstOrDefault()),
			});
		}
		if (quoteQuery.Discounts.IsVoluntarilyDeductible)
		{
			discountCover.Add(new NameValueModel
			{
				Id = quoteQuery.Discounts.VoluntarilyDeductibleId,
				Name = "Voluntary Deductible",
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("3")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("3")).Select(x => x.Premium).FirstOrDefault()),
			});
		}
		if (quoteQuery.Discounts.IsAntiTheft)
		{
			discountCover.Add(new NameValueModel
			{
				Id = quoteQuery.Discounts.AntiTheftId,
				Name = "ARAI Approved Anti-Theft Device",
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("11")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("11")).Select(x => x.Premium).FirstOrDefault()),
			});
		}
		if (result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("23")).Any())
		{
			discountCover.Add(new NameValueModel
			{
				Name = "No Claim Bonus",
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("23")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("23")).Select(x => x.Premium).FirstOrDefault()),
			});
		}

		return discountCover;
	}
	private static List<NameValueModel> SetAccessoriesCover(QuoteQueryModel quoteQuery, RelianceQuoteResponseDto result)
	{
		var accessoriesCover = new List<NameValueModel>();
		if (quoteQuery.Accessories.IsCNG)
		{
			accessoriesCover.Add(new NameValueModel
			{
				Id = quoteQuery.Accessories.CNGId,
				Name = "CNG/LPG Accessory Cover OD",
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("4")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("4")).Select(x => x.Premium).FirstOrDefault()),
			}
			);
			accessoriesCover.Add(new NameValueModel
			{
				Id = quoteQuery.Accessories.CNGId,
				Name = "CNG/LPG Accessory Cover TP",
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("18")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("18")).Select(x => x.Premium).FirstOrDefault()),
			}
			);
		}
		else if (!quoteQuery.Accessories.IsCNG && (quoteQuery.VehicleDetails.FuelId.Equals("3") || quoteQuery.VehicleDetails.Fuel.Equals("4")))
		{
			accessoriesCover.Add(new NameValueModel
			{
				Id = quoteQuery.Accessories.CNGId,
				Name = "CNG/LPG Inbuilt Accessory Cover OD",
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("404")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("404")).Select(x => x.Premium).FirstOrDefault()),
			}
			);
			accessoriesCover.Add(new NameValueModel
			{
				Id = quoteQuery.Accessories.CNGId,
				Name = "CNG/LPG Inbuilt Accessory Cover TP",
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("18")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("18")).Select(x => x.Premium).FirstOrDefault()),
			}
			);
		}
		if (quoteQuery.Accessories.IsElectrical)
		{
			accessoriesCover.Add(new NameValueModel
			{
				Id = quoteQuery.Accessories.ElectricalId,
				Name = "Electrical Accessory Cover",
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("1")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("1")).Select(x => x.Premium).FirstOrDefault()),
			});
		}
		if (quoteQuery.Accessories.IsNonElectrical)
		{
			accessoriesCover.Add(new NameValueModel
			{
				Id = quoteQuery.Accessories.NonElectricalId,
				Name = "Non-Electrical Accessory Cover",
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("2")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("2")).Select(x => x.Premium).FirstOrDefault()),
			});
		}
		return accessoriesCover;
	}
	private static List<NameValueModel> SetAddonsCover(QuoteQueryModel quoteQuery, ReliancePackageAddonResponseModel packageResponse, RelianceQuoteResponseDto result)
	{
		var addOnCover = new List<NameValueModel>();

		if (quoteQuery.AddOns.IsGeoAreaExtension)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "Geo Area Extension OD",
				Id = quoteQuery.AddOns.GeoAreaExtensionId,
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("5")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("5")).Select(x => x.Premium).FirstOrDefault()),
			});
			addOnCover.Add(new NameValueModel
			{
				Name = "Geo Area Extension TP",
				Id = quoteQuery.AddOns.GeoAreaExtensionId,
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("6")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = IsApplicableEntity(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("6")).Select(x => x.Premium).FirstOrDefault()),
			});
		}
		//Setting Addon based on package
		if (packageResponse.IsSecurePremium && Convert.ToDouble(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID == "375").Select(x => x.Premium).FirstOrDefault()) > 0)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "Zero Dep",
				Id = quoteQuery.AddOns.ZeroDebtId,
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("375")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = true
			});
			addOnCover.Add(new NameValueModel
			{
				Id = quoteQuery.AddOns.PersonalBelongingId,
				Name = "Personal Belongings",
				Value = "Included",
				IsApplicable = true
			});
			addOnCover.Add(new NameValueModel
			{
				Name = "Consumables",
				Id = quoteQuery.AddOns.ConsumableId,
				Value = "Included",
				IsApplicable = true
			});
			addOnCover.Add(new NameValueModel
			{
				Id = quoteQuery.AddOns.KeyAndLockProtectionId,
				Name = "Key And Lock Protection",
				Value = "Included",
				IsApplicable = true
			});
			addOnCover.Add(new NameValueModel
			{
				Name = "Engine Gearbox Protection",
				Id = quoteQuery.AddOns.EngineProtectionId,
				Value = "Included",
				IsApplicable = true
			});
			addOnCover.Add(new NameValueModel
			{
				Name = "Tyre Protection",
				Id = quoteQuery.AddOns.TyreProtectionId,
				Value = "Included",
				IsApplicable = true
			});
		}
		else if (packageResponse.IsSecurePlus && Convert.ToDouble(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID == "374").Select(x => x.Premium).FirstOrDefault()) > 0)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "Zero Dep",
				Id = quoteQuery.AddOns.ZeroDebtId,
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("374")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = true
			});
			addOnCover.Add(new NameValueModel
			{
				Id = quoteQuery.AddOns.PersonalBelongingId,
				Name = "Personal Belongings",
				Value = "Included",
				IsApplicable = true
			});
			addOnCover.Add(new NameValueModel
			{
				Name = "Consumables",
				Id = quoteQuery.AddOns.ConsumableId,
				Value = "Included",
				IsApplicable = true
			});
			addOnCover.Add(new NameValueModel
			{
				Id = quoteQuery.AddOns.KeyAndLockProtectionId,
				Name = "Key And Lock Protection",
				Value = "Included",
				IsApplicable = true
			});
			addOnCover.Add(new NameValueModel
			{
				Name = "Engine Gearbox Protection",
				Id = quoteQuery.AddOns.EngineProtectionId,
				Value = "Included",
				IsApplicable = true
			});
			if (quoteQuery.AddOns.IsTyreProtectionRequired)
			{
				addOnCover.Add(new NameValueModel
				{
					Name = "Tyre Protection",
					Id = quoteQuery.AddOns.TyreProtectionId,
					IsApplicable = false
				});
			}
		}
		else if (packageResponse.IsZeroDept && Convert.ToDouble(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID == "10").Select(x => x.Premium).FirstOrDefault()) > 0)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "Zero Dep",
				Id = quoteQuery.AddOns.ZeroDebtId,
				Value = RoundOffValue(result.MotorPolicy.lstPricingResponse.Where(x => x.CoverID.Equals("10")).Select(x => x.Premium).FirstOrDefault()),
				IsApplicable = true
			});
			if (quoteQuery.AddOns.IsPersonalBelongingRequired)
			{
				addOnCover.Add(new NameValueModel
				{
					Id = quoteQuery.AddOns.PersonalBelongingId,
					Name = "Personal Belongings",
					IsApplicable = false
				});
			}
			if (quoteQuery.AddOns.IsConsumableRequired)
			{
				addOnCover.Add(new NameValueModel
				{
					Name = "Consumables",
					Id = quoteQuery.AddOns.ConsumableId,
					IsApplicable = false
				});
			}
			if (quoteQuery.AddOns.IsKeyAndLockProtectionRequired)
			{
				addOnCover.Add(new NameValueModel
				{
					Id = quoteQuery.AddOns.KeyAndLockProtectionId,
					Name = "Key And Lock Protection",
					IsApplicable = false
				});
			}
			if (quoteQuery.AddOns.IsEngineProtectionRequired)
			{
				addOnCover.Add(new NameValueModel
				{
					Name = "Engine Gearbox Protection",
					Id = quoteQuery.AddOns.EngineProtectionId,
					IsApplicable = false
				});
			}
			if (quoteQuery.AddOns.IsTyreProtectionRequired)
			{
				addOnCover.Add(new NameValueModel
				{
					Name = "Tyre Protection",
					Id = quoteQuery.AddOns.TyreProtectionId,
					IsApplicable = false
				});
			}
		}

		if (quoteQuery.AddOns.IsReturnToInvoiceRequired)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "RTI",
				Id = quoteQuery.AddOns.ReturnToInvoiceIdId,
				IsApplicable = false
			});
		}
		if (quoteQuery.AddOns.IsDailyAllowance)
		{
			addOnCover.Add(new NameValueModel
			{
				Id = quoteQuery.AddOns.DailyAllowanceId,
				Name = "Daily Allowance",
				IsApplicable = false
			});
		}
		if (quoteQuery.AddOns.IsEMIProtectorRequired)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "EMI Protection",
				Id = quoteQuery.AddOns.EMIProtectorId,
				IsApplicable = false
			});
		}
		if (quoteQuery.AddOns.IsLimitedOwnPremisesRequired)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "Limited to Own Premises",
				Id = quoteQuery.AddOns.LimitedOwnPremisesId,
				IsApplicable = false
			});
		}
		if (quoteQuery.AddOns.IsLossOfDownTimeRequired)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "Loss of Down Time Protection",
				Id = quoteQuery.AddOns.LossOfDownTimeId,
				IsApplicable = false
			});
		}
		if (quoteQuery.AddOns.IsNCBRequired)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "No Claim Bonus Protection",
				Id = quoteQuery.AddOns.NCBId,
				IsApplicable = false
			});
		}
		if (quoteQuery.AddOns.IsRimProtectionRequired)
		{
			addOnCover.Add(new NameValueModel
			{
				Id = quoteQuery.AddOns.RimProtectionId,
				Name = "RIM Protection",
				IsApplicable = false
			});
		}
		if (quoteQuery.AddOns.IsTowingRequired)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "Towing Protection",
				Id = quoteQuery.AddOns.TowingId,
				IsApplicable = false
			});
		}
		if (quoteQuery.AddOns.IsRoadSideAssistanceRequired)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "Road Side Assitance",
				Id = quoteQuery.AddOns.RoadSideAssistanceId,
				IsApplicable = false
			});
		}
		if (quoteQuery.AddOns.IsRoadSideAssistanceWiderRequired)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "Road Side Assitance Wider",
				Id = quoteQuery.AddOns.RoadSideAssistanceWiderId,
				IsApplicable = false
			});
		}
		if (quoteQuery.AddOns.IsRoadSideAssistanceAdvanceRequired)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "Road Side Assitance Advance",
				Id = quoteQuery.AddOns.RoadSideAssistanceAdvanceId,
				IsApplicable = false
			});
		}
		if (quoteQuery.AddOns.IsIMT23)
		{
			addOnCover.Add(new NameValueModel
			{
				Name = "IMT 23(Lamp/ tyre tube/ Headlight etc )",
				Id = quoteQuery.AddOns.IMT23Id,
				IsApplicable = false
			});
		}

		return addOnCover;
	}
	private static string RemoveXmlDefinition(string xml)
	{
		XDocument xdoc = XDocument.Parse(xml);
		xdoc.Declaration = null;

		return xdoc.ToString();
	}
	private static bool IsApplicableEntity(object _val)
	{
		string val = Convert.ToString(_val);
		return !(string.IsNullOrEmpty(val) || val == "0" || Convert.ToDecimal(val) == 0);
	}
	private static string RoundOffValue(string _val)
	{
		decimal val = Math.Round(Convert.ToDecimal(_val));
		if (val < 0)
			val = -val;
		return val.ToString();
	}
	private static IEnumerable<string> VehicleNumberSplit(string input)
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
	private static int NestedTernaryOperatorMethod(bool value1, bool value2)
	{
		if (value1)
		{
			if (value2)
				return 3;
			return 5;
		}
		else
			return 1;
	}
	private void UpdateICLogs(int id, string applicationId, string data)
	{
		var logsModel = new LogsModel
		{
			Id = id,
			ResponseBody = data,
			ResponseTime = DateTime.Now,
			ApplicationId = applicationId
		};
		_commonService.UpdateLogs(logsModel);
	}
	private async Task<int> InsertICLogs(string requestBody, string leadId, string api, string token, string header, string stage)
	{
		var logsModel = new LogsModel
		{
			InsurerId = _relianceConfig.InsurerId,
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
	private static ReliancePackageAddonResponseModel ReliancePackageLogic(QuoteQueryModel quoteQueryModel, LstAddonCovers lstAddonCovers)
	{
		ReliancePackageAddonResponseModel response = new();

		if (quoteQueryModel.AddOns.IsTyreProtectionRequired && lstAddonCovers.AddonCovers.Exists(x => x.CoverageID == "375"))
		{
			response.IsSecurePremium = true;
		}
		else if (lstAddonCovers.AddonCovers.Exists(x => x.CoverageID == "374") &&
			(quoteQueryModel.AddOns.IsPersonalBelongingRequired || quoteQueryModel.AddOns.IsConsumableRequired ||
			quoteQueryModel.AddOns.IsKeyAndLockProtectionRequired || quoteQueryModel.AddOns.IsEngineProtectionRequired))
		{
			response.IsSecurePlus = true;
		}
		else if (quoteQueryModel.AddOns.IsZeroDebt && lstAddonCovers.AddonCovers.Exists(x => x.CoverageID == "10"))
		{
			response.IsZeroDept = true;
		}
		return response;
	}
	public async Task<QuoteConfirmResponseModel> QuoteConfirmDetails(QuoteTransactionDbModel quoteTransactionDbModel, QuoteConfirmRequestModel quoteConfirmCommand, CancellationToken cancellationToken)
	{
		QuoteConfirmResponseModel quoteResponseVM = new QuoteConfirmResponseModel();
		bool isSelfInspection = false;
		bool isPolicyTypeSelfInspection = false;
		bool isBreakIn = false;
		bool isZeroDept = false;
		bool isEngineCover = false;
		var vehicleNumber = string.Join("-", VehicleNumberSplit(quoteConfirmCommand.VehicleNumber));
		var prevPolicyEndDate = string.Empty;

		if (!quoteConfirmCommand.IsBrandNewVehicle)
		{
			if (_policyTypeConfig.SATP.Equals(quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId))
			{
				_ = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.TPPolicyStartDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
				prevPolicyEndDate = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.TPPolicyEndDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
			}
			else
			{
				_ = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.ODPolicyStartDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
				prevPolicyEndDate = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.ODPolicyEndDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
			}
		}

		XmlDocument doc = new XmlDocument();
		doc.LoadXml(quoteTransactionDbModel.QuoteTransactionRequest.RequestBody);
		string result = JsonConvert.SerializeXmlNode(doc);
		RelianceRequestDto requestBody = JsonConvert.DeserializeObject<RelianceRequestDto>(result);
		//Response XML Deserialize END

		string policyStartDate = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.PolicyStartDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

		string policyEndDate = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.PolicyEndDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

		string mnufacturingYear = string.Empty;

		if (quoteConfirmCommand.PolicyDates.ManufacturingDate != null && quoteConfirmCommand.PolicyDates.ManufacturingDate.Length > 4)
		{
			mnufacturingYear = quoteConfirmCommand.PolicyDates.ManufacturingDate.Substring(quoteConfirmCommand.PolicyDates.ManufacturingDate.Length - 4);
		}
		else if (quoteConfirmCommand.PolicyDates.ManufacturingDate != null && quoteConfirmCommand.PolicyDates.ManufacturingDate.Length == 4)
		{
			mnufacturingYear = quoteConfirmCommand.PolicyDates.ManufacturingDate;
		}
		requestBody.PolicyDetails.Policy.Cover_From = policyStartDate;
		requestBody.PolicyDetails.Policy.Cover_To = policyEndDate;
		requestBody.PolicyDetails.Vehicle.Registration_date = quoteTransactionDbModel.LeadDetail.IsBrandNew ? DateTime.Today.AddDays(-2).ToString("dd/MM/yyyy") :
		DateTime.ParseExact(quoteConfirmCommand.PolicyDates.RegistrationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
		requestBody.PolicyDetails.Risk.DateOfPurchase = quoteTransactionDbModel.LeadDetail.IsBrandNew ? DateTime.Today.AddDays(-2).ToString("dd/MM/yyyy") :
			DateTime.ParseExact(quoteConfirmCommand.PolicyDates.RegistrationDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
		requestBody.PolicyDetails.Cover.TrailerDetails.TrailerInfo.ManufactureYear = mnufacturingYear;

		requestBody.PolicyDetails.Risk.EngineNo = quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine != null ? quoteTransactionDbModel.QuoteConfirmDetailsModel.Engine : requestBody.PolicyDetails.Risk.EngineNo;

		requestBody.PolicyDetails.Risk.Chassis = quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis != null ? quoteTransactionDbModel.QuoteConfirmDetailsModel.Chassis : requestBody.PolicyDetails.Risk.Chassis;

		requestBody.PolicyDetails.Cover.TrailerDetails.TrailerInfo.Registration_No = quoteConfirmCommand.IsBrandNewVehicle ? "NEW" : vehicleNumber;

		if (!quoteConfirmCommand.IsPACover)
		{
			requestBody.PolicyDetails.Cover.IsPAToOwnerDriverCoverd = true;
			requestBody.PolicyDetails.Cover.PACoverToOwner.IsChecked = true;
			requestBody.PolicyDetails.Cover.PACoverToOwner.CPAcovertenure = Convert.ToInt32(quoteConfirmCommand.PACoverTenure);
		}
		else
		{
			requestBody.PolicyDetails.Cover.IsPAToOwnerDriverCoverd = false;
			requestBody.PolicyDetails.Cover.PACoverToOwner.IsChecked = false;
			requestBody.PolicyDetails.Cover.PACoverToOwner.CPAcovertenure = 0;
		}

		if (!quoteConfirmCommand.IsBrandNewVehicle && quoteConfirmCommand.PreviousPolicy.IsPreviousPolicy)
		{
			if (quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive))
			{
				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyEndDate = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.ODPolicyEndDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyStartDate = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.ODPolicyStartDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyNo = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
				requestBody.PolicyDetails.PreviousInsuranceDetails.IsClaimedLastYear = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? "true" : "false";
				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearNCB = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? 0 : Convert.ToInt32(quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue);
			}
			else if (quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD))
			{
				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyEndDate = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.ODPolicyEndDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyStartDate = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.ODPolicyStartDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyNo = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber;
				requestBody.PolicyDetails.PreviousInsuranceDetails.IsClaimedLastYear = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? "true" : "false";
				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearNCB = quoteConfirmCommand.PreviousPolicy.IsPreviousYearClaim ? 0 : Convert.ToInt32(quoteTransactionDbModel.QuoteConfirmDetailsModel.NCBValue);
			}
			else
			{
				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyEndDate = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.TPPolicyEndDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyStartDate = DateTime.ParseExact(quoteConfirmCommand.PolicyDates.TPPolicyStartDate, "dd-MMM-yyyy", CultureInfo.InvariantCulture).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);

				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyNo = quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP;
				requestBody.PolicyDetails.PreviousInsuranceDetails.IsClaimedLastYear = "false";
				requestBody.PolicyDetails.PreviousInsuranceDetails.PrevYearNCB = 0;
			}
		}

		if (!quoteConfirmCommand.IsBrandNewVehicle && DateTime.ParseExact(prevPolicyEndDate, "dd-MM-yyyy", CultureInfo.InvariantCulture) < DateTime.Now)
			isBreakIn = true;
		if ((requestBody.PolicyDetails.Cover.IsNilDepreciation || requestBody.PolicyDetails.Cover.IsSecurePremium || requestBody.PolicyDetails.Cover.IsSecurePlus) && !quoteConfirmCommand.isPrevPolicyNilDeptCover)
			isZeroDept = true;
		if ((requestBody.PolicyDetails.Cover.IsSecurePremium || requestBody.PolicyDetails.Cover.IsSecurePlus) && !quoteConfirmCommand.isPrevPolicyEngineCover)
			isEngineCover = true;

		var requestBodyFraming = JsonConvert.SerializeObject(requestBody);
		_logger.LogInformation("Reliance Quote Confirm Response {request}", requestBodyFraming);

		XmlDocument xmlRequest = JsonConvert.DeserializeXmlNode(requestBodyFraming);
		var xmlRequestFraming = RemoveXmlDefinition(xmlRequest.OuterXml.ToString());

		HttpResponseMessage confirmQuote = await GetQuoteResponse(quoteTransactionDbModel?.LeadDetail?.LeadID, "QuoteConfirm", xmlRequestFraming.ToString(), cancellationToken);
		QuoteConfirmDetailsResponseModel quoteConfirm = new QuoteConfirmDetailsResponseModel();

		string responseBody = string.Empty;
		string commonResponse = quoteTransactionDbModel.QuoteTransactionRequest.CommonResponse;
		QuoteResponseModel updatedResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(commonResponse);
		var leadId = quoteTransactionDbModel.LeadDetail.LeadID;
		RelianceQuoteResponseDto quoteResponseObject = new RelianceQuoteResponseDto();
		RelianceTPQuoteResponseDto quoteTPResponseObject = new RelianceTPQuoteResponseDto();

		if (!quoteConfirmCommand.IsBrandNewVehicle)
		{
			if (!string.IsNullOrEmpty(quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId))
			{
				if (quoteTransactionDbModel.LeadDetail.PrevPolicyTypeId.Equals(_policyTypeConfig.SATP) && quoteConfirmCommand.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive))
				{
					isPolicyTypeSelfInspection = true;
				}
			}
			if (isBreakIn || isZeroDept || isEngineCover)
			{
				isSelfInspection = true;
			}
		}

		if (isPolicyTypeSelfInspection || isSelfInspection)
		{
			quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.OK;
			quoteConfirm.ValidationMessage = BreakInMessage;
			quoteConfirm.IsBreakin = true;
			quoteConfirm.IsSelfInspection = true;
			quoteConfirm.isNavigateToQuote = true;
		}
		else
		{
			if (confirmQuote.IsSuccessStatusCode)
			{
				var response = await confirmQuote.Content.ReadAsStringAsync(cancellationToken);

				_logger.LogInformation("Reliance Quote Confirm Response {responseBody}", response);
				XmlDocument docXML = new XmlDocument();
				docXML.LoadXml(response);
				string resulttoJson = JsonConvert.SerializeXmlNode(docXML);
				responseBody = resulttoJson;
				quoteResponseObject = JsonConvert.DeserializeObject<RelianceQuoteResponseDto>(resulttoJson);
				var lstTaxComponent = new List<TaxComponent>();
				var isBrancket = quoteResponseObject.MotorPolicy.LstTaxComponentDetails.TaxComponent.ToString().Contains("[");
				JToken token;
				//JToken token1;
				var taxComp = quoteResponseObject.MotorPolicy?.LstTaxComponentDetails?.TaxComponent;
				var taxAmount = string.Empty;
				if (isBrancket)
				{
					if (taxComp != null)
					{
						foreach (var taxItem in taxComp)
						{
							var tokenn = JObject.Parse(taxItem?.ToString());
							if (tokenn != null)
							{
								lstTaxComponent.Add(JsonConvert.DeserializeObject<TaxComponent>(tokenn.ToString()));
							}
						}

						var totalTax = lstTaxComponent.Sum(tax => Convert.ToDouble(tax.Amount));
						taxAmount = Math.Round(totalTax).ToString();
					}
				}
				else
				{
					token = JObject.Parse(quoteResponseObject.MotorPolicy.LstTaxComponentDetails?.TaxComponent?.ToString());
					var taxComponent = JsonConvert.DeserializeObject<TaxComponent>(token.ToString());
					lstTaxComponent.Add(taxComponent);
					var totalTax = Math.Round(Convert.ToDouble(taxComponent.Amount));
					taxAmount = Convert.ToString(totalTax);
				}
				_logger.LogInformation("Reliance Quote Confirm Response {responseBody}", responseBody);

				if ((quoteResponseObject != null || !string.IsNullOrEmpty(quoteResponseObject.MotorPolicy.NetPremium)))
				{
					quoteConfirm = new QuoteConfirmDetailsResponseModel
					{
						InsurerStatusCode = (int)HttpStatusCode.OK,
						InsurerName = "Reliance",
						NewPremium = quoteResponseObject.MotorPolicy.FinalPremium,
						InsurerId = _relianceConfig.InsurerId,
						IDV = Convert.ToInt32(Math.Round(Convert.ToDecimal(quoteResponseObject.MotorPolicy.IDV))),
						NCB = quoteResponseObject.MotorPolicy.CurrentYearNCB.Split(".")[0],
						Tax = new ServiceTaxModel
						{
							totalTax = taxAmount
						},
						TotalPremium = Math.Round(Convert.ToDouble(quoteResponseObject.MotorPolicy.NetPremium)).ToString(),
						GrossPremium = Math.Round(Convert.ToDouble(quoteResponseObject.MotorPolicy.FinalPremium)).ToString(),
						TransactionId = quoteResponseObject.MotorPolicy.TraceID
					};
				}
				else
				{
					responseBody = JsonConvert.SerializeObject(response);
					_logger.LogInformation("Reliance Response {responseBody}", responseBody);
					quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.OK;
					quoteConfirm.ValidationMessage = BreakInMessage;
					quoteConfirm.IsBreakin = true;
					quoteConfirm.IsSelfInspection = true;
					quoteConfirm.isNavigateToQuote = true;
				}
				updatedResponse.GrossPremium = quoteConfirm.NewPremium;
			}
			else
			{
				var stream = await confirmQuote.Content.ReadAsStreamAsync(cancellationToken);
				if (!quoteConfirmCommand.IsBrandNewVehicle)
				{
					quoteTPResponseObject = stream.DeserializeFromJson<RelianceTPQuoteResponseDto>();
					responseBody = JsonConvert.SerializeObject(quoteTPResponseObject);
				}
				else
				{
					quoteResponseObject = stream.DeserializeFromJson<RelianceQuoteResponseDto>();
					responseBody = JsonConvert.SerializeObject(quoteResponseObject);
				}
				_logger.LogInformation("Reliance Quote Confirm Response {responseBody}", responseBody);
				quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
			}
		}
		quoteResponseVM = new QuoteConfirmResponseModel()
		{
			quoteConfirmResponse = quoteConfirm,
			quoteResponse = updatedResponse,
			RequestBody = requestBodyFraming,
			ResponseBody = responseBody,
			LeadId = leadId
		};
		return quoteResponseVM;
	}
	public async Task<Tuple<string, string, SaveCKYCResponse, CreateLeadModel>> GetCKYCResponse(RelianceCKYCCommand relianceCKYCCommand, CancellationToken cancellationToken)
	{
		string responseBody = string.Empty;
		string requestBody = string.Empty;
		SaveCKYCResponse saveCKYCResponse = new SaveCKYCResponse();
		CreateLeadModel createLeadModel = new CreateLeadModel();
		createLeadModel.PermanentAddress = new LeadAddressModel();
		var id = 0;
		try
		{
			RelianceCKYCRequestModel request = new RelianceCKYCRequestModel()
			{
				PAN = relianceCKYCCommand.DocumentId,
				DOB = relianceCKYCCommand.CustomerType.Equals("I", StringComparison.OrdinalIgnoreCase) ? Convert.ToDateTime(relianceCKYCCommand.DateOfBirth).ToString("dd-MM-yyyy") : Convert.ToDateTime(relianceCKYCCommand.DateOfInsertion).ToString("dd-MM-yyyy"),
				CKYC = string.Empty,
				MOBILE = relianceCKYCCommand.Mobile,
				PINCODE = string.Empty,
				BIRTHYEAR = string.Empty,
				ReturnURL = _relianceConfig.CKYCPOARedirectionURL + relianceCKYCCommand.QuoteTransactionId + "/" + _applicationClaims.GetUserId(),
				UNIQUEID = relianceCKYCCommand.TransactionId,
				CIN = relianceCKYCCommand.DocumentType.ToUpper() != "CIN" ? "" : relianceCKYCCommand.CIN,
				VOTERID = relianceCKYCCommand.VoterId,
				DL_No = relianceCKYCCommand.DrivingLicenceNumber,
				PASSPORT = relianceCKYCCommand.PassportNumber,
				AADHAAR_NO = relianceCKYCCommand.AadharNumber,
				FULLNAME = relianceCKYCCommand.FullName,
				GENDER = relianceCKYCCommand.Gender
			};

			switch (relianceCKYCCommand.DocumentType.ToUpper())
			{
				case ("AADHAAR"):
					request.AADHAAR_NO = relianceCKYCCommand.DocumentId;
					break;
				case ("PAN"):
					request.PAN = relianceCKYCCommand.DocumentId;
					break;
				case ("CKYC"):
					request.CKYC = relianceCKYCCommand.DocumentId;
					break;
				case ("DL"):
					request.DL_No = relianceCKYCCommand.DocumentId;
					break;
				case ("PASPORTNO"):
					request.PASSPORT = relianceCKYCCommand.DocumentId;
					break;
				case ("VOTERID"):
					request.VOTERID = relianceCKYCCommand.DocumentId;
					break;
				case ("CIN"):
					request.CIN = relianceCKYCCommand.CIN;
					break;
			}

			requestBody = JsonConvert.SerializeObject(request);
			_logger.LogInformation("GetCKYCResponse requestBody {requestBody}", requestBody);
			_client.DefaultRequestHeaders.Clear();
			_client.DefaultRequestHeaders.Add("Subscription-Key", _relianceConfig.SubscriptionKey);
			id = await InsertICLogs(requestBody, relianceCKYCCommand.LeadId, _relianceConfig.CKYCVerifyURL, null, JsonConvert.SerializeObject(_client.DefaultRequestHeaders), "KYC");
			try
			{
				var response = await _client.PostAsync(_relianceConfig.CKYCVerifyURL, new StringContent(requestBody, Encoding.UTF8, "application/json"),
				cancellationToken);

				if (!response.IsSuccessStatusCode)
				{
					responseBody = response.ReasonPhrase;
					_logger.LogError("GetCKYCResponse error {responseBody}", responseBody);
				}
				else
				{
					var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
					var result = stream.DeserializeFromJson<RelianceCKYCResponseModel>();
					responseBody = JsonConvert.SerializeObject(result);
					_logger.LogInformation("GetCKYCResponse responseBody {responseBody}", responseBody);
					saveCKYCResponse.IsKYCRequired = false;
					if (result != null && result.success && result.Endpoint_2_URL == null)
					{
						createLeadModel.LeadName = result.kyc_data.CKYC.result.PERSONAL_DETAILS.FULLNAME.Remove(0, 3);
						if (relianceCKYCCommand.CustomerType.Equals("I"))
						{

							createLeadModel.DOB = string.Join("-", result.kyc_data.CKYC.result.PERSONAL_DETAILS.DOB.Split("-").ToArray().Reverse());
						}
						else
						{
							createLeadModel.DateOfIncorporation = string.Join("-", result.kyc_data.CKYC.result.PERSONAL_DETAILS.DOB.Split("-").ToArray().Reverse());
						}
						createLeadModel.Gender = result.kyc_data.CKYC.result.PERSONAL_DETAILS.GENDER;
						createLeadModel.PhoneNumber = result.kyc_data.CKYC.result.PERSONAL_DETAILS.MOB_NUM;
						createLeadModel.PANNumber = result.kyc_data.CKYC.result.PERSONAL_DETAILS.PAN;
						createLeadModel.AadharNumber = string.Empty;
						createLeadModel.ckycNumber = result.kyc_data.CKYC.req_id;
						createLeadModel.DrivingLicenceNumber = string.Empty;
						createLeadModel.VoterId = string.Empty;
						createLeadModel.PassportNo = string.Empty;
						createLeadModel.CIN = string.Empty;
						createLeadModel.kyc_id = result.Unique_Id;
						createLeadModel.CKYCstatus = result.kyc_data.CKYC.success ? "Success" : "Failed";
						createLeadModel.PermanentAddress = new LeadAddressModel()
						{
							AddressType = "PRIMARY",
							Address1 = result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_LINE1,
							Address2 = result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_LINE2,
							Address3 = result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_CITY + " " + result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_DIST + " " + result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_STATE,
							Pincode = result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_PIN,
						};
						createLeadModel.CommunicationAddress = new LeadAddressModel()
						{
							AddressType = "SECONDARY",
							Address1 = result.kyc_data.CKYC.result.PERSONAL_DETAILS.CORRES_LINE1,
							Address2 = result.kyc_data.CKYC.result.PERSONAL_DETAILS.CORRES_LINE2,
							Address3 = result.kyc_data.CKYC.result.PERSONAL_DETAILS.CORRES_CITY + " " + result.kyc_data.CKYC.result.PERSONAL_DETAILS.CORRES_DIST + " " + result.kyc_data.CKYC.result.PERSONAL_DETAILS.CORRES_STATE,
							Pincode = result.kyc_data.CKYC.result.PERSONAL_DETAILS.CORRES_PIN,
						};
						string address = string.Empty;
						saveCKYCResponse.Name = result.kyc_data.CKYC.result.PERSONAL_DETAILS.FULLNAME.Remove(0, 3);
						saveCKYCResponse.Gender = result.kyc_data.CKYC.result.PERSONAL_DETAILS.GENDER == "M" ? "Male" : "Female";
						saveCKYCResponse.DOB = result.kyc_data.CKYC.result.PERSONAL_DETAILS.DOB != null || result.kyc_data.CKYC.result.PERSONAL_DETAILS.DOB != "" ? string.Join("-", result.kyc_data.CKYC.result.PERSONAL_DETAILS.DOB.Split("-").ToArray().Reverse()) : null;
						saveCKYCResponse.KYCId = result.Unique_Id;
						saveCKYCResponse.CKYCNumber = result.kyc_data.CKYC.req_id;
						saveCKYCResponse.IsKYCRequired = true;
						saveCKYCResponse.KYC_Status = KYC_SUCCESS;
						saveCKYCResponse.Message = KYC_SUCCESS;
						saveCKYCResponse.InsurerName = _relianceConfig.InsurerName;
						address = string.IsNullOrEmpty(result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_LINE1) ? "" : result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_LINE1 + ",";
						address += string.IsNullOrEmpty(result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_LINE2) ? "" : result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_LINE2 + ",";
						address += string.IsNullOrEmpty(result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_LINE3) ? "" : result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_LINE3 + ",";
						address += $"{result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_CITY},{result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_STATE},{result.kyc_data.CKYC.result.PERSONAL_DETAILS.PERM_PIN}";
						saveCKYCResponse.Address = address;
						UpdateICLogs(id, relianceCKYCCommand?.TransactionId, responseBody);
						return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
					}
					else if (result != null && !result.success && result.Endpoint_2_URL != null)
					{
						saveCKYCResponse.KYC_Status = POA_REQUIRED;
						saveCKYCResponse.Message = POA_REQUIRED;
						saveCKYCResponse.IsKYCRequired = true;
						saveCKYCResponse.redirect_link = result.Endpoint_2_URL;
						saveCKYCResponse.InsurerName = _relianceConfig.InsurerName;
						return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
					}
					saveCKYCResponse.KYC_Status = FAILED;
					saveCKYCResponse.Message = Convert.ToString(result.message);
					saveCKYCResponse.InsurerName = _relianceConfig.InsurerName;
				}
				UpdateICLogs(id, relianceCKYCCommand?.TransactionId, responseBody);
				return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
			}
			catch (Exception ex)
			{
				saveCKYCResponse.KYC_Status = FAILED;
				saveCKYCResponse.Message = MESSAGE;
				_logger.LogError("Reliance Ckyc Error {exception}", ex.Message);
				UpdateICLogs(id, relianceCKYCCommand?.TransactionId, ex.Message);
				return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
			}
		}
		catch (Exception ex)
		{
			saveCKYCResponse.KYC_Status = FAILED;
			saveCKYCResponse.Message = MESSAGE;
			_logger.LogError("Reliance Ckyc Error {exception}", ex.Message);
			return Tuple.Create(requestBody, responseBody, saveCKYCResponse, createLeadModel);
		}
	}
	public async Task<ProposalResponseModel> CreateProposal(RelianceRequestDto proposalQuery, RelianceProposalRequest proposalRequest, CreateLeadModel createLeadModel, CancellationToken cancellationToken)
	{
		ProposalResponseModel proposalResponseVM = new ProposalResponseModel();
		var proposalVm = new QuoteResponseModel();
		string requestBody = string.Empty;
		string responseBody = string.Empty;
		var id = 0;
		var namesList = !string.IsNullOrEmpty(proposalRequest.PersonalDetails.firstName) ? proposalRequest.PersonalDetails.firstName.TrimStart().Split(' ') : Array.Empty<string>();

		string fullAddress = proposalRequest.AddressDetails.addressLine1;
		string[] arr = fullAddress.Split(' ');
		string addr1 = "";
		for (int i = 0; i < arr.Length - 2; i++)
		{
			addr1 += arr[i] + " ";
		}
		try
		{
			var relianceRequest = new Domain.Reliance.PolicyDetails
			{
				UserID = proposalQuery.PolicyDetails.UserID,
				ProductCode = proposalQuery.PolicyDetails.ProductCode,
				SourceSystemID = proposalQuery.PolicyDetails.SourceSystemID,
				AuthToken = proposalQuery.PolicyDetails.AuthToken,
				ClientDetails = new Domain.Reliance.ClientDetails()
				{
					ClientType = !string.IsNullOrEmpty(proposalRequest.PersonalDetails.companyName) ? 1 : 0,
					LastName = namesList.Length > 1 ? (!string.IsNullOrWhiteSpace(namesList[0]) ? namesList[0] : namesList[1]) : string.Empty,
					MidName = namesList.Length > 2 ? (!string.IsNullOrWhiteSpace(namesList[1]) ? namesList[1] : namesList[2]) : string.Empty,
					ForeName = !string.IsNullOrEmpty(proposalRequest.PersonalDetails.salutation) ? proposalRequest.PersonalDetails.salutation : string.Empty,
					CorporateName = !string.IsNullOrEmpty(proposalRequest.PersonalDetails.companyName) ? proposalRequest.PersonalDetails.companyName : string.Empty,
					OccupationID = !string.IsNullOrWhiteSpace(proposalRequest.PersonalDetails.occupation) ? proposalRequest.PersonalDetails.occupation : string.Empty,
					DOB = proposalRequest.PersonalDetails.dateOfBirth,
					Gender = proposalRequest.PersonalDetails.gender,
					PhoneNo = proposalRequest.PersonalDetails.mobile,
					MobileNo = proposalRequest.PersonalDetails.mobile,
					GSTIN = "",
					EmailID = proposalRequest.PersonalDetails.emailId,
					Salutation = !string.IsNullOrEmpty(proposalRequest.PersonalDetails.salutation) ? proposalRequest.PersonalDetails.salutation : "M/S",
					MaritalStatus = Convert.ToInt32(_relianceConfig.MaritalStatus),
					Nationality = Convert.ToInt32(_relianceConfig.Nationality),
					ClientAddress = new ClientAddress()
					{
						CommunicationAddress = new CommunicationAddress()
						{
							AddressType = 0,
							Address1 = !string.IsNullOrEmpty(addr1) ? addr1 : proposalRequest.AddressDetails.addressLine1,
							Address2 = !string.IsNullOrEmpty(arr[arr.Length - 2]) ? arr[arr.Length - 2] : ".",
							Address3 = !string.IsNullOrEmpty(arr[arr.Length - 1]) ? arr[arr.Length - 1] : ".",
							CityID = Convert.ToInt32(proposalRequest.AddressDetails.city),
							DistrictID = Convert.ToInt32(proposalRequest.AddressDetails.communication_pincode),
							StateID = Convert.ToInt32(proposalRequest.AddressDetails.state),
							Pincode = Convert.ToInt32(proposalRequest.AddressDetails.pincode),
							Country = _relianceConfig.Country,
							NearestLandmark = string.Empty
						},
						PermanentAddress = new PermanentAddress()
						{
							AddressType = 0,
							Address1 = !string.IsNullOrEmpty(addr1) ? addr1 : proposalRequest.AddressDetails.addressLine1,
							Address2 = !string.IsNullOrEmpty(arr[arr.Length - 2]) ? arr[arr.Length - 2] : ".",
							Address3 = !string.IsNullOrEmpty(arr[arr.Length - 1]) ? arr[arr.Length - 1] : ".",
							CityID = Convert.ToInt32(proposalRequest.AddressDetails.city),
							DistrictID = Convert.ToInt32(proposalRequest.AddressDetails.communication_pincode),
							StateID = Convert.ToInt32(proposalRequest.AddressDetails.state),
							Pincode = Convert.ToInt32(proposalRequest.AddressDetails.pincode),
							Country = _relianceConfig.Country,
							NearestLandmark = string.Empty
						},
						RegistrationAddress = new RegistrationAddress()
						{
							AddressType = 0,
							Address1 = !string.IsNullOrEmpty(addr1) ? addr1 : proposalRequest.AddressDetails.addressLine1,
							Address2 = !string.IsNullOrEmpty(arr[arr.Length - 2]) ? arr[arr.Length - 2] : ".",
							Address3 = !string.IsNullOrEmpty(arr[arr.Length - 1]) ? arr[arr.Length - 1] : ".",
							CityID = Convert.ToInt32(proposalRequest.AddressDetails.city),
							DistrictID = Convert.ToInt32(proposalRequest.AddressDetails.communication_pincode),
							StateID = Convert.ToInt32(proposalRequest.AddressDetails.state),
							Pincode = Convert.ToInt32(proposalRequest.AddressDetails.pincode),
							Country = _relianceConfig.Country,
							NearestLandmark = string.Empty
						},
					},
				},
				Policy = new Domain.Reliance.Policy()
				{
					BusinessType = proposalQuery.PolicyDetails.Policy.BusinessType,
					POSType = string.Empty,
					POSAadhaarNumber = string.Empty,
					CoverFrom = proposalQuery.PolicyDetails.Policy.Cover_From,
					CoverTo = proposalQuery.PolicyDetails.Policy.Cover_To,
					BranchCode = proposalQuery.PolicyDetails.Policy.Branch_Code,
					AgentName = proposalQuery.PolicyDetails.Policy.AgentName,
					Productcode = proposalQuery.PolicyDetails.Policy.productcode,
					OtherSystemName = proposalQuery.PolicyDetails.Policy.OtherSystemName,
					IsMotorQuote = proposalQuery.PolicyDetails.Policy.isMotorQuote,
					IsMotorQuoteFlow = proposalQuery.PolicyDetails.Policy.isMotorQuoteFlow,
					TPPolicyNumber = !string.IsNullOrEmpty(createLeadModel.PrevPolicyNumber) ? createLeadModel.PrevPolicyNumber : string.Empty,
					TPPolicyStartDate = !string.IsNullOrEmpty(createLeadModel.PreviousSAODPolicyStartDate) ? createLeadModel.PreviousSAODPolicyStartDate : string.Empty,
					TPPolicyEndDate = !string.IsNullOrEmpty(createLeadModel.PrevPolicyExpiryDate) ? createLeadModel.PrevPolicyExpiryDate : string.Empty,
					TPPolicyInsurer = !string.IsNullOrEmpty(proposalQuery.PolicyDetails.PreviousInsuranceDetails.PrevInsuranceID) ? proposalQuery.PolicyDetails.PreviousInsuranceDetails.PrevInsuranceID : string.Empty
				},
				Risk = new Domain.Reliance.Risk()
				{
					VehicleMakeID = proposalQuery.PolicyDetails.Risk.VehicleMakeID,
					VehicleModelID = proposalQuery.PolicyDetails.Risk.VehicleModelID,
					CubicCapacity = proposalQuery.PolicyDetails.Risk.CubicCapacity,
					Zone = proposalQuery.PolicyDetails.Risk.Zone,
					RTOLocationID = proposalQuery.PolicyDetails.Risk.RTOLocationID,
					ExShowroomPrice = proposalQuery.PolicyDetails.Risk.ExShowroomPrice,
					IDV = proposalQuery.PolicyDetails.Risk.IDV,
					DateOfPurchase = proposalQuery.PolicyDetails.Risk.DateOfPurchase,
					ManufactureMonth = proposalQuery.PolicyDetails.Risk.ManufactureMonth,
					ManufactureYear = proposalQuery.PolicyDetails.Risk.ManufactureYear,
					EngineNo = proposalRequest.VehicleDetails.engineNumber,
					Chassis = proposalRequest.VehicleDetails.chassisNumber,
					IsVehicleHypothicated = proposalRequest.VehicleDetails.isFinancier.Equals("Yes"),
					FinanceType = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? "1" : string.Empty,
					FinancierName = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails.financer : proposalQuery.PolicyDetails.Risk.FinancierName,
					FinancierAddress = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails.branch : proposalQuery.PolicyDetails.Risk.FinancierAddress,
					FinancierCity = proposalRequest.VehicleDetails.isFinancier.Equals("Yes") ? proposalRequest.VehicleDetails.branch : proposalQuery.PolicyDetails.Risk.FinancierCity,
					IsRegAddressSameasCommAddress = proposalQuery.PolicyDetails.Risk.IsRegAddressSameasCommAddress,
					IsPermanentAddressSameasCommAddress = proposalQuery.PolicyDetails.Risk.IsPermanentAddressSameasCommAddress,
					IsRegAddressSameasPermanentAddress = proposalQuery.PolicyDetails.Risk.IsRegAddressSameasPermanentAddress,
					VehicleVariant = proposalQuery.PolicyDetails.Risk.VehicleVariant,
					StateOfRegistrationID = proposalQuery.PolicyDetails.Risk.StateOfRegistrationID,
					RtoRegionCode = proposalQuery.PolicyDetails.Risk.Rto_RegionCode,

				},
				Vehicle = new Domain.Reliance.Vehicle()
				{
					RegistrationNumber = proposalQuery.PolicyDetails.Vehicle.Registration_Number,
					ISNewVehicle = proposalQuery.PolicyDetails.Vehicle.IsNewVehicle,
					RegistrationDate = proposalQuery.PolicyDetails.Vehicle.Registration_date,
					SeatingCapacity = proposalQuery.PolicyDetails.Vehicle.SeatingCapacity,
					TypeOfFuel = proposalQuery.PolicyDetails.Vehicle.TypeOfFuel,
					BodyIDV = proposalQuery.PolicyDetails.Vehicle.BodyIDV,
					ChassisIDV = proposalQuery.PolicyDetails.Vehicle.ChassisIDV,
					BodyPrice = proposalQuery.PolicyDetails.Vehicle.BodyPrice,
					ChassisPrice = proposalQuery.PolicyDetails.Vehicle.ChassisPrice,
					ISmanufacturerfullybuild = proposalQuery.PolicyDetails.Vehicle.ISmanufacturerfullybuild,
				},
				PreviousInsuranceDetails = new Domain.Reliance.PreviousInsuranceDetails()
				{
					PrevYearInsurer = proposalQuery.PolicyDetails.PreviousInsuranceDetails.PrevYearInsurer,
					PrevYearPolicyNo = proposalQuery.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyNo,
					PrevYearPolicyStartDate = proposalQuery.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyStartDate,
					PrevYearPolicyEndDate = proposalQuery.PolicyDetails.PreviousInsuranceDetails.PrevYearPolicyEndDate,
					IsClaimedLastYear = proposalQuery.PolicyDetails.PreviousInsuranceDetails.IsClaimedLastYear,
					IsPreviousPolicyDetailsAvailable = proposalQuery.PolicyDetails.PreviousInsuranceDetails.IsVehicleOfPreviousPolicySold
				},
				NCBEligibility = new Domain.Reliance.NCBEligibility()
				{
					IsNCBApplicable = true,
					NCBEligibilityCriteria = proposalQuery.PolicyDetails.NCBEligibility.NCBEligibilityCriteria,
					PreviousNCB = proposalQuery.PolicyDetails.NCBEligibility.PreviousNCB,
					CurrentNCB = proposalQuery.PolicyDetails.NCBEligibility.CurrentNCB
				},
				Cover = new Domain.Reliance.Cover()
				{
					IsNilDepreciation = proposalQuery.PolicyDetails.Cover.IsNilDepreciation,
					NilDepreciationCoverage = new Domain.Reliance.NilDepreciationCoverages()
					{
						NilDepreciationCoverage = new Domain.Reliance.NilDepreciationCoverages()
						{
							IsMandatory = proposalQuery.PolicyDetails.Cover.NilDepreciationCoverage.NilDepreciationCoverage.IsMandatory,
							IsChecked = proposalQuery.PolicyDetails.Cover.NilDepreciationCoverage.NilDepreciationCoverage.IsChecked,
							NoOfItems = proposalQuery.PolicyDetails.Cover.NilDepreciationCoverage.NilDepreciationCoverage.NoOfItems,
							PackageName = proposalQuery.PolicyDetails.Cover.NilDepreciationCoverage.NilDepreciationCoverage.PackageName,
							ApplicableRate = !string.IsNullOrEmpty(proposalQuery.PolicyDetails.Cover.NilDepreciationCoverage.NilDepreciationCoverage.ApplicableRate) ? Convert.ToDouble(proposalQuery.PolicyDetails.Cover.NilDepreciationCoverage.NilDepreciationCoverage.ApplicableRate) : 0,
							PolicyCoverID = proposalQuery.PolicyDetails.Cover.NilDepreciationCoverage.NilDepreciationCoverage.PolicyCoverID,
						}
					},
					IsTotalCover = proposalQuery.PolicyDetails.Cover.IsTotalCover,
					TotalCover = new TotalCover()
					{
						totalCover = new TotalCover()
						{
							IsMandatory = proposalQuery.PolicyDetails.Cover.TotalCover.totalCover.IsMandatory,
							IsChecked = proposalQuery.PolicyDetails.Cover.TotalCover.totalCover.IsChecked,
							NoOfItems = proposalQuery.PolicyDetails.Cover.TotalCover.totalCover.NoOfItems,
							PackageName = proposalQuery.PolicyDetails.Cover.TotalCover.totalCover.PackageName,
							PolicyCoverID = proposalQuery.PolicyDetails.Cover.TotalCover.totalCover.PolicyCoverID,
						}
					},
					IsRegistrationCover = proposalQuery.PolicyDetails.Cover.IsRegistrationCover,
					RegistrationCost = new RegistrationCost()
					{
						registrationCost = new RegistrationCost()
						{
							IsChecked = proposalQuery.PolicyDetails.Cover.RegistrationCost.registrationCost.IsChecked,
							NoOfItems = proposalQuery.PolicyDetails.Cover.RegistrationCost.registrationCost.NoOfItems,
							IsMandatory = proposalQuery.PolicyDetails.Cover.RegistrationCost.registrationCost.IsMandatory,
							PackageName = proposalQuery.PolicyDetails.Cover.RegistrationCost.registrationCost.PackageName,
							SumInsured = proposalQuery.PolicyDetails.Cover.RegistrationCost.registrationCost.SumInsured
						}
					},
					IsRoadTaxcover = proposalQuery.PolicyDetails.Cover.IsRoadTaxcover,
					RoadTax = new RoadTax()
					{
						roadTax = new RoadTax()
						{
							IsMandatory = proposalQuery.PolicyDetails.Cover.RoadTax.roadTax.IsMandatory,
							IsChecked = proposalQuery.PolicyDetails.Cover.RoadTax.roadTax.IsChecked,
							NoOfItems = proposalQuery.PolicyDetails.Cover.RoadTax.roadTax.NoOfItems,
							PackageName = proposalQuery.PolicyDetails.Cover.RoadTax.roadTax.PackageName,
							SumInsured = proposalQuery.PolicyDetails.Cover.RoadTax.roadTax.SumInsured,
							PolicyCoverID = proposalQuery.PolicyDetails.Cover.RoadTax.roadTax.PolicyCoverID,
						}
					},
					IsInsurancePremium = proposalQuery.PolicyDetails.Cover.IsInsurancePremium,
					InsurancePremium = new InsurancePremium()
					{
						insurancePremium = new InsurancePremium()
						{
							IsChecked = proposalQuery.PolicyDetails.Cover.InsurancePremium.IsChecked,
							IsMandatory = proposalQuery.PolicyDetails.Cover.InsurancePremium.insurancePremium.IsMandatory,
							NoOfItems = proposalQuery.PolicyDetails.Cover.InsurancePremium.insurancePremium.NoOfItems,
							PackageName = proposalQuery.PolicyDetails.Cover.InsurancePremium.insurancePremium.PackageName,
							SumInsured = proposalQuery.PolicyDetails.Cover.InsurancePremium.insurancePremium.SumInsured
						}
					},
					IsPAToUnnamedPassengerCovered = proposalQuery.PolicyDetails.Cover.IsPAToUnnamedPassengerCovered,
					NoOfUnnamedPassenegersCovered = proposalQuery.PolicyDetails.Cover.NoOfUnnamedPassenegersCovered,
					UnnamedPassengersSI = proposalQuery.PolicyDetails.Cover.UnnamedPassengersSI,
					PAToUnNamedPassenger = new Domain.Reliance.PAToUnNamedPassengers()
					{
						PAToUnNamedPassenger = new Domain.Reliance.PAToUnNamedPassengers()
						{
							IsMandatory = proposalQuery.PolicyDetails.Cover.PAToUnNamedPassenger.PAToUnNamedPassenger.IsMandatory,
							IsChecked = proposalQuery.PolicyDetails.Cover.PAToUnNamedPassenger.PAToUnNamedPassenger.IsChecked,
							PolicyCoverID = string.Empty,
							PackageName = string.Empty,
							NoOfItems = proposalQuery.PolicyDetails.Cover.PAToUnNamedPassenger.PAToUnNamedPassenger.NoOfItems,
							SumInsured = proposalQuery.PolicyDetails.Cover.PAToUnNamedPassenger.PAToUnNamedPassenger.SumInsured
						}
					},
					IsPAToOwnerDriverCoverd = !string.IsNullOrEmpty(proposalRequest.PersonalDetails.companyName) ? false : proposalQuery.PolicyDetails.Cover.IsPAToOwnerDriverCoverd,
					PACoverToOwner = new Domain.Reliance.PACoverToOwners()
					{
						PACoverToOwner = new Domain.Reliance.PACoverToOwners()
						{
							CPAcovertenure = proposalQuery.PolicyDetails.Cover.PACoverToOwner.CPAcovertenure,
							NomineeName = proposalQuery.PolicyDetails.Cover.PACoverToOwner.NomineeName,
							NomineeDOB = proposalQuery.PolicyDetails.Cover.PACoverToOwner.NomineeDOB,
							NomineeRelationship = proposalQuery.PolicyDetails.Cover.PACoverToOwner.NomineeRelationship
						}
					},
					IsLiabilityToPaidDriverCovered = proposalQuery.PolicyDetails.Cover.IsLiabilityToPaidDriverCovered,
					LiabilityToPaidDriver = new Domain.Reliance.LiabilityToPaidDrivers()
					{
						LiabilityToPaidDriver = new Domain.Reliance.LiabilityToPaidDrivers()
						{
							IsMandatory = proposalQuery.PolicyDetails.Cover.LiabilityToPaidDriver.LiabilityToPaidDriver.IsMandatory,
							IsChecked = proposalQuery.PolicyDetails.Cover.LiabilityToPaidDriver.LiabilityToPaidDriver.IsChecked,
							PolicyCoverID = string.Empty,
							PackageName = string.Empty,
							NoOfItems = Convert.ToInt32(proposalQuery.PolicyDetails.Cover.LiabilityToPaidDriver.LiabilityToPaidDriver.NoOfItems)
						}
					},
					IsSecurePlus = proposalQuery.PolicyDetails.Cover.IsSecurePlus,
					SecurePlus = new Domain.Reliance.SecurePluss()
					{
						SecurePlus = new Domain.Reliance.SecurePluss()
						{
							IsMandatory = proposalQuery.PolicyDetails.Cover.SecurePlus.SecurePlus.IsMandatory,
							IsChecked = proposalQuery.PolicyDetails.Cover.SecurePlus.SecurePlus.IsChecked,
							PolicyCoverID = string.Empty,
							PackageName = string.Empty,
							NoOfItems = string.Empty,
							ApplicableRate = proposalQuery.PolicyDetails.Cover.SecurePlus.SecurePlus.ApplicableRate
						}
					},
					IsSecurePremium = proposalQuery.PolicyDetails.Cover.IsSecurePremium,
					SecurePremium = new Domain.Reliance.SecurePremiums()
					{
						SecurePremium = new Domain.Reliance.SecurePremiums()
						{
							IsMandatory = proposalQuery.PolicyDetails.Cover.SecurePremium.SecurePremium.IsMandatory,
							IsChecked = proposalQuery.PolicyDetails.Cover.SecurePremium.SecurePremium.IsChecked,
							PolicyCoverID = string.Empty,
							PackageName = string.Empty,
							NoOfItems = string.Empty,
							ApplicableRate = proposalQuery.PolicyDetails.Cover.SecurePremium.SecurePremium.ApplicableRate
						}
					},
					IsAntiTheftDeviceFitted = proposalQuery.PolicyDetails.Cover.IsAntiTheftDeviceFitted,
					AntiTheftDeviceDiscount = new AntiTheftDeviceDiscount()
					{
						antiTheftDeviceDiscount = new AntiTheftDeviceDiscount()
						{
							IsChecked = proposalQuery.PolicyDetails.Cover.AntiTheftDeviceDiscount.IsChecked,
							IsMandatory = proposalQuery.PolicyDetails.Cover.AntiTheftDeviceDiscount.antiTheftDeviceDiscount.IsMandatory,
							NoOfItems = proposalQuery.PolicyDetails.Cover.AntiTheftDeviceDiscount.antiTheftDeviceDiscount.NoOfItems,
							PackageName = proposalQuery.PolicyDetails.Cover.AntiTheftDeviceDiscount.antiTheftDeviceDiscount.PackageName
						}
					},
					IsAutomobileAssociationMember = proposalQuery.PolicyDetails.Cover.IsAutomobileAssociationMember,
					AutomobileAssociationMembershipDiscount = new AutomobileAssociationMembershipDiscount()
					{
						automobileAssociationMembershipDiscount = new AutomobileAssociationMembershipDiscount()
						{
							IsChecked = proposalQuery.PolicyDetails.Cover.AutomobileAssociationMembershipDiscount.IsChecked,
							IsMandatory = proposalQuery.PolicyDetails.Cover.AutomobileAssociationMembershipDiscount.automobileAssociationMembershipDiscount.IsMandatory,
							NoOfItems = proposalQuery.PolicyDetails.Cover.AntiTheftDeviceDiscount.antiTheftDeviceDiscount.NoOfItems,
							PackageName = proposalQuery.PolicyDetails.Cover.AntiTheftDeviceDiscount.antiTheftDeviceDiscount.PackageName
						}
					},
					IsVoluntaryDeductableOpted = proposalQuery.PolicyDetails.Cover.IsVoluntaryDeductableOpted,

					VoluntaryDeductableAmount = proposalQuery.PolicyDetails.Cover.VoluntaryDeductableAmount,
					VoluntaryDeductible = new Domain.Reliance.VoluntaryDeductibles()
					{
						VoluntaryDeductible = new Domain.Reliance.VoluntaryDeductibles()
						{
							IsMandatory = proposalQuery.PolicyDetails.Cover.VoluntaryDeductible.VoluntaryDeductible.IsMandatory,
							IsChecked = proposalQuery.PolicyDetails.Cover.VoluntaryDeductible.VoluntaryDeductible.IsChecked,
							PolicyCoverID = string.Empty,
							PackageName = string.Empty,
							NoOfItems = string.Empty,
							SumInsured = Convert.ToInt32(proposalQuery.PolicyDetails.Cover.VoluntaryDeductible.VoluntaryDeductible.SumInsured)
						}
					},
					IsElectricalItemFitted = proposalQuery.PolicyDetails.Cover.IsElectricalItemFitted,
					ElectricalItemsTotalSI = Convert.ToInt32(proposalQuery.PolicyDetails.Cover.ElectricalItemsTotalSI),
					ElectricItems = new Domain.Reliance.ElectricItems()
					{
						ElectricalItems = new Domain.Reliance.ElectricalItems()
						{
							ElectricalItemsID = proposalQuery.PolicyDetails.Cover.ElectricItems.ElectricalItems.ElectricalItemsID,
							PolicyId = proposalQuery.PolicyDetails.Cover.ElectricItems.ElectricalItems.PolicyId,
							SerialNo = proposalQuery.PolicyDetails.Cover.ElectricItems.ElectricalItems.SerialNo,
							MakeModel = proposalQuery.PolicyDetails.Cover.ElectricItems.ElectricalItems.MakeModel,
							ElectricPremium = proposalQuery.PolicyDetails.Cover.ElectricItems.ElectricalItems.ElectricPremium,
							Description = proposalQuery.PolicyDetails.Cover.ElectricItems.ElectricalItems.Description,
							ElectricalAccessorySlNo = proposalQuery.PolicyDetails.Cover.ElectricItems.ElectricalItems.ElectricalAccessorySlNo,
							SumInsured = Convert.ToInt32(proposalQuery.PolicyDetails.Cover.ElectricItems.ElectricalItems.SumInsured),
						}
					},
					IsNonElectricalItemFitted = proposalQuery.PolicyDetails.Cover.IsNonElectricalItemFitted,
					NonElectricalItemsTotalSI = Convert.ToInt32(proposalQuery.PolicyDetails.Cover.NonElectricalItemsTotalSI),
					NonElectricItems = new Domain.Reliance.NonElectricItems()
					{
						NonElectricalItems = new Domain.Reliance.NonElectricalItems()
						{
							NonElectricalItemsID = proposalQuery.PolicyDetails.Cover.NonElectricItems.NonElectricalItems.NonElectricalItemsID,
							PolicyID = proposalQuery.PolicyDetails.Cover.NonElectricItems.NonElectricalItems.PolicyID,
							SerialNo = proposalQuery.PolicyDetails.Cover.NonElectricItems.NonElectricalItems.SerialNo,
							MakeModel = proposalQuery.PolicyDetails.Cover.NonElectricItems.NonElectricalItems.MakeModel,
							NonElectricPremium = proposalQuery.PolicyDetails.Cover.NonElectricItems.NonElectricalItems.NonElectricPremium,
							Description = proposalQuery.PolicyDetails.Cover.NonElectricItems.NonElectricalItems.Description,
							NonElectricalAccessorySlNo = proposalQuery.PolicyDetails.Cover.NonElectricItems.NonElectricalItems.NonElectricalAccessorySlNo,
							SumInsured = proposalQuery.PolicyDetails.Cover.NonElectricItems.NonElectricalItems.SumInsured
						}
					},
					IsBiFuelKit = proposalQuery.PolicyDetails.Cover.IsBiFuelKit,
					BifuelKit = new Domain.Reliance.BifuelKits()
					{
						BifuelKit = new Domain.Reliance.BifuelKits()
						{
							Fueltype = proposalQuery.PolicyDetails.Cover.BifuelKit.BifuelKit.Fueltype,
							IsChecked = proposalQuery.PolicyDetails.Cover.BifuelKit.BifuelKit.IsChecked,
							ISLpgCng = proposalQuery.PolicyDetails.Cover.BifuelKit.BifuelKit.ISLpgCng,
							SumInsured = proposalQuery.PolicyDetails.Cover.BifuelKit.BifuelKit.SumInsured
						}
					},
					BiFuelKitSi = Convert.ToInt32(proposalQuery.PolicyDetails.Cover.BiFuelKitSi),
					IsTPPDCover = proposalQuery.PolicyDetails.Cover.IsTPPDCover,
					IsBasicODCoverage = proposalQuery.PolicyDetails.Cover.IsBasicODCoverage,
					IsBasicLiability = proposalQuery.PolicyDetails.Cover.IsBasicLiability,
					IsGeographicalAreaExtended = proposalQuery.PolicyDetails.Cover.IsGeographicalAreaExtended
				}
			};

			XmlSerializer xmlSerializer = new XmlSerializer(typeof(Domain.Reliance.PolicyDetails));
			StringBuilder requestBuilder = new StringBuilder();
			StringWriter stringWriter = new StringWriter(requestBuilder);
			xmlSerializer.Serialize(stringWriter, relianceRequest);
			requestBody = RemoveXmlDefinition(requestBuilder.ToString());
			_logger.LogInformation("Reliance CreateProposal RequestBody {request}", requestBody);
			_client.DefaultRequestHeaders.Clear();
			id = await InsertICLogs(requestBody, createLeadModel.LeadID, _relianceConfig.BaseURL + _relianceConfig.ProposalWrapperURL, null, null, "Proposal");
			try
			{
				var proposalResponse = await _client.PostAsync(_relianceConfig.BaseURL + _relianceConfig.ProposalWrapperURL, new StringContent(requestBody, Encoding.UTF8, "application/xml"), cancellationToken);
				if (!proposalResponse.IsSuccessStatusCode)
				{
					responseBody = proposalResponse.ReasonPhrase;
					_logger.LogError("Reliance CreateProposal Response {responseBody}", responseBody);
					UpdateICLogs(id, null, responseBody);
					proposalVm.ValidationMessage = "Failed";
					proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
					UpdateICLogs(id, null, responseBody);
					return proposalResponseVM;
				}
				else
				{
					var response = await proposalResponse.Content.ReadAsStringAsync(cancellationToken);
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(response);
					string serializeXML = JsonConvert.SerializeXmlNode(doc);
					string lstPricingCheck = "[";
					var relianceProposalDto = new RelianceResponseDto();
					var relianceTaxListProposalDto = new RelianceTaxListResponseDto();
					var relianceTPProposalDto = new RelianceTPQuoteResponseDto();
					if (serializeXML.Contains(lstPricingCheck))
					{
						if (serializeXML.Count(x => lstPricingCheck.Contains(x)) > 1)
						{
							relianceTaxListProposalDto = JsonConvert.DeserializeObject<RelianceTaxListResponseDto>(serializeXML);
							responseBody = JsonConvert.SerializeObject(relianceTaxListProposalDto);
						}
						else
						{
							relianceProposalDto = JsonConvert.DeserializeObject<RelianceResponseDto>(serializeXML);
							responseBody = JsonConvert.SerializeObject(relianceProposalDto);
						}

					}
					else
					{
						relianceTPProposalDto = JsonConvert.DeserializeObject<RelianceTPQuoteResponseDto>(serializeXML);
						responseBody = JsonConvert.SerializeObject(relianceTPProposalDto);
					}

					_logger.LogInformation("Reliance Proposal Response {responseBody}", responseBody);

					UpdateICLogs(id, null, responseBody);

					var lstTaxComponent = new List<TaxComponent>();
					bool isBrancket = relianceTaxListProposalDto.MotorPolicy == null ? false : relianceTaxListProposalDto.MotorPolicy.LstTaxComponentDetails.TaxComponent.ToString().Contains("[");

					var taxAmount = string.Empty;
					if (isBrancket)
					{
						var taxComp = relianceTaxListProposalDto?.MotorPolicy?.LstTaxComponentDetails.TaxComponent;
						if (taxComp != null)
						{
							double taxAmount1 = 0;
							foreach (var taxItem in taxComp)
							{
								var tokenn1 = taxItem; 
								if (tokenn1 != null)
								{
									taxAmount1 = taxAmount1 + Convert.ToDouble(tokenn1.Amount);
								}
							}
							taxAmount = Math.Round(taxAmount1).ToString();
						}
					}
					else
					{
						var taxComponent = relianceProposalDto.MotorPolicy.LstTaxComponentDetails?.TaxComponent;
						var totalTax = Math.Round(Convert.ToDouble(taxComponent.Amount));
						taxAmount = Convert.ToString(totalTax);
					}
					proposalVm = new QuoteResponseModel
					{

						Tax = new ServiceTax()
						{
							totalTax = taxAmount,
							//totalTax = Math.Round(serializeXML.Contains(lstPricingCheck) ? (serializeXML.Count(x => lstPricingCheck.Contains(x)) > 1 ? Convert.ToDouble(relianceTaxListProposalDto.MotorPolicy.LstTaxComponentDetails.TaxComponent[0].Amount) : Convert.ToDouble(relianceProposalDto.MotorPolicy.LstTaxComponentDetails.TaxComponent.Amount)) : Convert.ToDouble(relianceTPProposalDto.MotorPolicy.LstTaxComponentDetails.TaxComponent.Amount)).ToString(),
						},
						ApplicationId = serializeXML.Contains(lstPricingCheck) ? (serializeXML.Count(x => lstPricingCheck.Contains(x)) > 1 ? relianceTaxListProposalDto.MotorPolicy.ProposalNo : relianceProposalDto.MotorPolicy.ProposalNo) : relianceTPProposalDto.MotorPolicy.ProposalNo,
						InsurerName = _relianceConfig.InsurerName,
						InsurerStatusCode = (int)HttpStatusCode.OK,
						TotalPremium = serializeXML.Contains(lstPricingCheck) ? (serializeXML.Count(x => lstPricingCheck.Contains(x)) > 1 ? Convert.ToString(relianceTaxListProposalDto.MotorPolicy.NetPremium) : Convert.ToString(relianceProposalDto.MotorPolicy.NetPremium)) : Convert.ToString(relianceTPProposalDto.MotorPolicy.NetPremium),
						GrossPremium = serializeXML.Contains(lstPricingCheck) ? (serializeXML.Count(x => lstPricingCheck.Contains(x)) > 1 ? Convert.ToString(relianceTaxListProposalDto.MotorPolicy.FinalPremium) : Convert.ToString(relianceProposalDto.MotorPolicy.FinalPremium)) : Convert.ToString(relianceTPProposalDto.MotorPolicy.FinalPremium),
						IsBreakIn = false,
						IsSelfInspection = false,
						PolicyNumber = serializeXML.Contains(lstPricingCheck) ? (serializeXML.Count(x => lstPricingCheck.Contains(x)) > 1 ? relianceTaxListProposalDto.MotorPolicy.ProposalNo : relianceProposalDto.MotorPolicy.ProposalNo) : relianceTPProposalDto.MotorPolicy.ProposalNo

					};
				}

				proposalResponseVM = new ProposalResponseModel()
				{
					quoteResponseModel = proposalVm,
					RequestBody = requestBody,
					ResponseBody = responseBody
				};
				UpdateICLogs(id, null, responseBody);
				return proposalResponseVM;
			}
			catch (Exception ex)
			{
				_logger.LogError("Reliance Proposal Error {exception}", ex.Message);
				proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
				UpdateICLogs(id, null, ex.Message);
				return proposalResponseVM;
			}
		}
		catch (Exception ex)
		{
			_logger.LogError("Reliance Proposal Error {exception}", ex.Message);
			proposalVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
			return proposalResponseVM;
		}
	}
	public async Task<RelianceUploadDocumentResponseModel> GetCKYCPOAResponse(string transactionId, string kycId, CancellationToken cancellationToken)
	{
		RelianceUploadDocumentResponseModel uploadDocumentVm = new RelianceUploadDocumentResponseModel();
		string responseBody = string.Empty;
		string requestBody = string.Empty;
		var id = 0;
		UploadCKYCDocumentResponse uploadCKYCDocumentVM = new UploadCKYCDocumentResponse();
		CreateLeadModel createLeadModel = new CreateLeadModel();

		try
		{
			string searchTypeL = "kyc_id";
			string searchValueU = kycId.ToUpper();
			var client = new RestClient(_relianceConfig.CKYCVerifyURL + searchTypeL + "=" + searchValueU + "&redirect_url=" + _relianceConfig.CKYCPOARedirectionURL + transactionId + "/" + _applicationClaims.GetUserId());
			var request = new RestRequest(string.Empty, Method.Get);
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
			id = await InsertICLogs(request.ToString(), string.Empty, _relianceConfig.CKYCVerifyURL, null, JsonConvert.SerializeObject(request), "KYC");
			try
			{
				var result = await client.ExecuteAsync(request, cancellationToken);
				requestBody = client.BuildUri(request).ToString();
				_logger.LogInformation("Reliance CKYC POI Request {Request}", requestBody);
				responseBody = result.Content;
				_logger.LogInformation("Reliance CKYC POI Request {Response}", responseBody);

				uploadDocumentVm = new RelianceUploadDocumentResponseModel()
				{
					RequestBody = requestBody,
					ResponseBody = responseBody
				};

				var response = JsonConvert.DeserializeObject<RelianceCkycFetchModel>(responseBody);

				if (response != null && response.success.Equals(true) && response.data.iskycVerified.Equals(1))
				{
					DateTime date = DateTime.ParseExact(response.data.dob, "dd/MM/yyyy", null);
					string dob = date.ToString("yyyy-MM-dd");
					var namesList = response.data.name.Split(' ');
					createLeadModel.LeadName = namesList.Length > 0 ? namesList[0] : string.Empty;
					createLeadModel.LastName = namesList.Length > 1 ? namesList[1] : string.Empty;
					createLeadModel.MiddleName = namesList.Length > 2 ? namesList[2] : string.Empty;
					createLeadModel.DOB = dob;
					createLeadModel.PhoneNumber = response.data.mobile;
					createLeadModel.Email = response.data.email;
					createLeadModel.PANNumber = response.data.pan;
					createLeadModel.ckycNumber = response.data.ckycNumber;
					createLeadModel.kyc_id = response.data.kyc_id;
					createLeadModel.CKYCstatus = response.data.status;
					createLeadModel.PermanentAddress = new LeadAddressModel
					{
						AddressType = "PRIMARY",
						Address1 = response.data?.permanentAddress1,
						Address2 = response.data?.permanentAddress2,
						Address3 = response.data?.permanentAddress3,
						Pincode = response.data?.permanentPincode
					};
					uploadCKYCDocumentVM.Name = namesList.Length > 0 ? namesList[0] : string.Empty;
					uploadCKYCDocumentVM.LastName = namesList.Length > 1 ? namesList[1] : string.Empty;
					uploadCKYCDocumentVM.MiddleName = namesList.Length > 2 ? namesList[2] : string.Empty;
					uploadCKYCDocumentVM.DOB = response.data.dob;
					uploadCKYCDocumentVM.Address = response.data.permanentAddress;
					uploadCKYCDocumentVM.CKYCStatus = POA_SUCCESS;
					uploadCKYCDocumentVM.Message = POA_SUCCESS;
					uploadCKYCDocumentVM.KYCId = kycId;
					uploadCKYCDocumentVM.TransactionId = transactionId;
					uploadCKYCDocumentVM.InsurerName = _relianceConfig.InsurerName;

					uploadDocumentVm.uploadCKYCDocumentResponse = uploadCKYCDocumentVM;
					uploadDocumentVm.createLeadModel = createLeadModel;
					return uploadDocumentVm;
				}
				else if (response != null && response.success.Equals(true) && response.data.iskycVerified.Equals(0))
				{
					uploadCKYCDocumentVM.CKYCStatus = FAILED;
					uploadCKYCDocumentVM.Message = MESSAGE;
					uploadCKYCDocumentVM.KYCId = kycId;
					uploadCKYCDocumentVM.TransactionId = transactionId;
					uploadCKYCDocumentVM.InsurerName = _relianceConfig.InsurerName;
				}
				uploadCKYCDocumentVM.CKYCStatus = FAILED;
				uploadCKYCDocumentVM.Message = MESSAGE;
				uploadCKYCDocumentVM.KYCId = kycId;
				uploadCKYCDocumentVM.TransactionId = transactionId;
				uploadCKYCDocumentVM.InsurerName = _relianceConfig.InsurerName;

				uploadDocumentVm.uploadCKYCDocumentResponse = uploadCKYCDocumentVM;
				uploadDocumentVm.createLeadModel = createLeadModel;
				UpdateICLogs(id, transactionId, responseBody);
				return uploadDocumentVm;
			}
			catch (Exception ex)
			{
				UpdateICLogs(id, transactionId, ex.Message);
				return default;
			}
		}
		catch (Exception ex)
		{
			uploadCKYCDocumentVM.CKYCStatus = FAILED;
			uploadCKYCDocumentVM.Message = MESSAGE;
			uploadCKYCDocumentVM.KYCId = kycId;
			uploadCKYCDocumentVM.TransactionId = transactionId;
			uploadCKYCDocumentVM.InsurerName = _relianceConfig.InsurerName;
			_logger.LogError("Reliance Upload CKYC Error {exception}", ex.Message);
			uploadDocumentVm.uploadCKYCDocumentResponse = uploadCKYCDocumentVM;
			uploadDocumentVm.createLeadModel = createLeadModel;
			return uploadDocumentVm;
		}
	}
	public async Task<ReliancePaymentWrapperModel> GetDocumentPDFBase64(string leadId, string policyNumber, string documentLink, CancellationToken cancellationToken)
	{
		var id = 0;
		var requestBody = string.Empty;
		var responseBody = string.Empty;
		try
		{
			_logger.LogInformation("Reliance GetDocumentPDFBase64 Request {Request}", documentLink);
			id = await InsertICLogs(string.Empty, leadId, documentLink, string.Empty, string.Empty, "Payment");
			try
			{
				ReliancePolicyDocumentRequestResponse request = new ReliancePolicyDocumentRequestResponse();
				ReliancePolicyDocumentResponse documentResponse = new ReliancePolicyDocumentResponse();
				request.PolicyNumber = policyNumber;
				request.SecureAuthToken = _relianceConfig.SecureAuthToken;
				request.SourceSystemID = _relianceConfig.UserID;
				request.EndorsementNo = string.Empty;
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(ReliancePolicyDocumentRequestResponse));
				StringBuilder requestBuilder = new StringBuilder();
				StringWriter stringWriter = new StringWriter(requestBuilder);
				xmlSerializer.Serialize(stringWriter, request);
				requestBody = RemoveXmlDefinition(requestBuilder.ToString());
				_client.DefaultRequestHeaders.Clear();
				var response = await _client.PostAsync(_relianceConfig.PolicyDownloadURL, new StringContent(requestBody, Encoding.UTF8, "application/xml"), cancellationToken);
				if (!response.IsSuccessStatusCode)
				{
					responseBody = response.ReasonPhrase;
					_logger.LogError("Reliance GetDocumentPDFBase64 Response {responseBody}", responseBody);
					UpdateICLogs(id, null, responseBody);
				}
				else
				{
					var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
					XmlDocument doc = new XmlDocument();
					doc.LoadXml(responseString);
					string result = JsonConvert.SerializeXmlNode(doc);
					documentResponse = JsonConvert.DeserializeObject<ReliancePolicyDocumentResponse>(result);
					responseBody = JsonConvert.SerializeObject(documentResponse);
					_logger.LogInformation("Reliance GetDocumentPDFBase64 Response {responseBody}", responseBody);
					UpdateICLogs(id, null, responseBody);
					var policyDocumentdetail = new ReliancePaymentWrapperModel()
					{
						Data = new Data()
						{
							PdfBase64 = Encoding.ASCII.GetBytes(documentResponse.GenerateScheduleResponse.DownloadLink),
							PolicDocumentLink = documentResponse.GenerateScheduleResponse.DownloadLink
						}
					};
					policyDocumentdetail.Data.DocumentBase64 = Convert.ToBase64String(policyDocumentdetail.Data.PdfBase64);
					return policyDocumentdetail;
				}
			}
			catch (Exception ex)
			{
				UpdateICLogs(id, null, ex.Message);
				_logger.LogError("Reliance GetDocumentPDFBase64 Exception {Exception}", ex.Message);
				return default;
			}
		}
		catch (Exception ex)
		{
			_logger.LogError("Reliance GetDocumentPDFBase64 Exception {Exception}", ex.Message);
			return default;
		}
		return default;
	}
}
