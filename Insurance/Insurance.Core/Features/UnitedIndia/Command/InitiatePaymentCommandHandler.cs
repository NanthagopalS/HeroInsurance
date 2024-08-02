using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.UnitedIndia;
using MediatR;

namespace Insurance.Core.Features.UnitedIndia.Command;

public class InitiatePaymentCommand : InitiatePaymentRequestDto, IRequest<HeroResult<string>>
{
}
public class InitiatePaymentCommandHandler : IRequestHandler<InitiatePaymentCommand, HeroResult<string>>
{
    private readonly IUnitedIndiaRepository _unitedIndiaRepository;
    public InitiatePaymentCommandHandler(IUnitedIndiaRepository unitedIndiaRepository)
    {
        _unitedIndiaRepository = unitedIndiaRepository;
    }
    
    public async Task<HeroResult<string>> Handle(InitiatePaymentCommand request, CancellationToken cancellationToken)
    {
        string PaymentURL = string.Empty;
        PaymentURL = await _unitedIndiaRepository.InitiatePayment(request, cancellationToken);
        if (!string.IsNullOrEmpty(PaymentURL))
        {
            return HeroResult<string>.Success(PaymentURL);
        }
        return HeroResult<string>.Fail("Failed to Initiate Payment");
    }
}
