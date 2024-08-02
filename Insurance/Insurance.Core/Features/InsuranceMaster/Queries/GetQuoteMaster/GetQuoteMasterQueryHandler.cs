using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.InsuranceMaster.Queries.GetQuoteMaster;

public record GetQuoteMasterQuery : IRequest<HeroResult<QuoteMasterModel>>
{
    public string VehicleTypeId { get; set; }
    public string PolicyTypeId { get; set; }
}
public class GetQuoteMasterQueryHandler : IRequestHandler<GetQuoteMasterQuery, HeroResult<QuoteMasterModel>>
{
    private readonly IInsuranceMasterRepository _insuranceMasterRepository;
    private readonly IMapper _mapper;
    public GetQuoteMasterQueryHandler(IInsuranceMasterRepository insuranceMasterRepository, IMapper mapper)
    {
        _insuranceMasterRepository = insuranceMasterRepository ?? throw new ArgumentException(nameof(insuranceMasterRepository));
        _mapper = mapper;
    }
    public async Task<HeroResult<QuoteMasterModel>> Handle(GetQuoteMasterQuery request, CancellationToken cancellationToken)
    {
        var result = await _insuranceMasterRepository.GetQuote(request.VehicleTypeId, request.PolicyTypeId, cancellationToken).ConfigureAwait(false);
        if (result != null)
        {
            return HeroResult<QuoteMasterModel>.Success(result);
        }
        return null;
    }
}
