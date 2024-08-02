using Insurance.Core.Features.Bajaj.Queries.GetQuote;
using Insurance.Domain.AllReportAndMIS;
using Insurance.Domain.GoDigit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Contracts.Persistence
{
    /// <summary>
    /// IReportAndMISRepository
    /// </summary>
    public interface IReportAndMISRepository
    {
        Task<AllReportAndMISResponseModel> AllReportAndMIS(AllReportAndMISRequestModel request, CancellationToken cancellationToken);
    }
}
