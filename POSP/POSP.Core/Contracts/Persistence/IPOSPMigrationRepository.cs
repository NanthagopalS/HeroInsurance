using POSP.Domain.Migration;
using System.Data;

namespace POSP.Core.Contracts.Persistence
{
    public interface IPOSPMigrationRepository
    {
        Task<POSPMigrationResponceModal> DumpExcelRecordsToDatabase(DataTable reqModel, CancellationToken cancellationToken);
    }
}
