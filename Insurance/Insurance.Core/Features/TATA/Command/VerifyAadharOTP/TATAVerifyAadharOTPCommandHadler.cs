using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.TATA.Command.CKYC;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Core.Features.TATA.Command.VerifyAadharOTP
{
    public class TATAVerifyAadharOTPCommand : POAAadharOTPSubmitRequestModel, IRequest<HeroResult<SaveCKYCResponse>>
    {
        public string QuoteTransactionId { get; set; }
    }
    public class TATAVerifyAadharOTPCommandHadler : IRequestHandler<TATAVerifyAadharOTPCommand, HeroResult<SaveCKYCResponse>>
    {
        private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";
        private readonly ITATARepository _tataRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly TATAConfig _tataConfig;
        public TATAVerifyAadharOTPCommandHadler(ITATARepository tATARepository, IQuoteRepository quoteRepository, IOptions<TATAConfig> options)
        {
            _tataRepository = tATARepository;
            _quoteRepository = quoteRepository;
            _tataConfig = options.Value;
        }
        public async Task<HeroResult<SaveCKYCResponse>> Handle(TATAVerifyAadharOTPCommand request, CancellationToken cancellationToken)
        {
            var kycData = await _tataRepository.GetDetailsForKYCAfterProposal(request.QuoteTransactionId, cancellationToken);
            if (kycData == null)
            {
                return HeroResult<SaveCKYCResponse>.Fail(ValidationMessage);
            }
            POAAadharOTPSubmitRequestModel poaAadharOTPSubmitRequestModel = new POAAadharOTPSubmitRequestModel()
            {
                ClientId = request.ClientId,
                IdNumber = request.IdNumber,
                OTP = request.OTP,
                ProposalNo = kycData?.PolicyNumber,
                CustomerName = kycData?.LeadName,
                LeadId = kycData?.LeadID
            };
            var verifyOTPResonse = await _tataRepository.POAAadharOTPSubmit(poaAadharOTPSubmitRequestModel, cancellationToken);
            if (verifyOTPResonse != null && verifyOTPResonse.SaveCKYCResponse != null && verifyOTPResonse.SaveCKYCResponse.InsurerStatusCode.Equals(200))
            {
                KYCDetailsModel kycDetailsModel = new KYCDetailsModel()
                {
                    LeadId = kycData?.LeadID,
                    InsurerId = _tataConfig.InsurerId,
                    QuoteTransactionId = request.QuoteTransactionId,
                    Stage = "POA",
                    RequestBody = verifyOTPResonse?.RequestBody,
                    ResponseBody = verifyOTPResonse?.ResponseBody,
                    PhotoId = verifyOTPResonse?.SaveCKYCResponse?.PhotoId,
                    KYCId = verifyOTPResonse?.SaveCKYCResponse?.KYCId,
                    CKYCNumber = verifyOTPResonse?.SaveCKYCResponse?.CKYCNumber,
                    CKYCStatus = verifyOTPResonse?.SaveCKYCResponse?.KYC_Status
                };
                var kycInsertDetails = await _quoteRepository.InsertKYCDetailsAfterProposal(kycDetailsModel, cancellationToken);
                if (kycInsertDetails != null)
                {
                    return HeroResult<SaveCKYCResponse>.Success(verifyOTPResonse.SaveCKYCResponse);
                }
                return HeroResult<SaveCKYCResponse>.Fail("KYC data insertion failed");
            }
            return HeroResult<SaveCKYCResponse>.Fail(verifyOTPResonse?.SaveCKYCResponse?.Message);
        }
    }
}
