using Identity.Domain.APILogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Contracts.Persistence;

public interface ILogRepository
{
	Task<int> InsertAPILogs(APILogsModel apiLogsModel);
}
