using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetProductCategory
{
    public record GetProductCategoryQuery : IRequest<HeroResult<IEnumerable<GetProductCategoryVm>>>
    {

    }


    public class GetProductCategoryQueryHandler : IRequestHandler<GetProductCategoryQuery, HeroResult<IEnumerable<GetProductCategoryVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetProductCategoryQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetProductCategoryVm>>> Handle(GetProductCategoryQuery request, CancellationToken cancellationToken)
        {
            var getExamInstructionsDetail = await _pospRepository.GetProductCategory(cancellationToken).ConfigureAwait(false);
            if (getExamInstructionsDetail is not null)
            {
                var listInsurer = _mapper.Map<IEnumerable<GetProductCategoryVm>>(getExamInstructionsDetail);
                return HeroResult<IEnumerable<GetProductCategoryVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetProductCategoryVm>>.Fail("No Record Found");
        }
    }
}
