using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.HelpAndSupport.Queries.GetSubConcernType
{
    public record GetSubConcernTypeQuery : IRequest<HeroResult<IEnumerable<GetSubConcernTypeVm>>>
    {
        public string? ConcernTypeId { get; set; }
    }
    public class GetSubConcernTypeQueryHandler : IRequestHandler<GetSubConcernTypeQuery, HeroResult<IEnumerable<GetSubConcernTypeVm>>>
    {
        private readonly IHelpAndSupportRepository _helpAndSupportRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="mapper"></param>
        public GetSubConcernTypeQueryHandler(IHelpAndSupportRepository helpAndSupportRepository, IMapper mapper)
        {
            _helpAndSupportRepository = helpAndSupportRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetSubConcernTypeVm>>> Handle(GetSubConcernTypeQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _helpAndSupportRepository.GetSubConcernType(request.ConcernTypeId, cancellationToken);
            if (modelResult.Any())
            {
                var listUserRoleModel = _mapper.Map<IEnumerable<GetSubConcernTypeVm>>(modelResult);
                return HeroResult<IEnumerable<GetSubConcernTypeVm>>.Success(listUserRoleModel);
            }
            return HeroResult<IEnumerable<GetSubConcernTypeVm>>.Fail("No Record Found");
        }
    }
}
