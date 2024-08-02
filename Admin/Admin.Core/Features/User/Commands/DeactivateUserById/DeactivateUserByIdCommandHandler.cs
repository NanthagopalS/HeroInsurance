using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.UpdateActivateDeActivateBU;
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

namespace Admin.Core.Features.User.Commands.DeactivateUserById
{
    public record DeactivateUserByIdCommand:IRequest<HeroResult<bool>>
    {
        public string UserId { get; set; }
        public int IsActive { get; set; } = 0;
        public string? DeactivatePOSPId { get; set; } = null;
    }
    public class DeactivateUserByIdCommandHandler : IRequestHandler<DeactivateUserByIdCommand, HeroResult<bool>>
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper _mapper;

        public DeactivateUserByIdCommandHandler(IUserRepository catRepository, IMapper mapper)
        {
            userRepository = catRepository ?? throw new ArgumentNullException(nameof(catRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(DeactivateUserByIdCommand catInsertCommand, CancellationToken cancellationToken)
        {
            var result = await userRepository.DeactivateUserById(catInsertCommand, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
