using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetInsuranceType;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetLeadStage
{
    public record GetLeadStageQuery : IRequest<HeroResult<IEnumerable<GetLeadStageVm>>> { }
    internal class GetLeadStageQueryHandler : IRequestHandler<GetLeadStageQuery, HeroResult<IEnumerable<GetLeadStageVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetLeadStageQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetLeadStageVm>>> Handle(GetLeadStageQuery request, CancellationToken cancellationToken)
        {
            var leadType = await _userRepository.GetLeadStage(cancellationToken);
            if (leadType.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetLeadStageVm>>(leadType);
                return HeroResult<IEnumerable<GetLeadStageVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetLeadStageVm>>.Fail("No Record Found");
        }
    }
}
