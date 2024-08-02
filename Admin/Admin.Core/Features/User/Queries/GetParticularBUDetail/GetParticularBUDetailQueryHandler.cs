using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetAllBUDetail;
using Admin.Domain.Roles;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetParticularBUDetail
{
    public class GetParticularBUDetailQuery : IRequest<HeroResult<IEnumerable<GetParticularBUDetailVm>>>
    {
        public string BUId { get; set; }
    }
    public class GetParticularBUDetailQueryHandler : IRequestHandler<GetParticularBUDetailQuery, HeroResult<IEnumerable<GetParticularBUDetailVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetParticularBUDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetParticularBUDetailVm>>> Handle(GetParticularBUDetailQuery request, CancellationToken cancellationToken)
        {
            var particularbuDetailMapInput = _mapper.Map<ParticularBUDetailModel>(request);
            var particularBuDetail = await _userRepository.GetParticularBUDetail(request.BUId, cancellationToken).ConfigureAwait(false);
            if (particularBuDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetParticularBUDetailVm>>(particularBuDetail);
                return HeroResult<IEnumerable<GetParticularBUDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetParticularBUDetailVm>>.Fail("No Record Found");
        }

    }
}
