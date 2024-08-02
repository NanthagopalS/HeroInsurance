using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.ManualPolicy.Query;
using Insurance.Core.Features.ManualPolicy.Query.GetManualPolicyNature;
using Insurance.Domain.ManualPolicy;
using Insurance.Persistence.Configuration;
using System.Data;

namespace Insurance.Persistence.Repository
{
    public class ManualPolicyRepository : IManualPolicyRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IApplicationClaims _applicationClaims;

        public ManualPolicyRepository(ApplicationDBContext context, IApplicationClaims _claims)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _applicationClaims = _claims ?? throw new ArgumentNullException(nameof(_claims));
        }
        public async Task<ManualPolicyReponseModel> DumpPolicyExcelRecordsToDatabase(DataTable reqModel, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CreatedBy", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("@TempTable", reqModel, DbType.Object, ParameterDirection.Input);
            var result = await connection.QueryAsync<ManualPolicyReponseModel>("[dbo].[Insurance_ManualPolicyMigration]", parameters,
                         commandType: CommandType.StoredProcedure);
            return result.First();
        }

        public async Task<EmailManualPolicyValidationResponce> GetManualPolicyValidationDetails(CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserID", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_ManualPolicyMigrationResponce]", parameters,
                         commandType: CommandType.StoredProcedure);
            IEnumerable<ManualPolicyDumpRecords> manualvalidation = result.Read<ManualPolicyDumpRecords>();
            IEnumerable<ManualPolicyDumpErrors> manualerrorlogs = result.Read<ManualPolicyDumpErrors>();
            IEnumerable<ManualPolicyCount> manualPolicyCounts = result.Read<ManualPolicyCount>();
            return new EmailManualPolicyValidationResponce()
            {
                manualPolicyCounts = manualPolicyCounts.First(),
                ManualPolicyDumpErrors = manualerrorlogs,
                ManualPolicyDumpRecords = manualvalidation
            };
        }


        public async Task<GetManualPolicyListModel> GetManualPolicyList(GetManualPolicyListQuery request, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@Product", request.Product, DbType.String, ParameterDirection.Input);
            parameters.Add("@CreatedBy", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("@Moter", request.Moter, DbType.String, ParameterDirection.Input);
            parameters.Add("@PolicyType", request.PolicyType, DbType.String, ParameterDirection.Input);
            parameters.Add("@PolicySource", request.PolicySource, DbType.String, ParameterDirection.Input);
            parameters.Add("@StartDate", request.StartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@EndDate", request.EndDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@SearchText", request.SearchText, DbType.String, ParameterDirection.Input);
            parameters.Add("@CurrentPageSize", request.CurrentPageSize, DbType.Int32, ParameterDirection.Input);
            parameters.Add("@CurrentPageIndex", request.CurrentPageIndex, DbType.Int32, ParameterDirection.Input);


            var result = await connection.QueryAsync<ManualPolicyList>("[dbo].[Insurance_GetManualPolicyList]", parameters,
                commandType: CommandType.StoredProcedure);
            int totalRecords = 0;
            if (result.Any())
            {
                totalRecords = result.FirstOrDefault().TotalRecords;
                GetManualPolicyListModel resp = new GetManualPolicyListModel
                {
                    ManualPolicyList = result,
                    TotalRecords = totalRecords,
                };
                return resp;
            }
            return default;


        }

		public async Task<IEnumerable<GetManualPolicyNatureResponseModel>> GetManualPolicyNatureList(GetManualPolicyNatureQuery request, CancellationToken cancellationToken)
		{
			using var connection = _context.CreateConnection();
			var parameters = new DynamicParameters();
			var result = await connection.QueryAsync<GetManualPolicyNatureResponseModel>("[dbo].[Insurance_GetManualPolicyNature]", parameters,
						 commandType: CommandType.StoredProcedure);
			return result;
		}
	}
}
