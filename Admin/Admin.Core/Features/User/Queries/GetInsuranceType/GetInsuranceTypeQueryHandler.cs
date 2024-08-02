using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetFunnelChart;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetInsuranceType
{
    public record GetInsuranceTypeQuery : IRequest<HeroResult<IEnumerable<GetInsuranceTypeVm>>>{}
    public class GetInsuranceTypeQueryHandler : IRequestHandler<GetInsuranceTypeQuery, HeroResult<IEnumerable<GetInsuranceTypeVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetInsuranceTypeQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetInsuranceTypeVm>>> Handle(GetInsuranceTypeQuery request, CancellationToken cancellationToken)
        {
            var insuranceType = await _userRepository.GetInsuranceType(cancellationToken);
            if (insuranceType.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetInsuranceTypeVm>>(insuranceType);
                return HeroResult<IEnumerable<GetInsuranceTypeVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetInsuranceTypeVm>>.Fail("No Record Found");
        }
    }
}
