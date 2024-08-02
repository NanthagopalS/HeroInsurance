using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using System.Transactions;

namespace Insurance.Core.Features.GoDigit.Command
{
    public class GodigitCreateProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
    {

    }
    public class GodigitCreateProposalCommandHandler : IRequestHandler<GodigitCreateProposalCommand, HeroResult<QuoteResponseModel>>
    {
        private readonly IGoDigitRepository _goDigitRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly GoDigitConfig _goDigitConfig;
        private readonly IApplicationClaims _applicationClaims;

        public GodigitCreateProposalCommandHandler(IMapper mapper, IGoDigitRepository goDigitRepository, IQuoteRepository quoteRepository, IOptions<GoDigitConfig> options, IApplicationClaims applicationClaims)
        {
            _goDigitRepository = goDigitRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository;
            _goDigitConfig = options.Value;
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        }
        public async Task<HeroResult<QuoteResponseModel>> Handle(GodigitCreateProposalCommand request, CancellationToken cancellationToken)
        {
            var createLeadModel = _mapper.Map<ProposalRequestModel>(request);
            var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(createLeadModel.QuoteTransactionID, cancellationToken);

            var proposalResponse = await _goDigitRepository.CreateProposal(createLeadModel, cancellationToken).ConfigureAwait(false);
            proposalResponse.IsSharePaymentLink = request.IsSharePaymentLink;
            await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken);

            if (proposalResponse != null && proposalResponse.quoteResponseModel.InsurerStatusCode == 200)
            {
                if (proposalResponse.quoteResponseModel.IsBreakIn || proposalResponse.quoteResponseModel.IsSelfInspection)
                {
                    //
                    CreateLeadModel _leadDetails = (_quotedetails.LeadDetail);
                    InsertBreakInDetailsModel breakInModel = new InsertBreakInDetailsModel()
                    {
                        LeadId = _leadDetails.LeadID,
                        IsBreakIn = true,
                        PolicyNumber = proposalResponse.PolicyNumber,
                        BreakInId = proposalResponse.PolicyNumber,
                        BreakinInspectionURL = string.Empty,//proposalResponse.quoteResponseModel.BreakinInspectionURL,
                        BreakInInspectionAgency = string.Empty
                    };
                    var res = _quoteRepository.InsertBreakInDetails(breakInModel, cancellationToken);
                    //
                    proposalResponse.quoteResponseModel.CKYCStatus = "KYC_SUCCESS";
                    proposalResponse.quoteResponseModel.BreakinStatus = "Initiated";
                    proposalResponse.quoteResponseModel.IsSelfInspection = true;
                    proposalResponse.quoteResponseModel.InspectionId = proposalResponse.PolicyNumber;
                    proposalResponse.quoteResponseModel.ValidationMessage = "Vehicle Inspection is Initiated," + _goDigitConfig.InsurerName + " team will reach out for conducting inspection. Inspection ID: " + proposalResponse.PolicyNumber + " Please save it for future reference.";
                    return HeroResult<QuoteResponseModel>.Success(proposalResponse.quoteResponseModel);
                }
                else
                {
                    proposalResponse.quoteResponseModel.CKYCStatus = "KYC_SUCCESS";
                    proposalResponse.quoteResponseModel.IsBreakIn = false;
                    proposalResponse.quoteResponseModel.IsSelfInspection = false;
                    proposalResponse.quoteResponseModel.InspectionId = proposalResponse.PolicyNumber;

                    var proposalResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse.quoteResponseModel);
                    proposalResponseModel.Type = "INSERT";
                    var paymentURLResponse = await _goDigitRepository.CreatePaymentLink(_quotedetails?.LeadDetail.LeadID, proposalResponseModel.ApplicationId, $"{_goDigitConfig.PGRedirectionURL}{proposalResponse.quoteResponseModel.ApplicationId}/{_applicationClaims.GetUserId()}",
                                                                                                                             $"{_goDigitConfig.PGRedirectionURL}{proposalResponse.quoteResponseModel.ApplicationId}/{_applicationClaims.GetUserId()}",
                                                                                                                             cancellationToken).ConfigureAwait(false);
                    if (paymentURLResponse != null && paymentURLResponse.InsurerStatusCode == 200)
                    {
                        proposalResponse.quoteResponseModel.PaymentURLLink = paymentURLResponse.PaymentURL;
                        proposalResponseModel.PaymentURLLink = paymentURLResponse.PaymentURL;

                        var paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken).ConfigureAwait(false);
                        if (paymentTransactionId != null)
                        {
                            return HeroResult<QuoteResponseModel>.Success(proposalResponse.quoteResponseModel);
                        }
                        return HeroResult<QuoteResponseModel>.Fail("Insert Payment Transaction Failed");
                    }
                    return HeroResult<QuoteResponseModel>.Fail(paymentURLResponse.ValidationMessage);
                }
            }
            return HeroResult<QuoteResponseModel>.Fail(proposalResponse.quoteResponseModel.ValidationMessage);
        }
    }
}
