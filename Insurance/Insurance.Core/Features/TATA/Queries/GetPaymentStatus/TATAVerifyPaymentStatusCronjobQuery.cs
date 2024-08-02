using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.TATA.Queries.GetBreakIn;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
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
    public class TATAVerifyPaymentStatusCronjobQuery : IRequest<HeroResult<PaymentDetailsVm>>
    {
        public string PaymentId { get; set; }
        public string LeadId { get; set; }
        public string VehicleTypeId { get; set; }
        public string ProposalNumber { get; set; }
    }

    public class TATAVerifyPaymentStatusCronjobQueryHandle : IRequestHandler<TATAVerifyPaymentStatusCronjobQuery, HeroResult<PaymentDetailsVm>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly ITATARepository _tataRepository;
        private readonly TATAConfig _tataConfig;
        private readonly IMapper _mapper;
        private readonly IApplicationClaims _applicationClaims;
        public TATAVerifyPaymentStatusCronjobQueryHandle(IQuoteRepository quoteRepository, ITATARepository tataRepository, IMapper mapper, IOptions<TATAConfig> tataConfig, IApplicationClaims applicationClaims)
        {
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _tataRepository = tataRepository ?? throw new ArgumentNullException(nameof(tataRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _tataConfig = tataConfig.Value;
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        }

        public async Task<HeroResult<PaymentDetailsVm>> Handle(TATAVerifyPaymentStatusCronjobQuery request, CancellationToken cancellationToken)
        {
            var paymentDetailsVm = new PaymentDetailsVm();
            var quoteResponse = new QuoteResponseModel();

            quoteResponse.InsurerId = _tataConfig.InsurerId;
            quoteResponse.ApplicationId = request.PaymentId;
            quoteResponse.Type = "UPDATE";

            var verifyPaymentStatus = await _tataRepository.VerifyPaymentDetails(request.VehicleTypeId, request.PaymentId, request.LeadId, cancellationToken);
            if(verifyPaymentStatus != null && verifyPaymentStatus.status.Equals(200) && verifyPaymentStatus.data.payment_status.Equals("Success"))
            {
                quoteResponse.PaymentStatus = "Payment Completed";
                quoteResponse.CKYCStatus = "Done";
                quoteResponse.PaymentTransactionNumber = verifyPaymentStatus.data?.encrypted_policy_id;
                quoteResponse.PolicyNumber = verifyPaymentStatus?.data?.policy_no;

                var policyDocData = await _tataRepository.GetPolicyDocument(verifyPaymentStatus.data?.encrypted_policy_id, request.VehicleTypeId, request.LeadId, cancellationToken);
                
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
                quoteResponse.PolicyNumber = request.ProposalNumber;
            }
            else if(verifyPaymentStatus != null && verifyPaymentStatus.status.Equals(200) && verifyPaymentStatus.data.payment_status.Equals("Pending"))//need to check assumed
            {
                quoteResponse.CKYCStatus = "Done";
                quoteResponse.PaymentStatus = "Payment Pending";
                quoteResponse.PolicyNumber = request.ProposalNumber;
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
    }
}
