using Dapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Leads;
using Insurance.Core.Features.Leads.GetPaymentStatus;
using Insurance.Domain.Leads;
using Insurance.Persistence.Configuration;
using System.Data;

namespace Insurance.Persistence.Repository;
public class LeadsRepository : ILeadsRepository
{
    private readonly ApplicationDBContext _context;
    private readonly IApplicationClaims _applicationClaims;

    /// <summary>
    /// lead details repository
    /// </summary>
    /// <param name="context"></param>
    /// <param name="applicationClaims"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public LeadsRepository(ApplicationDBContext context, IApplicationClaims applicationClaims)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
    }

    /// <summary>
    /// Get dashboard lead details
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<GetLeadManagementDetailModel> GetDashboardLeadDetails(GetLeadManagementDetailQuery request, CancellationToken cancellationToken)
    {
        request.UserId = !string.IsNullOrEmpty(request.UserId)? request.UserId : _applicationClaims.GetUserId();

        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@ViewLeadsType", !string.IsNullOrWhiteSpace(request.ViewLeadsType) ? request.ViewLeadsType.ToUpper() : "LEAD", DbType.String, ParameterDirection.Input);
        parameters.Add("@UserId", request.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("@SearchText", request.SearchText, DbType.String, ParameterDirection.Input);
        parameters.Add("@LeadType", request.LeadType, DbType.String, ParameterDirection.Input);
        parameters.Add("@POSPId", request.POSPId, DbType.String, ParameterDirection.Input);
        parameters.Add("@PolicyType", request.PolicyType, DbType.String, ParameterDirection.Input);
        parameters.Add("@PreQuote", request.PreQuote, DbType.String, ParameterDirection.Input);
        parameters.Add("@AllStatus", request.AllStatus, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", request.StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", request.EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", request.PageIndex, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", request.PageSize, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@IsFromDashboard", request.IsFromDashboard, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("@IsFromPaymentGatewayScreen", request.IsFromPaymentGatewayScreen, DbType.Boolean, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Insurance_GetDashboardLeadDetails]", parameters,
            commandType: CommandType.StoredProcedure);
        int totalRecords = 0;
        IEnumerable<LeadDetailModelList> leadDetailModelList = result.Read<LeadDetailModelList>();
        if (leadDetailModelList.Any())
        {
            totalRecords = leadDetailModelList.FirstOrDefault().TotalRecord;
        }
        GetLeadManagementDetailModel response = new()
        {
            LeadDetailModelList = leadDetailModelList,
            TotalRecords = totalRecords
        };
        return response;
    }
    /// <summary>
    /// get payment status master data
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<PaymentStatusListResponceModel>> GetPaymentStatusList(GetPaymentStatusListQuery request, CancellationToken cancellationToken)
    {

        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();

        var result = await connection.QueryAsync<PaymentStatusListResponceModel>("[dbo].[Insurance_GetPaymentStatusList]", parameters, commandType: CommandType.StoredProcedure);

        return result;

    }


}

