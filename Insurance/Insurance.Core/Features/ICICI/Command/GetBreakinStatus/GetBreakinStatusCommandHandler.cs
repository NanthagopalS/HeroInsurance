using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Core.Features.ICICI.Command.GetBreakinStatus;

public record GetBreakinStatusQuery : IRequest<HeroResult<string>>
{
}

public class GetBreakinStatusCommandHandler : IRequestHandler<GetBreakinStatusQuery, HeroResult<string>>
{
    private readonly IICICIRepository _iCICIRepository;
    private readonly ICICIConfig _iciciConfig;
    private readonly IQuoteRepository _quoteRepository;
    public GetBreakinStatusCommandHandler(IICICIRepository iCICIRepository, IOptions<ICICIConfig> iciciConfig, IQuoteRepository quoteRepository)
    {
        _iCICIRepository = iCICIRepository;
        _iciciConfig = iciciConfig.Value;
        _quoteRepository = quoteRepository;
    }
    public async Task<HeroResult<string>> Handle(GetBreakinStatusQuery request, CancellationToken cancellationToken)
    {
        var breakinStatusResponse = await _iCICIRepository.BreakinInspectionStatus(cancellationToken);
        var leadModel = new CreateLeadModel();
        if (breakinStatusResponse == null)
        {
            return HeroResult<string>.Fail("Vehicle Inspection is In Progress," + _iciciConfig.InsurerName + " team will reach out for conducting inspection.");
        }

        bool isBreakinApproved = false;

        if (breakinStatusResponse.status && breakinStatusResponse.inspectionStatus.Equals("Recommended", StringComparison.Ordinal))
        {
            isBreakinApproved = true;
        }

        string cleraInspection = await _iCICIRepository.ClearBreakinInspectionStatus(breakinStatusResponse?.breakInInsuranceID,
                                                                              isBreakinApproved,
                                                                              breakinStatusResponse?.correlationId,
                                                                              cancellationToken);

        if (!string.IsNullOrWhiteSpace(cleraInspection) && isBreakinApproved)
        {
            leadModel = new CreateLeadModel()
            {
                IsBreakinApproved = isBreakinApproved,
                Stage = "Proposal",
                BreakinId = breakinStatusResponse.breakInInsuranceID,
            };
            await _iCICIRepository.UpdateBreakinStatus(leadModel, cancellationToken);
            return HeroResult<string>.Success($"Vehicle Inspection is Completed Successfully with {_iciciConfig.InsurerName}.Inspection ID: {breakinStatusResponse.breakInInsuranceID}");
        }
        else if(!string.IsNullOrWhiteSpace(cleraInspection) && !isBreakinApproved)
        {
            leadModel.IsBreakinApproved = isBreakinApproved;
            leadModel.BreakinId = breakinStatusResponse.breakInInsuranceID;
            await _iCICIRepository.UpdateBreakinStatus(leadModel, cancellationToken);
            return HeroResult<string>.Fail($"Vehicle Inspection is Not Successful with {_iciciConfig.InsurerName}.Inspection ID: {breakinStatusResponse.breakInInsuranceID}.Please retry with other insurer or reach out to us for assistance.");
        }
        return HeroResult<string>.Fail("Vehicle Inspection is In Progress," + _iciciConfig.InsurerName + " team will reach out for conducting inspection.");
    }
}
