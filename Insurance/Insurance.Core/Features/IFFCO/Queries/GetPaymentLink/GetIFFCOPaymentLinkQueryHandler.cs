using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.IFFCO.Queries.GetPaymentLink;

public class GetIFFCOPaymentLinkQuery : IRequest<HeroResult<string>>
{
    public string InsurerId { get; set; }
    public string QuoteTransactionId { get; set; }
}

public class GetIFFCOPaymentLinkQueryHandler : IRequestHandler<GetIFFCOPaymentLinkQuery, HeroResult<string>>
{
    private readonly IMapper _mapper;
    private readonly IQuoteRepository _quoteRepository;
    private readonly IIFFCORepository _iFFCORepository;
    private readonly IFFCOConfig _iFFCOConfig;
    private readonly IApplicationClaims _applicationClaims;
    public GetIFFCOPaymentLinkQueryHandler(IMapper mapper,
        IQuoteRepository quoteRepository,
        IIFFCORepository iFFCORepository,
        IOptions<IFFCOConfig> options,
        IApplicationClaims applicationClaims)
    {
        _mapper = mapper;
        _quoteRepository = quoteRepository;
        _iFFCORepository = iFFCORepository;
        _iFFCOConfig = options.Value;
        _applicationClaims = applicationClaims;
    }
    public async Task<HeroResult<string>> Handle(GetIFFCOPaymentLinkQuery request, CancellationToken cancellationToken)
    {
        var paymentLink = string.Empty;
        var proposalResponse = (dynamic)null;
        var paymentTransactionId = (dynamic)null;
        QuoteResponseModel proposalResponseModel = new QuoteResponseModel();
        var requestModel = _mapper.Map<PaymentStatusRequestModel>(request);
        var breakinDetails = await _iFFCORepository.GetBreakinDetails(request.QuoteTransactionId, cancellationToken);
        if (breakinDetails != null)
        {
            bool isCorporate = !breakinDetails.CarOwnedBy.Equals("INDIVIDUAL");
            if (breakinDetails.PaymentLink != null)
            {
                var breakinPaymentDetails = await _quoteRepository.GetBreakInPaymentDetails(requestModel, cancellationToken);
                if (breakinPaymentDetails != null)
                {
                    var proposalRequest = await _iFFCORepository.GetPaymentLink(breakinPaymentDetails.ProposalRequest, isCorporate, cancellationToken);
                    if (proposalRequest != null)
                    {
                        paymentLink = $"{_iFFCOConfig.ProposalSumbitFormURL}{request.QuoteTransactionId}/{_applicationClaims.GetUserId()}?uniqId={proposalRequest.Item2}";
                        var insertPaymentLink = await _iFFCORepository.UpdateLeadPaymentLink(request.InsurerId, request.QuoteTransactionId, paymentLink, proposalRequest.Item2, proposalRequest.Item1, proposalRequest.Item3, cancellationToken);
                        if (insertPaymentLink != null)
                        {
                            return HeroResult<string>.Success(insertPaymentLink);
                        }
                    }
                }
                return HeroResult<string>.Fail("Failed To Create PaymentLink");
            }
            else if (breakinDetails.IsBreakin && breakinDetails.IsBreakinApproved)
            {
                var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(request.QuoteTransactionId, cancellationToken);
                if (_quotedetails != null)
                {
                    proposalResponse = await _iFFCORepository.CreateProposal(_quotedetails, cancellationToken);
                    proposalResponse.quoteResponseModel.Type = "INSERT";

                    proposalResponse.QuoteTransactionId = request.QuoteTransactionId;

                    await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken);

                    proposalResponse.quoteResponseModel.PaymentURLLink = $"{_iFFCOConfig.ProposalSumbitFormURL}{proposalResponse.quoteResponseModel.TransactionID}/{_applicationClaims.GetUserId()}?uniqId={proposalResponse.quoteResponseModel.PolicyNumber}";

                    proposalResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse.quoteResponseModel);
                    proposalResponseModel.InsurerId = _iFFCOConfig.InsurerId;
                    proposalResponseModel.TransactionID = request.QuoteTransactionId;
                    proposalResponseModel.ProposalNumber = proposalResponse.quoteResponseModel.PolicyNumber;
                    paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken).ConfigureAwait(false);
                    if (paymentTransactionId != null)
                    {
                        return HeroResult<string>.Success(proposalResponse.quoteResponseModel.PaymentURLLink);
                    }
                    return HeroResult<string>.Fail("Insert Payment Transaction Failed");
                }
                else
                {
                    return HeroResult<string>.Fail(proposalResponse.quoteResponseModel?.ValidationMessage);
                }
            }
            return HeroResult<string>.Fail("Something went wrong, please try again");
        }
        return HeroResult<string>.Fail("Something went wrong, please try again");
    }
}
