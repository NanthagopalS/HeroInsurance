using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetUserCategory
{
    public record GetUserCategoryQuery : IRequest<HeroResult<IEnumerable<GetUserCategoryVm>>>;


    public class GetUserCategoryQueryHandler : IRequestHandler<GetUserCategoryQuery, HeroResult<IEnumerable<GetUserCategoryVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetUserCategoryQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<GetUserCategoryVm>>> Handle(GetUserCategoryQuery request, CancellationToken cancellationToken)
        {
            var roleTypeResult = await _userRepository.GetUserCategory(cancellationToken).ConfigureAwait(false);
            if (roleTypeResult.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetUserCategoryVm>>(roleTypeResult);
                return HeroResult<IEnumerable<GetUserCategoryVm>>.Success(listInsurer);
            }
            return HeroResult<IEnumerable<GetUserCategoryVm>>.Fail("No Record Found");
        }
    }

}
