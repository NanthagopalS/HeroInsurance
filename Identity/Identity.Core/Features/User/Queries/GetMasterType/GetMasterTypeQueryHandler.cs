using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Querries.GetMasterType
{
    public record GetPOSPUserMasterQuery : IRequest<HeroResult<MasterTypeVm>>
    {

    }
    public class GetPOSPUserMasterQueryHandler : IRequestHandler<GetPOSPUserMasterQuery, HeroResult<MasterTypeVm>>
    {
    
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        public GetPOSPUserMasterQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<MasterTypeVm>> Handle(GetPOSPUserMasterQuery request, CancellationToken cancellationToken)
        {
            var POSPMasterResult = await _userRepository.GetPOSPUserMaster( cancellationToken).ConfigureAwait(false);

            if (POSPMasterResult != null)
            {
                var result = _mapper.Map<MasterTypeVm>(POSPMasterResult);
  
                return HeroResult<MasterTypeVm>.Success(result);
            }

            return HeroResult<MasterTypeVm>.Fail("No record found");
        }
    }
}
