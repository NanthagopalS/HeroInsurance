using Insurance.Core.Features.Leads;
using Insurance.Core.Features.Leads.GetPaymentStatus;
using Insurance.Domain.Leads;
namespace Insurance.Core.Contracts.Persistence;
public interface ILeadsRepository
{
    Task<GetLeadManagementDetailModel> GetDashboardLeadDetails(GetLeadManagementDetailQuery request, CancellationToken cancellationToken);
    Task<IEnumerable<PaymentStatusListResponceModel>> GetPaymentStatusList(GetPaymentStatusListQuery req, CancellationToken cancellationToken);
}
