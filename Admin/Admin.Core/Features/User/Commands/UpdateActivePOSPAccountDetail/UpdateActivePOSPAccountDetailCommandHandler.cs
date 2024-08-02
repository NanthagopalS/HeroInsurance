using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.UpdateActivateDeActivateTrainingManagement;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.UpdateActivePOSPAccountDetail
{
    public class UpdateActivePOSPAccountDetailCommand: IRequest<HeroResult<bool>>
    {
        public string? POSPUserId { get; set; }
        public string? PreSaleUserId { get; set; }
        public string? PostSaleUserId { get; set; }
        public string? MarketingUserId { get; set; }
        public string? ClaimUserId { get; set; }
        public string? SourcedBy { get; set; }
        public string? CreatedBy { get; set; }
        public string? ServicedBy { get; set; }

    }
    public class UpdateActivePOSPAccountDetailCommandHandler : IRequestHandler<UpdateActivePOSPAccountDetailCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateActivateDeActivateTrainingManagementCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateActivePOSPAccountDetailCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="UpdateActivePOSPAccountDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateActivePOSPAccountDetailCommand updateActivePOSPAccountDetailCommand, CancellationToken cancellationToken)
        {
            var pospAccountDetail = _mapper.Map<UpdateActivePOSPAccountDetailModel>(updateActivePOSPAccountDetailCommand);
            var result = await _userRepository.UpdateActivePOSPAccountDetail(updateActivePOSPAccountDetailCommand, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
