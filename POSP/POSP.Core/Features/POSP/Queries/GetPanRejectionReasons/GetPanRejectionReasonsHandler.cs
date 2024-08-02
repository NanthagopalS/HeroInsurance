using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;

namespace POSP.Core.Features.POSP.Queries.GetPanRejectionReasons
{
    public record GetPanRejectionReasonsQuery : IRequest<HeroResult<IEnumerable<GetPanRejectionReasonsVm>>>
    {

    }

    public class GetPanRejectionReasonsHandler : IRequestHandler<GetPanRejectionReasonsQuery, HeroResult<IEnumerable<GetPanRejectionReasonsVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;

        public GetPanRejectionReasonsHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<GetPanRejectionReasonsVm>>> Handle(GetPanRejectionReasonsQuery getPanRejectionReasonsQuery, CancellationToken cancellationToken)
        {
            var result = await _pospRepository.GetPanRejectionReasons(getPanRejectionReasonsQuery, cancellationToken);
            if (result is not null)
            {
                var RejectionsList = _mapper.Map<IEnumerable<GetPanRejectionReasonsVm>>(result);
                return HeroResult<IEnumerable<GetPanRejectionReasonsVm>>.Success(RejectionsList);
            }
            return HeroResult<IEnumerable<GetPanRejectionReasonsVm>>.Fail("Failed to get records");
        }
    }
}
