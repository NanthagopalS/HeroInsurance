using Admin.Core.Contracts.Persistence;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.UpdateActivateDeActivateTrainingManagement
{
    public class UpdateActivateDeActivateTrainingManagementCommand : IRequest<HeroResult<bool>>
    {
        public string TrainingMaterialId { get; set; }
        public bool IsActive { get; set; }
    }
    public class UpdateActivateDeActivateTrainingManagementCommandHandler : IRequestHandler<UpdateActivateDeActivateTrainingManagementCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateActivateDeActivateTrainingManagementCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateActivateDeActivateTrainingManagementCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="UpdateActivateDeActivateTrainingManagementCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateActivateDeActivateTrainingManagementCommand UpdateActivateDeActivateTrainingManagementCommand, CancellationToken cancellationToken)
        {
            var UpdateActivateDeActivateTrainingManagement = _mapper.Map<UpdateActivateDeActivateTrainingManagementModel>(UpdateActivateDeActivateTrainingManagementCommand);
            var result = await _userRepository.UpdateActivateDeActivateTrainingManagement(UpdateActivateDeActivateTrainingManagement, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
