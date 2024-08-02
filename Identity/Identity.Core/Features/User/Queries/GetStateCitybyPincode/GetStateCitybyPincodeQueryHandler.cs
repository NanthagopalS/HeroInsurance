using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.GetStateCitybyPincode
{
    public record GetStateCitybyPincodeQuery : IRequest<HeroResult<GetStateCitybyPincodeVm>>
    {
        public string Pincode { get; set; }
    }
    public class GetStateCitybyPincodeQueryHandler : IRequestHandler<GetStateCitybyPincodeQuery, HeroResult<GetStateCitybyPincodeVm>>
    {

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialize
        /// </summary>
        public GetStateCitybyPincodeQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<GetStateCitybyPincodeVm>> Handle(GetStateCitybyPincodeQuery request, CancellationToken cancellationToken)
        {
            var getStateCitybyPincodeResult = await _userRepository.GetStateCitybyPincode(request.Pincode,
                cancellationToken).ConfigureAwait(false);

            if (getStateCitybyPincodeResult != null)
            {
                var result = _mapper.Map<GetStateCitybyPincodeVm>(getStateCitybyPincodeResult);

                return HeroResult<GetStateCitybyPincodeVm>.Success(result);
            }

            return HeroResult<GetStateCitybyPincodeVm>.Fail("No record found");
        }
    }
}
