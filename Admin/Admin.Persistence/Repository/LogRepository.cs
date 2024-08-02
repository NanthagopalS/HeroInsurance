using Admin.Core.Contracts.Persistence;
using Admin.Domain.APILogs;
using Admin.Persistence.Configuration;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Persistence.Repository;

public class LogRepository : ILogRepository
{
	private readonly LogsDBContext _logContext;
    public LogRepository(LogsDBContext logsDBContext)
    {
		_logContext = logsDBContext ?? throw new ArgumentNullException(nameof(logsDBContext));
	}
    public async Task<int> InsertAPILogs(APILogsModel apiLogsModel)
	{
		using var connection = _logContext.CreateConnection();
		var parameters = new DynamicParameters();
		parameters.Add("RequestMethod", apiLogsModel.RequestMethod, DbType.String, ParameterDirection.Input);
		parameters.Add("RequestPath", apiLogsModel.RequestPath, DbType.String, ParameterDirection.Input);
		parameters.Add("RequestUrl", apiLogsModel.RequestUrl, DbType.String, ParameterDirection.Input);
		parameters.Add("RequestBody", apiLogsModel.RequestBody, DbType.String, ParameterDirection.Input);
		parameters.Add("ResponseStatusCode", apiLogsModel.ResponseStatusCode, DbType.String, ParameterDirection.Input);
		parameters.Add("ResponseBody", apiLogsModel.ResponseBody, DbType.String, ParameterDirection.Input);
		parameters.Add("UserId", apiLogsModel.UserId, DbType.String, ParameterDirection.Input);
		
		var result = await connection.ExecuteAsync("[dbo].[HeroLogs_InsertLogs]", parameters, commandType: CommandType.StoredProcedure);
		return result;
	}
}
