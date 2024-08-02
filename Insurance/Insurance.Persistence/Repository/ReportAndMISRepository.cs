using Dapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Domain.AllReportAndMIS;
using Insurance.Domain.Customer;
using Insurance.Domain.ReportAndMIS;
using Insurance.Persistence.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Persistence.Repository
{
    public class ReportAndMISRepository : IReportAndMISRepository
    {
        private readonly ApplicationDBContext _context;
        public ReportAndMISRepository(ApplicationDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<AllReportAndMISResponseModel> AllReportAndMIS(AllReportAndMISRequestModel request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<GetAllReportResponseModel> GetAllReport(GetAllReportRequestModel getAllReportRequest, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@EnquirId", getAllReportRequest.EnquirId, DbType.String, ParameterDirection.Input);
            parameters.Add("@ProductType", getAllReportRequest.ProductType, DbType.String, ParameterDirection.Input);
            parameters.Add("@Insurertype", getAllReportRequest.Insurertype, DbType.String, ParameterDirection.Input);
            parameters.Add("@Stage", getAllReportRequest.Stage, DbType.String, ParameterDirection.Input);
            parameters.Add("@StartDate", getAllReportRequest.Startdate, DbType.String, ParameterDirection.Input);
            parameters.Add("@EndDate", getAllReportRequest.EndDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@CurrentPageIndex", getAllReportRequest.CurrentPageIndex, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@CurrentPageSize", getAllReportRequest.CurrentPageSize, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@CreatedBy", getAllReportRequest.CreatedBy, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<GetAllReportModel>("[dbo].[Insurance_GetAllReport]", parameters,
                commandType: CommandType.StoredProcedure);
            if (result.Any())
            {
                int totalRecords = result.FirstOrDefault().TotalRecord;
                GetAllReportResponseModel resp = new GetAllReportResponseModel
                {
                    GetAllReportModel = result,
                    TotalRecords = totalRecords,
                };
                return resp;
            }
            return default;
        }

    }
}
