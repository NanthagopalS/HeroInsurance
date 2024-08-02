using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.User.Commands.DeleteExamManagementDetail
{
    public record DeleteExamManagementDetailCommand : IRequest<HeroResult<bool>>
    {
        public string? QuestionId { get; set; }
    }

    public class DeleteExamManagementDetailCommandHandler : IRequestHandler<DeleteExamManagementDetailCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userrepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="request"></param>
        /// <param name="mapper"></param>
        public DeleteExamManagementDetailCommandHandler(IUserRepository userrepository, IMapper mapper)
        {
            _userrepository = userrepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(DeleteExamManagementDetailCommand request, CancellationToken cancellationToken)
        {
            var result = await _userrepository.DeleteExamManagementDetail(request.QuestionId, cancellationToken);

            return HeroResult<bool>.Success(result);
        }
    }
}
