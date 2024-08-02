using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.UpdateAgreementStatusByUserId
{
    public class UpdateAgreementStatusByUserIdCommand : IRequest<HeroResult<bool>>
    {
        public string UserId { get; set; }
        public string ProcessType { get; set; }
    }
    public class UpdateAgreementStatusByUserIdCommandHandler : IRequestHandler<UpdateAgreementStatusByUserIdCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateActivateDeActivateTrainingManagementCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateAgreementStatusByUserIdCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="UpdateAgreementStatusByUserIdCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateAgreementStatusByUserIdCommand UpdateAgreementStatusByUserIdCommand, CancellationToken cancellationToken)
        {
            var updateAgreementStatusByUserIdModel = _mapper.Map<UpdateAgreementStatusByUserIdModel>(UpdateAgreementStatusByUserIdCommand);
            var result = await _userRepository.UpdateAgreementStatusByUserId(updateAgreementStatusByUserIdModel, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
