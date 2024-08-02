using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Responses;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;

namespace Insurance.Core.Features.Chola.Queries.GetPaymentWrapper
{
    public class GetPaymentWrapperQuery : CholaPaymentResponseModel, IRequest<HeroResult<PaymentDetailsVm>>
    {
       
    }
    public class GetPaymentWrapperQueryHandler : IRequestHandler<GetPaymentWrapperQuery, HeroResult<PaymentDetailsVm>>
    {
        private readonly IMapper _mapper;
        private readonly ICholaRepository _cholaRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly CholaConfig _cholaConfig;
        public GetPaymentWrapperQueryHandler(IMapper mapper, ICholaRepository cholaRepository, IQuoteRepository quoteRepository, IOptions<CholaConfig> options)
        {
            _mapper = mapper;
            _cholaRepository = cholaRepository;
            _quoteRepository = quoteRepository;
            _cholaConfig = options.Value;
        }

        public async Task<HeroResult<PaymentDetailsVm>> Handle(GetPaymentWrapperQuery request, CancellationToken cancellationToken)
        {
            PaymentDetailsVm paymentDetailsVm = new PaymentDetailsVm();
            var reqmodel = _mapper.Map<QuoteResponseModel>(request);
            reqmodel.InsurerId = _cholaConfig.InsurerId;

            var requestLead = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
            {
                ApplicationId = reqmodel?.ApplicationId,
                InsurerId = _cholaConfig.InsurerId
            };
            var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(requestLead, cancellationToken);
            if (request != null &&  request.AuthStatus.Equals("0300"))
            {
                reqmodel.PaymentStatus = "Payment Completed";
                reqmodel.BankPaymentRefNum = request.TxnReferenceNo;
                var requestmodel = new CholaPaymentTaggingRequestModel()
                {
                    Amount = reqmodel.GrossPremium,
                    PaymentId = reqmodel.ApplicationId,
                    TransactionDate = reqmodel.PaymentDate,
                    TransactionReferenceNumber = reqmodel.PaymentTransactionNumber,
                    LeadId = leadDetails?.LeadID
                };
                var res = await _cholaRepository.GetPaymentDetails(requestmodel, cancellationToken);
                if (res != null && res.Data != null && res.Data.policy_id != null)
                {
                    reqmodel.ProposalNumber = res.Data.policy_id;
                    reqmodel.PolicyNumber = res.Data.policy_no;
                    reqmodel.PolicyDocumentLink = res.Data.PolicyURL;
                    var documentId = await _quoteRepository.UploadPolicyDocumentMangoDB(res.Data.PdfBase64,cancellationToken);
                    if (documentId != null)
                    {
                        reqmodel.DocumentId = documentId;
                    }
                }
            }
            else if (request != null && request.AuthStatus.Equals("0002"))
            {
                reqmodel.PaymentStatus = "Payment Pending";
            }
            else
            {
                reqmodel.PaymentStatus = "Payment Incomplete";
            }
           var insertPaymentData = await _quoteRepository.InsertPaymentTransaction(reqmodel, cancellationToken);
            if (insertPaymentData != null)
            {
                paymentDetailsVm = _mapper.Map<PaymentDetailsVm>(insertPaymentData);
                paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.OK;
                return HeroResult<PaymentDetailsVm>.Success(paymentDetailsVm);
            }
            return HeroResult<PaymentDetailsVm>.Fail("Fail in Payment Details Updation");
        }
    }
}
