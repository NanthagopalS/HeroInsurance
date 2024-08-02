using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Commands.InsertExamCertificate;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.InsertAgreement
{
    public record AgreementUploadCommand : IRequest<HeroResult<bool>>
    {
        public string? UserId { get; set; }
        public string? PreSignedAgreementId { get; set; }
        public byte[]? SignatureImage { get; set; }
        public string? Environment { get; set; }

    }
    public class InsertAgreementCommandHandler : IRequestHandler<AgreementUploadCommand, HeroResult<bool>>
    {
        private readonly IPOSPRepository _posprepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userDocumentRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertAgreementCommandHandler(IPOSPRepository posprepository, IMapper mapper)
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
        public async Task<HeroResult<bool>> Handle(AgreementUploadCommand agrrementcommand, CancellationToken cancellationToken)
        {
            var agreementdetailmodel = _mapper.Map<AgreementModel>(agrrementcommand);
            var result = await _posprepository.AgreementUpload(agreementdetailmodel);
            return HeroResult<bool>.Success(result);
        }

    }
}
