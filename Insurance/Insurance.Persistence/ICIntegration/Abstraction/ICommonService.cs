using Insurance.Domain.Quote;

namespace Insurance.Persistence.ICIntegration.Abstraction;

public interface ICommonService
{
    Task<int> InsertLogs(LogsModel logsModel);
    Task UpdateLogs(LogsModel logsModel);
    Task<int> InsertQuoteLogs(LogsModel logsModel);
}
