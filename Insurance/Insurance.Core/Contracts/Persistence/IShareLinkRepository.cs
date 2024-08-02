using Insurance.Core.Features.ShareLink.Command.AuthenticateUser;
using Insurance.Core.Features.ShareLink.Command.SendNotification;
using Insurance.Core.Features.ShareLink.Command.SendOTP;
using Insurance.Core.Features.ShareLink.Command.VerifyOTP;
using Insurance.Domain.ShareLink;

namespace Insurance.Core.Contracts.Persistence;

public interface IShareLinkRepository
{
    Task<string> SendNotification(SendNotificationCommand request, CancellationToken cancellationToken);
    Task<string> SendSMS(SendOTPCommand request, CancellationToken cancellationToken);
    Task<string> VerifyOTP(VerifyOTPCommand request, CancellationToken cancellationToken);
    Task<AuthenticateUserVm> AuthenticateUser(AuthenticateUserCommand request, CancellationToken cancellationToken);
}
