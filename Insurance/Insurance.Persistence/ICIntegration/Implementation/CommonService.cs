using Dapper;
using Insurance.Domain.Quote;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Abstraction;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Insurance.Persistence.ICIntegration.Implementation;

public class CommonService : ICommonService
{
    private readonly LogsDBContext _logContext;
    public CommonService(LogsDBContext logsDBContext)
    {
        _logContext = logsDBContext ?? throw new ArgumentNullException(nameof(logsDBContext));
    }

    public async Task<int> InsertLogs(LogsModel logsModel)
    {
        using var connection = _logContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("InsurerId", logsModel.InsurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("RequestBody", logsModel.RequestBody, DbType.String, ParameterDirection.Input);
        parameters.Add("Headers", logsModel.Headers, DbType.String, ParameterDirection.Input);
        parameters.Add("Token", logsModel.Token, DbType.String, ParameterDirection.Input);
        parameters.Add("API", logsModel.API, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", logsModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadId", logsModel.LeadId, DbType.String, ParameterDirection.Input);
        parameters.Add("Stage", logsModel.Stage, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<int>("[dbo].[InsuranceLogs_InsertLogs]", parameters, commandType: CommandType.StoredProcedure);
        return result.FirstOrDefault();
    }

    public async Task UpdateLogs(LogsModel logsModel)
    {
        using var connection = _logContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("Id", logsModel.Id, DbType.String, ParameterDirection.Input);
        parameters.Add("ResponseBody", logsModel.ResponseBody, DbType.String, ParameterDirection.Input);
        parameters.Add("ResponseTime", logsModel.ResponseTime, DbType.DateTime, ParameterDirection.Input);
        parameters.Add("ApplicationId", logsModel.ApplicationId, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[InsuranceLogs_UpdateLogs]", parameters, commandType: CommandType.StoredProcedure);
    }
    public async Task<int> InsertQuoteLogs(LogsModel logsModel)
    {
        using var connection = _logContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("InsurerId", logsModel.InsurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("RequestBody", logsModel.RequestBody, DbType.String, ParameterDirection.Input);
        parameters.Add("ResponseBody", logsModel.ResponseBody, DbType.String, ParameterDirection.Input);
        parameters.Add("Headers", logsModel.Headers, DbType.String, ParameterDirection.Input);
        parameters.Add("Token", logsModel.Token, DbType.String, ParameterDirection.Input);
        parameters.Add("API", logsModel.API, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", logsModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadId", logsModel.LeadId, DbType.String, ParameterDirection.Input);
        parameters.Add("Stage", logsModel.Stage, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<int>("[dbo].[InsuranceLogs_InsertQuoteLogs]", parameters, commandType: CommandType.StoredProcedure);
        return result.FirstOrDefault();
    }
}
