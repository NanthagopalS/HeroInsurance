using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.IFFCO;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;

namespace Insurance.Core.Features.IFFCO.Command.SavePaymentStatus
{
    public class SavePaymentStatusCommand : IFFCOPaymentResponseModel,IRequest<HeroResult<PaymentDetailsVm>>
    {
    }
    public class SavePaymentStatusCommandHandler : IRequestHandler<SavePaymentStatusCommand, HeroResult<PaymentDetailsVm>>
    {
        private readonly IIFFCORepository _iFFCORepository;
        private readonly IMapper _mapper;
        private readonly IFFCOConfig _iFFCOConfig;
        private readonly IQuoteRepository _quoteRepository;
        public SavePaymentStatusCommandHandler(IIFFCORepository iFFCORepository, IMapper mapper, IOptions<IFFCOConfig> options, IQuoteRepository quoteRepository)
        {
            _iFFCORepository = iFFCORepository;
            _mapper = mapper;
            _iFFCOConfig = options.Value;
            _quoteRepository = quoteRepository;
        }
        public async Task<HeroResult<PaymentDetailsVm>> Handle(SavePaymentStatusCommand request, CancellationToken cancellationToken)
        {
            var paymentDetailsVm = new PaymentDetailsVm();
            var quoteResponse = new QuoteResponseModel();

            var quoteTransactionId = await _iFFCORepository.GetProposalQuotetransactionId(request.ProposalNumber, cancellationToken);
            if (quoteTransactionId != null)
            {
                quoteResponse.TransactionID = quoteTransactionId;
                quoteResponse.InsurerId = _iFFCOConfig.InsurerId;
                quoteResponse.Type = "UPDATE";
                quoteResponse.PaymentTransactionNumber = request.TransactionId;
                quoteResponse.CKYCStatus = "DONE";
                quoteResponse.PaymentDate = DateTime.Now.ToString("yyyy-MM-dd");

                var paymentResponse = _mapper.Map<IFFCOPaymentResponseModel>(request);

                if (request != null && paymentResponse.StatusMessage.Contains("SUCCESSFULLY") || paymentResponse.StatusMessage.Contains("PAYMENT_ACCEPTED"))
                {
                    quoteResponse.PaymentStatus = "Payment Completed";
                    quoteResponse.PolicyNumber = paymentResponse.PolicyNumber;
                    var policyDocData = new IFFCOPolicyDocumentResponse();

                    var leadDetailsrequest = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
                    {
                        QuoteTransactionId = quoteTransactionId,
                        InsurerId = _iFFCOConfig.InsurerId
                    };
                    var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(leadDetailsrequest, cancellationToken);
                    paymentResponse.LeadId = leadDetails?.LeadID;

                    if (!string.IsNullOrEmpty(paymentResponse.PolicyNumber))
                    {
                        policyDocData = await _iFFCORepository.GetPolicyDocumentURL(paymentResponse, cancellationToken);
                    }

                    if (policyDocData != null && !string.IsNullOrEmpty(policyDocData.policyDownloadLink))
                    {
                        quoteResponse.PolicyDocumentLink = policyDocData.policyDownloadLink;
                        byte[] policyString = await _iFFCORepository.GetPolicyDocument(leadDetails?.LeadID, policyDocData.policyDownloadLink, cancellationToken);
                        if (policyString != null)
                        {
                            var documentId = await _quoteRepository.UploadPolicyDocumentMangoDB(policyString, cancellationToken);
                            if (documentId != null)
                            {
                                quoteResponse.DocumentId = documentId;
                            }
                        }
                    }
                }
                else
                {
                    quoteResponse.PaymentStatus = "Payment Failed";
                }
                quoteResponse.ApplicationId = request.ProposalNumber;
                await _iFFCORepository.UpdateIFFCOProposalId(quoteTransactionId, request.ProposalNumber, cancellationToken);

                var updatePaymentDetails = await _quoteRepository.InsertPaymentTransaction(quoteResponse, cancellationToken);
                paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.OK;
                paymentDetailsVm.InsurerId = _iFFCOConfig.InsurerId;
                //paymentDetailsVm.QuoteTransactionId = updatePaymentDetails?.QuoteTransactionId;
                if (updatePaymentDetails != null)
                {
                    paymentDetailsVm = _mapper.Map<PaymentDetailsVm>(updatePaymentDetails);
                    return HeroResult<PaymentDetailsVm>.Success(paymentDetailsVm);
                }
                return HeroResult<PaymentDetailsVm>.Success(paymentDetailsVm);
            }
            return HeroResult<PaymentDetailsVm>.Fail("Fail to get quotetrnasactionId");
        }
    }
}
