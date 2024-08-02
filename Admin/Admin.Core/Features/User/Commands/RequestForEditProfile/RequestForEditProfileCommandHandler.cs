using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.BUUpdate;
using Admin.Domain.Roles;
using Admin.Core.Responses;
using MediatR;


namespace Admin.Core.Features.User.Commands.BUInsert
{
    public class RequestForEditProfileCommand : IRequest<HeroResult<bool>>
    {
        public string? UserId { get; set; }
        public string? RequestType { get; set; }
        public string? NewRequestTypeContent { get; set; }
        public string? CreatedBy { get; set; }
    }

    public class RequestForEditProfileCommandHandler : IRequestHandler<RequestForEditProfileCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public RequestForEditProfileCommandHandler(IUserRepository buRepository, IMapper mapper)
        {
            _userRepository = buRepository ?? throw new ArgumentNullException(nameof(buRepository));
            _mapper = mapper;
        }
        public async Task<HeroResult<bool>> Handle(RequestForEditProfileCommand cmd, CancellationToken cancellationToken)
        {
            var inputModel = _mapper.Map<RequestForEditProfileInputModel>(cmd);
            var result = await _userRepository.RequestForEditProfile(inputModel, cancellationToken);
            return HeroResult<bool>.Success(result);
        }

    }

}
