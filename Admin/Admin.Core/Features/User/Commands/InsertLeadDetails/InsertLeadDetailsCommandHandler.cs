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

namespace Admin.Core.Features.User.Commands.InsertLeadDetails
{
    public record InsertLeadDetailsCommand : IRequest<HeroResult<InsertLeadDetailsModel>>
    {
        public string? LeadName { get; set; }
        public string? LeadPhoneNumber { get; set; }
        public string? LeadEmailId { get; set; }
        public string? UserId { get; set; }

    }
    public class InsertLeadDetailsCommandHandler : IRequestHandler<InsertLeadDetailsCommand, HeroResult<InsertLeadDetailsModel>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// InsertExamInstructionsDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertLeadDetailsCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="InsertPOSPTrainingDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<InsertLeadDetailsModel>> Handle(InsertLeadDetailsCommand insertLeadDetailsCommand, CancellationToken cancellationToken)
        {
            var insertLead = _mapper.Map<InsertLeadDetailsModel>(insertLeadDetailsCommand);
            var result = await _userRepository.InsertLeadDetails(insertLeadDetailsCommand.LeadName, insertLeadDetailsCommand.LeadPhoneNumber, insertLeadDetailsCommand.LeadEmailId, insertLeadDetailsCommand.UserId, cancellationToken);
            return HeroResult<InsertLeadDetailsModel>.Success(result);
        }
    }
}
