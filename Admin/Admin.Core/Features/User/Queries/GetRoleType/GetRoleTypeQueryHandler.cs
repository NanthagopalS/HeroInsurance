using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetUserBreadcrumStatusDetail;
using Admin.Core.Features.User.Queries.GetUserDetail;
using Admin.Core.Features.User.Queries.UserDocument;
using Admin.Core.Features.User.Querries.GetMasterType;
using Admin.Domain.Roles;
using Admin.Domain.User;
using Admin.Core.Responses;
using MediatR;
using System.Threading;

namespace Admin.Core.Features.User.Queries.GetRoleType
{
    //RoleTypeResponseModel
    public record GetRoleTypeQuery : IRequest<HeroResult<IEnumerable<RoleTypeVm>>>;


    public class GetRoleTypeQueryHandler : IRequestHandler<GetRoleTypeQuery, HeroResult<IEnumerable<RoleTypeVm>>>
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
