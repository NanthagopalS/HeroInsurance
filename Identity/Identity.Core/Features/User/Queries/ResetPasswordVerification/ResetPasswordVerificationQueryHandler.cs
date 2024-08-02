using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User;


public record ResetPasswordVerificationQuery : IRequest<HeroResult<bool>>
{
    public string UserId { get; set; }
    public string GuId { get; set; }
}

public class ResetPasswordVerificationQueryHandler : IRequestHandler<ResetPasswordVerificationQuery, HeroResult<bool>>
{
    private readonly IUserRepository _userRepository;

    public ResetPasswordVerificationQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<HeroResult<bool>> Handle(ResetPasswordVerificationQuery request, CancellationToken cancellationToken)
    {
        var isValidEmailLink = await _userRepository.ResetPasswordVerification(request.GuId, request.UserId);
        return HeroResult<bool>.Success(isValidEmailLink);
    }
}
