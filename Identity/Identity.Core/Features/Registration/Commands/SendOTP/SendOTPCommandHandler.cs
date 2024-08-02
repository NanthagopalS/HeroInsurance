using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;
using ThirdPartyUtilities.Abstraction;

namespace Identity.Core.Features.Registration.Commands.SendOTP;
public record SendOTPCommand : IRequest<HeroResult<SendOTPVm>>
{
    //[Required]
    public string MobileNo { get; set; }
}

public class SendOTPCommandHandler : IRequestHandler<SendOTPCommand, HeroResult<SendOTPVm>>
{
    private readonly IAuthenticateRepository _authenticateRepository;
    private readonly IMapper _mapper;
    private readonly ICustomUtility _utility;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userCreationRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public SendOTPCommandHandler(IMapper mapper, IAuthenticateRepository authenticateRepository, ICustomUtility utility)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _authenticateRepository = authenticateRepository ?? throw new ArgumentNullException(nameof(authenticateRepository));
        _utility = utility ?? throw new ArgumentNullException(nameof(utility));
    }

    public async Task<HeroResult<SendOTPVm>> Handle(SendOTPCommand request, CancellationToken cancellationToken)
    {
        request.MobileNo = _utility.Base64DecodeForRequest(request.MobileNo);
        var result = await _authenticateRepository.SendSMS(request.MobileNo);
        if (result is not null)
        {
            var sendOTPVm = _mapper.Map<SendOTPVm>(result);
            return HeroResult<SendOTPVm>.Success(sendOTPVm);
        }
        return HeroResult<SendOTPVm>.Fail("Failed to send OTP");
    }
}
