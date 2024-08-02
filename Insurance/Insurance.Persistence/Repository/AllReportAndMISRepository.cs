using Dapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.AllReportAndMIS.Query.GetAllReportAndMIS;
using Insurance.Domain.AllReportAndMIS;
using Insurance.Domain.Customer;
using Insurance.Persistence.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Persistence.Repository
{
    public class AllReportAndMISRepository : IReportAndMISRepository
    {
        private readonly ApplicationDBContext _context;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="context"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public AllReportAndMISRepository(ApplicationDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        public async Task<AllReportAndMISResponseModel> AllReportAndMIS(AllReportAndMISRequestModel request, CancellationToken cancellationToken)
        {
            try
            {
                using var connection = _context.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@EnquiryId", request.EnquiryId, DbType.String, ParameterDirection.Input);
                parameters.Add("@ProductType", request.ProductType, DbType.String, ParameterDirection.Input);
                parameters.Add("@Insurertype", request.Insurertype, DbType.String, ParameterDirection.Input);
                parameters.Add("@Stage", request.Stage, DbType.String, ParameterDirection.Input);
                parameters.Add("@StartDate", request.StartDate, DbType.String, ParameterDirection.Input);
                parameters.Add("@EndDate", request.EndDate, DbType.String, ParameterDirection.Input);
                parameters.Add("@CurrentPageIndex", request.CurrentPageIndex, DbType.Int64, ParameterDirection.Input);
                parameters.Add("@CurrentPageSize", request.CurrentPageSize, DbType.Int64, ParameterDirection.Input);

                var result = await connection.QueryAsync<AllReportAndMISModel>("[dbo].[Insurance_GetAllReport]", parameters,
                commandType: CommandType.StoredProcedure);
                if (result.Count() > 0)
                {
                    int totalRecords = result.FirstOrDefault().TotalRecord;
                    AllReportAndMISResponseModel resp = new AllReportAndMISResponseModel
                    {
                        AllReportAndMISModels = result,
                        TotalRecord = totalRecords,
                    };
                    return resp;
                }
                return default;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
