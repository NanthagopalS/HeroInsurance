using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Commands.UserRoleGetMapping
{
    public class GetParticularRoleDetail : IRequest<HeroResult<IEnumerable<ParticularRoleDetailVM>>>
    {
        public string? RoleName { get; set; }
        public string? RoleTypeId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? PageIndex { get; set; }
    }
    public class GetParticularRoleDetailHandler : IRequestHandler<GetParticularRoleDetail, HeroResult<IEnumerable<ParticularRoleDetailVM>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetParticularRoleDetailHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<ParticularRoleDetailVM>>> Handle(GetParticularRoleDetail request, CancellationToken cancellationToken)
        {
            var modelResult = await _userRepository.GetParticularRoleDetail(request.RoleTypeId, cancellationToken).ConfigureAwait(false);

            if (modelResult.Any())
            {
                var listUserRoleModel = _mapper.Map<IEnumerable<ParticularRoleDetailVM>>(modelResult);
                return HeroResult<IEnumerable<ParticularRoleDetailVM>>.Success(listUserRoleModel);
            }
            return HeroResult<IEnumerable<ParticularRoleDetailVM>>.Fail("No Record Found");
        }

    }
}
