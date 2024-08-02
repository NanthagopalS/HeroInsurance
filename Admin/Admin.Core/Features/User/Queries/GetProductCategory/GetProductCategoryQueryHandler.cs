using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetProductCategory
{
    public class GetProductCategoryQuery : IRequest<HeroResult<IEnumerable<GetProductCategoryVm>>>
    {

    }
    public class GetProductCategoryQueryHandler : IRequestHandler<GetProductCategoryQuery, HeroResult<IEnumerable<GetProductCategoryVm>>>
    {


        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetProductCategoryQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<GetProductCategoryVm>>> Handle(GetProductCategoryQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _userRepository.GetProductCategory(cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listInsurerModel = _mapper.Map<IEnumerable<GetProductCategoryVm>>(modelResult);
                return HeroResult<IEnumerable<GetProductCategoryVm>>.Success(listInsurerModel);
            }
            return HeroResult<IEnumerable<GetProductCategoryVm>>.Fail("No Record Found");
        }
    }
}
