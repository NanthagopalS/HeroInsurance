using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Admin.Core.Features.HelpAndSupport.RaiseRequest
{
    public class RaiseRequestCommand : IRequest<HeroResult<bool>>
    {
        public string? ConcernTypeId { get; set; }
        public string? SubConcernTypeId { get; set; }
        public string? SubjectText { get; set; }
        public string? DetailText { get; set; }
        public string? DocumentId { get; set; }
        public string? UserId { get; set; }
    }
    public class RaiseRequestCommandHandler : IRequestHandler<RaiseRequestCommand, HeroResult<bool>>
    {
        private readonly IHelpAndSupportRepository _IHelpAndSupportRepository;
        private readonly IMapper _mapper;

        public RaiseRequestCommandHandler(IHelpAndSupportRepository helpAndSupportRepository, IMapper mapper)
        {
            _IHelpAndSupportRepository = helpAndSupportRepository ?? throw new ArgumentNullException(nameof(helpAndSupportRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(RaiseRequestCommand command, CancellationToken cancellationToken)
        {
            var result = await _IHelpAndSupportRepository.InsertRaiseRequest(command, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
