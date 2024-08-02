using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetCardsDetail;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetRelationshipManager
{
    public record GetRelationshipManagerQuery : IRequest<HeroResult<IEnumerable<GetRelationshipManagerVm>>>
    {
        public string UserId { get; set; }
    }
    public class GetRelationshipManagerQueryHandler : IRequestHandler<GetRelationshipManagerQuery, HeroResult<IEnumerable<GetRelationshipManagerVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetRelationshipManagerQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetRelationshipManagerVm>>> Handle(GetRelationshipManagerQuery request, CancellationToken cancellationToken)
        {
            var getcardsDetail = await _userRepository.GetRelationshipManager(request.UserId, cancellationToken);
            if (getcardsDetail != null)
            {
                var result = _mapper.Map<IEnumerable<GetRelationshipManagerVm>>(getcardsDetail);

                return HeroResult<IEnumerable<GetRelationshipManagerVm>>.Success(result);
            }

            return HeroResult<IEnumerable<GetRelationshipManagerVm>>.Fail("No record found");
        }
    }
}
