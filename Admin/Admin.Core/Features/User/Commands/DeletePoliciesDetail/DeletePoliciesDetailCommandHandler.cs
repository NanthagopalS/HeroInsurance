using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using MediatR;

namespace Admin.Core.Features.User.Commands.DeletePoliciesDetail
{
    public record DeletePoliciesDetailCommand : IRequest<HeroResult<bool>>
    {
        public string POSPId { get; set; }
    }

    public class DeletePoliciesDetailCommandHandler : IRequestHandler<DeletePoliciesDetailCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userrepository;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public DeletePoliciesDetailCommandHandler(IUserRepository userrepository)
        {
            _userrepository = userrepository;        
        }

        public async Task<HeroResult<bool>> Handle(DeletePoliciesDetailCommand request, CancellationToken cancellationToken)
        {
            var result = await _userrepository.DeletePoliciesDetail(request.POSPId, cancellationToken);

            return HeroResult<bool>.Success(result);
        }
    }
}
