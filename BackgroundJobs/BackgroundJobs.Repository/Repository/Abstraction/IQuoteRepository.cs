using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackgroundJobs.Repository.Repository.Abstraction
{
    public interface IQuoteRepository
    {
        public Task MoveQuoteDataToArchive();
    }
}
