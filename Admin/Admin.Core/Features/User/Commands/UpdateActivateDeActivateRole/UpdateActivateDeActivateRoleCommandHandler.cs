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

namespace Admin.Core.Features.User.Commands.UpdateActivateDeActivateRole
{
    public class UpdateActivateDeActivateRoleCommand : IRequest<HeroResult<bool>>
    {
        public string? RoleId { get; set; }
        public bool IsActive { get; set; }


    }
    public class UpdateActivateDeActivateRoleCommandHandler : IRequestHandler<UpdateActivateDeActivateRoleCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateBUDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateActivateDeActivateRoleCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="updateBUDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateActivateDeActivateRoleCommand UpdateActivateDeActivateRoleCommand, CancellationToken cancellationToken)
        {
            var UpdateActivateDeActivateRole = _mapper.Map<UpdateActivateDeActivateRoleModel>(UpdateActivateDeActivateRoleCommand);
            var result = await _userRepository.UpdateActivateDeActivateRole(UpdateActivateDeActivateRole, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
