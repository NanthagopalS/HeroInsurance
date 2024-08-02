using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.TATA.Queries.GetBreakIn;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.TATA.Queries.GetPaymentStatus
{
    public class TATAVerifyPaymentStatus : TATAPaymentResponseModel, IRequest<HeroResult<PaymentDetailsVm>>
    {
    }

    public class TATAVerifyPaymentStatusHandle : IRequestHandler<TATAVerifyPaymentStatus, HeroResult<PaymentDetailsVm>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly ITATARepository _tataRepository;
        private readonly TATAConfig _tataConfig;
        private readonly IMapper _mapper;
        private readonly IApplicationClaims _applicationClaims;
        public TATAVerifyPaymentStatusHandle(IQuoteRepository quoteRepository, ITATARepository tataRepository, IMapper mapper, IOptions<TATAConfig> tataConfig, IApplicationClaims applicationClaims)
        {
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _tataRepository = tataRepository ?? throw new ArgumentNullException(nameof(tataRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _tataConfig = tataConfig.Value;
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        }

        public async Task<HeroResult<PaymentDetailsVm>> Handle(TATAVerifyPaymentStatus request, CancellationToken cancellationToken)
        {
            var requestModel = _mapper.Map<TATAPaymentResponseModel>(request);
            var paymentDetailsVm = new PaymentDetailsVm();

            var leadIdAndPaymentId = await _tataRepository.GetLeadIdAndPaymentId(requestModel.QuoteTransactionId, cancellationToken);

            var quoteResponse = new QuoteResponseModel()
            {
                InsurerId = _tataConfig.InsurerId,
                Type = "UPDATE",
                ApplicationId = leadIdAndPaymentId?.PaymentId
            };

            if(leadIdAndPaymentId != null)
            {
                var verifyPaymentStatus = await _tataRepository.VerifyPaymentDetails(leadIdAndPaymentId.VehicleTypeId, leadIdAndPaymentId?.PaymentId, leadIdAndPaymentId?.LeadId, cancellationToken);
                if (verifyPaymentStatus != null && verifyPaymentStatus.status.Equals(200) && verifyPaymentStatus.data.payment_status.Equals("Success"))
                {
                    quoteResponse.PaymentStatus = "Payment Completed";
                    quoteResponse.CKYCStatus = "Done";
                    quoteResponse.PaymentTransactionNumber = verifyPaymentStatus.data?.encrypted_policy_id;
                    quoteResponse.PolicyNumber = verifyPaymentStatus?.data?.policy_no;

                    var policyDocData = await _tataRepository.GetPolicyDocument(verifyPaymentStatus.data?.encrypted_policy_id, leadIdAndPaymentId.VehicleTypeId, leadIdAndPaymentId?.LeadId, cancellationToken);

                    if (policyDocData != null && policyDocData.InsurerStatusCode.Equals(200) && policyDocData.PolicyDocumentBase64 != null)
                    {
                        byte[] documentBase64 = Convert.FromBase64String(policyDocData.PolicyDocumentBase64);

                        var documentId = await _quoteRepository.UploadPolicyDocumentMangoDB(documentBase64, cancellationToken);
                        if (documentId != null)
                        {
                            quoteResponse.DocumentId = documentId;
                        }
                    }
                }
                //need to check with IC for remaining status
                else if (verifyPaymentStatus != null && 
                    (verifyPaymentStatus.status.Equals(-102) || (verifyPaymentStatus.status.Equals(200) && verifyPaymentStatus.data.payment_status.Equals("Fail"))))
                {
                    quoteResponse.CKYCStatus = "Done";
                    quoteResponse.PaymentStatus = "Payment Incomplete";
                    quoteResponse.PolicyNumber = leadIdAndPaymentId.ProposalNumber;
                }
                else if (verifyPaymentStatus != null && verifyPaymentStatus.status.Equals(200) && verifyPaymentStatus.data.payment_status.Equals("Pending"))//need to check assumed
                {
                    quoteResponse.CKYCStatus = "Done";
                    quoteResponse.PaymentStatus = "Payment Pending";
                    quoteResponse.PolicyNumber = leadIdAndPaymentId.ProposalNumber;
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
