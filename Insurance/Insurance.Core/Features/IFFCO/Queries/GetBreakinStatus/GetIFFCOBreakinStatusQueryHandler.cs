using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.IFFCO.Queries.GetBreakinStatus;


public record GetIFFCOBreakinStatusQuery : IRequest<HeroResult<string>>
{
    public string BreakinId { get; set; }
    public string QuoteTransactionId { get; set; }
    public string ProposalNumber { get; set; }
    public string LeadId { get; set; }
}

public class GetIFFCOBreakinStatusQueryHandler : IRequestHandler<GetIFFCOBreakinStatusQuery, HeroResult<string>>
{
    private readonly IIFFCORepository _iFFCORepository;
    private readonly IFFCOConfig _iFFCOConfig;
    private readonly IQuoteRepository _quoteRepository;
    public GetIFFCOBreakinStatusQueryHandler(IIFFCORepository iFFCORepository, IOptions<IFFCOConfig> options, IQuoteRepository quoteRepository)
    {
        _iFFCORepository = iFFCORepository;
        _iFFCOConfig = options.Value;
        _quoteRepository = quoteRepository;
    }
    public async Task<HeroResult<string>> Handle(GetIFFCOBreakinStatusQuery request, CancellationToken cancellationToken)
    {
        var leadDetailsrequest = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
        {
            QuoteTransactionId = request.QuoteTransactionId,
            InsurerId = _iFFCOConfig.InsurerId
        };
        var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(leadDetailsrequest, cancellationToken);
        request.LeadId = leadDetails?.LeadID;

        var breakinStatusResponse = await _iFFCORepository.BreakinInspectionStatus(request.LeadId, request.BreakinId, cancellationToken);
        var leadModel = new CreateLeadModel();
        leadModel.QuoteTransactionID = request.QuoteTransactionId;
        bool isBreakinApproved = false;
        if (breakinStatusResponse != null)
        {
            if (breakinStatusResponse.Status.Equals("APPROVED"))
            {
                isBreakinApproved = true;
                leadModel.BreakInStatus = isBreakinApproved;
                leadModel.Stage = "Proposal";
                leadModel.BreakinId = request.BreakinId;
                leadModel.InsurerId = _iFFCOConfig.InsurerId;
                var updateLead = _quoteRepository.UpdateLeadDetails(leadModel, cancellationToken);
                return HeroResult<string>.Success($"Vehicle Inspection is Completed Successfully with {_iFFCOConfig.InsurerName}.Inspection ID: {request.BreakinId}");
            }
            else if (breakinStatusResponse.Status.Equals(""))
            {
                leadModel.IsBreakinApproved = false;
                var updateLead = _quoteRepository.UpdateLeadDetails(leadModel, cancellationToken);
                return HeroResult<string>.Fail($"Vehicle Inspection is Not Successful with {_iFFCOConfig.InsurerName}.Inspection ID: {request.BreakinId}.Please retry with other insurer or reach out to us for assistance.");
            }
            else 
            {
                return HeroResult<string>.Fail($"Vehicle Inspection is In Progress,{_iFFCOConfig.InsurerName} team will reach out for conducting inspection.");
            }
        }
        return HeroResult<string>.Fail("No Record Found");
    }
}
