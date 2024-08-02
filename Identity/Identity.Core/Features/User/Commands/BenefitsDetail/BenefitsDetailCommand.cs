using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.Benefits.Commands.BenefitsDetail
{
    public record BenefitsDetailCommand: IRequest<HeroResult<bool>>
    {
        public string Id { get; set; }
        public string BenefitsTitle { get; set; }
        public string BenefitsDescription { get; set; }
        
    }
    public class BenefitsDetailCommandHandler : IRequestHandler<BenefitsDetailCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userDetailRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// BenefitsDetailCommandHandler
        /// </summary>
        /// <param name="benefitsDetailRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public BenefitsDetailCommandHandler(IUserRepository userDetailRepository, IMapper mapper)
        {
            _userDetailRepository = userDetailRepository ?? throw new ArgumentNullException(nameof(userDetailRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="benefitsDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(BenefitsDetailCommand benefitsDetailCommand, CancellationToken cancellationToken)
        {
            var benefitsDetailModel = _mapper.Map<BenefitDetailModel>(benefitsDetailCommand);
            var result = await _userDetailRepository.UpdateBenefitsDetail(benefitsDetailModel, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
}
