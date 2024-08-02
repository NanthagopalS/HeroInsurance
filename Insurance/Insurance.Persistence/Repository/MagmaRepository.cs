using Dapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Magma.Queries.GetQuote;
using Insurance.Domain.Magma;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Options;
using System.Data;
using Insurance.Domain.HDFC;

namespace Insurance.Persistence.Repository;
public class MagmaRepository : IMagmaRepository
{
    private readonly IMagmaService _magmaService;
    private readonly ApplicationDBContext _applicationDBContext;
    private readonly MagmaConfig _magmaConfig;
    private readonly IGoDigitRepository _goDigitRepository;

    public MagmaRepository(IMagmaService magmaService,
                           ApplicationDBContext applicationDBContext,
                           IOptions<MagmaConfig> options,
                           IGoDigitRepository goDigitRepository)
    {
        _magmaService = magmaService ?? throw new ArgumentNullException(nameof(magmaService));
        _applicationDBContext = applicationDBContext ?? throw new ArgumentNullException(nameof(applicationDBContext));
        _magmaConfig = options?.Value;
        _goDigitRepository = goDigitRepository ?? throw new ArgumentNullException(nameof(goDigitRepository));
    }

    public async Task<QuoteResponseModel> GetQuote(GetMagmaQuoteQuery query, CancellationToken cancellationToken)
    {
        var quoteQuery = await QuoteMasterMapping(query, cancellationToken);

        if (quoteQuery != null)
        {
            //todo Call Quote API
            var quoteResponse = await _magmaService.GetQuote(quoteQuery, cancellationToken);

            await _goDigitRepository.QuoteTransaction(quoteResponse.Item1, quoteResponse.Item2, quoteResponse.Item3, "Quote", _magmaConfig.InsurerId, query.LeadId, 0, 0, 0, null);

            if (quoteResponse.Item1.InsurerName != null)
            {
                quoteResponse.Item1.InsurerId = _magmaConfig.InsurerId;
                return quoteResponse.Item1;
            }
        }
        return default;
    }

    private async Task<QuoteQueryModel> QuoteMasterMapping(GetMagmaQuoteQuery query, CancellationToken cancellationToken)
    {
        var addOnId = (query.AddOnsList != null && query.AddOnsList.Any()) ? String.Join(",", query.AddOnsList.Select(x => $"{x.AddOnId} ")) : string.Empty;
        var paCoverId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList.Select(x => $"{x.PACoverId} ")) : string.Empty;
        var accessoryId = (query.AccessoryList != null && query.AccessoryList.Any()) ? String.Join(",", query.AccessoryList.Select(x => $"{x.AccessoryId} ")) : string.Empty;
        var discountId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList.Select(x => $"{x.DiscountId}")) : string.Empty;
        var paCoverExtensionId = (query.PACoverList != null && query.PACoverList.Any()) ? String.Join(",", query.PACoverList.Select(x => $"{x.PACoverExtensionId}")) : string.Empty;
        var discountExtensionId = (query.DiscountList != null && query.DiscountList.Any()) ? String.Join(",", query.DiscountList.Select(x => $"{x.DiscountExtensionId}")) : string.Empty;



        using var connection = _applicationDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("PACoverId", paCoverId, DbType.String, ParameterDirection.Input);
        parameters.Add("AccessoryId", accessoryId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddonId", addOnId, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountId", discountId, DbType.String, ParameterDirection.Input);
        parameters.Add("InsurerId", _magmaConfig.InsurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("RTOId", query.RTOId, DbType.String, ParameterDirection.Input);
        parameters.Add("VariantId", query.VariantId, DbType.String, ParameterDirection.Input);
        parameters.Add("NCBId", query.PreviousPolicy?.NCBId, DbType.String, ParameterDirection.Input);
        parameters.Add("PolicyTypeId", query.PreviousPolicy?.PreviousPolicyTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleTypeId", query.VehicleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("PACoverExtensionId", paCoverExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("DiscountExtensionId", discountExtensionId, DbType.String, ParameterDirection.Input);
        parameters.Add("VehicleNumber", query.VehicleNumber, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetBajajQuoteMasterMapping]", parameters,
                     commandType: CommandType.StoredProcedure);

        var paCoverList = result.Read<PACoverModel>();
        var accessoryList = result.Read<AccessoryModel>();
        var addOnList = result.Read<AddonsModel>();
        var discountList = result.Read<DiscountModel>();
        var paCoverExtensionList = result.Read<PACoverExtensionModel>();
        var discountExtensionList = result.Read<DiscountExtensionModel>();
        var codeList = result.Read<RTOVehiclePreviousInsurerModel>();
        var configNameValueList = result.Read<ConfigNameValueModel>();

        var quoteQuery = new QuoteQueryModel
        {
            AddOns = new Domain.GoDigit.AddOns(),
            Discounts = new Discounts(),
            PACover = new PACover(),
            Accessories = new Accessories(),
            VehicleDetails = new VehicleDetails(),
            PreviousPolicyDetails = new PreviousPolicy()
        };
        if (discountExtensionList.Any())
        {
            foreach (var item in discountExtensionList)
            {
                quoteQuery.VoluntaryExcess = item.DiscountExtension;
            }
        }
        if (paCoverExtensionList.Any())
        {
            quoteQuery.GeogExtension = String.Join(",", paCoverExtensionList.Select(x => $"{x.PACoverExtension} "));
        }

        if (accessoryList.Any())
        {
            foreach (var item in accessoryList)
            {
                if (item.Accessory.Equals("Cng"))
                {
                    quoteQuery.Accessories.IsCNG = true;
                    quoteQuery.Accessories.CNGValue = query.AccessoryList.Where(x => x.AccessoryId == item.AccessoryId).Select(x => x.AccessoryValue).FirstOrDefault();
                }
                else if (item.Accessory.Equals("Electrical"))
                {
                    quoteQuery.Accessories.IsElectrical = true;
                    quoteQuery.Accessories.ElectricalValue = query.AccessoryList.Where(x => x.AccessoryId == item.AccessoryId).Select(x => x.AccessoryValue).FirstOrDefault();
                }
                else if (item.Accessory.Equals("NonElectrical"))
                {
                    quoteQuery.Accessories.IsNonElectrical = true;
                    quoteQuery.Accessories.NonElectricalValue = query.AccessoryList.Where(x => x.AccessoryId == item.AccessoryId).Select(x => x.AccessoryValue).FirstOrDefault();
                }
            }
        }
        if (discountList.Any())
        {
            foreach (var item in discountList)
            {
                if (item.DiscountName.Equals("Voluntary Deductible"))
                {
                    quoteQuery.Discounts.IsVoluntarilyDeductible = true;

                }
            }
        }
        if (paCoverList.Any())
        {
            foreach (var item in paCoverList)
            {
                if (item.CoverName.Equals("UnnamedPersonalAccidentCover"))
                {
                    quoteQuery.PACover.IsUnnamedPersonalAccidentCover = true;
                    quoteQuery.PACover.UnnamedPersonalAccidentCoverValue = (int)query.PACoverList.Where(x => x.PACoverId == item.PACoverId).Select(x => x.PACoverValue).FirstOrDefault();
                }
                else if (item.CoverName.Equals("UnnamedLLO"))
                {
                    quoteQuery.PACover.IsUnnamedLLO = true;
                    quoteQuery.PACover.UnnamedLLOValue = (int)query.PACoverList.Where(x => x.PACoverId == item.PACoverId).Select(x => x.PACoverValue).FirstOrDefault();
                }
                else if (item.CoverName.Equals("UnnamedOWNERDRIVER"))
                {
                    quoteQuery.PACover.IsUnnamedOWNERDRIVER = true;
                    quoteQuery.PACover.UnnamedOwnerDriverValue = (int)query.PACoverList.Where(x => x.PACoverId == item.PACoverId).Select(x => x.PACoverValue).FirstOrDefault();
                }
                else if (item.CoverName.Equals("UnnamedGEOG"))
                {
                    quoteQuery.PACover.IsGeoAreaExtension = true;
                    quoteQuery.PACover.GeoAreaExtensionValue = (int)query.PACoverList.Where(x => x.PACoverId == item.PACoverId).Select(x => x.PACoverValue).FirstOrDefault();
                }
            }
        }

        string previousInsurerCode = string.Empty;
        string ncbValue = string.Empty;
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
            quoteQuery.VehicleDetails.ManufactureDate = codeData.ManufactureDate;
            quoteQuery.PlanType = codeData.PlanType;
        }

        if (query.PreviousPolicy != null && query.PreviousPolicy.IsPreviousPolicy)
        {
            quoteQuery.PreviousPolicyDetails = new PreviousPolicy
            {
                IsPreviousInsurerKnown = query.PreviousPolicy.IsPreviousPolicy,
                PreviousPolicyNumber = query.PreviousPolicy.PreviousPolicyNumber,
                PreviousPolicyExpiryDateSAOD = string.IsNullOrWhiteSpace(query.PreviousPolicy.SAODPolicyExpiryDate) ?
                   query.PreviousPolicy.TPPolicyExpiryDate : query.PreviousPolicy.SAODPolicyExpiryDate,
                PreviousPolicyExpiryDateSATP = query.PreviousPolicy.TPPolicyExpiryDate,
                PreviousInsurerCode = previousInsurerCode,
                PreviousNoClaimBonus = ncbValue,
                IsClaimInLastYear = query.PreviousPolicy.IsPreviousYearClaim,
            };
        }

        

        quoteQuery.RegistrationYear = query.RegistrationYear;
        quoteQuery.ConfigNameValueModels = configNameValueList;
        quoteQuery.VehicleTypeId = query.VehicleTypeId;
        return quoteQuery;
    }
}
