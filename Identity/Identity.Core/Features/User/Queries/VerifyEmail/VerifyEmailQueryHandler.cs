using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User;


public record GetVerifyEmailQuery : IRequest<HeroResult<bool>>
{
    public string UserId { get; set; }
    public string GuId { get; set; }
}

public class VerifyEmailQueryHandler : IRequestHandler<GetVerifyEmailQuery, HeroResult<bool>>
{
    private readonly IUserRepository _userRepository;

    public VerifyEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<HeroResult<bool>> Handle(GetVerifyEmailQuery request, CancellationToken cancellationToken)
    {
        var isValidEmailLink = await _userRepository.VerifyEmail(request.GuId, request.UserId);
        return HeroResult<bool>.Success(isValidEmailLink);
    }
}
