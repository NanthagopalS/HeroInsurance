using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.ICICI.Command.CKYC;

public class ICICICKYCCommand : CKYCModel, IRequest<HeroResult<SaveCKYCResponse>>
{
    public string QuoteTransactionId { get; set; }
}

public class ICICICKYCCommandHandler : IRequestHandler<ICICICKYCCommand, HeroResult<SaveCKYCResponse>>
{
    private readonly IICICIRepository _iCICIRepository;
    public ICICICKYCCommandHandler(IICICIRepository iCICIRepository)
    {
        _iCICIRepository = iCICIRepository;
    }

    public async Task<HeroResult<SaveCKYCResponse>> Handle(ICICICKYCCommand request, CancellationToken cancellationToken)
    {
        var response = await _iCICIRepository.SaveCKYC(request, cancellationToken);
        if (response == null)
            return HeroResult<SaveCKYCResponse>.Fail("CKYC Failed");

        return HeroResult<SaveCKYCResponse>.Success(response);
    }
}
