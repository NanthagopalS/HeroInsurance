using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetRoleType;
using Admin.Core.Features.User.Querries.GetMasterType;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Queries.GetModel
{
    public record GetModelQuery : IRequest<HeroResult<IEnumerable<ModelVm>>>
    {
        public string? ModuleGroupName { get; set; }
    }


    public class GetModelQueryHandler : IRequestHandler<GetModelQuery, HeroResult<IEnumerable<ModelVm>>>
    {


        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetModelQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<ModelVm>>> Handle(GetModelQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _userRepository.GetModuleDetails(request.ModuleGroupName, cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listInsurerModel = _mapper.Map<IEnumerable<ModelVm>>(modelResult);
                return HeroResult<IEnumerable<ModelVm>>.Success(listInsurerModel);
            }
            return HeroResult<IEnumerable<ModelVm>>.Fail("No Record Found");
        }
    }
}