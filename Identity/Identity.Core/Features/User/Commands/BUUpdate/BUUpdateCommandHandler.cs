using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.Roles;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Commands.BUUpdate
{
    public record BUUpdateCommand : IRequest<HeroResult<bool>>
    {
        public int BUID { get; set; }
        public bool IsActive { get; set; }

    }
   
    public class BUUpdateCommandHandler : IRequestHandler<BUUpdateCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _BURepository;
        private readonly IMapper _mapper;

        public BUUpdateCommandHandler(IUserRepository buRepository, IMapper mapper)
        {
            _BURepository = buRepository ?? throw new ArgumentNullException(nameof(buRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(BUUpdateCommand buUpdateCommand, CancellationToken cancellationToken)
        {
            var buInputModel = _mapper.Map<BUUpdateInputModel>(buUpdateCommand);
            var result = await _BURepository.BUUpdateDetails(buInputModel, cancellationToken);

            return HeroResult<bool>.Success(result);
        }
    }    
   
}
