using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Customer.Queries.GetCustomersList;
using Insurance.Core.Features.Customer.Queries.GetRenewalDetailsById;
using Insurance.Domain.Customer;
using Insurance.Persistence.Configuration;
using System.Data;

namespace Insurance.Persistence.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IApplicationClaims _applicationClaims;

        public CustomerRepository(ApplicationDBContext context, IApplicationClaims applicationClaims)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        }
        public async Task<GetCustomersResponseModel> GetCustomersList(GetCustomersListQuery getCustomersListRequest, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CustomerType", getCustomersListRequest.CustomerType, DbType.String, ParameterDirection.Input);
            parameters.Add("@SearchText", getCustomersListRequest.SearchText, DbType.String, ParameterDirection.Input);
            parameters.Add("@PolicyType", getCustomersListRequest.PolicyType, DbType.String, ParameterDirection.Input);
            parameters.Add("@StartDate", getCustomersListRequest.StartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@EndDate", getCustomersListRequest.EndDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@CurrentPageIndex", getCustomersListRequest.CurrentPageIndex, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@CurrentPageSize", getCustomersListRequest.CurrentPageSize, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@CreatedBy", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("@PolicyNature", getCustomersListRequest.PolicyNature, DbType.String, ParameterDirection.Input);
            parameters.Add("@PolicyStatus", getCustomersListRequest.PolicyStatus, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<GetCustomersListModel>("[dbo].[Insurance_GetCustomerDetail]", parameters,
                commandType: CommandType.StoredProcedure);
            int totalRecords = 0;
            if (result.Any())
            {
                totalRecords = result.FirstOrDefault().TotalRecord;
                GetCustomersResponseModel resp = new GetCustomersResponseModel
                {
                    GetCustomersListModel = result,
                    TotalRecords = totalRecords,
                };
                return resp;
            }
            else
            {
                GetCustomersResponseModel resp = new GetCustomersResponseModel
                {
                    GetCustomersListModel = result,
                    TotalRecords = totalRecords,
                };
                return resp;
            }
        }
        public async Task<GetRenewalDetailsByIdResponceModel> GetRenewalDetailsById(GetRenewalDetailsByIdQuery GetCustomersDetailInputModel, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LeadId", GetCustomersDetailInputModel.LeadId, DbType.String, ParameterDirection.Input);

            var result = await connection.QueryAsync<GetRenewalDetailsByIdResponceModel>("[dbo].[Insurance_GetRenewalDetailsById]", parameters, commandType: CommandType.StoredProcedure);

            var proposalData = result.FirstOrDefault();


            return proposalData;
        }

        public async Task<GetCustomerLeadDetailModel> GetParticularLeadDetailById(string LeadId, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@LeadId", LeadId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetParticularLeadDetailById]", parameters,
                commandType: CommandType.StoredProcedure);
            GetCustomerLeadDetailModel response = new()
            {
                PersonalDetail = result.Read<PersonalDetailModel>(),
                VehicleDetail = result.Read<VehicleDetailModel>()

            };
            return response;
        }
    }
}
