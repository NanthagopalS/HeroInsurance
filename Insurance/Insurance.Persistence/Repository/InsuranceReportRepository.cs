using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.AllReportAndMIS.Query.BusinessSummery;
using Insurance.Core.Features.AllReportAndMIS.RequestandResponseH;
using Insurance.Domain.AllReportAndMIS.BusinessSummery;
using Insurance.Domain.AllReportAndMIS.BusinessSummerym;
using Insurance.Persistence.Configuration;
using System.Data;

namespace Insurance.Persistence.Repository
{
    public class InsuranceReportRepository : IInsuranceReportRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IApplicationClaims _applicationClaims;

        /// <summary>
        /// repository to get insurance related reports
        /// </summary>
        /// <param name="applicationClaims"></param>
        /// <param name="context"></param>
        public InsuranceReportRepository(ApplicationDBContext context, IApplicationClaims applicationClaims)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        }
        /// <summary>
        /// get business summery report record from DB
        /// </summary>
        /// <param name="requestModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<BusinessSummeryResponceModel> BusinessSummeryReport(BusunessSummeryQuery requestModel, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("@SearchText", requestModel.SearchText, DbType.String, ParameterDirection.Input);
            parameters.Add("@Insurers", requestModel.Insurers, DbType.String, ParameterDirection.Input);
            parameters.Add("@InsuranceType", requestModel.InsuranceType, DbType.String, ParameterDirection.Input);
            parameters.Add("@StartDate", requestModel.StartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@EndDate", requestModel.EndDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@CurrentPageSize", requestModel.CurrentPageSize, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@CurrentPageIndex", requestModel.CurrentPageIndex, DbType.Int64, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetBusinessSummeryReport]", parameters, commandType: CommandType.StoredProcedure);
            int totalRecords = 0;
            IEnumerable<BusinessSummeryRecords> records = result.Read<BusinessSummeryRecords>();
            if (records.Any())
            {
                totalRecords = records.FirstOrDefault().TotalRecord;
                BusinessSummeryResponceModel response = new()
                {
                    BusinessSummeryRecords = records,
                    TotalRecords = totalRecords
                };
                return response;
            }
            return new BusinessSummeryResponceModel() { TotalRecords = totalRecords };
        }

        /// <summary>
        /// RequestandResponseReport
        /// </summary>
        /// <param name="requestandResponseQuery"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<RequestandResponseModel> RequestandResponseReport(RequestandResponseQuery requestandResponseQuery, CancellationToken cancellationToken)
        {
            RequestandResponseModel response = new();
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LeadId", requestandResponseQuery.LeadId, DbType.String, ParameterDirection.Input);
            parameters.Add("@InsurerId", requestandResponseQuery.InsurerId, DbType.String, ParameterDirection.Input);
            parameters.Add("@ProductId", requestandResponseQuery.ProductId, DbType.String, ParameterDirection.Input);
            parameters.Add("@StageId", requestandResponseQuery.StageId, DbType.String, ParameterDirection.Input);
            parameters.Add("@StartDate", requestandResponseQuery.StartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@EndDate", requestandResponseQuery.EndDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@CurrentPageSize", requestandResponseQuery.CurrentPageSize, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@CurrentPageIndex", requestandResponseQuery.CurrentPageIndex, DbType.Int64, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_RequestandResponseRequest]", parameters, commandType: CommandType.StoredProcedure);
            int totalRecords = 0;
            IEnumerable<RequestandResponseRecord> records = result.Read<RequestandResponseRecord>();
            if (records.Any())
            {
                totalRecords = records.FirstOrDefault().TotalRecords;
                response.RequestandResponseRecord = records;
                response.TotalRecords = totalRecords;
                return response;
            }
            return response;
        }
    }
}
