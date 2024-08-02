using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetParticularBUDetail;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetParticularLeadDetail
{
    public class GetParticularLeadDetailQuery : IRequest<HeroResult<GetParticularLeadDetailVm>>
    {
        public string? LeadId { get; set; }
    }
    public class GetParticularLeadDetailQueryHandler : IRequestHandler<GetParticularLeadDetailQuery, HeroResult<GetParticularLeadDetailVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetParticularLeadDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetParticularLeadDetailVm>> Handle(GetParticularLeadDetailQuery request, CancellationToken cancellationToken)
        {
            //var getParticularLeadDetail = _mapper.Map<LeadDetail>(request);
            var leadDetail = await _userRepository.GetParticularLeadDetail(request.LeadId, cancellationToken).ConfigureAwait(false);
            if (leadDetail != null)
            {
                var listInsurer = _mapper.Map<GetParticularLeadDetailVm>(leadDetail);
                return HeroResult<GetParticularLeadDetailVm>.Success(listInsurer);
            }

            return HeroResult<GetParticularLeadDetailVm>.Fail("No Record Found");
        }
    }
}
