using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetFunnelChart;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetDeactivatedUser
{
    public record GetDeactivatedUserQuery : IRequest<HeroResult<GetDeactivatedUserVm>>
    {
        public string? SearchText { get; set; }
        public string? RelationManagerId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int CurrentPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
    }
    public class GetDeactivatedUserQueryHandler : IRequestHandler<GetDeactivatedUserQuery, HeroResult<GetDeactivatedUserVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetDeactivatedUserQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetDeactivatedUserVm>> Handle(GetDeactivatedUserQuery request, CancellationToken cancellationToken)
        {
            var getDeactivatedUsers = await _userRepository.GetDeactivatedUser(request,cancellationToken);
            if (getDeactivatedUsers != null)
            {
                var listInsurer = _mapper.Map<GetDeactivatedUserVm>(getDeactivatedUsers);
                return HeroResult<GetDeactivatedUserVm>.Success(listInsurer);
            }

            return HeroResult<GetDeactivatedUserVm>.Fail("No Record Found");
        }
    }
}
