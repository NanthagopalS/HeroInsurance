using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.ShareLink.Command.AuthenticateUser;


public record AuthenticateUserCommand : IRequest<HeroResult<AuthenticateUserVm>>
{
    public string UserId { get; set; }
    public string TransactionId { get; set; }
    public string LeadId { get; set; }
    public string StageId { get; set; }
}
public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, HeroResult<AuthenticateUserVm>>
{
    private readonly IShareLinkRepository _shareLinkRepository;
    public AuthenticateUserCommandHandler(IShareLinkRepository shareLinkRepository)
    {
        _shareLinkRepository = shareLinkRepository;
    }
    public async Task<HeroResult<AuthenticateUserVm>> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        var response = await _shareLinkRepository.AuthenticateUser(request, cancellationToken);
        if(response != null)
        {
            return HeroResult<AuthenticateUserVm>.Success(response);
        }
        return HeroResult<AuthenticateUserVm>.Success("Failed To Authenticate User");
    }
}
