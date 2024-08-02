using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.UpdateAgreementStatusByUserId
{
    public class UpdateExamParticularQuestionCommand : IRequest<HeroResult<bool>>
    {
        public string? QuestionId { get; set; }
        public string? OptionId1 { get; set; }
        public string? OptionValue1 { get; set; }
        public string? OptionId2 { get; set; }
        public string? OptionValue2 { get; set; }
        public string? OptionId3 { get; set; }
        public string? OptionValue3 { get; set; }
        public string? OptionId4 { get; set; }
        public string? OptionValue4 { get; set; }
        public int? CorrectAnswerIndex { get; set; }
    }
    public class UpdateExamParticularQuestionCommandHandler : IRequestHandler<UpdateExamParticularQuestionCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateExamParticularQuestionCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateExamParticularQuestionCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }

        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="updateExamParticularQuestionCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateExamParticularQuestionCommand updateExamParticularQuestionCommand, CancellationToken cancellationToken)
        {
            var model = _mapper.Map<UpdateExamParticularQuestionModel>(updateExamParticularQuestionCommand);
            var result = await _userRepository.UpdateExamParticularQuestion(model, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
