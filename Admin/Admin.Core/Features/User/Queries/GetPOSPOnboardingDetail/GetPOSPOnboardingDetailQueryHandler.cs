using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetParticularPOSPDetailForIIBDashboard;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetPOSPOnboardingDetail
{
    public record GetPOSPOnboardingDetailQuery : IRequest<HeroResult<IEnumerable<GetPOSPOnboardingDetailVm>>>;
    public class GetPOSPOnboardingDetailQueryHandler : IRequestHandler<GetPOSPOnboardingDetailQuery, HeroResult<IEnumerable<GetPOSPOnboardingDetailVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPOnboardingDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPOnboardingDetailVm>>> Handle(GetPOSPOnboardingDetailQuery request, CancellationToken cancellationToken)
        {
            var iIBDashboard = await _userRepository.GetPOSPOnboardingDetail(cancellationToken).ConfigureAwait(false);
            if (iIBDashboard.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPOnboardingDetailVm>>(iIBDashboard);
                return HeroResult<IEnumerable<GetPOSPOnboardingDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPOnboardingDetailVm>>.Fail("No Record Found");
        }
    }
}
