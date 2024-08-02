using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Core.Features.HDFC.Queries.GetPaymentStatus
{
    public class GetPaymentQuery : HDFCPaymentResponseModel, IRequest<HeroResult<PaymentDetailsVm>>
    {

    }
    public class GetPaymentQueryHandler :  IRequestHandler<GetPaymentQuery, HeroResult<PaymentDetailsVm>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly IHDFCRepository _hdfcRepository;
        private readonly HDFCConfig _hdfcConfig;
        private readonly IApplicationClaims _applicationClaims;
        public GetPaymentQueryHandler(IQuoteRepository quoteRepository, IMapper mapper, IHDFCRepository hdfcRepository, IOptions<HDFCConfig> options, IApplicationClaims applicationClaims)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
            _hdfcRepository = hdfcRepository;
            _hdfcConfig = options.Value;
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        }

        public async Task<HeroResult<PaymentDetailsVm>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
        {
            var paymentDetailsVm = new PaymentDetailsVm();
           
            var quoteResponse = _mapper.Map<QuoteResponseModel>(request);
            quoteResponse.InsurerId = _hdfcConfig.InsurerId;

            var requestModel = new HDFCPolicyRequestModel()
            {
                ApplicationId = quoteResponse.ApplicationId,
                BankName= quoteResponse.BankName,
                GrossPremium = quoteResponse.GrossPremium,
                PaymentDate= quoteResponse.PaymentDate
            };

            if (request != null && request.PaymentStatus.Equals("SPD"))
            {
                quoteResponse.PaymentStatus = "Payment Completed";

                var policyDocData = await _hdfcRepository.GetPolicyDocument(requestModel, cancellationToken);

                if (policyDocData != null && policyDocData.InsurerStatusCode.Equals(200))
                {
                    quoteResponse.CustomerId= policyDocData.CustomerId;
                    quoteResponse.PolicyNumber= policyDocData.PolicyNumber;
                    if(policyDocData.PolicyDocumentBase64 != null)
                    {
                        byte[] documentBase64 = Convert.FromBase64String(policyDocData.PolicyDocumentBase64);

                        var documentId = await _quoteRepository.UploadPolicyDocumentMangoDB(documentBase64, cancellationToken);
                        if (documentId != null)
                        {
                            quoteResponse.DocumentId = documentId;
                        }
                    }
                }
            }
            else if (request != null && request.PaymentStatus.Equals("UPD"))
            {
                quoteResponse.PaymentStatus = "Payment Incomplete";
            }
            else if (request != null && request.PaymentStatus.Equals("TBC"))
            {
                quoteResponse.PaymentStatus = "Payment ToBeConfiremd";
            }
            else if (request != null && request.PaymentStatus.Equals("PPD"))
            {
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
    }
}
