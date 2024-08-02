using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.User.Queries.GetParticularBUDetail
{
    public class GetParticularHelpAndSupportQuery : IRequest<HeroResult<IEnumerable<GetParticularHelpAndSupportVm>>>
    {
        public string? RequestId { get; set; }
    }
    public class GetParticularHelpAndSupportQueryHandler : IRequestHandler<GetParticularHelpAndSupportQuery, HeroResult<IEnumerable<GetParticularHelpAndSupportVm>>>
    {
        private readonly IHelpAndSupportRepository _HelpAndSupportRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="helpAndSupportRepository"></param>
        /// <param name="mapper"></param>
        public GetParticularHelpAndSupportQueryHandler(IHelpAndSupportRepository helpAndSupportRepository, IMapper mapper)
        {
            _HelpAndSupportRepository = helpAndSupportRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetParticularHelpAndSupportVm>>> Handle(GetParticularHelpAndSupportQuery request, CancellationToken cancellationToken)
        {
            var particularBuDetail = await _HelpAndSupportRepository.GetParticularHelpAndSupport(request.RequestId, cancellationToken);
            if (particularBuDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetParticularHelpAndSupportVm>>(particularBuDetail);
                return HeroResult<IEnumerable<GetParticularHelpAndSupportVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetParticularHelpAndSupportVm>>.Fail("No Record Found");
        }

    }
}
