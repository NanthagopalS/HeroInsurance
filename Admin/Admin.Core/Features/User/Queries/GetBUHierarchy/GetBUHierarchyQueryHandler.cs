using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetAllBUDetail;
using Admin.Core.Features.User.Queries.GetBUHeadDetail;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetBUHierarchy
{
    public record GetBUHierarchyQuery : IRequest<HeroResult<IEnumerable<GetBUHierarchyVm>>>;
    public class GetBUHierarchyQueryHandler : IRequestHandler<GetBUHierarchyQuery, HeroResult<IEnumerable<GetBUHierarchyVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetBUHierarchyQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetBUHierarchyVm>>> Handle(GetBUHierarchyQuery request, CancellationToken cancellationToken)
        {
            var userListResponseModel = await _userRepository.GetBUHierarchy(cancellationToken);
            if (userListResponseModel.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetBUHierarchyVm>>(userListResponseModel);
                return HeroResult<IEnumerable<GetBUHierarchyVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetBUHierarchyVm>>.Fail("No Record Found");
        }
    }
}
