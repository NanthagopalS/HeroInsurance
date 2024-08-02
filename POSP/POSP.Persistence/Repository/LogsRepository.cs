using Dapper;
using POSP.Core.Contracts.Persistence;
using POSP.Domain.APILogs;
using POSP.Persistence.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Persistence.Repository;

public class LogsRepository : ILogsRepository
{
	private readonly LogsDBContext _logContext;
	public LogsRepository(LogsDBContext logsDBContext)
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
