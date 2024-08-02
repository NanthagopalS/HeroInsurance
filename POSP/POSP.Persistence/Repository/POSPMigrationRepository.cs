using Dapper;
using POSP.Core.Contracts.Common;
using POSP.Core.Contracts.Persistence;
using POSP.Domain.Migration;
using POSP.Persistence.Configuration;
using System.Data;

namespace POSP.Persistence.Repository
{
    public class POSPMigrationRepository : IPOSPMigrationRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IApplicationClaims _applicationClaims;

        public POSPMigrationRepository(ApplicationDBContext context, IApplicationClaims _claims)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _applicationClaims = _claims ?? throw new ArgumentNullException(nameof(_claims));
        }
        public async Task<POSPMigrationResponceModal> DumpExcelRecordsToDatabase(DataTable reqModel, CancellationToken cancellationToken)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@CreatedBy", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            parameters.Add("@TempTable", reqModel, DbType.Object, ParameterDirection.Input);
            var result = await connection.QueryAsync<POSPMigrationResponceModal>("[dbo].[POSP_POSPMigrationInsertion]", parameters,
                         commandType: CommandType.StoredProcedure);
            return result.First();
        }
    }
}
