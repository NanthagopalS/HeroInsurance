using POSP.Domain.APILogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Contracts.Persistence;

public interface ILogsRepository
{
	Task<int> InsertAPILogs(APILogsModel apiLogsModel);
}
