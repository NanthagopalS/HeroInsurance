using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.Bajaj.Command.CKYC;

public class BajajCKYCCommand : CKYCModel, IRequest<HeroResult<SaveCKYCResponse>>
{
    public string QuoteTransactionId { get; set; }
}
public class BajajCKYCCommandHandler : IRequestHandler<BajajCKYCCommand, HeroResult<SaveCKYCResponse>>
{
    private readonly IBajajRepository _bajajRepository;
    public BajajCKYCCommandHandler(IBajajRepository bajajRepository)
    {
        _bajajRepository = bajajRepository;
    }
    public async Task<HeroResult<SaveCKYCResponse>> Handle(BajajCKYCCommand request, CancellationToken cancellationToken)
    {
        var response = await _bajajRepository.SaveCKYC(request, cancellationToken);
        if (response == null)
            return HeroResult<SaveCKYCResponse>.Fail("CKYC Failed");

        return HeroResult<SaveCKYCResponse>.Success(response);
    }
}
