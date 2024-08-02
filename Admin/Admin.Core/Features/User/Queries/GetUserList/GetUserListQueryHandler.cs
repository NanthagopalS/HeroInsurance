using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetBuDetail;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetBUHeadDetail
{
    public record GetUserListQuery : IRequest<HeroResult<IEnumerable<GetUserListVm>>> 
    {
        public string? RoleId;
    }

    public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, HeroResult<IEnumerable<GetUserListVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetUserListQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetUserListVm>>> Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {
            var userListResponseModel = await _userRepository.GetUserList(request.RoleId, cancellationToken);
            if (userListResponseModel.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetUserListVm>>(userListResponseModel);
                return HeroResult<IEnumerable<GetUserListVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetUserListVm>>.Fail("No Record Found");
        }
    }
}
