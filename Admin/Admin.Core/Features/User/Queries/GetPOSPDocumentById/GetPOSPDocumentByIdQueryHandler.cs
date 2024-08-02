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
    public class GetPOSPDocumentByIdQuery : IRequest<HeroResult<GetPOSPDocumentByIdVm>>
    {
        public string? DocumentId { get; set; }
    }
    public class GetPOSPDocumentByIdQueryHandler : IRequestHandler<GetPOSPDocumentByIdQuery, HeroResult<GetPOSPDocumentByIdVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPDocumentByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetPOSPDocumentByIdVm>> Handle(GetPOSPDocumentByIdQuery request, CancellationToken cancellationToken)
        {
            var leadDetail = await _userRepository.GetPOSPDocumentById(request.DocumentId, cancellationToken).ConfigureAwait(false);
            if (leadDetail != null)
            {
                var listInsurer = _mapper.Map<GetPOSPDocumentByIdVm>(leadDetail);
                return HeroResult<GetPOSPDocumentByIdVm>.Success(listInsurer);
            }
            return HeroResult<GetPOSPDocumentByIdVm>.Fail("No Record Found");
        }
    }
}
