using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetParticularRoleDetail
{
    public class GetParticularRoleDetailQuery : IRequest<HeroResult<IEnumerable<GetParticularRoleDetailVm>>>
    {
        public string RoleId { get; set; }

    }

    public class GetParticularRoleDetailQueryHandler : IRequestHandler<GetParticularRoleDetailQuery, HeroResult<IEnumerable<GetParticularRoleDetailVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetParticularRoleDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetParticularRoleDetailVm>>> Handle(GetParticularRoleDetailQuery request, CancellationToken cancellationToken)
        {
           // var particularbuDetailMapInput = _mapper.Map<UpdateUserRoleMappingDetailModel>(request);
            var getParticularRoleDetail = await _userRepository.GetParticularRoleDetail(request.RoleId, cancellationToken).ConfigureAwait(false);
            if (getParticularRoleDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetParticularRoleDetailVm>>(getParticularRoleDetail);
                return HeroResult<IEnumerable<GetParticularRoleDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetParticularRoleDetailVm>>.Fail("No Record Found");
        }
    }
}
