using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.BUUpdate;
using Admin.Domain.Roles;
using Admin.Core.Responses;
using MediatR;


namespace Admin.Core.Features.User.Commands.BUInsert
{
    public class BUInsertCommand : IRequest<HeroResult<bool>>
    {
        public string BuHeadId { get; set; }
        public string HerirachyLevelId { get; set; }
        public string BUName { get; set; }
    }

    public class BUInsertCommandHandler : IRequestHandler<BUInsertCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _BURepository;
        private readonly IMapper _mapper;

        public BUInsertCommandHandler(IUserRepository buRepository, IMapper mapper)
        {
            _BURepository = buRepository ?? throw new ArgumentNullException(nameof(buRepository));
            _mapper = mapper;
        }
        public async Task<HeroResult<bool>> Handle(BUInsertCommand buInsertCommand, CancellationToken cancellationToken)
        {
            var buInputModel = _mapper.Map<BUInsertInputModel>(buInsertCommand);
            var result = await _BURepository.BUInsertDetails(buInputModel, cancellationToken);

            return HeroResult<bool>.Success(result);
        }

    }

}
