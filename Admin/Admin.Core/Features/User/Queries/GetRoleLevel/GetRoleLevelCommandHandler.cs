using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetRoleType;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Queries.GetRoleLevel
{
    public record GetRoleLevelCommand : IRequest<HeroResult<IEnumerable<RoleLevelVM>>>;
   
    public class GetRoleLevelCommandHandler : IRequestHandler<GetRoleLevelCommand, HeroResult<IEnumerable<RoleLevelVM>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetRoleLevelCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<RoleLevelVM>>> Handle(GetRoleLevelCommand request, CancellationToken cancellationToken)
        {
            var roleTypeResult = await _userRepository.GetRoleLevelDetails(cancellationToken).ConfigureAwait(false);
            if (roleTypeResult.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<RoleLevelVM>>(roleTypeResult);
                return HeroResult<IEnumerable<RoleLevelVM>>.Success(listInsurer);
            }
            return HeroResult<IEnumerable<RoleLevelVM>>.Fail("No Record Found");
        }

    }
}
