using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.UpdateActivateDeActivateBU;
using Admin.Domain.Roles;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.UpdateDocumentStatus
{
    public class UpdateDocumentStatusCommand : IRequest<HeroResult<bool>>
    {
        public string? DocumentId { get; set; }
        public bool IsApprove { get; set; }
        public string? BackOfficeRemark { get; set; }


    }
    internal class UpdateDocumentStatusCommandHandler : IRequestHandler<UpdateDocumentStatusCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        /// <summary>
        /// UpdateBUDetailCommandHandler
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UpdateDocumentStatusCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="updateBUDetailCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(UpdateDocumentStatusCommand updateDocumentStatusCommand, CancellationToken cancellationToken)
        {
            var UpdateActivateDeActivateBU = _mapper.Map<ParticularLeadUploadedDocumentModel>(updateDocumentStatusCommand);
            var result = await _userRepository.UpdateDocumentStatus(updateDocumentStatusCommand, cancellationToken);
            return HeroResult<bool>.Success(result);

        }
    }
 
}
