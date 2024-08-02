using Admin.Domain.APILogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPartyUtilities.Implementation;

namespace Admin.Core.Contracts.Persistence;

public interface ILogRepository
{
	Task<int> InsertAPILogs(APILogsModel apiLogsModel);
}
