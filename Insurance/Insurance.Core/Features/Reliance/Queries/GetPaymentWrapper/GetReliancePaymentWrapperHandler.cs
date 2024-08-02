using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Responses;
using Insurance.Domain.Reliance;
using Insurance.Domain.GoDigit;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;
using Insurance.Domain.Quote;

namespace Insurance.Core.Features.Reliance.Queries.GetPaymentWrapper
{
    public class GetReliancePaymentWrapperQuery : ReliancePaymentResponseModel, IRequest<HeroResult<PaymentDetailsVm>>
    {
       
    }
    public class GetReliancePaymentWrapperQueryHandler : IRequestHandler<GetReliancePaymentWrapperQuery, HeroResult<PaymentDetailsVm>>
    {
        private readonly IMapper _mapper;
        private readonly IRelianceRepository _relianceRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly RelianceConfig _relianceConfig;
        private readonly IIFFCORepository _iFFCORepository;
        public GetReliancePaymentWrapperQueryHandler(IMapper mapper, IRelianceRepository relianceRepository, IQuoteRepository quoteRepository, IOptions<RelianceConfig> options, IIFFCORepository iFFCORepository)
        {
            _mapper = mapper;
            _relianceRepository = relianceRepository;
            _quoteRepository = quoteRepository;
            _relianceConfig = options.Value;
            _iFFCORepository = iFFCORepository;
        }

        public async Task<HeroResult<PaymentDetailsVm>> Handle(GetReliancePaymentWrapperQuery request, CancellationToken cancellationToken)
        {
            PaymentDetailsVm paymentDetailsVm = new PaymentDetailsVm();

            var reqmodel = _mapper.Map<QuoteResponseModel>(request);
            reqmodel.InsurerId = _relianceConfig.InsurerId;
            reqmodel.ApplicationId = request.ProposalNumber;
            if (request != null &&  request.TransactionStatus.Equals("Success") && !string.IsNullOrEmpty(request.PoliCyNumber))
            {
                reqmodel.PaymentStatus = "Payment Completed";
                reqmodel.BankPaymentRefNum = request.TransactionNumber;
                var productCode = !string.IsNullOrEmpty(request.ProductCode) ? request.ProductCode : "2311";
                // Get LeadId From QuoteTransaction Id
                var leadDetailsrequest = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
                {
                    QuoteTransactionId = request.QuoteTransactionId,
                    InsurerId = _relianceConfig.InsurerId
                };
                var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(leadDetailsrequest, cancellationToken);
                
                var res = await _relianceRepository.GetPaymentDetails(leadDetails?.LeadID, request.PoliCyNumber, productCode, cancellationToken);
                if (res != null && res.Data != null && res.Data.DocumentBase64 != null)
                {
                    reqmodel.ProposalNumber = request.ProposalNumber;
                    reqmodel.PolicyNumber = request.PoliCyNumber;
                    reqmodel.ApplicationId = request.ProposalNumber;
                    reqmodel.PolicyDocumentLink = res.Data.PolicDocumentLink;
                    var documentId = await _quoteRepository.UploadPolicyDocumentMangoDB(res.Data.PdfBase64,cancellationToken);
                    if (documentId != null)
                    {
                        reqmodel.DocumentId = documentId;
                    }
                }
            }
            else if (request != null && request.TransactionStatus.Equals("Failure"))
            {
                reqmodel.PaymentStatus = "Payment Incomplete";
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
