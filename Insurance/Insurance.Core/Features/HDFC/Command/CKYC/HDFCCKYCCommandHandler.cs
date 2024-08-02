using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.HDFC;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.HDFC.Command.CKYC;

public class HDFCCKYCCommand : CKYCModel, IRequest<HeroResult<SaveCKYCResponse>>
{
    public string QuoteTransactionId { get; set; }
    
}
public class HDFCCKYCCommandHandler : IRequestHandler<HDFCCKYCCommand, HeroResult<SaveCKYCResponse>>
{
    private readonly IHDFCRepository _hdfcRepository;
    private readonly IQuoteRepository _quoteRepository;
    private readonly HDFCConfig _hdfcConfig;
    public HDFCCKYCCommandHandler(IHDFCRepository hDFCRepository, IQuoteRepository quoteRepository, IOptions<HDFCConfig> options)
    {
        _hdfcRepository = hDFCRepository;
        _quoteRepository = quoteRepository;
        _hdfcConfig = options.Value;
    }

    public async Task<HeroResult<SaveCKYCResponse>> Handle(HDFCCKYCCommand request, CancellationToken cancellationToken)
    {
        // Get LeadId From QuoteTransaction Id
        var leadDetailsrequest = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
        {
            QuoteTransactionId = request.QuoteTransactionId,
            InsurerId = _hdfcConfig.InsurerId
        };
        var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(leadDetailsrequest, cancellationToken);
        request.LeadId = leadDetails?.LeadID;

        var ckycResponse = await _hdfcRepository.SaveCKYC(request, cancellationToken);

        var dataSaveResponse = await _quoteRepository.SaveLeadDetails(_hdfcConfig.InsurerId, request.QuoteTransactionId,  ckycResponse.Item1, ckycResponse.Item2, "POI", ckycResponse.Item4,cancellationToken);

        SaveCKYCResponse saveCKYCResponse = ckycResponse.Item3;
        saveCKYCResponse.LeadID = dataSaveResponse.LeadID;
        saveCKYCResponse.CKYCNumber = dataSaveResponse.CKYCNumber;
        saveCKYCResponse.KYCId = dataSaveResponse.KYCId;

        if (ckycResponse is null)
            return HeroResult<SaveCKYCResponse>.Fail("CKYC Failed");

        return HeroResult<SaveCKYCResponse>.Success(saveCKYCResponse);
    }
}
