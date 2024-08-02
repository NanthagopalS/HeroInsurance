using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.GetUserListForDepartmentTagging
{
    public record GetUserListForDepartmentTaggingQuery : IRequest<HeroResult<IEnumerable<GetUserListForDepartmentTaggingVm>>>
    {
        public string TaggingType { get; set; }
    }
    public class GetUserListForDepartmentTaggingQueryHandler : IRequestHandler<GetUserListForDepartmentTaggingQuery, HeroResult<IEnumerable<GetUserListForDepartmentTaggingVm>>>
    {
        private readonly IUserRepository _quoteRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public GetUserListForDepartmentTaggingQueryHandler(IUserRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetUserListForDepartmentTaggingVm>>> Handle(GetUserListForDepartmentTaggingQuery request, CancellationToken cancellationToken)
        {
            var errorCodeResult = await _quoteRepository.GetUserListForDepartmentTagging(request.TaggingType, cancellationToken).ConfigureAwait(false);
            if (errorCodeResult.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetUserListForDepartmentTaggingVm>>(errorCodeResult);
                return HeroResult<IEnumerable<GetUserListForDepartmentTaggingVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetUserListForDepartmentTaggingVm>>.Fail("No Record Found");
        }
    }
}
