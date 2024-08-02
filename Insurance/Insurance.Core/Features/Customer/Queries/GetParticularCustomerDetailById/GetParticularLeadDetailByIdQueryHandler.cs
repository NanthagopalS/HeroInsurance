using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.Customer.Queries.GetParticularCustomerDetailById
{
    public class GetParticularLeadDetailByIdQuery : IRequest<HeroResult<GetParticularLeadDetailByIdVm>>
    {
        public string LeadId { get; set; }
    }
    public class GetParticularLeadDetailByIdQueryHandler : IRequestHandler<GetParticularLeadDetailByIdQuery, HeroResult<GetParticularLeadDetailByIdVm>>
    {
        private readonly ICustomerRepository _userRepository;
        private readonly IMapper _mapper;
        public string LeadId;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetParticularLeadDetailByIdQueryHandler(ICustomerRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetParticularLeadDetailByIdVm>> Handle(GetParticularLeadDetailByIdQuery request, CancellationToken cancellationToken)
        {
            //var getParticularLeadDetail = _mapper.Map<LeadDetail>(request);
            var leadDetail = await _userRepository.GetParticularLeadDetailById(request.LeadId, cancellationToken).ConfigureAwait(false);
            if (leadDetail is not null)
            {
                var listInsurer = _mapper.Map<GetParticularLeadDetailByIdVm>(leadDetail);
                return HeroResult<GetParticularLeadDetailByIdVm>.Success(listInsurer);
            }

            return HeroResult<GetParticularLeadDetailByIdVm>.Fail("No Record Found");
        }
    }
}
