using Insurance.Domain.APILogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Contracts.Persistence;

public interface ILogsRepository
{
	Task<int> InsertAPILogs(APILogsModel apiLogsModel);
}
