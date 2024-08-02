using FirebaseAdmin.Messaging;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Chola.Queries.GetCKYCStatus;
using Insurance.Core.Responses;
using Insurance.Domain.Chola;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Core.Features.Chola.Queries.GetPOAStatus
{
    public record GetPOAStatusQuery : IRequest<HeroResult<UploadCKYCDocumentResponse>>
    {
        public string QuoteTransactionId { get; set; }
    }
    public class GetPOAStatusHandler : IRequestHandler<GetPOAStatusQuery, HeroResult<UploadCKYCDocumentResponse>>
    {
        private readonly ICholaRepository _cholaRepository;
        private readonly CholaConfig _cholaConfig;
        private readonly IQuoteRepository _quoteRepository;

        public GetPOAStatusHandler(ICholaRepository cholaRepository, IOptions<CholaConfig> options, IQuoteRepository quoteRepository)
        {
            _cholaRepository = cholaRepository;
            _cholaConfig = options.Value;
            _quoteRepository = quoteRepository;
        }
        public async Task<HeroResult<UploadCKYCDocumentResponse>> Handle(GetPOAStatusQuery request, CancellationToken cancellationToken)
        {
            string address = string.Empty;
            var kycVerifiedModel = new UploadCKYCDocumentResponse();
            var Poaresponse = await _cholaRepository.GetKycPOA(request,cancellationToken);
            var requestData = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
            {
                QuoteTransactionId = request.QuoteTransactionId,
                InsurerId = _cholaConfig.InsurerId
            };
            var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(requestData, cancellationToken);
            Poaresponse.LeadId = leadDetails?.LeadID;

            var cKycStatusResponse = await _cholaRepository.GetCKYCStatus(Poaresponse, cancellationToken);

            if (!string.IsNullOrEmpty(cKycStatusResponse.CreateLeadModel.CKYCstatus))
            {
                CreateLeadModel createLeadModelObject = cKycStatusResponse.CreateLeadModel;

                

                var response = await _quoteRepository.SaveLeadDetails(_cholaConfig.InsurerId, Poaresponse.QuoteTransactionId, cKycStatusResponse.RequestBody, cKycStatusResponse.ResponseBody, "POI", createLeadModelObject, cancellationToken);
                if (cKycStatusResponse.CreateLeadModel.CKYCstatus.Equals("approved"))
                {
                    kycVerifiedModel.CKYCStatus = "SUCCESS";
                }
                else if (cKycStatusResponse.CreateLeadModel.CKYCstatus.Equals("Rejected"))
                {
                    kycVerifiedModel.CKYCStatus = "FAILED";
                }
                else
                {
                    kycVerifiedModel.CKYCStatus = "PENDING";
                }
                kycVerifiedModel.InsurerId = _cholaConfig.InsurerId;
                kycVerifiedModel.LeadID = response.LeadID;
                kycVerifiedModel.TransactionId = Poaresponse.QuoteTransactionId;
                kycVerifiedModel.KYCId = createLeadModelObject.kyc_id;
                kycVerifiedModel.CKYCNumber = createLeadModelObject.ckycNumber;
                kycVerifiedModel.InsurerName = _cholaConfig.InsurerName;
                kycVerifiedModel.Name = createLeadModelObject.LeadName;
                kycVerifiedModel.DOB = createLeadModelObject.DOB;
                kycVerifiedModel.Gender = createLeadModelObject.Gender;
                
                address = string.IsNullOrEmpty(createLeadModelObject.PermanentAddress.Address1) ? string.Empty : createLeadModelObject.PermanentAddress.Address1 + " ";
                address += string.IsNullOrEmpty(createLeadModelObject.PermanentAddress.Address2) ? string.Empty : createLeadModelObject.PermanentAddress.Address2 + " ";
                address += string.IsNullOrEmpty(createLeadModelObject.PermanentAddress.Address3) ? string.Empty : createLeadModelObject.PermanentAddress.Address3 + " ";
                address += string.IsNullOrEmpty(createLeadModelObject.PermanentAddress.Pincode) ? string.Empty : createLeadModelObject.PermanentAddress.Pincode;
                
                kycVerifiedModel.Address = address.Trim();

                return HeroResult<UploadCKYCDocumentResponse>.Success(kycVerifiedModel);
            }
            return HeroResult<UploadCKYCDocumentResponse>.Fail("Fail to Fetch CKYC POA Details");
        }
    }
}
