using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetTrainingMaterial;
public record GetPOSPCPTrainingMaterialQuery : IRequest<HeroResult<IEnumerable<GetPOSPTrainingMaterialDetailVm>>>
{
    public string? ModuleType { get;set; }
    public string? TrainingId { get;set; }

}
public class GetPOSPTrainingMaterialQueryHandler : IRequestHandler<GetPOSPCPTrainingMaterialQuery, HeroResult<IEnumerable<GetPOSPTrainingMaterialDetailVm>>>
{
    private readonly IPOSPRepository _pospRepository;
    private readonly IMapper _mapper;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="pospRepository"></param>
    /// <param name="mapper"></param>
    public GetPOSPTrainingMaterialQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
    {
        _pospRepository = pospRepository;
        _mapper = mapper;
    }
    public async Task<HeroResult<IEnumerable<GetPOSPTrainingMaterialDetailVm>>> Handle(GetPOSPCPTrainingMaterialQuery request, CancellationToken cancellationToken)
    {
        var POSPConfigurationDetail = await _pospRepository.GetTrainingMaterialDetail(request.ModuleType,request.TrainingId, cancellationToken).ConfigureAwait(false);
        if (POSPConfigurationDetail.Any())
        {
            var listInsurer = _mapper.Map<IEnumerable<GetPOSPTrainingMaterialDetailVm>>(POSPConfigurationDetail);
            return HeroResult<IEnumerable<GetPOSPTrainingMaterialDetailVm>>.Success(listInsurer);
        }

        return HeroResult<IEnumerable<GetPOSPTrainingMaterialDetailVm>>.Fail("No Record Found");
    }
}


