using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.TicketManagement.Queries.GetDeactivationTicketManagementDetail
{
    public class GetDeactivationTicketManagementDetailQuery : IRequest<HeroResult<GetDeactivationTicketManagementDetailVm>>
    {
        public string? SearchText { get; set; }
        public string? RelationshipManagerId { get; set; }
        public string? PolicyType { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? CurrentPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
    }
    public class GetDeactivationTicketManagementDetailQueryHandler : IRequestHandler<GetDeactivationTicketManagementDetailQuery, HeroResult<GetDeactivationTicketManagementDetailVm>>
    {


        private readonly ITicketManagementRepository _ticketManagement;
        private readonly IMapper _mapper;


        public GetDeactivationTicketManagementDetailQueryHandler(ITicketManagementRepository ticketManagement, IMapper mapper)
        {
            _ticketManagement = ticketManagement;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetDeactivationTicketManagementDetailVm>> Handle(GetDeactivationTicketManagementDetailQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _ticketManagement.GetDeactivationTicketManagementDetail(request.SearchText, request.RelationshipManagerId, request.PolicyType, request.StartDate, request.EndDate, request.CurrentPageIndex, request.CurrentPageSize, cancellationToken);
            if (modelResult != null)
            {
                var listInsurerModel = _mapper.Map<GetDeactivationTicketManagementDetailVm>(modelResult);
                return HeroResult<GetDeactivationTicketManagementDetailVm>.Success(listInsurerModel);
            }
            return HeroResult<GetDeactivationTicketManagementDetailVm>.Fail("No Record Found");
        }
    }
}
