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

namespace Admin.Core.Features.User.Commands.InsertStampInstruction
{
    public class InsertStampInstructionCommand : IRequest<HeroResult<bool>>
    {
        //public string? Id { get; set; }
        public string? SrNo { get; set; }
        public string? Instruction { get; set; }
    }
    public class InsertStampInstructionCommandHandler : IRequestHandler<InsertStampInstructionCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// InsertStampInstructionCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertStampInstructionCommandHandler(IUserRepository userRepository, IMapper mapper)
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
        public async Task<HeroResult<bool>> Handle(InsertStampInstructionCommand request, CancellationToken cancellationToken)
        {
            var updateParticularPOSPDetailForIIBDashboard = _mapper.Map<InsertStampInstructionModel>(request);
            var result = await _userRepository.InsertStampInstruction(request.SrNo, request.Instruction, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
