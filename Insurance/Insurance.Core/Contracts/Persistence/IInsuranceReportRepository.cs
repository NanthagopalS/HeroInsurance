using Insurance.Core.Features.AllReportAndMIS.Query.BusinessSummery;
using Insurance.Core.Features.AllReportAndMIS.RequestandResponseH;
using Insurance.Domain.AllReportAndMIS.BusinessSummery;
using Insurance.Domain.AllReportAndMIS.BusinessSummerym;

namespace Insurance.Core.Contracts.Persistence
{
    /// <summary>
    /// interface for insurance report repository
    /// </summary>
    public interface IInsuranceReportRepository
    {
        Task<BusinessSummeryResponceModel> BusinessSummeryReport(BusunessSummeryQuery requestModel, CancellationToken cancellationToken);
        Task<RequestandResponseModel> RequestandResponseReport(RequestandResponseQuery requestandResponseQuery,CancellationToken cancellationToken);
    }
}
