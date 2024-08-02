using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetParticularLeadDetail;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetParticularLeadUploadedDocument
{
    public class GetParticularLeadUploadedDocumentQuery : IRequest<HeroResult<IEnumerable<GetParticularLeadUploadedDocumentVm>>>
    {
        public string? UserId { get; set; }
    }
    public class GetParticularLeadUploadedDocumentQueryHandler : IRequestHandler<GetParticularLeadUploadedDocumentQuery, HeroResult<IEnumerable<GetParticularLeadUploadedDocumentVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetParticularLeadUploadedDocumentQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetParticularLeadUploadedDocumentVm>>> Handle(GetParticularLeadUploadedDocumentQuery request, CancellationToken cancellationToken)
        {
            var leadDetail = await _userRepository.GetParticularLeadUploadedDocument(request.UserId, cancellationToken).ConfigureAwait(false);
            if (leadDetail.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetParticularLeadUploadedDocumentVm>>(leadDetail);
                return HeroResult<IEnumerable<GetParticularLeadUploadedDocumentVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetParticularLeadUploadedDocumentVm>>.Fail("No Record Found");
        }
    }
}
