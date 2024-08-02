using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetBuDetail;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetCardsDetail
{
    public record GetCardsDetailQuery : IRequest<HeroResult<GetCardsDetailVm>>
    {
        public string UserId { get; set; }
    }
    public class GetCardsDetailQueryHandler : IRequestHandler<GetCardsDetailQuery, HeroResult<GetCardsDetailVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetCardsDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetCardsDetailVm>> Handle(GetCardsDetailQuery request, CancellationToken cancellationToken)
        {
            var getcardsDetail = await _userRepository.GetCardsDetail(request.UserId, cancellationToken);
            if (getcardsDetail != null)
            {
                var result = _mapper.Map<GetCardsDetailVm>(getcardsDetail);

                return HeroResult<GetCardsDetailVm>.Success(result);
            }

            return HeroResult<GetCardsDetailVm>.Fail("No record found");
        }
    }

}
