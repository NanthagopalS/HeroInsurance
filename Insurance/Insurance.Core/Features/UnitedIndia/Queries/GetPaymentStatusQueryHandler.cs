using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.UnitedIndia;
using Insurance.Domain.UnitedIndia.Payment;
using MediatR;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Insurance.Domain.TATA;
using Microsoft.Extensions.Logging;

namespace Insurance.Core.Features.UnitedIndia.Queries
{
    public class GetPaymentStatusQuery : InitiatePaymentRequestDto, IRequest<HeroResult<PaymentDetailsVm>>
    {

    }
    public class GetUnitedIndiaPaymentStatusQueryHandler : IRequestHandler<GetPaymentStatusQuery, HeroResult<PaymentDetailsVm>>
    {
        private readonly IUnitedIndiaRepository _unitedIndiaRepository;
        private readonly UnitedIndiaConfig _unitedIndiaConfig;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetUnitedIndiaPaymentStatusQueryHandler> _logger;
        public GetUnitedIndiaPaymentStatusQueryHandler(IUnitedIndiaRepository unitedIndiaRepository,
            IOptions<UnitedIndiaConfig> unitedIndiaConfig,
            IQuoteRepository quoteRepository, IMapper mapper,
            ILogger<GetUnitedIndiaPaymentStatusQueryHandler> logger)
        {
            _unitedIndiaRepository = unitedIndiaRepository;
            _quoteRepository = quoteRepository;
            _unitedIndiaConfig = unitedIndiaConfig.Value;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger;
        }
        public async Task<HeroResult<PaymentDetailsVm>> Handle(GetPaymentStatusQuery request, CancellationToken cancellationToken)
        {
            var requestModel = _mapper.Map<InitiatePaymentRequestDto>(request);
            var paymentDetailsVm = new PaymentDetailsVm();

            var leadIdAndPaymentId = await _unitedIndiaRepository.GetLeadIdAndPaymentId(requestModel.QuoteTransactionId, cancellationToken);
            _logger.LogInformation("UIIC GetLeadIdAndPaymentId response {response}", leadIdAndPaymentId);
            var quoteResponse = new QuoteResponseModel()
            {
                InsurerId = _unitedIndiaConfig.InsurerId,
                Type = "UPDATE",
                ApplicationId = leadIdAndPaymentId?.PaymentId
            };
            if (leadIdAndPaymentId != null)
            {
                var verifyPaymentStatus = await _unitedIndiaRepository.GetPaymentStatus(requestModel, cancellationToken);
                _logger.LogInformation("UIIC GetPaymentStatus response {response}", verifyPaymentStatus);

                if (verifyPaymentStatus != null && verifyPaymentStatus.body.resultInfo.resultCode.Equals("01") && verifyPaymentStatus.body.resultInfo.resultStatus.Equals("TXN_SUCCESS"))
                {
                    quoteResponse.PaymentStatus = "Payment Completed";
                    quoteResponse.CKYCStatus = "Done";
                    quoteResponse.PaymentTransactionNumber = verifyPaymentStatus.body?.txnId;
                    //quoteResponse.CustomerId = requestModel.userInfo.custId;

                    UIICPaymentInfoHEADER PaymentInforequestModel = new UIICPaymentInfoHEADER();
                    PaymentInforequestModel.NUM_PREMIUM_AMOUNT = verifyPaymentStatus.body?.txnAmount;
                    PaymentInforequestModel.DAT_UTR_DATE = verifyPaymentStatus.body?.txnDate;
                    PaymentInforequestModel.NUM_REFERENCE_NUMBER = request.num_reference_number;//"202311163478620";
                    PaymentInforequestModel.NUM_UTR_PAYMENT_AMOUNT = verifyPaymentStatus.body?.txnAmount;
                    PaymentInforequestModel.TXT_BANK_CODE = verifyPaymentStatus.body?.gatewayName;
                    PaymentInforequestModel.TXT_BANK_NAME = verifyPaymentStatus.body?.bankName; 
                    PaymentInforequestModel.TXT_MERCHANT_ID = verifyPaymentStatus.body?.mid;
                    PaymentInforequestModel.TXT_TRANSACTION_ID = verifyPaymentStatus.body?.orderId;
                    PaymentInforequestModel.TXT_UTR_NUMBER = verifyPaymentStatus.body?.txnId;
                    var paymentInforesponse = await _unitedIndiaRepository.GetPaymentInfo(PaymentInforequestModel, leadIdAndPaymentId.LeadId, cancellationToken);

                    _logger.LogInformation("UIIC GetPaymentInfo response {response}", paymentInforesponse);

                    if (paymentInforesponse != null && string.IsNullOrEmpty(paymentInforesponse?.Body?.paymentInfoResponse?.@return?.ROOT?.HEADER?.TXT_ERR_MSG))
                    {
                        quoteResponse.PolicyNumber = paymentInforesponse?.Body?.paymentInfoResponse?.@return?.ROOT?.HEADER?.TXT_NEW_POLICY_NUMBER;
                    }
                    else
                    {
                        //quoteConfirm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                        //quoteConfirm.ValidationMessage = response?.Body?.CalculatePremiumResponse?.@return?.ROOT?.HEADER?.TXT_ERR_MSG;
                    }
                    //var policyDocData = await _tataRepository.GetPolicyDocument(verifyPaymentStatus.data?.encrypted_policy_id, leadIdAndPaymentId.VehicleTypeId, leadIdAndPaymentId?.LeadId, cancellationToken);

                    //if (policyDocData != null && policyDocData.InsurerStatusCode.Equals(200) && policyDocData.PolicyDocumentBase64 != null)
                    //{
                    //    byte[] documentBase64 = Convert.FromBase64String(policyDocData.PolicyDocumentBase64);

                    //    var documentId = await _quoteRepository.UploadPolicyDocumentMangoDB(documentBase64, cancellationToken);
                    //    if (documentId != null)
                    //    {
                    //        quoteResponse.DocumentId = documentId;
                    //    }
                    //}
                }
                else if (verifyPaymentStatus != null && verifyPaymentStatus.body.resultInfo.resultStatus.Equals("TXN_FAILURE"))
                {
                    quoteResponse.CKYCStatus = "Done";
                    quoteResponse.PaymentStatus = "Payment Incomplete";
                }
                else if (verifyPaymentStatus != null && verifyPaymentStatus.body.resultInfo.resultStatus.Equals("PENDING"))
                {
                    quoteResponse.CKYCStatus = "Done";
                    quoteResponse.PaymentStatus = "Payment Pending";
                }
                var updatePaymentDetails = await _quoteRepository.InsertPaymentTransaction(quoteResponse, cancellationToken);
                if (updatePaymentDetails != null)
                {
                    paymentDetailsVm = _mapper.Map<PaymentDetailsVm>(updatePaymentDetails);
                    paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.OK;
                    return HeroResult<PaymentDetailsVm>.Success(paymentDetailsVm);
                }
                return HeroResult<PaymentDetailsVm>.Fail("Fail in Payment Details Updation");
            }
            else
            {
                return HeroResult<PaymentDetailsVm>.Fail("Fail to get PayentId and LeadId");
            }
        }
    }

}
