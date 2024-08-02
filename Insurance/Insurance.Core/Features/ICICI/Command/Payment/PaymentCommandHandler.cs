using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using System.Net;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Insurance.Domain.Bajaj;

namespace Insurance.Core.Features.ICICI.Command.Payment;

public class PaymentCommandHandler : IRequestHandler<PaymentCommand, HeroResult<PaymentDetailsVm>>
{
    private readonly IICICIRepository _iCICIRepository;
    private readonly IMapper _mapper;
    private readonly IQuoteRepository _quoteRepository;
    private readonly InsurerIdConfig _insurerIdConfig;
    public PaymentCommandHandler(IICICIRepository iCICIRepository,
                                 IMapper mapper,
                                 IQuoteRepository quoteRepository,
                                 IOptions<InsurerIdConfig> option)
    {
        _iCICIRepository = iCICIRepository;
        _mapper = mapper;
        _quoteRepository = quoteRepository;
        _insurerIdConfig = option.Value;
    }

    public async Task<HeroResult<PaymentDetailsVm>> Handle(PaymentCommand request, CancellationToken cancellationToken)
    {
        ICICIPaymentTaggingResponseDto paymentTaggingResponse;
        QuoteResponseModel quoteResponse = new QuoteResponseModel();
        PaymentCKCYResponseModel paymentResponse = new PaymentCKCYResponseModel();
        PaymentDetailsVm paymentDetailsVm = new PaymentDetailsVm();
        paymentDetailsVm.InsurerId = _insurerIdConfig.ICICI;
        quoteResponse.ApplicationId = request.CorrelationId;

        var leadDetailsrequest = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
        {
            ApplicationId = request.CorrelationId,
            InsurerId = _insurerIdConfig.ICICI
        };
        var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(leadDetailsrequest, cancellationToken);

        var transactionEnquiryResponse = await _iCICIRepository.GetPaymentEnquiry(request.TransactionId, leadDetails.LeadID, cancellationToken);

        if (transactionEnquiryResponse != null) //&& transactionEnquiryResponse.Status == 0
        {
            var paymentTagging = await _iCICIRepository.GetPaymentTagging(transactionEnquiryResponse, request.TransactionId, request.CorrelationId);

            if (paymentTagging != null)
            {
                paymentTaggingResponse = paymentTagging.Item1;
                quoteResponse.InsurerStatusCode = paymentTaggingResponse.iCICIPaymentTaggingResponse.statusMessage.ToLower().Equals("success") ? 200 : 400;
                quoteResponse.TransactionID = paymentTaggingResponse.QuoteTransactionId;
                if (paymentTaggingResponse.iCICIPaymentTaggingResponse?.paymentMappingResponse != null)
                {
                    var paymentMapResponse = paymentTaggingResponse.iCICIPaymentTaggingResponse?.paymentMappingResponse;
                    if (paymentMapResponse != null && paymentMapResponse.paymentMapResponseList.Any())
                    {
                        quoteResponse.ProposalNumber = paymentMapResponse.paymentMapResponseList.FirstOrDefault().proposalNo;
                        quoteResponse.PolicyNumber = paymentMapResponse.paymentMapResponseList.FirstOrDefault().policyNo;
                        paymentTaggingResponse.PolicyNumber = paymentMapResponse.paymentMapResponseList.FirstOrDefault().policyNo;
                    }
                }
                else
                {
                    var paymentTagResponse = paymentTaggingResponse.iCICIPaymentTaggingResponse?.paymentTagResponse;
                    if (paymentTagResponse != null && paymentTagResponse.paymentTagResponseList.Any())
                    {
                        quoteResponse.ProposalNumber = paymentTagResponse.paymentTagResponseList.FirstOrDefault().proposalNo;
                        quoteResponse.PolicyNumber = paymentTagResponse.paymentTagResponseList.FirstOrDefault().policyNo;
                        paymentTaggingResponse.PolicyNumber = paymentTagResponse.paymentTagResponseList.FirstOrDefault().policyNo;
                    }
                }
                quoteResponse.GrossPremium = paymentTaggingResponse.Amount;
                quoteResponse.PaymentStatus = paymentTaggingResponse.iCICIPaymentTaggingResponse.statusMessage;
                quoteResponse.CustomerId = paymentTaggingResponse.CustomerId;
                quoteResponse.PaymentTransactionNumber = paymentTaggingResponse.iCICIPaymentTaggingResponse?.paymentEntryResponse?.paymentID;
                quoteResponse.PaymentStatus = "Payment Completed";
                quoteResponse.Type = "UPDATE";
                quoteResponse.CKYCStatus = "DONE";
                quoteResponse.BankPaymentRefNum = transactionEnquiryResponse?.PGtransactionId;
                quoteResponse.PaymentDate = DateTime.Now.ToString("yyyy-MM-dd");
                var policy = await _iCICIRepository.GetPloicy(paymentTaggingResponse, paymentTagging.Item2);
                if (policy != null)
                {
                    string documentId = await _quoteRepository.UploadPolicyDocumentMangoDB(policy, cancellationToken);
                    quoteResponse.DocumentId = documentId;
                }
            }
            else
            {
                quoteResponse.Type = "UPDATE";
                quoteResponse.PaymentStatus = "Payment Failed";
                quoteResponse.BankPaymentRefNum = transactionEnquiryResponse?.PGtransactionId;
                quoteResponse.PaymentDate = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }
        else
        {
            quoteResponse.Type = "UPDATE";
            quoteResponse.PaymentStatus = "Payment Failed";
            quoteResponse.BankPaymentRefNum = transactionEnquiryResponse?.PGtransactionId;
            quoteResponse.PaymentDate = DateTime.Now.ToString("yyyy-MM-dd");
        }
        paymentResponse = await _quoteRepository.InsertPaymentTransaction(quoteResponse, cancellationToken);
        if (paymentResponse != null)
        {
            paymentDetailsVm = _mapper.Map<PaymentDetailsVm>(paymentResponse);
            paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.OK;
            return HeroResult<PaymentDetailsVm>.Success(paymentDetailsVm);
        }
        return HeroResult<PaymentDetailsVm>.Fail("Fail in Payment Details Updation");
    }
}
