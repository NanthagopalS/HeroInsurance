using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Commands.BUUpdate;
using Identity.Domain.Roles;
using Identity.Core.Responses;
using MediatR;


namespace Identity.Core.Features.User.Commands.BUInsert
{
    public class BUInsertCommand : IRequest<HeroResult<bool>>
    {
        public int Roletypeid { get; set; }
        public int BULevelID { get; set; }
        public string BUName { get; set; }
        public bool IsActive { get; set; }
        public string RoleId { get; set; }
        public string CreatedBy { get; set; }
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
