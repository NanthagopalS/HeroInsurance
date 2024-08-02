using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.ICICI.Command.UploadICICICKYCDocument;

public class UploadICICICKYCDocumentCommand : CKYCModel, IRequest<HeroResult<UploadCKYCDocumentResponse>>
{
    public string QuoteTransactionId { get; set; }
}
public class UploadICICICKYCDocumentCommandHandler : IRequestHandler<UploadICICICKYCDocumentCommand, HeroResult<UploadCKYCDocumentResponse>>
{
    private readonly IICICIRepository _iCICIRepository;
    public UploadICICICKYCDocumentCommandHandler(IICICIRepository iCICIRepository)
    {
        _iCICIRepository = iCICIRepository;
    }

    public async Task<HeroResult<UploadCKYCDocumentResponse>> Handle(UploadICICICKYCDocumentCommand request, CancellationToken cancellationToken)
    {
        var cKYCData = await _iCICIRepository.UploadCKYCDocument(request, cancellationToken).ConfigureAwait(false);

        if (cKYCData != null)
        {
            return HeroResult<UploadCKYCDocumentResponse>.Success(cKYCData);
        }
        return HeroResult<UploadCKYCDocumentResponse>.Fail("CKYC Document Upload Fail");
    }
}
