using Admin.Core.Contracts.Common;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.Mmv.GetHeroVariantLists;
using Admin.Core.Features.Mmv.ResetMvvMappingForIcVariant;
using Admin.Core.Features.Mmv.UpdateVariantsMapping;
using Admin.Core.Features.Mmv.VariantMappingStatus;
using Admin.Domain.Mmv;
using Admin.Persistence.Configuration;
using Dapper;
using System.Data;

namespace Admin.Persistence.Repository
{
    public class MmvRepository : IMmvRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IApplicationClaims _claims;

        public MmvRepository(ApplicationDBContext context,IApplicationClaims applicationClaims)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _claims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        }
        public async Task<GetHeroVariantListsResponceModel> GetHeroVariantLists(GetHeroVariantListsQuery getHeroVariantListsQuery, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", getHeroVariantListsQuery.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("ModelId", getHeroVariantListsQuery.ModelId, DbType.String, ParameterDirection.Input);
            parameters.Add("VariantIds", getHeroVariantListsQuery.VariantIds, DbType.String, ParameterDirection.Input);
            parameters.Add("FuelTypes", getHeroVariantListsQuery.FuelTypes, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetHeroVariantLists]", parameters, commandType: CommandType.StoredProcedure);
            GetHeroVariantListsResponceModel getHeroVariantListsResponceModel = new()
            {
                HeroVariantLists = result.Read<HeroVariantLists>(),
                ICVariantPosibilityLists = result.Read<ICVariantPosibilityLists>(),
            };
            return getHeroVariantListsResponceModel;
        }

		public async Task<UpdateVariantsMappingResponceModel> UpdateVariantsMapping(UpdateVariantsMappingCommand updateVariantsMappingCommand,DataTable updateDataTable,DataTable newRecordDataTable, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", updateVariantsMappingCommand.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("UpdateVariantsMappingTableType", updateDataTable, DbType.Object, ParameterDirection.Input);
            parameters.Add("newRecordDataTable", newRecordDataTable, DbType.Object, ParameterDirection.Input);
            parameters.Add("UpdatedBy", _claims.GetUserId(), DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<UpdateVariantsMappingResponceModel>("[dbo].[Admin_UpdateHeroVariantListsForIcId]", parameters, commandType: CommandType.StoredProcedure);
            return result.First();
        }
		public async Task<IEnumerable<GetCustomMmvSearchResponseModel>> GetAllVariantForCustomModel(GetCustomMmvSearchQuery getVariantMappingStatusQuery, CancellationToken cancellationToken)
		{
            string[] modelNameArray = getVariantMappingStatusQuery.VariantSearch.Split(";");
			using var connection = _context.CreateConnection();
			var parameters = new DynamicParameters();
			parameters.Add("InsurerId", getVariantMappingStatusQuery.InsurerId, DbType.String, ParameterDirection.Input);
			parameters.Add("MakeName", getVariantMappingStatusQuery.MakeName, DbType.String, ParameterDirection.Input);
			parameters.Add("ModelName1", modelNameArray.ElementAtOrDefault(0) is not null? modelNameArray[0]:null, DbType.String, ParameterDirection.Input);
			parameters.Add("ModelName2", modelNameArray.ElementAtOrDefault(1) is not null ? modelNameArray[1] : null, DbType.String, ParameterDirection.Input);
			parameters.Add("ModelName3", modelNameArray.ElementAtOrDefault(2) is not null? modelNameArray[2]:null, DbType.String, ParameterDirection.Input);
			parameters.Add("ModelName4", modelNameArray.ElementAtOrDefault(3) is not null? modelNameArray[3]:null, DbType.String, ParameterDirection.Input);
			parameters.Add("ModelName5", modelNameArray.ElementAtOrDefault(4) is not null? modelNameArray[4]:null, DbType.String, ParameterDirection.Input);
			parameters.Add("ModelName6", modelNameArray.ElementAtOrDefault(5) is not null? modelNameArray[5]:null, DbType.String, ParameterDirection.Input);
			var result = await connection.QueryAsync<GetCustomMmvSearchResponseModel>("[dbo].[Admin_GetAllVariantForCustomModel]", parameters, commandType: CommandType.StoredProcedure);
            return result;
		}

        public async Task<bool> ResetMvvMappingForIcVariant(ResetMvvMappingForIcVariantCommand resetMvvMappingForIcVariantCommand, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", resetMvvMappingForIcVariantCommand.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("HeroVariantId", resetMvvMappingForIcVariantCommand.HeroVariantId, DbType.String, ParameterDirection.Input);
            parameters.Add("IcVariantId", resetMvvMappingForIcVariantCommand.IcVariantId, DbType.String, ParameterDirection.Input);
            parameters.Add("UpdatedBy", _claims.GetUserId(), DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_ResetMvvMappingForIcVariant]", parameters, commandType: CommandType.StoredProcedure);
            return result>0;
        }
    }
}
