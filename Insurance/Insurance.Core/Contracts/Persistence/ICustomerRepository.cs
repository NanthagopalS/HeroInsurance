using Insurance.Core.Features.Customer.Queries.GetCustomersList;
using Insurance.Core.Features.Customer.Queries.GetRenewalDetailsById;
using Insurance.Domain.Customer;

namespace Insurance.Core.Contracts.Persistence
{
    public interface ICustomerRepository
    {
            Task<GetCustomersResponseModel> GetCustomersList(GetCustomersListQuery getCustomersListRequest, CancellationToken cancellationToken);
            Task<GetRenewalDetailsByIdResponceModel> GetRenewalDetailsById(GetRenewalDetailsByIdQuery GetCustomersDetailInputModel, CancellationToken cancellationToken);
            Task<GetCustomerLeadDetailModel> GetParticularLeadDetailById(string LeadId, CancellationToken cancellationToken);
    }
}
