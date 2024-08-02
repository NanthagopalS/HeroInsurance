using Dapper;
using System.Data;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Configuration;
using ThirdPartyUtilities.Models.Log;

namespace ThirdPartyUtilities.Implementation;

public class LogService : ILogService
{
    private readonly SignzyLogsDBContext _signzyLogsDBContext;
    public LogService(SignzyLogsDBContext signzyLogsDBContext)
    {
        _signzyLogsDBContext = signzyLogsDBContext ?? throw new ArgumentNullException(nameof(signzyLogsDBContext));
    }

    public async Task<int> InsertLog(LogModel logsModel)
    {
        using var connection = _signzyLogsDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("InsurerId", logsModel.InsurerId, DbType.String, ParameterDirection.Input);
        parameters.Add("RequestBody", logsModel.RequestBody, DbType.String, ParameterDirection.Input);
        parameters.Add("Headers", logsModel.Headers, DbType.String, ParameterDirection.Input);
        parameters.Add("Token", logsModel.Token, DbType.String, ParameterDirection.Input);
        parameters.Add("API", logsModel.API, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", logsModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadId", logsModel.LeadId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<int>("[dbo].[InsuranceLogs_InsertLogs]", parameters, commandType: CommandType.StoredProcedure);
        return result.FirstOrDefault();
    }

    public async Task UpdateLog(LogModel logsModel)
    {
        using var connection = _signzyLogsDBContext.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("Id", logsModel.Id, DbType.String, ParameterDirection.Input);
        parameters.Add("ResponseBody", logsModel.ResponseBody, DbType.String, ParameterDirection.Input);
        parameters.Add("ResponseTime", logsModel.ResponseTime, DbType.DateTime, ParameterDirection.Input);
        parameters.Add("ApplicationId", logsModel.ApplicationId, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[InsuranceLogs_UpdateLogs]", parameters, commandType: CommandType.StoredProcedure);
    }

}
