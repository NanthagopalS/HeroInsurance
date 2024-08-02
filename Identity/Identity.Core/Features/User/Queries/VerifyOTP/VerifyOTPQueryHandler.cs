using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User;

public record GetVerifyOTPQuery : IRequest<HeroResult<bool>>
{
    public string UserId { get; set; }
    public string OTP { get; set; }
}

public class VerifyOTPQueryHandler : IRequestHandler<GetVerifyOTPQuery, HeroResult<bool>>
{
    private readonly IUserRepository _userRepository;

    public VerifyOTPQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<HeroResult<bool>> Handle(GetVerifyOTPQuery request, CancellationToken cancellationToken)
    {
        var isValidOTP = await _userRepository.VerifyOTP(request.OTP, request.UserId);
        return HeroResult<bool>.Success(isValidOTP);
    }
}
