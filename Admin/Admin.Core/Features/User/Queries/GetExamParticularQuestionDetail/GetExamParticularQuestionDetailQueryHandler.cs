using Admin.Core.Contracts.Persistence;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetParticularUserRoleMappingDetail
{
    public class GetExamParticularQuestionDetailQuery : IRequest<HeroResult<IEnumerable<GetExamParticularQuestionDetailVm>>>
    {
        public string? QuestionId  { get; set; }

    }

    public class GetExamParticularQuestionDetailQueryHandler : IRequestHandler<GetExamParticularQuestionDetailQuery, HeroResult<IEnumerable<GetExamParticularQuestionDetailVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetExamParticularQuestionDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetExamParticularQuestionDetailVm>>> Handle(GetExamParticularQuestionDetailQuery request, CancellationToken cancellationToken)
        {
            var particularBuDetail = await _userRepository.GetExamParticularQuestionDetail(request.QuestionId, cancellationToken).ConfigureAwait(false);
            if (particularBuDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetExamParticularQuestionDetailVm>>(particularBuDetail);
                return HeroResult<IEnumerable<GetExamParticularQuestionDetailVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetExamParticularQuestionDetailVm>>.Fail("No Record Found");
        }
    }
}
