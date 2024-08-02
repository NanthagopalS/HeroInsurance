using Dapper;
using POSP.Core.Contracts.Common;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.Reports;
using POSP.Domain.Reports;
using POSP.Persistence.Configuration;
using System.Data;

namespace POSP.Persistence.Repository
{
    /// <summary>
    /// repository for POSP Report repo
    /// </summary>
    public class POSPReportRepositoty : IPOSPReportRepository
    {
        private readonly IApplicationClaims _applicationClaims;
        private readonly ApplicationDBContext _context;
        /// <summary>
        /// conctructor for posp report
        /// </summary>
        /// <param name="applicationClaims"></param>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public POSPReportRepositoty(IApplicationClaims applicationClaims, ApplicationDBContext context)
        {
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        /// <summary>
        ///  new and old posp report 
        /// </summary>
        /// <param name="pOSPManagementInputModel"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<NewAndOldPOSPReportResponceModel> NewAndOldPOSPReport(NewAndOldPOSPReportQuery pOSPManagementInputModel, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@StartDate", pOSPManagementInputModel.StartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@EndDate", pOSPManagementInputModel.EndDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@Searchtext", pOSPManagementInputModel.SearchText, DbType.String, ParameterDirection.Input);
            parameters.Add("@StageId", pOSPManagementInputModel.StageId, DbType.String, ParameterDirection.Input);
            parameters.Add("@UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("@CurrentPageIndex", pOSPManagementInputModel.CurrentPageIndex, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@CurrentPageSize", pOSPManagementInputModel.CurrentPageSize, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@status", pOSPManagementInputModel.status, DbType.String, ParameterDirection.Input);

            IEnumerable<NewAndOldPOSPReportRecords> result;
            if (pOSPManagementInputModel.IsExportResponce)
            {
                result = await connection.QueryAsync<NewAndOldPOSPReportRecords>("[dbo].[POSP_GetNewAndOldPOSPReport]", parameters,
                             commandType: CommandType.StoredProcedure);
            }
            else
            {
                result = await connection.QueryAsync<NewAndOldPOSPReportRecords>("[dbo].[POSP_GetNewAndOldPOSPReportUI]", parameters,
                             commandType: CommandType.StoredProcedure);
            }
            NewAndOldPOSPReportResponceModel response = new();
            response.TotalRecords = 0;
            if (result is not null && result.Any())
            {
                response.ReportRecords = result;
                response.TotalRecords = result.FirstOrDefault().TotalRecords;
            }
            return response;
        }
    }
}

