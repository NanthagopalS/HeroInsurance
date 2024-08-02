using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.TATA.Command.CKYC;

public class TATACKYCCommand : CKYCModelAfterProposal, IRequest<HeroResult<SaveCKYCResponse>>
{
    public string QuoteTransactionId { get; set; }
}
public class TATACKYCCommandHandler : IRequestHandler<TATACKYCCommand, HeroResult<SaveCKYCResponse>>
{
    private const string ValidationMessage = "We encountered some issue, please retry or reach out to us for help";
    private readonly ITATARepository _tataRepository;
    private readonly IQuoteRepository _quoteRepository;
    private readonly TATAConfig _tataConfig;
    public TATACKYCCommandHandler(ITATARepository tATARepository, IQuoteRepository quoteRepository, IOptions<TATAConfig> options)
    {
        _tataRepository = tATARepository;
        _quoteRepository = quoteRepository;
        _tataConfig = options.Value;
    }

	public async Task<HeroResult<SaveCKYCResponse>> Handle(TATACKYCCommand request, CancellationToken cancellationToken)
	{
        TATACKYCStatusResponseModel result = new();
        var kycData = await _tataRepository.GetDetailsForKYCAfterProposal(request.QuoteTransactionId, cancellationToken);
        TATACKYCRequestModel tataCKYCRequestModel = new TATACKYCRequestModel()
        {
			IDType = request.DocumentType,
			IdNumber = request.DocumentId,
			Name = request.IsCompany ? kycData?.CompanyName :  kycData?.LeadName,
			ProposalNumber = kycData?.PolicyNumber,
			DateOfBirth = Convert.ToDateTime(kycData?.DOB).ToString("dd-MM-yyyy"),
			Gender = kycData!= null && kycData.Gender.Equals("Male") ? "M" : "F",
			ReqId = kycData?.KYC_RequestId,
			LeadId = kycData?.LeadID,
            IsCompany = request.IsCompany
        };

        if(request.IsPOI)
        {
            result = await _tataRepository.PanCKYCVerification(tataCKYCRequestModel, cancellationToken);
        }
        else
        {
            result = await _tataRepository.POACKYCVerification(tataCKYCRequestModel, cancellationToken);
        }
        
        if(result != null && result.SaveCKYCResponse != null && result.SaveCKYCResponse.InsurerStatusCode.Equals(200))
        {
            //if(result.SaveCKYCResponse.IsDocumentUpload) 
            //{

            //}

            KYCDetailsModel kycDetailsModel = new KYCDetailsModel()
            {
                LeadId = kycData?.LeadID,
                InsurerId = _tataConfig.InsurerId,
                QuoteTransactionId = request.QuoteTransactionId,
                Stage = request.IsPOI ? "POI" : "POA",
                RequestBody = result?.RequestBody,
                ResponseBody = result?.ResponseBody,
                PhotoId = result?.SaveCKYCResponse?.PhotoId,
                KYCId = result?.SaveCKYCResponse?.KYCId,
                CKYCNumber = result?.SaveCKYCResponse?.CKYCNumber,
                CKYCStatus = result?.SaveCKYCResponse?.KYC_Status
            };
            var kycInsertDetails = await _quoteRepository.InsertKYCDetailsAfterProposal(kycDetailsModel, cancellationToken);
            if (kycInsertDetails is not null)
            {
                return HeroResult<SaveCKYCResponse>.Success(result.SaveCKYCResponse);
            }
        }
        return HeroResult<SaveCKYCResponse>.Fail(ValidationMessage);

    }


}

