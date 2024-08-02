using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;


namespace Insurance.Core.Features.InsuranceMaster.Queries.GetInsuranceMaster
{
    public class GetInsuranceMasterQuery : IRequest<HeroResult<IEnumerable<GetInsuranceMasterVm>>>
    {
    }

    public class GetInsuranceMasterQueryHandler : IRequestHandler<GetInsuranceMasterQuery, HeroResult<IEnumerable<GetInsuranceMasterVm>>>
    {
        private readonly IInsuranceMasterRepository _quoteRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        public GetInsuranceMasterQueryHandler(IInsuranceMasterRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<GetInsuranceMasterVm>>> Handle(GetInsuranceMasterQuery request, CancellationToken cancellationToken)
        {
            var insuranceModel = await _quoteRepository.GetInsuranceMaster(cancellationToken).ConfigureAwait(false);
            if (insuranceModel == null)
            {
                return HeroResult<IEnumerable<GetInsuranceMasterVm>>.Fail("No record found");
            }

            var insuranceMappedList = _mapper.Map<IEnumerable<GetInsuranceMasterVm>>(insuranceModel);
            return HeroResult<IEnumerable<GetInsuranceMasterVm>>.Success(insuranceMappedList);
        }
    }
}