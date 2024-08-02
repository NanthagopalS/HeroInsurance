using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.UpdateBUDetail;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.UpdateBUStatus
{
    public class UpdateBUStatusComand : IRequest<HeroResult<bool>>
    {
        public string? BUId { get; set; }
        public bool? IsActive { get; set; }

    }
    public class UpdateBUStatusCommandHandler : IRequestHandler<UpdateBUStatusComand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateBUDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateBUStatusCommandHandler(IUserRepository userRepository, IMapper mapper)
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
        public async Task<HeroResult<bool>> Handle(UpdateBUStatusComand updateBUStatusCommand, CancellationToken cancellationToken)
        {
            //var updateBUStatus = _mapper.Map<UpdateBUStatusResonse>(updateBUStatusCommand);
            var result = await _userRepository.UpdateBUStatus(updateBUStatusCommand.BUId, updateBUStatusCommand.IsActive, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
