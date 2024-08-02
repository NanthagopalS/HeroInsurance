using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.Mmv.ResetMvvMappingForIcVariant
{
    public class ResetMvvMappingForIcVariantCommand : IRequest<HeroResult<bool>>
    {
        public string InsurerId { get; set; }
        public string HeroVariantId { get; set; }
        public string IcVariantId { get; set; }
    }
    public class ResetMvvMappingForIcVariantCommandHandler : IRequestHandler<ResetMvvMappingForIcVariantCommand, HeroResult<bool>>
    {
        private readonly IMmvRepository _immvRepository;
        private readonly IMapper _mapper;
        public ResetMvvMappingForIcVariantCommandHandler(IMmvRepository mmvRepository, IMapper mapper)
        {
            _immvRepository = mmvRepository ?? throw new ArgumentNullException(nameof(mmvRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<HeroResult<bool>> Handle(ResetMvvMappingForIcVariantCommand request, CancellationToken cancellationToken)
        {
            var HeroVariants = await _immvRepository.ResetMvvMappingForIcVariant(request, cancellationToken);
            var HeroVariantslist = _mapper.Map<bool>(HeroVariants);
            return HeroResult<bool>.Success(HeroVariantslist);
        }
    }
}
