using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Queries.GetUserBreadcrumStatusDetail;
using Identity.Core.Features.User.Queries.GetUserDetail;
using Identity.Core.Features.User.Queries.UserDocument;
using Identity.Core.Features.User.Querries.GetMasterType;
using Identity.Domain.Roles;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;
using System.Threading;

namespace Identity.Core.Features.User.Queries.GetRoleType
{
    //RoleTypeResponseModel
    public record GetRoleTypeQuery : IRequest<HeroResult<IEnumerable<RoleTypeVm>>>;


    internal class GetRoleTypeQueryHandler : IRequestHandler<GetRoleTypeQuery, HeroResult<IEnumerable<RoleTypeVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetRoleTypeQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<RoleTypeVm>>> Handle(GetRoleTypeQuery request, CancellationToken cancellationToken)
        {
            var roleTypeResult = await _userRepository.GetRoleTypeDetails(cancellationToken).ConfigureAwait(false);
            if (roleTypeResult.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<RoleTypeVm>>(roleTypeResult);
                return HeroResult<IEnumerable<RoleTypeVm>>.Success(listInsurer);
            }
            return HeroResult<IEnumerable<RoleTypeVm>>.Fail("No Record Found");
        }       

    }
}
