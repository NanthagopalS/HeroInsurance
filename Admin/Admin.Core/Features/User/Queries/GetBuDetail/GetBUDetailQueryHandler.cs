using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetBuDetail
{
    public record GetBUDetailQuery : IRequest<HeroResult<IEnumerable<GetBUDetailVm>>>
    {
        public string? RoleTypeId { get; set; }
    }
    public class GetBUDetailQueryHandler : IRequestHandler<GetBUDetailQuery, HeroResult<IEnumerable<GetBUDetailVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetBUDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetBUDetailVm>>> Handle(GetBUDetailQuery request, CancellationToken cancellationToken)
        {
            var getBuDetail = await _userRepository.GetBUDetail(request.RoleTypeId, cancellationToken);
            if (getBuDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetBUDetailVm>>(getBuDetail);
                return HeroResult<IEnumerable<GetBUDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetBUDetailVm>>.Fail("No Record Found");
        }
    }
}
