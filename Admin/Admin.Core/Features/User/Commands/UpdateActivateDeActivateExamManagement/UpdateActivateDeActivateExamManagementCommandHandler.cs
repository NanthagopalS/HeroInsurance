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

namespace Admin.Core.Features.User.Commands.UpdateActivateDeActivateExamManagement
{
    public class UpdateActivateDeActivateExamManagementCommand : IRequest<HeroResult<bool>>
    {
        public string? QuestionId { get; set; }
        public bool? IsActive { get; set; }
    }
    public class UpdateActivateDeActivateExamManagementCommandHandler : IRequestHandler<UpdateActivateDeActivateExamManagementCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateBUDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateActivateDeActivateExamManagementCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="UpdateActivateDeActivateExamManagementCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateActivateDeActivateExamManagementCommand UpdateActivateDeActivateExamManagementCommand, CancellationToken cancellationToken)
        {
            var UpdateActivateDeActivateExamManagement = _mapper.Map<UpdateActivateDeActivateExamManagementModel>(UpdateActivateDeActivateExamManagementCommand);
            var result = await _userRepository.UpdateActivateDeActivateExamManagement(UpdateActivateDeActivateExamManagement, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
