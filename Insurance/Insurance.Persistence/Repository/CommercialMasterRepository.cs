using AutoMapper;
using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.CommercialMaster.Query.GetCommercialVehicleOtherDetailsAskOptions;
using Insurance.Domain.CommercialMaster;
using Insurance.Domain.CommercialVehicle;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Persistence.Configuration;
using Microsoft.Extensions.Options;
using System.Data;
using ThirdPartyUtilities.Abstraction;

namespace Insurance.Persistence.Repository
{
    public class CommercialMasterRepository : ICommercialMasterRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly ISignzyService _signzyService;
        private readonly LogoConfig _logoConfig;
        private readonly IApplicationClaims _applicationClaims;
        private readonly IMapper _mapper;

        public CommercialMasterRepository(ApplicationDBContext context, ISignzyService signzyService, IOptions<LogoConfig> options, IApplicationClaims applicationClaims, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _signzyService = signzyService ?? throw new ArgumentNullException(nameof(signzyService));
            _logoConfig = options.Value;
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Get GetMakeModelFuel List
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns> 
        public async Task<CommercialVehicleCategory> GetCommercialCategory(CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetCategorySubCategory]", parameters,
                commandType: CommandType.StoredProcedure);
            var childRecords = result.Read<CommercialSubCategory>();
            CommercialVehicleCategory response = new()
            {
                Category = result.Read<CommercialCategory>(),
            };
            foreach (var master in response.Category)
            {
                master.SubCategory = from subCat in childRecords where subCat.MasterCategoryId == master.CategoryId select subCat;
            }
            return response;
        }

        /// <summary>
        /// GetCommercialVehicleOtherDetailsAskOptions List
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns> 
        public async Task<CommercialVehicleAskAdditionalsDetailsModel> GetCommercialVehicleOtherDetailsAskOptions(GetCommercialVehicleOtherDetailsAskOptionsQuery getCommercialVehicleOtherDetailsAskOptionsQuery, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("VariantId", getCommercialVehicleOtherDetailsAskOptionsQuery.variantid, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetCommercialVehicleOtherDetailsAskOptions]", parameters, commandType: CommandType.StoredProcedure);

            var commercialVehicleAskOptionsSwitch = result.Read<CommercialVehicleAskOptionsSwitch>();

            CommercialVehicleAskAdditionalsDetailsModel commercialVehicleAskAdditionalsDetailsModel = new();
            commercialVehicleAskAdditionalsDetailsModel.CommercialVehicleCategory = new List<CommercialVehicleCategoryModel>();

            var vehicleBodyTypes = result.Read<NameValueModel>();
            CommercialVehicleCategoryModel bodyType = new();
            bodyType.Name = "CVVehicleType";
            bodyType.SubCategoryList = vehicleBodyTypes;
            bodyType.Ask = true;
            bodyType.label = "Body Type";
            commercialVehicleAskAdditionalsDetailsModel.CommercialVehicleCategory.Add(bodyType);

            var vehicleUseNatureTypes = result.Read<NameValueModel>();
            CommercialVehicleCategoryModel useNatureType = new();
            useNatureType.Name = "UsageNatureType";
            useNatureType.SubCategoryList = vehicleUseNatureTypes;
            useNatureType.Ask = vehicleUseNatureTypes.Any();
            useNatureType.label = "Vehicle Use";
            commercialVehicleAskAdditionalsDetailsModel.CommercialVehicleCategory.Add(useNatureType);


            var askForIfTrailerOptions = result.Read<NameValueModel>();
            CommercialVehicleCategoryModel askForIfTrailerOptionsType = new();
            askForIfTrailerOptionsType.Name = "askForIfTrailerOptions";
            askForIfTrailerOptionsType.SubCategoryList = askForIfTrailerOptions;
            askForIfTrailerOptionsType.Ask = commercialVehicleAskOptionsSwitch.First().AskForIfTrailer;
            askForIfTrailerOptionsType.label = "Trailer";
            commercialVehicleAskAdditionalsDetailsModel.CommercialVehicleCategory.Add(askForIfTrailerOptionsType);

            var askForHazardusVehicleUseOptions = result.Read<NameValueModel>();
            CommercialVehicleCategoryModel askForHazardusVehicleUseOptionsType = new();
            askForHazardusVehicleUseOptionsType.Name = "askForHazardusVehicleUseOptions";
            askForHazardusVehicleUseOptionsType.SubCategoryList = askForHazardusVehicleUseOptions;
            askForHazardusVehicleUseOptionsType.Ask = commercialVehicleAskOptionsSwitch.First().AskForHazardusVehicleUse;
            askForHazardusVehicleUseOptionsType.label = "Vehicle Use (Hazardus)";
            commercialVehicleAskAdditionalsDetailsModel.CommercialVehicleCategory.Add(askForHazardusVehicleUseOptionsType);

            var commercialVehicleCarrierTypeOptions = result.Read<NameValueModel>();
            CommercialVehicleCategoryModel commercialVehicleCarrierTypeOptionsType = new();
            commercialVehicleCarrierTypeOptionsType.Name = "commercialVehicleCarrierTypeOptions";
            commercialVehicleCarrierTypeOptionsType.SubCategoryList = commercialVehicleCarrierTypeOptions;
            commercialVehicleCarrierTypeOptionsType.Ask = commercialVehicleAskOptionsSwitch.First().AskCarrierType;
            commercialVehicleCarrierTypeOptionsType.label = "Carrier Type";
            commercialVehicleAskAdditionalsDetailsModel.CommercialVehicleCategory.Add(commercialVehicleCarrierTypeOptionsType);

            var vehicleUseTypes = result.Read<NameValueModel>();
            CommercialVehicleCategoryModel usageType = new();
            usageType.Name = "UsageType";
            usageType.SubCategoryList = vehicleUseTypes;
            usageType.Ask = vehicleUseTypes.Any();
            usageType.label = "Vehicle Use";
            commercialVehicleAskAdditionalsDetailsModel.CommercialVehicleCategory.Add(usageType);

            var PCVVehicleCategory = result.Read<NameValueModel>();
            CommercialVehicleCategoryModel pCVVehicleCategoryType = new();
            pCVVehicleCategoryType.Name = "PCVVehicleCategory";
            pCVVehicleCategoryType.SubCategoryList = PCVVehicleCategory;
            pCVVehicleCategoryType.Ask = PCVVehicleCategory.Any();
            pCVVehicleCategoryType.label = "PCV Vehicle Category";
            commercialVehicleAskAdditionalsDetailsModel.CommercialVehicleCategory.Add(pCVVehicleCategoryType);


            return commercialVehicleAskAdditionalsDetailsModel;
        }
    }
}
