using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetAllPOSPDetailForIIBDashboard;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetParticularPOSPDetailForIIBDashboard
{
    public class GetParticularPOSPDetailForIIBDashboardQuery : IRequest<HeroResult<IEnumerable<GetParticularPOSPDetailForIIBDashboardVm>>>
    {
        public string? UserId { get; set; }
    }
    internal class GetParticularPOSPDetailForIIBDashboardQueryHandler : IRequestHandler<GetParticularPOSPDetailForIIBDashboardQuery, HeroResult<IEnumerable<GetParticularPOSPDetailForIIBDashboardVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetParticularPOSPDetailForIIBDashboardQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetParticularPOSPDetailForIIBDashboardVm>>> Handle(GetParticularPOSPDetailForIIBDashboardQuery request, CancellationToken cancellationToken)
        {
            var particularPOSPDetailForIIBDashboard = await _userRepository.GetParticularPOSPDetailForIIBDashboard(request.UserId, cancellationToken).ConfigureAwait(false);
            if (particularPOSPDetailForIIBDashboard.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetParticularPOSPDetailForIIBDashboardVm>>(particularPOSPDetailForIIBDashboard);
                return HeroResult<IEnumerable<GetParticularPOSPDetailForIIBDashboardVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetParticularPOSPDetailForIIBDashboardVm>>.Fail("No Record Found");
        }

    }
}
