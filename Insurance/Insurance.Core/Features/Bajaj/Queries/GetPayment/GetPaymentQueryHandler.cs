using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Responses;
using Insurance.Domain.Bajaj;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Quote;
using Insurance.Domain.Reliance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace Insurance.Core.Features.Bajaj.Queries.GetPayment;

public record GetPaymentQuery : IRequest<HeroResult<PaymentDetailsVm>>
{
    public string Status { get; set; }
    public string Amount { get; set; }
    public string TransactionNo { get; set; }
    public string ReferenceNo { get; set; }
    public string EndorsementNo { get; set; }
    public string QuoteNo { get; set; }
    public string TransactionId { get; set; }
    public bool IsTP { get; set; }
    public string LeadId { get; set; }
}
public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, HeroResult<PaymentDetailsVm>>
{
    private readonly IMapper _mapper;
    private readonly IBajajRepository _bajajRepository;
    private readonly IQuoteRepository _quoteRepository;
    private readonly BajajConfig _bajajConfig;
    public GetPaymentQueryHandler(IMapper mapper, IBajajRepository bajajRepository, IQuoteRepository quoteRepository, IOptions<BajajConfig> options)
    {
        _mapper = mapper;
        _bajajRepository = bajajRepository;
        _quoteRepository = quoteRepository;
        _bajajConfig = options.Value;
    }

    public async Task<HeroResult<PaymentDetailsVm>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
    {
        PaymentCKCYResponseModel paymentResponseModel = new PaymentCKCYResponseModel();
        PaymentDetailsVm paymentDetailsVm = new PaymentDetailsVm();
        var quoteResponse = _mapper.Map<QuoteResponseModel>(request);
        var leadDetailsrequest = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
        {
            ApplicationId = quoteResponse.ApplicationId,
            InsurerId = _bajajConfig.InsurerId
        };
        var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(leadDetailsrequest, cancellationToken);
        bool paymentStatus = request.Status.ToLower().Equals("success") || request.Status.ToLower().Equals("y");
        if(paymentStatus)//&& request.ReferenceNo != null
        {
            quoteResponse.PaymentStatus = "Payment Completed";
        }
        //else if (paymentStatus && request.ReferenceNo == null)
        //{
        //    quoteResponse.PaymentStatus = "Payment received, process pending";
        //}
        else
        {
            quoteResponse.PaymentStatus = "Payment Incomplete";
        }

        if (paymentStatus && !string.IsNullOrWhiteSpace(quoteResponse.PolicyNumber))
        {
            if (request.IsTP)
            {
                if (request.Status.ToLower().Equals("success") &&  !string.IsNullOrWhiteSpace(request.ReferenceNo))
                {
                    var generatePolicyResponse = await _bajajRepository.GetPolicy(leadDetails?.LeadID, request.ReferenceNo, request.IsTP);
                    if (generatePolicyResponse != null)
                    {
                        string documentId = await _quoteRepository.UploadPolicyDocumentMangoDB(generatePolicyResponse.Item1, cancellationToken);
                        quoteResponse.DocumentId = documentId;
                    }
                }
            }
            else
            {
                quoteResponse.IsTP = false;
                if (request.Status.ToLower().Equals("y") && !string.IsNullOrWhiteSpace(request.ReferenceNo))
                {
                    var generatePolicyResponse = await _bajajRepository.GetPolicy(leadDetails?.LeadID, request.ReferenceNo, request.IsTP);
                    if (generatePolicyResponse != null)
                    {
                        string documentId = await _quoteRepository.UploadPolicyDocumentMangoDB(generatePolicyResponse.Item1, cancellationToken);
                        quoteResponse.DocumentId = documentId;
                    }
                }
            }
        }
        paymentResponseModel = await _quoteRepository.InsertPaymentTransaction(quoteResponse, cancellationToken).ConfigureAwait(false);


        if (paymentResponseModel != null)
        {
            paymentDetailsVm = _mapper.Map<PaymentDetailsVm>(paymentResponseModel);
            paymentDetailsVm.ProposalNumber = request.TransactionId;
            paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.OK;
            return HeroResult<PaymentDetailsVm>.Success(paymentDetailsVm);
        }

        paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
        return HeroResult<PaymentDetailsVm>.Fail("Failed to get the payment status");
    }
}
