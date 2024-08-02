using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.GetErrorCode
{
    public record GetErrorCodeQuery : IRequest<HeroResult<IEnumerable<GetErrorCodeVm>>>
    {
        public string ErrorType { get; set; }
    }
    public class GetErrorCodeQueryHandler : IRequestHandler<GetErrorCodeQuery, HeroResult<IEnumerable<GetErrorCodeVm>>>
    {
        private readonly IUserRepository _quoteRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public GetErrorCodeQueryHandler(IUserRepository quoteRepository, IMapper mapper)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetErrorCodeVm>>> Handle(GetErrorCodeQuery request, CancellationToken cancellationToken)
        {
            var errorCodeResult = await _quoteRepository.GetErrorCode(request.ErrorType, cancellationToken).ConfigureAwait(false);
            if (errorCodeResult.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetErrorCodeVm>>(errorCodeResult);
                return HeroResult<IEnumerable<GetErrorCodeVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetErrorCodeVm>>.Fail("No Record Found");
        }
    }
}
