using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetBuDetail;
using Admin.Domain.Roles;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetAllBUUser
{
    public record GetAllBUUserRoleQuery : IRequest<HeroResult<IEnumerable<GetAllBUUserRoleVm>>>
    {
        public string? BUId { get; set; }
    }
    internal class GetAllBUUserRoleQueryHandler : IRequestHandler<GetAllBUUserRoleQuery, HeroResult<IEnumerable<GetAllBUUserRoleVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetAllBUUserRoleQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetAllBUUserRoleVm>>> Handle(GetAllBUUserRoleQuery request, CancellationToken cancellationToken)
        {
            var getBuDetail = await _userRepository.GetAllBUUserRole(request, cancellationToken);
            if (getBuDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetAllBUUserRoleVm>>(getBuDetail);
                return HeroResult<IEnumerable<GetAllBUUserRoleVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetAllBUUserRoleVm>>.Fail("No Record Found");
        }
    }
}
