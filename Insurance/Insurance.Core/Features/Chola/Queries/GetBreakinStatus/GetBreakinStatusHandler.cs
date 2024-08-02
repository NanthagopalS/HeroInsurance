using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Chola;
using Insurance.Domain.ICICI;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.Chola.Queries.GetBreakinStatus
{
    public record GetBreakinStatusQuery : IRequest<HeroResult<string>>
    {
        public string ReferenceNumber { get; set; }
        public string LeadId { get; set; }
    }
    public class GetBreakinStatusHandler:IRequestHandler<GetBreakinStatusQuery,HeroResult<string>>
    {
        private readonly ICholaRepository _cholaRepository;
        private readonly CholaConfig _cholaConfig;
        public GetBreakinStatusHandler(ICholaRepository cholaRepository, IOptions<CholaConfig> options )
        {
            _cholaRepository = cholaRepository;
            _cholaConfig = options.Value;
        }

        public async Task<HeroResult<string>> Handle(GetBreakinStatusQuery request, CancellationToken cancellationToken)
        {
            var stage = string.Empty;
            var breakInResponse = await _cholaRepository.GetBreakInStatus(request, cancellationToken);
            if (breakInResponse != null)
            {
                if (breakInResponse.Status.ToUpper().Contains("APPROVED"))
                {
                    stage = "Proposal";
                    await _cholaRepository.UpdatedIsBreakinApproved(breakInResponse.ReferenceNumber, true, stage, breakInResponse.LastUpdatedDate.ToString(), breakInResponse.IntermediaryName, cancellationToken);
                    return HeroResult<string>.Success($"Vehicle Inspection is Completed Successfully with {_cholaConfig.InsurerName}.Inspection ID: {breakInResponse.ReferenceNumber}");
                }
                else if(breakInResponse.Status.ToUpper().Contains("REJECTED") && !string.IsNullOrEmpty(breakInResponse.Status))
                {
                    stage   = "BreakIn";
                    await _cholaRepository.UpdatedIsBreakinApproved(breakInResponse.ReferenceNumber, false, stage, DateTime.Now.ToString(),string.Empty,cancellationToken);
                    return HeroResult<string>.Fail($"Vehicle Inspection is Not Successful with {_cholaConfig.InsurerName}.Inspection ID: {breakInResponse.ReferenceNumber}.Please retry with other insurer or reach out to us for assistance.");
                }
                return HeroResult<string>.Fail("Vehicle Inspection is In Progress," + _cholaConfig.InsurerName + " team will reach out for conducting inspection.");
            }
            return HeroResult<string>.Fail("Failed to Get Data From GetBreakInStatus API");
        }
    }
}
