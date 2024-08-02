using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;

namespace POSP.Core.Features.POSP.Queries.GetPOSPCardDetail
{
    public class GetPOSPCardDetailQuery : IRequest<HeroResult<GetPOSPCardDetailVm>>
    {

    }
    public class GetPOSPCardDetailQueryHandler : IRequestHandler<GetPOSPCardDetailQuery, HeroResult<GetPOSPCardDetailVm>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPCardDetailQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }



        public async Task<HeroResult<GetPOSPCardDetailVm>> Handle(GetPOSPCardDetailQuery request, CancellationToken cancellationToken)
        {
            var userDetailResult = await _pospRepository.GetPOSPCardDetail(cancellationToken);
            var listInsurer = _mapper.Map<GetPOSPCardDetailVm>(userDetailResult);
            if (userDetailResult is not null)
            {
                return HeroResult<GetPOSPCardDetailVm>.Success(listInsurer);
            }

            return HeroResult<GetPOSPCardDetailVm>.Fail("No Record Found");
        }
    }
}
