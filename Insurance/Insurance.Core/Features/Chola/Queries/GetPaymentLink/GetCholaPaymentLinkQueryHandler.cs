using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Core.Features.Chola.Queries.GetPaymentLink
{

    public class GetCholaPaymentLinkQuery : IRequest<HeroResult<string>>
    {
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
    }
    public class GetCholaPaymentLinkQueryHandler : IRequestHandler<GetCholaPaymentLinkQuery, HeroResult<string>>
    {
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;
        private readonly ICholaRepository _cholaRepository;
        private readonly CholaConfig _cholaConfig;
        private readonly IApplicationClaims _applicationClaims;

        public GetCholaPaymentLinkQueryHandler(IMapper mapper, IQuoteRepository quoteRepository, ICholaRepository cholaRepository, IOptions<CholaConfig> options, IApplicationClaims applicationClaims)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _cholaRepository = cholaRepository;
            _cholaConfig = options.Value;
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        }
        public async Task<HeroResult<string>> Handle(GetCholaPaymentLinkQuery request, CancellationToken cancellationToken)
        {
            var requestModel = _mapper.Map<PaymentStatusRequestModel>(request);
            var breakinDetails = await _cholaRepository.GetBreakinDetails(request.QuoteTransactionId, cancellationToken);
            if (breakinDetails != null)
            {
                if (breakinDetails.PaymentLink != null)
                {
                    var breakinPaymentDetails = await _quoteRepository.GetBreakInPaymentDetails(requestModel, cancellationToken);
                    if (breakinPaymentDetails != null)
                    {
                        var checksumID = _cholaRepository.PaymentURLGeneration(breakinPaymentDetails.ApplicationId, breakinPaymentDetails.GrossPremium, request.QuoteTransactionId);

                        string paymentURL = $"{_cholaConfig.PaymentGenerationURLLink}msg={_cholaConfig.MerchantId}|{breakinPaymentDetails.ApplicationId}|NA|{breakinPaymentDetails.GrossPremium}|NA|NA|NA|{_cholaConfig.Currency}|NA|{_cholaConfig.Code1}|{_cholaConfig.SecurityId}|NA|NA|{_cholaConfig.Code2}|NA|{_cholaConfig.AdditionalInformation2}|NA|NA|NA|NA|NA|{_cholaConfig.PGStatusRedirectionURL}{request.QuoteTransactionId}/{_applicationClaims.GetUserId()}|{checksumID}";

                        if (!string.IsNullOrEmpty(checksumID))
                        {
                            var paymentLink = await _quoteRepository.UpdateLeadPaymentLink(request.InsurerId, request.QuoteTransactionId, paymentURL, breakinPaymentDetails.ApplicationId, cancellationToken);
                            if (!string.IsNullOrEmpty(paymentLink))
                            {
                                return HeroResult<string>.Success(paymentLink);
                            }
                        }
                    }
                    return HeroResult<string>.Fail("Failed To Create PaymentLink");
                }
                else if (breakinDetails.IsBreakin && breakinDetails.IsBreakinApproved)
                {
                    SaveQuoteTransactionModel proposalResponse = new SaveQuoteTransactionModel();
                    var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(request.QuoteTransactionId, cancellationToken);
                    if (_quotedetails != null)
                    {
                        var cholaCkycDetails = new CholaCKYCRequestModel();
                        CholaServiceRequestModel _cholaProposal = JsonConvert.DeserializeObject<CholaServiceRequestModel>(_quotedetails.QuoteTransactionRequest?.RequestBody);
                        _cholaProposal.quote_id = _quotedetails.QuoteTransactionRequest?.TransactionId;
                        _cholaProposal.proposal_id = _quotedetails.QuoteTransactionRequest?.ProposalId;

                        CreateLeadModel _leadDetails = (_quotedetails.LeadDetail);
                        CholaProposalRequest _cholaProposalRequest = JsonConvert.DeserializeObject<CholaProposalRequest>(_quotedetails.ProposalRequestBody);

                        if (!string.IsNullOrWhiteSpace(_quotedetails.CKYCRequestBody))
                        {
                            cholaCkycDetails = JsonConvert.DeserializeObject<CholaCKYCRequestModel>(_quotedetails.CKYCRequestBody);
                        }
                        proposalResponse = await _cholaRepository.CreateProposal(_cholaProposal, _cholaProposalRequest, _leadDetails, cholaCkycDetails, cancellationToken);

                        proposalResponse.TransactionId = _quotedetails.QuoteTransactionRequest?.TransactionId;
                        proposalResponse.PolicyNumber = _quotedetails.QuoteTransactionRequest?.ProposalId;
                        proposalResponse.PolicyId = _quotedetails.QuoteTransactionRequest?.PolicyId;
                        proposalResponse.QuoteTransactionId = request.QuoteTransactionId;

                    }
                    if (proposalResponse != null && proposalResponse.quoteResponseModel.InsurerStatusCode == 200)
                    {
                        await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken); // Need to save only on success of proposal
                        var checksumID = _cholaRepository.PaymentURLGeneration(proposalResponse.quoteResponseModel.ApplicationId, proposalResponse.quoteResponseModel.GrossPremium, proposalResponse.quoteResponseModel.TransactionID);

                        string paymentURL = $"{_cholaConfig.PaymentGenerationURLLink}msg={_cholaConfig.MerchantId}|{proposalResponse.quoteResponseModel.ApplicationId}|NA|{proposalResponse.quoteResponseModel.GrossPremium}|NA|NA|NA|{_cholaConfig.Currency}|NA|{_cholaConfig.Code1}|{_cholaConfig.SecurityId}|NA|NA|{_cholaConfig.Code2}|NA|{_cholaConfig.AdditionalInformation2}|NA|NA|NA|NA|NA|{_cholaConfig.PGStatusRedirectionURL}{proposalResponse.quoteResponseModel.TransactionID}/{_applicationClaims.GetUserId()}|{checksumID}";

                        if (!string.IsNullOrEmpty(checksumID))
                        {
                            var paymentLink = await _quoteRepository.UpdateLeadPaymentLink(request.InsurerId, proposalResponse.quoteResponseModel.TransactionID, paymentURL, proposalResponse.quoteResponseModel.ApplicationId, cancellationToken);
                            if (!string.IsNullOrEmpty(paymentLink))
                            {
                                return HeroResult<string>.Success(paymentLink);
                            }
                        }
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
}
