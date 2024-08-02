using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Core.Features.ShareLink.Command.VerifyOTP;
public record VerifyOTPCommand : IRequest<HeroResult<string>>
{
    public string OTP { get; set; }
    public string LeadId { get; set; }
}
public class VerifyOTPCommandHandler : IRequestHandler<VerifyOTPCommand, HeroResult<string>>
{
    private readonly IShareLinkRepository _shareLinkRepository;
    public VerifyOTPCommandHandler(IShareLinkRepository shareLinkRepository)
    {
        _shareLinkRepository = shareLinkRepository;
    }
    public async Task<HeroResult<string>> Handle(VerifyOTPCommand request, CancellationToken cancellationToken)
    {
        var result = await _shareLinkRepository.VerifyOTP(request, cancellationToken);
        if (result != null && result.Equals("success"))
        {
            return HeroResult<string>.Success("OTP Verify successfully");
        }
        return HeroResult<string>.Fail("Invalid OTP");
    }
}
