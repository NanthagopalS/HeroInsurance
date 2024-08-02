using Admin.Domain.Notification;
using Admin.Domain.TicketManagement;
using Admin.Domain.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Contracts.Persistence
{
    public interface ITicketManagementRepository
    {
        Task<GetTicketManagementDetailResponseModel> GetTicketManagementDetail(string? TicketType, string? SearchText, string? RelationshipManagerIds, string? PolicyType, string? StartDate, string? EndDate, int? CurrentPageIndex, int? CurrentPageSize, CancellationToken cancellationToken);
        Task<IEnumerable<GetTicketManagementDetailByIdModel>> GetTicketManagementDetailById(string? TicketId, CancellationToken cancellationToken);
        Task<IEnumerable<GetPOSPDetailsByIDToDeActivateResponseModel>> GetPOSPDetailsByIDToDeActivate(string? POSPId, CancellationToken cancellationToken);
        Task<GetDeactivationTicketManagementDetailResponseModel> GetDeactivationTicketManagementDetail(string? SearchText, string? RelationshipManagerId, string? PolicyType, string? StartDate, string? EndDate, int? CurrentPageIndex, int? CurrentPageSize, CancellationToken cancellationToken);
        Task<bool> UpdateTicketManagementDetailById(string? TicketId, string? Description,string? Status, CancellationToken cancellationToken);
        Task<IEnumerable<GetPOSPDetailsByDeactiveTicketIdResponceModel>> GetPOSPDetailsByDeactiveTicketId(string? POSPId, CancellationToken cancellationToken);

    }
}