using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.InsertAgreementId
{
    public record InsertAgreementIdCommand : IRequest<HeroResult<POSPAgreementDocumentModel>>
    {
        public string? UserId { get; set; }
        public string? AgreementId { get; set; }
        public string? ProcessType { get; set; }
    }





    public class InsertAgreementIdCommandHandler : IRequestHandler<InsertAgreementIdCommand, HeroResult<POSPAgreementDocumentModel>>
    {
        private readonly IPOSPRepository _posprepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userDocumentRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertAgreementIdCommandHandler(IPOSPRepository posprepository, IMapper mapper)
        {
            _posprepository = posprepository ?? throw new ArgumentNullException(nameof(posprepository));
            _mapper = mapper;
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<POSPAgreementDocumentModel>> Handle(InsertAgreementIdCommand insertAgreementIdcommand, CancellationToken cancellationToken)
        {
            var agreementDetailModel = _mapper.Map<POSPAgreementDocumentModel>(insertAgreementIdcommand);
            var result = await _posprepository.InsertAgreementId(agreementDetailModel);
            return HeroResult<POSPAgreementDocumentModel>.Success(result);
        }

    }
}
