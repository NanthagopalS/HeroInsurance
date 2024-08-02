using ThirdPartyUtilities.Models.Log;

namespace ThirdPartyUtilities.Abstraction;

public interface ILogService
{
    Task<int> InsertLog(LogModel logsModel);
    Task UpdateLog(LogModel logsModel);
}
