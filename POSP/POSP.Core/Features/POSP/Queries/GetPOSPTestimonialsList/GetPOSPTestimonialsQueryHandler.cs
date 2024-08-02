using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using ThirdPartyUtilities.Abstraction;

namespace POSP.Core.Features.POSP.Queries.GetFeedbackList
{
    public record GetPOSPTestimonialsQuery : IRequest<HeroResult<IEnumerable<GetPOSPTestimonialsVm>>>
    {

    }
    public class GetPOSPTestimonialsQueryHandler : IRequestHandler<GetPOSPTestimonialsQuery, HeroResult<IEnumerable<GetPOSPTestimonialsVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        private readonly IMongoDBService _mongodbService;

        /// <summary>
        /// Initialized
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="mongodbService"></param>
        public GetPOSPTestimonialsQueryHandler(IPOSPRepository pospRepository, IMapper mapper, IMongoDBService mongodbService)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<IEnumerable<GetPOSPTestimonialsVm>>> Handle(GetPOSPTestimonialsQuery request, CancellationToken cancellationToken)
        {
            var getPOSPProfileDetail = await _pospRepository.GetTestomonialLists(cancellationToken).ConfigureAwait(false);
            if (getPOSPProfileDetail.Any())
            {
                var pospDetails = _mapper.Map<IEnumerable<GetPOSPTestimonialsVm>>(getPOSPProfileDetail);
                foreach (var profileDetail in pospDetails)
                {
                    if (!string.IsNullOrEmpty(profileDetail.image))
                    {
                        profileDetail.imageBase64 = await _mongodbService.MongoDownloadTestomonialDoc(profileDetail.image);
                    }
                }
                return HeroResult<IEnumerable<GetPOSPTestimonialsVm>>.Success(pospDetails);
            }
            return HeroResult<IEnumerable<GetPOSPTestimonialsVm>>.Fail("No Record Found");
        }
    }
}
