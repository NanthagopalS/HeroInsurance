using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.UpdateBUDetail;
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

namespace Admin.Core.Features.User.Commands.UpdateParticularPOSPDetailForIIBDashboard
{
    public class UpdateParticularPOSPDetailForIIBDashboardCommand : IRequest<HeroResult<bool>>
    {
        public string? UserId { get; set; }
        public string? IIBStatus { get; set; }
        public string? IIBUploadStatus { get; set; }
    }
    public class UpdateParticularPOSPDetailForIIBDashboardCommandHandler : IRequestHandler<UpdateParticularPOSPDetailForIIBDashboardCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateBUDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateParticularPOSPDetailForIIBDashboardCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateParticularPOSPDetailForIIBDashboardCommand request, CancellationToken cancellationToken)
        {
            var updateParticularPOSPDetailForIIBDashboard = _mapper.Map<ParticularPOSPIIBDasboardStatusUpdate>(request);
            var result = await _userRepository.UpdateParticularPOSPDetailForIIBDashboard(request.UserId, request.IIBStatus, request.IIBUploadStatus, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
