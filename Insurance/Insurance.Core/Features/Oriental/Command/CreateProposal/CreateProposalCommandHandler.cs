using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Oriental;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.Oriental.Command.CreateProposal;

public class OrientalCreateProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
{
}
public class OrientalCreateProposalCommandHandler : IRequestHandler<OrientalCreateProposalCommand, HeroResult<QuoteResponseModel>>
{
    private readonly IQuoteRepository _quoteRepository;
    private readonly IOrientalRepository _orientalRepository;
    private readonly IMapper _mapper;
    private readonly OrientalConfig _orientalConfig;
    private const string KYC_SUCCESS = "KYC_SUCCESS";
    private const string POA_REQUIRED = "POA_REQUIRED";
    public OrientalCreateProposalCommandHandler(IQuoteRepository quoteRepository,
        IOrientalRepository orientalRepository,
        IMapper mapper,
        IOptions<OrientalConfig> options
        )
    {
        _quoteRepository = quoteRepository;
        _orientalRepository = orientalRepository;
        _mapper = mapper;
        _orientalConfig = options.Value;
    }
    public async Task<HeroResult<QuoteResponseModel>> Handle(OrientalCreateProposalCommand request, CancellationToken cancellationToken)
    {
        QuoteResponseModel proposalResponseModel;
        var paymentTransactionId = (dynamic)null;
        var quotedetails = await _quoteRepository.GetQuoteTransactionDetails(request.QuoteTransactionID, cancellationToken);
        if (quotedetails != null)
        {
            var proposalResponse = await _orientalRepository.CreateProposal(quotedetails, cancellationToken);
            if (proposalResponse != null && proposalResponse?.quoteResponseModel.ApplicationId != null)
            {
                proposalResponse.quoteResponseModel.Type = "INSERT";
                proposalResponse.IsSharePaymentLink = request.IsSharePaymentLink;
                proposalResponse.QuoteTransactionId = request.QuoteTransactionID;

                proposalResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse.quoteResponseModel);

                var ckycResponse = await _orientalRepository.GetCKYCDetails(quotedetails.ProposalRequestBody, proposalResponse.TransactionId, quotedetails.LeadDetail, request.QuoteTransactionID, cancellationToken);

                if (ckycResponse != null && ckycResponse.SaveCKYCResponse.KYC_Status.Equals("KYC_SUCCESS"))
                {
                    proposalResponseModel.CKYCStatus = KYC_SUCCESS;
                }
                else
                {
                    proposalResponseModel.CKYCStatus = POA_REQUIRED;
                    proposalResponseModel.CKYCFailReason = ckycResponse?.SaveCKYCResponse?.Message;
                    proposalResponse.quoteResponseModel.IsDocumentUpload = true;
                }
                await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken);
                proposalResponseModel.InsurerId = _orientalConfig.InsurerId;
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
