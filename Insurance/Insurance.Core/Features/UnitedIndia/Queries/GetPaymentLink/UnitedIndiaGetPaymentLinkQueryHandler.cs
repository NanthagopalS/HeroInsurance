using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using Insurance.Domain.UnitedIndia;
using MediatR;

namespace Insurance.Core.Features.UnitedIndia.Queries.GetPaymentLink;

public class UnitedIndiaGetPaymentLinkQuery : InitiatePaymentRequestDto, IRequest<HeroResult<string>>
{
    public string InsurerId { get; set; }
}
public class UnitedIndiaGetPaymentLinkQueryHandler : IRequestHandler<UnitedIndiaGetPaymentLinkQuery, HeroResult<string>>
{
    private readonly IUnitedIndiaRepository _unitedIndiaRepository;
    private readonly IQuoteRepository _quoteRepository;
    public UnitedIndiaGetPaymentLinkQueryHandler(IUnitedIndiaRepository unitedIndiaRepository,
        IQuoteRepository quoteRepository)
    {
        _unitedIndiaRepository = unitedIndiaRepository;
        _quoteRepository = quoteRepository;
    }
    public async Task<HeroResult<string>> Handle(UnitedIndiaGetPaymentLinkQuery request, CancellationToken cancellationToken)
    {
        PaymentStatusRequestModel paymentStatusRequestModel = new PaymentStatusRequestModel()
        {
            InsurerId = request.InsurerId,
            QuoteTransactionId = request.QuoteTransactionId
        };
        var breakinPaymentDetails = await _quoteRepository.GetBreakInPaymentDetails(paymentStatusRequestModel, cancellationToken);
        if (breakinPaymentDetails != null)
        {
            request.LeadId = breakinPaymentDetails.LeadId;
            request.orderId = breakinPaymentDetails.ApplicationId;
            request.num_reference_number = breakinPaymentDetails.ProposalNumber;
            request.userInfo = new Userinfo()
            {
                mobile = breakinPaymentDetails.PhoneNumber,
                custId = breakinPaymentDetails.CustomerId
            };
            request.txnAmount = breakinPaymentDetails.GrossPremium;

            var paymentLink = await _unitedIndiaRepository.InitiatePayment(request, cancellationToken);
            if (paymentLink != null)
            {
                var paymentLinkUpdate = await _quoteRepository.UpdateLeadPaymentLink(request.InsurerId, request.QuoteTransactionId, paymentLink, string.Empty, cancellationToken);
                if (!string.IsNullOrEmpty(paymentLinkUpdate))
                {
                    return HeroResult<string>.Success(paymentLink);
                }
                return HeroResult<string>.Success("UIIC GetPaymentLink - UpdateLeadPaymentLink Falied");
            }
            return HeroResult<string>.Fail("Issue with UIIC create paymentlink");
        }
        return HeroResult<string>.Fail("UIIC GetPaymentLink - GetBreakInPaymentDetails Falied");
    }
}
