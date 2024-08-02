using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.ShareLink.Command.SendOTP;

public record SendOTPCommand: IRequest<HeroResult<bool>>
{
    public string LeadId { get;set; }
}

public class SendOTPCommandHandler : IRequestHandler<SendOTPCommand, HeroResult<bool>>
{
    private readonly IShareLinkRepository _shareLinkRepository;
    public SendOTPCommandHandler(IShareLinkRepository shareLinkRepository)
    {
        _shareLinkRepository = shareLinkRepository;
    }
    public async Task<HeroResult<bool>> Handle(SendOTPCommand request, CancellationToken cancellationToken)
    {
        var result = await _shareLinkRepository.SendSMS(request, cancellationToken);
        if (result != null && result.Equals("success"))
        {
            return HeroResult<bool>.Success("OTP Sent successfully");
        }
        return HeroResult<bool>.Fail("Failed to send OTP");
    }
}