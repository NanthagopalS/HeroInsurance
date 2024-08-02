using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetPOSPStageDetail
{
    public record GetPOSPStageDetailQuery : IRequest<HeroResult<IEnumerable<GetPOSPStageDetailVm>>>;
    public class GetPOSPStageDetailQueryHandler : IRequestHandler<GetPOSPStageDetailQuery, HeroResult<IEnumerable<GetPOSPStageDetailVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPStageDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPStageDetailVm>>> Handle(GetPOSPStageDetailQuery request, CancellationToken cancellationToken)
        {
            var getPOSPStageDetail = await _userRepository.GetPOSPStageDetail(cancellationToken).ConfigureAwait(false);
            if (getPOSPStageDetail != null)
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPStageDetailVm>>(getPOSPStageDetail);
                return HeroResult<IEnumerable<GetPOSPStageDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPStageDetailVm>>.Fail("No Record Found");
        }
    }
}
