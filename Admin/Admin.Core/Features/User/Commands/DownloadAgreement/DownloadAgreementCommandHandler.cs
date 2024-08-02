using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetBuDetail;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.DownloadAgreement
{
    public record DownloadAgreementCommand : IRequest<HeroResult<IEnumerable<DownloadAgreementResponse>>>
    {
        public string? POSPId { get; set; }
    }
    internal class DownloadAgreementCommandHandler : IRequestHandler<DownloadAgreementCommand, HeroResult<IEnumerable<DownloadAgreementResponse>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public DownloadAgreementCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<DownloadAgreementResponse>>> Handle(DownloadAgreementCommand command, CancellationToken cancellationToken)
        {
            var download = await _userRepository.DownloadAgreement(command.POSPId, cancellationToken);
            if (download != null)
            {
                var listInsurer = _mapper.Map<IEnumerable<DownloadAgreementResponse>>(download);
                return HeroResult<IEnumerable<DownloadAgreementResponse>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<DownloadAgreementResponse>>.Fail("No Record Found");
        }
    }
}
