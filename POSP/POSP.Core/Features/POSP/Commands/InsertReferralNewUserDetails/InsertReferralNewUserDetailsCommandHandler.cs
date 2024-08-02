using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Commands.InsertPOSPTrainingProgressDetail;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.InsertReferralNewUserDetails
{
    public record InsertReferralNewUserDetailsCommand : IRequest<HeroResult<ReferralNewUserDetailsModel>>
    {
        public string ReferralMode { get; set; } // EMAIL/SMS
        public string UserName { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public string Environment { get; set; }
        public string ReferralUserId { get; set; }
    }
    internal class InsertReferralNewUserDetailsCommandHandler : IRequestHandler<InsertReferralNewUserDetailsCommand, HeroResult<ReferralNewUserDetailsModel>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// InsertPOSPTrainingProgressDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertReferralNewUserDetailsCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="insertReferralNewUserDetailsCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<ReferralNewUserDetailsModel>> Handle(InsertReferralNewUserDetailsCommand insertReferralNewUserDetailsCommand, CancellationToken cancellationToken)
        {
            var referralNewUser = _mapper.Map<ReferralNewUserDetailsModel>(insertReferralNewUserDetailsCommand);
            var result = await _pospRepository.InsertReferralNewUserDetails(insertReferralNewUserDetailsCommand, cancellationToken);
            return HeroResult<ReferralNewUserDetailsModel>.Success(result);
        }
    }
}
