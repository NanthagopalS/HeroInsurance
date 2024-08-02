using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Queries.GetRolePermission;
using Identity.Domain.Roles;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Commands.BUDetails
{
    public class BUDetailsCommand : IRequest<HeroResult<IEnumerable<BUDetailsVM>>>
    {
        public string BUName { get; set; }
        public string CreatedFrom { get; set; }
        public string CreatedTo { get; set; }
    }
   
    public class BUDetailsCommandHandler : IRequestHandler<BUDetailsCommand, HeroResult<IEnumerable<BUDetailsVM>>>
    {
         private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public BUDetailsCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
       
        public async Task<HeroResult<IEnumerable<BUDetailsVM>>> Handle(BUDetailsCommand request, CancellationToken cancellationToken)
        {
            var roleSearchInput = _mapper.Map<BUSearchInputModel>(request);
            var modelResult = await _userRepository.GetBUDetails(roleSearchInput, cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listInsurerModel = _mapper.Map<IEnumerable<BUDetailsVM>>(modelResult);
                return HeroResult<IEnumerable<BUDetailsVM>>.Success(listInsurerModel);
            }
            return HeroResult<IEnumerable<BUDetailsVM>>.Fail("No Record Found");
        }
        
    }

}
