using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetLeadStage;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetAllSharedReportingRole
{
    public record GetAllSharedReportingRoleQuery : IRequest<HeroResult<IEnumerable<GetAllSharedReportingRoleVm>>> { }
    internal class GetAllSharedReportingRoleQueryHandler : IRequestHandler<GetAllSharedReportingRoleQuery, HeroResult<IEnumerable<GetAllSharedReportingRoleVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetAllSharedReportingRoleQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetAllSharedReportingRoleVm>>> Handle(GetAllSharedReportingRoleQuery request, CancellationToken cancellationToken)
        {
            var leadType = await _userRepository.GetAllSharedReportingRole(cancellationToken);
            if (leadType.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetAllSharedReportingRoleVm>>(leadType);
                return HeroResult<IEnumerable<GetAllSharedReportingRoleVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetAllSharedReportingRoleVm>>.Fail("No Record Found");
        }

    }
}
