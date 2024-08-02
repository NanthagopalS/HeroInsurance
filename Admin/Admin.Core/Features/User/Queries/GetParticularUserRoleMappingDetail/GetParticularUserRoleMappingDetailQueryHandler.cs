using Admin.Core.Contracts.Persistence;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetParticularUserRoleMappingDetail
{
    public class GetParticularUserRoleMappingDetailQuery : IRequest<HeroResult<IEnumerable<GetParticularUserRoleMappingDetailVm>>>
    {
        public string UserRoleMappingId { get; set; }

    }

    public class GetParticularUserRoleMappingDetailQueryHandler : IRequestHandler<GetParticularUserRoleMappingDetailQuery, HeroResult<IEnumerable<GetParticularUserRoleMappingDetailVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetParticularUserRoleMappingDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetParticularUserRoleMappingDetailVm>>> Handle(GetParticularUserRoleMappingDetailQuery request, CancellationToken cancellationToken)
        {
            //var particularbuDetailMapInput = _mapper.Map<GetParticularUserRoleMappingDetailModel>(request);
            var particularBuDetail = await _userRepository.GetParticularUserRoleMappingDetail(request.UserRoleMappingId, cancellationToken).ConfigureAwait(false);
            if (particularBuDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetParticularUserRoleMappingDetailVm>>(particularBuDetail);
                return HeroResult<IEnumerable<GetParticularUserRoleMappingDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetParticularUserRoleMappingDetailVm>>.Fail("No Record Found");
        }
    }
}
