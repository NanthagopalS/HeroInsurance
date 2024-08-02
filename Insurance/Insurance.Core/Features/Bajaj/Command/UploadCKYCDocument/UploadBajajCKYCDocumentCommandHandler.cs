using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.Bajaj.Command.UploadCKYCDocument;

public class UploadBajajCKYCDocumentCommand : CKYCModel, IRequest<HeroResult<UploadCKYCDocumentResponse>>
{
    public string QuoteTransactionId { get; set; }
}
public class UploadBajajCKYCDocumentCommandHandler : IRequestHandler<UploadBajajCKYCDocumentCommand, HeroResult<UploadCKYCDocumentResponse>>
{
    private readonly IBajajRepository _bajajRepository;
    public UploadBajajCKYCDocumentCommandHandler(IBajajRepository bajajRepository)
    {
        _bajajRepository = bajajRepository;
    }
    public async Task<HeroResult<UploadCKYCDocumentResponse>> Handle(UploadBajajCKYCDocumentCommand request, CancellationToken cancellationToken)
    {
        var cKYCData = await _bajajRepository.UploadCKYCDocument(request, cancellationToken);

        if (cKYCData != null)
        {
            return HeroResult<UploadCKYCDocumentResponse>.Success(cKYCData);
        }

        return HeroResult<UploadCKYCDocumentResponse>.Fail("CKYC Document Upload Fail");
    }
}
