using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.HelpAndSupport.Queries.DeleteHelpAndSupport
{
    public class DeleteHelpAndSupportCommand : IRequest<HeroResult<bool>>
    {
        public string? RequestId { get; set; }
    }

    public class DeleteHelpAndSupportCommandHandler : IRequestHandler<DeleteHelpAndSupportCommand, HeroResult<bool>>
    {
        private readonly IHelpAndSupportRepository _helpSupportRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="helpSupportRepository"></param>
        /// <param name="mapper"></param>
        public DeleteHelpAndSupportCommandHandler(IHelpAndSupportRepository helpSupportRepository, IMapper mapper)
        {
            _helpSupportRepository = helpSupportRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(DeleteHelpAndSupportCommand request, CancellationToken cancellationToken)
        {
            var result = await _helpSupportRepository.DeleteHelpAndSupport(request.RequestId, cancellationToken);

            return HeroResult<bool>.Success(result);
        }
    }
}
