using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.IFFCO.Command.CreateCKYC;

public class CreateIFFCOCKYCCommand : CKYCModel, IRequest<HeroResult<UploadCKYCDocumentResponse>>
{
    public string QuoteTransactionId { get; set; }
    public string LeadId { get; set; }
    public string VehicleTypeId { get; set; }
}
public class CreateIFFCOCKYCCommandHandler : IRequestHandler<CreateIFFCOCKYCCommand, HeroResult<UploadCKYCDocumentResponse>>
{
    private readonly IIFFCORepository _iFFCORepository;
    private readonly IQuoteRepository _quoteRepository;
    private readonly IFFCOConfig _iFFCOConfig;
    public CreateIFFCOCKYCCommandHandler(IIFFCORepository iFFCORepository, IOptions<IFFCOConfig> option, IQuoteRepository quoteRepository)
    {
        _iFFCORepository = iFFCORepository;
        _iFFCOConfig = option.Value;
        _quoteRepository = quoteRepository;
    }
    public async Task<HeroResult<UploadCKYCDocumentResponse>> Handle(CreateIFFCOCKYCCommand request, CancellationToken cancellationToken)
    {
        var leadDetailsrequest = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
        {
            QuoteTransactionId = request.QuoteTransactionId,
            InsurerId = _iFFCOConfig.InsurerId
        };
        var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(leadDetailsrequest, cancellationToken);
        request.LeadId = leadDetails?.LeadID;
        request.VehicleTypeId = leadDetails?.VehicleTypeId;

        var ckycResponse = await _iFFCORepository.UploadCKYCDocument(request, cancellationToken).ConfigureAwait(false);

        if (ckycResponse != null && ckycResponse.CKYCStatus.Equals("POA_SUCCESS"))
        {
            return HeroResult<UploadCKYCDocumentResponse>.Success(ckycResponse);
        }
        return HeroResult<UploadCKYCDocumentResponse>.Fail(ckycResponse?.Message);
    }
}
