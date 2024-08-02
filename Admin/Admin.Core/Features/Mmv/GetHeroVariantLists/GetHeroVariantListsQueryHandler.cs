using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.Mmv.GetHeroVariantLists
{
    public class GetHeroVariantListsQuery : IRequest<HeroResult<GetHeroVariantListsQueryVm>>
    {
        public string InsurerId { get; set; }
        public string ModelId { get; set; }
        public string VariantIds { get; set; }
        public string FuelTypes { get; set; }
    }
    public class GetHeroVariantListsQueryHandler : IRequestHandler<GetHeroVariantListsQuery,HeroResult<GetHeroVariantListsQueryVm>>
    {
        private readonly IMmvRepository _immvRepository;
        private readonly IMapper _mapper;
        public GetHeroVariantListsQueryHandler(IMmvRepository mmvRepository, IMapper mapper)
        {
            _immvRepository = mmvRepository ?? throw new ArgumentNullException(nameof(mmvRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<HeroResult<GetHeroVariantListsQueryVm>> Handle(GetHeroVariantListsQuery request, CancellationToken cancellationToken)
        {
            var HeroVariants = await _immvRepository.GetHeroVariantLists(request, cancellationToken);
            if (HeroVariants is not null)
            {
                var HeroVariantslist = _mapper.Map<GetHeroVariantListsQueryVm>(HeroVariants);
                return HeroResult<GetHeroVariantListsQueryVm>.Success(HeroVariantslist);
            }
            return HeroResult<GetHeroVariantListsQueryVm>.Fail("No Record Found");
        }
    }
}
