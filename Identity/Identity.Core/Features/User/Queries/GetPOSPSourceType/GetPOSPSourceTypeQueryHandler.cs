using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.GetPOSPSourceType
{
    public record GetPOSPSourceTypeQuery : IRequest<HeroResult<IEnumerable<GetPOSPSourceTypeVm>>>;
    public class GetPOSPSourceTypeQueryHandler : IRequestHandler<GetPOSPSourceTypeQuery, HeroResult<IEnumerable<GetPOSPSourceTypeVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetPOSPSourceTypeQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<GetPOSPSourceTypeVm>>> Handle(GetPOSPSourceTypeQuery request, CancellationToken cancellationToken)
        {
            var pospSource = await _userRepository.GetPOSPSourceType(cancellationToken).ConfigureAwait(false);
            if (pospSource.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPSourceTypeVm>>(pospSource);
                return HeroResult<IEnumerable<GetPOSPSourceTypeVm>>.Success(listInsurer);
            }
            return HeroResult<IEnumerable<GetPOSPSourceTypeVm>>.Fail("No Record Found");
        }
    }
}
