using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using MediatR;

namespace Insurance.Core.Features.InsuranceMaster.Queries.GetFinancier
{
    public record GetFinancierQuery : IRequest<HeroResult<IEnumerable<FinancierModel>>>
    {

    }
    public class GetFinancierQueryHandler : IRequestHandler<GetFinancierQuery, HeroResult<IEnumerable<FinancierModel>>>
    {
        private readonly IInsuranceMasterRepository _insuranceRepository;
        private readonly IMapper _mapper;

        public GetFinancierQueryHandler(IInsuranceMasterRepository insuranceRepository, IMapper mapper)
        {
            _insuranceRepository = insuranceRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<FinancierModel>>> Handle(GetFinancierQuery request, CancellationToken cancellationToken)
        {
            var cityResult = await _insuranceRepository.GetFinancier(cancellationToken).ConfigureAwait(false);
            if (cityResult.Any())
            {
                var listCity = _mapper.Map<IEnumerable<FinancierModel>>(cityResult);
                return HeroResult<IEnumerable<FinancierModel>>.Success(listCity);
            }

            return HeroResult<IEnumerable<FinancierModel>>.Fail("No Record Found");

        }
    }
}
