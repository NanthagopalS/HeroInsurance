using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Bajaj;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Core.Features.Bajaj.Command;

public class BajajProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
{

}
internal class BajajProposalCommandHandler : IRequestHandler<BajajProposalCommand, HeroResult<QuoteResponseModel>>
{
    private readonly IBajajRepository _bajajRepository;
    private readonly IMapper _mapper;
    private readonly IQuoteRepository _quoteRepository;
    private readonly BajajConfig _bajajConfig;
    private const string SATPTwoWHeelerCode = "1806";
    private const string SATPFourWHeelerCode = "1805";

    public BajajProposalCommandHandler(IMapper mapper, IBajajRepository bajajRepository, IQuoteRepository quoteRepository, IOptions<BajajConfig> options)
    {
        _bajajRepository = bajajRepository;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _quoteRepository = quoteRepository;
        _bajajConfig = options.Value;
    }
    public async Task<HeroResult<QuoteResponseModel>> Handle(BajajProposalCommand request, CancellationToken cancellationToken)
    {
        var proposalRequestModel = _mapper.Map<ProposalRequestModel>(request);
        var paymentTransactionId = (dynamic)null;
        var quotedetails = await _quoteRepository.GetQuoteTransactionDetails(proposalRequestModel.QuoteTransactionID, cancellationToken);

        if (quotedetails != null)
        {
            BajajProposalRequestDto bajajProposal = JsonConvert.DeserializeObject<BajajProposalRequestDto>(quotedetails?.QuoteTransactionRequest?.RequestBody);
            CreateLeadModel leadDetails = (quotedetails?.LeadDetail);
            var response = new SaveQuoteTransactionModel();

            if (leadDetails != null && leadDetails.IsBreakin && !leadDetails.IsBreakinApproved && !bajajProposal.weomotpolicyin.product4digitcode.Equals(SATPTwoWHeelerCode) && !bajajProposal.weomotpolicyin.product4digitcode.Equals(SATPFourWHeelerCode))
            {
                response = await _bajajRepository.GenerateBreakinPin(quotedetails, cancellationToken);
            }
            else
            {
                response = await _bajajRepository.CreateProposal(quotedetails, cancellationToken);
            }
            if(response != null)
            {
                response.QuoteTransactionId = request.QuoteTransactionID;
                response.IsSharePaymentLink = request.IsSharePaymentLink;
                await _quoteRepository.SaveQuoteTransaction(response, cancellationToken);
                if (!string.IsNullOrEmpty(response.quoteResponseModel.ValidationMessage))
                {
                    return HeroResult<QuoteResponseModel>.Fail(response.quoteResponseModel.ValidationMessage);
                }
                response.quoteResponseModel.CKYCStatus = "KYC_SUCCESS";
                response.quoteResponseModel.Type = "INSERT";
                if (response.quoteResponseModel.IsBreakIn || response.quoteResponseModel.IsSelfInspection)
                {
                    InsertBreakInDetailsModel breakInModel = new InsertBreakInDetailsModel()
                    {
                        LeadId = leadDetails.LeadID,
                        IsBreakIn = true,
                        PolicyNumber = response.quoteResponseModel.PolicyNumber,
                        BreakInId = response.quoteResponseModel.BreakinId,
                        BreakinInspectionURL = response.quoteResponseModel.BreakinInspectionURL,
                        BreakInInspectionAgency = string.Empty
                    };
                    var res = _quoteRepository.InsertBreakInDetails(breakInModel, cancellationToken);
                    response.quoteResponseModel.BreakinStatus = "Initiated";
                    if (!string.IsNullOrEmpty(response.quoteResponseModel.BreakinId))
                    {
                        response.quoteResponseModel.ValidationMessage = "Vehicle Inspection is Initiated," + _bajajConfig.InsurerName + " team will reach out for conducting inspection. Inspection ID: " + response.quoteResponseModel.BreakinId + " Please save it for future reference.";
                    }
                }
                response.QuoteTransactionId = proposalRequestModel.QuoteTransactionID;
                paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(response.quoteResponseModel, cancellationToken).ConfigureAwait(false);
                if (paymentTransactionId != null)
                {
                    return HeroResult<QuoteResponseModel>.Success(response.quoteResponseModel);
                }
                return HeroResult<QuoteResponseModel>.Fail("fail to save data");
            }
        }
        return HeroResult<QuoteResponseModel>.Fail("No Record Found");
    }
}
