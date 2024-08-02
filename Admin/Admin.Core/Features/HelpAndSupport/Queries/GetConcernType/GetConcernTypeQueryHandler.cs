using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.HelpAndSupport.Queries.GetConcernType
{
    public record GetConcernTypeQuery : IRequest<HeroResult<IEnumerable<GetConcernTypeVm>>>
    {

    }
    public class GetConcernTypeQueryHandler : IRequestHandler<GetConcernTypeQuery, HeroResult<IEnumerable<GetConcernTypeVm>>>
    {
        private readonly IHelpAndSupportRepository _helpAndSupportRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="mapper"></param>
        public GetConcernTypeQueryHandler(IHelpAndSupportRepository helpAndSupportRepository, IMapper mapper)
        {
            _helpAndSupportRepository = helpAndSupportRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetConcernTypeVm>>> Handle(GetConcernTypeQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _helpAndSupportRepository.GetConcernType(cancellationToken);
            if (modelResult.Any())
            {
                var listUserRoleModel = _mapper.Map<IEnumerable<GetConcernTypeVm>>(modelResult);
                return HeroResult<IEnumerable<GetConcernTypeVm>>.Success(listUserRoleModel);
            }
            return HeroResult<IEnumerable<GetConcernTypeVm>>.Fail("No Record Found");
        }
    }
}
