using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.TicketManagement.Queries.GetTicketManagementDetailById
{
    public class GetTicketManagementDetailByIdQuery : IRequest<HeroResult<IEnumerable<GetTicketManagementDetailByIdVm>>>
    {
        public string? TicketId { get; set; }
    }
    public class GetTicketManagementDetailByIdQueryHandler : IRequestHandler<GetTicketManagementDetailByIdQuery, HeroResult<IEnumerable<GetTicketManagementDetailByIdVm>>>
    {


        private readonly ITicketManagementRepository _ticketManagement;
        private readonly IMapper _mapper;


        public GetTicketManagementDetailByIdQueryHandler(ITicketManagementRepository ticketManagement, IMapper mapper)
        {
            _ticketManagement = ticketManagement;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetTicketManagementDetailByIdVm>>> Handle(GetTicketManagementDetailByIdQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _ticketManagement.GetTicketManagementDetailById(request.TicketId, cancellationToken);
            if (modelResult != null)
            {
                var listInsurerModel = _mapper.Map<IEnumerable<GetTicketManagementDetailByIdVm>>(modelResult);
                return HeroResult<IEnumerable<GetTicketManagementDetailByIdVm>>.Success(listInsurerModel);
            }
            return HeroResult<IEnumerable<GetTicketManagementDetailByIdVm>>.Fail("No Record Found");
        }
    }
}
