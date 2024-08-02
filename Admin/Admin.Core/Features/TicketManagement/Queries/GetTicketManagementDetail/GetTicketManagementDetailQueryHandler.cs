using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.TicketManagement.Queries.GetTicketManagementDetail
{
    public class GetTicketManagementDetailQuery : IRequest<HeroResult<GetTicketManagementDetailVm>>
    {
        public string? TicketType { get; set; }
        public string? SearchText { get; set; }
        public string? RelationshipManagerIds { get; set; }
        public string? PolicyType { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? CurrentPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
    }
    public class GetTicketManagementDetailQueryHandler : IRequestHandler<GetTicketManagementDetailQuery, HeroResult<GetTicketManagementDetailVm>>
    {


        private readonly ITicketManagementRepository _ticketManagement;
        private readonly IMapper _mapper;


        public GetTicketManagementDetailQueryHandler(ITicketManagementRepository ticketManagement, IMapper mapper)
        {
            _ticketManagement = ticketManagement;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetTicketManagementDetailVm>> Handle(GetTicketManagementDetailQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _ticketManagement.GetTicketManagementDetail(request.TicketType, request.SearchText, request.RelationshipManagerIds, request.PolicyType, request.StartDate, request.EndDate, request.CurrentPageIndex, request.CurrentPageSize, cancellationToken);
            if (modelResult != null)
            {
                var listInsurerModel = _mapper.Map<GetTicketManagementDetailVm>(modelResult);
                return HeroResult<GetTicketManagementDetailVm>.Success(listInsurerModel);
            }
            return HeroResult<GetTicketManagementDetailVm>.Fail("No Record Found");
        }
    }
}
