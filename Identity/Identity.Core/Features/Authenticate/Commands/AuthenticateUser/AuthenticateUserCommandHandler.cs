using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;
using ThirdPartyUtilities.Abstraction;

namespace Identity.Core.Features.Authenticate;

public record AuthenticateUserCommand : IRequest<HeroResult<AuthenticateVm>>
{
    public string UserId { get; set; }
    public string OTP { get; set; }
}

public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, HeroResult<AuthenticateVm>>
{
    private readonly IAuthenticateRepository _authenticateRepository;
    private readonly IMapper _mapper;
    private readonly ICustomUtility _utility;

    public AuthenticateUserCommandHandler(IAuthenticateRepository authenticateRepository, IMapper mapper, ICustomUtility utility)
    {
        _authenticateRepository = authenticateRepository ?? throw new ArgumentNullException(nameof(authenticateRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _utility = utility ?? throw new ArgumentNullException(nameof(utility));
    }

    public async Task<HeroResult<AuthenticateVm>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        request.OTP = _utility.Base64DecodeForRequest(request.OTP);
        request.UserId = _utility.Base64DecodeForRequest(request.UserId);
        if (string.IsNullOrWhiteSpace(request.OTP) || string.IsNullOrWhiteSpace(request.UserId))
        {
            return HeroResult<AuthenticateVm>.Fail("Invalid OTP or UserId");
        }

        var authResponse = await _authenticateRepository.AuthenticateUser(request.UserId, request.OTP, cancellationToken);
        if (authResponse != null)
        {
            var result = _mapper.Map<AuthenticateVm>(authResponse);
            if (result.Token is null)
            {
                return HeroResult<AuthenticateVm>.Fail("Invalid OTP");
            }
            if (result.WrongOtpCount > 0)
            {
                return HeroResult<AuthenticateVm>.Fail("Invalid OTP");
            }
            return HeroResult<AuthenticateVm>.Success(result);
        }
        return HeroResult<AuthenticateVm>.Fail("Invalid OTP");
    }
}
