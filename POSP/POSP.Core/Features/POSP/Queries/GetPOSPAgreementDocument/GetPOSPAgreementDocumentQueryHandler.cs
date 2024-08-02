using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetPOSPAgreementDocument
{
    public class GetPOSPAgreementDocumentQuery : IRequest<HeroResult<IEnumerable<GetPOSPAgreementDocumentVm>>>
    {
        public string UserId { get; set; }
    }



    public class GetPOSPAgreementDocumentQueryHandler : IRequestHandler<GetPOSPAgreementDocumentQuery, HeroResult<IEnumerable<GetPOSPAgreementDocumentVm>>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetPOSPAgreementDocumentQueryHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPAgreementDocumentVm>>> Handle(GetPOSPAgreementDocumentQuery request, CancellationToken cancellationToken)
        {
            var getPOSPAgreementDocument = await _pospRepository.GetPOSPAgreementDocument(request.UserId, cancellationToken).ConfigureAwait(false);
            if (getPOSPAgreementDocument.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetPOSPAgreementDocumentVm>>(getPOSPAgreementDocument);
                return HeroResult<IEnumerable<GetPOSPAgreementDocumentVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetPOSPAgreementDocumentVm>>.Fail("No Record Found");
        }
    }
}
