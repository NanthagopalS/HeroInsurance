using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;

namespace Admin.Core.Features.User.Commands.DeleteTrainingManagementDetail
{
    public record DeleteTrainingManagementDetailCommand : IRequest<HeroResult<bool>>
    {
        public string TrainingMaterialId { get; set; }
    }

    public class DeleteTrainingManagementDetailCommandHandler : IRequestHandler<DeleteTrainingManagementDetailCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userrepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public DeleteTrainingManagementDetailCommandHandler(IUserRepository userrepository, IMapper mapper)
        {
            _userrepository = userrepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<bool>> Handle(DeleteTrainingManagementDetailCommand request, CancellationToken cancellationToken)
        {
            var result = await _userrepository.DeleteTrainingManagementDetail(request.TrainingMaterialId, cancellationToken);

            return HeroResult<bool>.Success(result);
        }
    }
}
