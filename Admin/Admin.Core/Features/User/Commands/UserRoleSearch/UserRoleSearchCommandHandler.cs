using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.User;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Commands.UserRoleSearch
{
    public record UserRoleSearchCommand : IRequest<HeroResult<IEnumerable<UserRoleVM>>>
    {
        public string EMPId { get; set; }
        public string RoleTypeName { get; set; }
        public string Status { get; set; }
        public string CreatedFrom { get; set; }
        public string CreatedTo { get; set; }
    }
    public class UserRoleSearchCommandHandler : IRequestHandler<UserRoleSearchCommand, HeroResult<IEnumerable<UserRoleVM>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public UserRoleSearchCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<UserRoleVM>>> Handle(UserRoleSearchCommand request, CancellationToken cancellationToken)
        {
            var roleSearchInput = _mapper.Map<UserRoleSearchInputModel>(request);
            var modelResult = await _userRepository.GetUserRoleMapping(roleSearchInput, cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listInsurerModel = _mapper.Map<IEnumerable<UserRoleVM>>(modelResult);
                return HeroResult<IEnumerable<UserRoleVM>>.Success(listInsurerModel);
            }
            return HeroResult<IEnumerable<UserRoleVM>>.Fail("No Record Found");
        }

    }
}