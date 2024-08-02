using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Identity.Domain.Roles;
using MediatR;
using Identity.Core.Features.User.Queries.GetAllRelationshipManager;
using Identity.Core.Responses;
using Identity.Core.Contracts.Persistence;

namespace Identity.Core.Features.User.Queries.GetAllRelationshipManager
{
        public record GetAllRelationshipManagerQueries : IRequest<HeroResult<IEnumerable<GetAllRelationshipManagerVM>>>
        {
            public string? UserId { get; set; }
        }

        public class GetAllRelationshipManagerQueriesHandler : IRequestHandler<GetAllRelationshipManagerQueries, HeroResult<IEnumerable<GetAllRelationshipManagerVM>>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;

            public GetAllRelationshipManagerQueriesHandler(IUserRepository userRepository, IMapper mapper)
            {
                _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<HeroResult<IEnumerable<GetAllRelationshipManagerVM>>> Handle(GetAllRelationshipManagerQueries request, CancellationToken cancellationToken)
            {
                var modelResult = await _userRepository.GetAllRelationshipManager(request.UserId, cancellationToken);

                if (modelResult.Any())
                {
                    var result = _mapper.Map<IEnumerable<GetAllRelationshipManagerVM>>(modelResult);
                    return HeroResult<IEnumerable<GetAllRelationshipManagerVM>>.Success(result);
                }

                return HeroResult<IEnumerable<GetAllRelationshipManagerVM>>.Fail("No Record Found");
            }
        }
    
}
