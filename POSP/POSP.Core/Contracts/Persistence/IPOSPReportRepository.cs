using POSP.Core.Features.Reports;
using POSP.Domain.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Contracts.Persistence
{
    /// <summary>
    /// interface repository for POSP Report
    /// </summary>
    public interface IPOSPReportRepository
    {
        Task<NewAndOldPOSPReportResponceModel> NewAndOldPOSPReport(NewAndOldPOSPReportQuery pOSPManagementInputModel, CancellationToken cancellationToken);
    }
}
