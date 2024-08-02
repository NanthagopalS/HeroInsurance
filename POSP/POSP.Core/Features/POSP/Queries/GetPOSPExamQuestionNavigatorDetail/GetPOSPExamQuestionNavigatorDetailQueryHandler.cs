using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetPOSPExamQuestionNavigatorDetail
{
    public record GetPOSPExamQuestionNavigatorDetailQuery : IRequest<HeroResult<IEnumerable<GetPOSPExamQuestionNavigatorDetailVm>>>
    {
        public string UserId { get; set; }
        public string ExamId { get; set; }

    }


    public class GetPOSPExamQuestionNavigatorDetailQueryHandler : IRequestHandler<GetPOSPExamQuestionNavigatorDetailQuery, HeroResult<IEnumerable<GetPOSPExamQuestionNavigatorDetailVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;

        public GetPOSPExamQuestionNavigatorDetailQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPExamQuestionNavigatorDetailVm>>> Handle(GetPOSPExamQuestionNavigatorDetailQuery request, CancellationToken cancellationToken)
        {
            var getPOSPExamQuestionNavigatorDetail = await _pospRepository.GetPOSPExamQuestionNavigatorDetail(request.UserId, request.ExamId, cancellationToken).ConfigureAwait(false);
            if (getPOSPExamQuestionNavigatorDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPExamQuestionNavigatorDetailVm>>(getPOSPExamQuestionNavigatorDetail);
                return HeroResult<IEnumerable<GetPOSPExamQuestionNavigatorDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPExamQuestionNavigatorDetailVm>>.Fail("No Record Found");
        }
    }

}