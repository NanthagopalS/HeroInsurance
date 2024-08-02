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

namespace Admin.Core.Features.User.Queries.GetAssistedBUDetails
{
    public record GetAssistedBUDetailsQuery : IRequest<HeroResult<GetAssistedBUDetailsVm>>
    {
        public string? RoleId { get; set; }
        public string? UserId { get; set; }
    }
    public class GetAssistedBUDetailsQueryHandler : IRequestHandler<GetAssistedBUDetailsQuery, HeroResult<GetAssistedBUDetailsVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetAssistedBUDetailsQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetAssistedBUDetailsVm>> Handle(GetAssistedBUDetailsQuery request, CancellationToken cancellationToken)
        {
            var getBuDetail = await _userRepository.GetAssistedBUDetails(request.RoleId, request.UserId, cancellationToken);
            if (getBuDetail != null)
            {
                var listInsurer = _mapper.Map<GetAssistedBUDetailsVm>(getBuDetail);
                return HeroResult<GetAssistedBUDetailsVm>.Success(listInsurer);
            }

            return HeroResult<GetAssistedBUDetailsVm>.Fail("No Record Found");
        }
    }

}
