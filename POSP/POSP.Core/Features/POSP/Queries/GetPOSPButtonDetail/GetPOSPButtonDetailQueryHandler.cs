using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Queries.GetPOSPMessageDetail;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetPOSPButtonDetail
{
    public record GetPOSPButtonDetailQuery : IRequest<HeroResult<IEnumerable<GetPOSPButtonDetailVm>>>
    {
        public string UserId { get; set; }

    }
    public class GetPOSPButtonDetailQueryHandler : IRequestHandler<GetPOSPButtonDetailQuery, HeroResult<IEnumerable<GetPOSPButtonDetailVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;

        public GetPOSPButtonDetailQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPButtonDetailVm>>> Handle(GetPOSPButtonDetailQuery request, CancellationToken cancellationToken)
        {
            var getPOSPButtonDetail = await _pospRepository.GetPOSPButtonDetail(request.UserId, cancellationToken).ConfigureAwait(false);
            if (getPOSPButtonDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPButtonDetailVm>>(getPOSPButtonDetail);
                return HeroResult<IEnumerable<GetPOSPButtonDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPButtonDetailVm>>.Fail("No Record Found");
        }
    }
   
}
