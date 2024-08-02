using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetLeadOverview
{
    public record GetLeadOverviewQuery : IRequest<HeroResult<IEnumerable<GetLeadOverviewVm>>>
    {
        public string? LeadType { get; set; }
        public string? UserId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

    }
    public class GetLeadOverviewQueryHandler : IRequestHandler<GetLeadOverviewQuery, HeroResult<IEnumerable<GetLeadOverviewVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetLeadOverviewQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetLeadOverviewVm>>> Handle(GetLeadOverviewQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _userRepository.GetLeadOverview(request.LeadType, request.UserId, request.StartDate, request.EndDate, cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listUserRoleModel = _mapper.Map<IEnumerable<GetLeadOverviewVm>>(modelResult);
                return HeroResult<IEnumerable<GetLeadOverviewVm>>.Success(listUserRoleModel);
            }
            return HeroResult<IEnumerable<GetLeadOverviewVm>>.Fail("No Record Found");
        }
    }
}
