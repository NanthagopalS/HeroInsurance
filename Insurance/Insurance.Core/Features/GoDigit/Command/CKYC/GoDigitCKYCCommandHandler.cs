using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.GoDigit.Command.CKYC;

public class GoDigitCKYCCommand : CKYCModel, IRequest<HeroResult<SaveCKYCResponse>>
{
    public string QuoteTransactionId { get; set; }
}
public class GoDigitCKYCCommandHandler : IRequestHandler<GoDigitCKYCCommand, HeroResult<SaveCKYCResponse>>
{
    private readonly IGoDigitRepository _goDigitRepository;
    public GoDigitCKYCCommandHandler(IGoDigitRepository goDigitRepository)
    {
        _goDigitRepository = goDigitRepository;
    }
    public async Task<HeroResult<SaveCKYCResponse>> Handle(GoDigitCKYCCommand request, CancellationToken cancellationToken)
    {
        var response = await _goDigitRepository.SaveCKYC(request, cancellationToken);
        if (response == null)
            return HeroResult<SaveCKYCResponse>.Fail("CKYC Failed");

        return HeroResult<SaveCKYCResponse>.Success(response);
    }
}
