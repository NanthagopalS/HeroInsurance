using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.ShareLink.Command.SendNotification;

public record SendNotificationCommand : IRequest<HeroResult<string>>
{
    public string LeadId { get; set; }
    public string EmailId { get; set; }
    public string MobileNumber { get; set; }
    public string Type { get; set; }
    public string InsurerId { get; set; }
}
public class SendNotificationCommandHandler : IRequestHandler<SendNotificationCommand, HeroResult<string>>
{
    private readonly IShareLinkRepository _shareLinkRepository;
    public SendNotificationCommandHandler(IShareLinkRepository shareLinkRepository)
    {
        _shareLinkRepository = shareLinkRepository;
    }

    public async Task<HeroResult<string>> Handle(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var response = await _shareLinkRepository.SendNotification(request, cancellationToken);
        if (response != null && response.Equals("success"))
        {
            return HeroResult<string>.Success("Plan details shared successfully");
        }
        return default;
    }
}
