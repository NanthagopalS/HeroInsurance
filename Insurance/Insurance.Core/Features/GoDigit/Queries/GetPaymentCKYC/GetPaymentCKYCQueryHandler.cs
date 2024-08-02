using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;

namespace Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC
{
    public record GetPaymentCKYCQuery : IRequest<HeroResult<PaymentDetailsVm>>
    {
        public string ApplicationId { get; set; }
        public string PaymentTransactionNumber { get; set; }
        public string LeadId { get; set; }
    }
    public class GetPaymentCKYCQueryHandler : IRequestHandler<GetPaymentCKYCQuery, HeroResult<PaymentDetailsVm>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly IGoDigitRepository _goDigitRepository;
        private readonly GoDigitConfig _goDigitConfig;

        public GetPaymentCKYCQueryHandler(IQuoteRepository quoteRepository, IMapper mapper, IGoDigitRepository goDigitRepository, IOptions<GoDigitConfig> options)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
            _goDigitRepository = goDigitRepository;
            _goDigitConfig = options.Value;
        }

        public async Task<HeroResult<PaymentDetailsVm>> Handle(GetPaymentCKYCQuery request, CancellationToken cancellationToken)
        {
            var paymentCKYCReqModel = _mapper.Map<GodigitPaymentCKYCReqModel>(request);
            var paymentDetailsVm = new PaymentDetailsVm();
            var requestLeadId = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
            {
                InsurerId = _goDigitConfig.InsurerId,
                ApplicationId = paymentCKYCReqModel.ApplicationId,
            };
            var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(requestLeadId, cancellationToken);
            paymentCKYCReqModel.LeadId = leadDetails?.LeadID;

            var paymentResponse = await _goDigitRepository.GetPaymentDetails(paymentCKYCReqModel, cancellationToken);

            if (paymentResponse != null)
            {
                var quoteResponse = _mapper.Map<QuoteResponseModel>(paymentResponse);

                if (quoteResponse.PaymentStatus.ToLower().Equals("payment completed"))
                {
                    quoteResponse.PaymentStatus = "Payment Completed";
                    paymentCKYCReqModel.PolicyNumber = paymentResponse.PolicyNumber;
                    var ckycResponse = await _goDigitRepository.GetCKYCDetails(paymentCKYCReqModel, cancellationToken);

                    if (ckycResponse != null)
                    {
                        quoteResponse.ProposalNumber = ckycResponse.ProposalNumber;
                        quoteResponse.CKYCStatus = ckycResponse.CKYCStatus;
                        quoteResponse.CKYCLink = ckycResponse.CKYCLink;
                        quoteResponse.CKYCFailReason = ckycResponse.CKYCFailedReason;
                        quoteResponse.PolicyNumber = ckycResponse.ProposalNumber;
                    }
                    if (ckycResponse != null && ckycResponse.CKYCStatus.ToLower().Equals("done"))
                    {
                        var policyDocument = await _goDigitRepository.GetPolicyDocumentPDF(paymentCKYCReqModel.LeadId, quoteResponse.ApplicationId, cancellationToken);

                        if (policyDocument != null)
                        {
                            quoteResponse.PolicyDocumentLink = policyDocument.schedulePath;

                            var documentBase64 = await _goDigitRepository.GetDocumentPDFBase64(quoteResponse.PolicyDocumentLink, cancellationToken);
                            if (documentBase64 != null)
                            {
                                var documentId = await _quoteRepository.UploadPolicyDocumentMangoDB(documentBase64, cancellationToken);
                                if (documentId != null)
                                {
                                    quoteResponse.DocumentId = documentId;
                                }
                            }
                        }
                    }
                }
                else if (quoteResponse.PaymentStatus.ToLower().Equals("payment incomplete"))
                {
                    quoteResponse.PaymentStatus = "Payment Incomplete";
                }
                var godigitPaymentCKCYResponseModel = await _quoteRepository.InsertPaymentTransaction(quoteResponse, cancellationToken);
                if (godigitPaymentCKCYResponseModel != null)
                {
                    paymentDetailsVm = _mapper.Map<PaymentDetailsVm>(godigitPaymentCKCYResponseModel);
                    paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.OK;
                    return HeroResult<PaymentDetailsVm>.Success(paymentDetailsVm);
                }
            }

            var paymentCKYCReq = new PaymentStatusRequestModel
            {
                ApplicationId = request.ApplicationId,
            };
            var paymentDetails = await _quoteRepository.GetPaymentStatus(paymentCKYCReq, cancellationToken).ConfigureAwait(false);
            if (paymentDetails != null)
            {
                paymentDetailsVm = _mapper.Map<PaymentDetailsVm>(paymentDetails);
                paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.OK;
                return HeroResult<PaymentDetailsVm>.Success(paymentDetailsVm);
            }
            return HeroResult<PaymentDetailsVm>.Success(paymentDetailsVm);
        }
    }
}
