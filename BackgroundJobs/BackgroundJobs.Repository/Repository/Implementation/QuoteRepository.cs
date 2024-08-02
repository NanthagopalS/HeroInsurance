using BackgroundJobs.Repository.Models;
using BackgroundJobs.Repository.Repository.Abstraction;
using BackgroundJobs.Configuration;
using System.Data;
using Dapper;

namespace BackgroundJobs.Repository.Repository.Implementation
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly ApplicationDBContext _context;
        public QuoteRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task MoveQuoteDataToArchive()
        {
            using var connection = _context.CreateConnection();
                var result = await connection.ExecuteAsync("[dbo].[Insurance_MoveQuoteDataToArchive]",
                                                      commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        }
    }
}
