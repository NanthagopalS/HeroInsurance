using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.TicketManagement.Command
{
    public class UpdateTicketManagementDetailByIdCommand : IRequest<HeroResult<bool>>
    {
        public string? TicketId { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
    }
    public class UpdateTicketManagementDetailByIdCommandHandler : IRequestHandler<UpdateTicketManagementDetailByIdCommand, HeroResult<bool>>
    {
        private readonly ITicketManagementRepository _iTicketManagementRepository;
        private readonly IMapper _mapper;

        public UpdateTicketManagementDetailByIdCommandHandler(ITicketManagementRepository iTicketManagementRepository, IMapper mapper)
        {
            _iTicketManagementRepository = iTicketManagementRepository ?? throw new ArgumentNullException(nameof(iTicketManagementRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(UpdateTicketManagementDetailByIdCommand command, CancellationToken cancellationToken)
        {
            var result = await _iTicketManagementRepository.UpdateTicketManagementDetailById(command.TicketId, command.Description, command.Status, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
