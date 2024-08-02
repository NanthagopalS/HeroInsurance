using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.GoDigit.Queries.GetPolicyStatus;


public record GetPolicyStatusQuery : IRequest<HeroResult<string>>
{
    public string PolicyNumber { get; set; }
    public string QuoteTransactionId { get; set; }
    public string LeadId { get; set; }
}
public class GetPolicyStatusQueryHandler : IRequestHandler<GetPolicyStatusQuery, HeroResult<string>>
{
    private readonly IGoDigitRepository _goDigitRepository;
    private readonly GoDigitConfig _goDigitConfig;
    private readonly IApplicationClaims _applicationClaims;
    private readonly IMapper _mapper;
    private readonly IQuoteRepository _quoteRepository;

    public GetPolicyStatusQueryHandler(IGoDigitRepository goDigitRepository,
                                       IOptions<GoDigitConfig> goDigitConfig,
                                       IApplicationClaims applicationClaims,
                                       IMapper mapper,
                                       IQuoteRepository quoteRepository)
    {
        _goDigitRepository = goDigitRepository;
        _goDigitConfig = goDigitConfig.Value;
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _quoteRepository = quoteRepository;
    }

    public async Task<HeroResult<string>> Handle(GetPolicyStatusQuery request, CancellationToken cancellationToken)
    {
        var response = await _goDigitRepository.GetPolicyStatus(request.LeadId, request.PolicyNumber, cancellationToken);
        if (response != null && response.InsurerStatusCode == 200)
        {

            if (response.BreakinStatus.ToLower().Equals("incomplete") || response.BreakinStatus.ToLower().Equals("pre_inspection_approved"))
            {
                var proposalResponseModel = _mapper.Map<QuoteResponseModel>(response);
                proposalResponseModel.Type = "INSERT";
                proposalResponseModel.TransactionID = request.QuoteTransactionId;
                var paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken).ConfigureAwait(false);

                if (paymentTransactionId != null)
                {
                    var paymentURLResponse = await _goDigitRepository.CreatePaymentLink(paymentTransactionId.LeadId, response.ApplicationId, $"{_goDigitConfig.PGRedirectionURL}{response.ApplicationId}/{_applicationClaims.GetUserId()}", $"{_goDigitConfig.PGRedirectionURL}{response.ApplicationId}/{_applicationClaims.GetUserId()}", cancellationToken);

                    if (paymentURLResponse != null && paymentURLResponse.InsurerStatusCode == 200)
                    {
                        response.PaymentURLLink = paymentURLResponse.PaymentURL;
                    }
                }
                await UpdateBreakInStatus(request.QuoteTransactionId,
                                          response.ProposalNumber,
                                          response.PaymentURLLink,
                                          "Proposal",
                                          true,
                                          cancellationToken);

                return HeroResult<string>.Success($"Vehicle Inspection is Completed Successfully with {_goDigitConfig.InsurerName}.Inspection ID: {response.ProposalNumber}");
            }
            else
            {
                await UpdateBreakInStatus(request.QuoteTransactionId, response.ProposalNumber, string.Empty, "BreakIn", false, cancellationToken);
                return HeroResult<string>.Fail($"Vehicle Inspection is Not Successful with {_goDigitConfig.InsurerName}.Inspection ID: {response?.ProposalNumber}.Please retry with other insurer or reach out to us for assistance.");
            }
        }
        return HeroResult<string>.Fail($"Vehicle Inspection is In Progress,{_goDigitConfig.InsurerName} team will reach out for conducting inspection. Inspection ID: {response?.PolicyNumber} Please save it for future reference.");
    }

    private async Task UpdateBreakInStatus(string quoteTransactionId, string proposalNumber, string paymentURL, string stage, bool breakInStatus, CancellationToken cancellationToken)
    {
        CreateLeadModel createLeadModel = new CreateLeadModel()
        {
            QuoteTransactionID = quoteTransactionId,
            PolicyNumber = proposalNumber,
            PaymentLink = paymentURL,
            Stage = stage,
            BreakInStatus = breakInStatus,
            InsurerId = _goDigitConfig.InsurerId
        };
        await _quoteRepository.UpdateLeadDetails(createLeadModel, cancellationToken);
    }
}
