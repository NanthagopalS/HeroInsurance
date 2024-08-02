using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Queries.GetPOSPExamQuestionNavigatorDetail;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetPOSPMessageDetail
{
    public record GetPOSPMessageDetailQuery: IRequest<HeroResult<IEnumerable<GetPOSPMessageDetailVm>>>
    {
        public string MessageKey { get; set; }

    }
    public class GetPOSPMessageDetailQueryHandler : IRequestHandler<GetPOSPMessageDetailQuery, HeroResult<IEnumerable<GetPOSPMessageDetailVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;

        public GetPOSPMessageDetailQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPMessageDetailVm>>> Handle(GetPOSPMessageDetailQuery request, CancellationToken cancellationToken)
        {
            var getPOSPMessageDetail = await _pospRepository.GetPOSPMessageDetail(request.MessageKey, cancellationToken).ConfigureAwait(false);
            if (getPOSPMessageDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPMessageDetailVm>>(getPOSPMessageDetail);
                return HeroResult<IEnumerable<GetPOSPMessageDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPMessageDetailVm>>.Fail("No Record Found");
        }
    }

}

