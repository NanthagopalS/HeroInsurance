using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetModel;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Queries.GetRoleBULevel
{

    public class GetRoleBULevelCommand:IRequest<HeroResult<IEnumerable<BULevelVm>>>
    {

    }
    public class GetRoleBULevelQueryHandler : IRequestHandler<GetRoleBULevelCommand, HeroResult<IEnumerable<BULevelVm>>>
    {


        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetRoleBULevelQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<BULevelVm>>> Handle(GetRoleBULevelCommand request, CancellationToken cancellationToken)
        {
            var modelResult = await _userRepository.GetRoleBULevelDetails(cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listInsurerModel = _mapper.Map<IEnumerable<BULevelVm>>(modelResult);
                return HeroResult<IEnumerable<BULevelVm>>.Success(listInsurerModel);
            }
            return HeroResult<IEnumerable<BULevelVm>>.Fail("No Record Found");
        }
    }
}
