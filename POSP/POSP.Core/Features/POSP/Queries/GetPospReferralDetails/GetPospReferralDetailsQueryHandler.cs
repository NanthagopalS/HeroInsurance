using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetPospReferralDetails
{
    public class GetPospReferralDetailsQuery : IRequest<HeroResult<GetPospReferralDetailsVm>>
    {
        public string? UserId { get; set; }

        public IEnumerable<ReferralTypeModel>? ReferralTypeModel { get; set; }
        public IEnumerable<RefferalMode>? RefferalMode { get; set; }
        public IEnumerable<RefferalDetails>? RefferalDetails { get; set; }
    }
    public class GetPospReferralDetailsQueryHandler : IRequestHandler<GetPospReferralDetailsQuery, HeroResult<GetPospReferralDetailsVm>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        public GetPospReferralDetailsQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<GetPospReferralDetailsVm>> Handle(GetPospReferralDetailsQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _pospRepository.GetPospReferralDetails(request.UserId, cancellationToken).ConfigureAwait(false);
            if (modelResult != null)
            {
                var listUserRoleModel = _mapper.Map<GetPospReferralDetailsVm>(modelResult);
                return HeroResult<GetPospReferralDetailsVm>.Success(listUserRoleModel);
            }
            return HeroResult<GetPospReferralDetailsVm>.Fail("No Record Found");
        }

    }

}
