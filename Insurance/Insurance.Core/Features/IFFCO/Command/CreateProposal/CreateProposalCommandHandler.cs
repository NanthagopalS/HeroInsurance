using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.IFFCO.Command.CreateProposal;

public class IFFCOCreateProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
{

}
public class CreateProposalCommandHandler : IRequestHandler<IFFCOCreateProposalCommand, HeroResult<QuoteResponseModel>>
{
    private readonly IQuoteRepository _quoteRepository;
    private readonly IIFFCORepository _iFFCORepository;
    private readonly IMapper _mapper;
    private readonly IFFCOConfig _iFFCOConfig;
    private readonly IApplicationClaims _applicationClaims;

    public CreateProposalCommandHandler(IQuoteRepository quoteRepository, IIFFCORepository iFFCORepository, IMapper mapper, IOptions<IFFCOConfig> option, IApplicationClaims applicationClaims)
    {
        _quoteRepository = quoteRepository;
        _iFFCORepository = iFFCORepository;
        _mapper = mapper;
        _iFFCOConfig = option.Value;
        _applicationClaims = applicationClaims;
    }
    public async Task<HeroResult<QuoteResponseModel>> Handle(IFFCOCreateProposalCommand request, CancellationToken cancellationToken)
    {
        var paymentTransactionId = (dynamic)null;
        var proposalResponse = (dynamic)null;
        QuoteResponseModel proposalResponseModel = new QuoteResponseModel();

        var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(request.QuoteTransactionID, cancellationToken);
        CreateLeadModel leadDetails = (_quotedetails?.LeadDetail);
        if (_quotedetails != null)
        {
            if (leadDetails != null && leadDetails.IsBreakin && !leadDetails.IsBreakinApproved)
            {
                proposalResponse = await _iFFCORepository.GenerateBreakin(_quotedetails, cancellationToken);
            }
            else
            {
                proposalResponse = await _iFFCORepository.CreateProposal(_quotedetails, cancellationToken);
            }

            if(proposalResponse != null)
            {
                proposalResponse.quoteResponseModel.CKYCStatus = "KYC_SUCCESS";
                proposalResponse.quoteResponseModel.Type = "INSERT";
                proposalResponse.IsSharePaymentLink = request.IsSharePaymentLink;

                if(leadDetails.IsBreakin || leadDetails.IsSelfInspection)
                {
                    InsertBreakInDetailsModel breakInModel = new InsertBreakInDetailsModel()
                    {
                        LeadId = leadDetails.LeadID,
                        IsBreakIn = true,
                        PolicyNumber = proposalResponse.quoteResponseModel.PolicyNumber,
                        BreakInId = proposalResponse.quoteResponseModel.BreakinId,
                        BreakinInspectionURL = proposalResponse.quoteResponseModel.BreakinInspectionURL,
                        BreakInInspectionAgency = string.Empty
                    };
                    var res = _quoteRepository.InsertBreakInDetails(breakInModel, cancellationToken);
                    
                    proposalResponse.quoteResponseModel.BreakinStatus = "Initiated";
                    proposalResponse.quoteResponseModel.ValidationMessage = "Vehicle Inspection is Initiated," + _iFFCOConfig.InsurerName + " team will reach out for conducting inspection. Inspection ID: " + proposalResponse.quoteResponseModel.BreakinId + " Please save it for future reference.";
                }
                else
                {
                    proposalResponse.QuoteTransactionId = request.QuoteTransactionID;

                    await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken);

                    proposalResponse.quoteResponseModel.PaymentURLLink = $"{_iFFCOConfig.ProposalSumbitFormURL}{proposalResponse.quoteResponseModel.TransactionID}/{_applicationClaims.GetUserId()}?uniqId={proposalResponse.quoteResponseModel.PolicyNumber}";
                }
                proposalResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse.quoteResponseModel);
                proposalResponseModel.InsurerId = _iFFCOConfig.InsurerId;
                proposalResponseModel.TransactionID = request.QuoteTransactionID;
                proposalResponseModel.ProposalNumber = proposalResponse.quoteResponseModel.PolicyNumber;
                paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken).ConfigureAwait(false);
                if (paymentTransactionId != null)
                {
                    return HeroResult<QuoteResponseModel>.Success(proposalResponse.quoteResponseModel);
                }
                return HeroResult<QuoteResponseModel>.Fail("Insert Payment Transaction Failed");
            }
            return HeroResult<QuoteResponseModel>.Fail("Create Proposal Failed");
        }
        return HeroResult<QuoteResponseModel>.Fail("No Record Found");
    }
}