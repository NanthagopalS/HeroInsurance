using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Features.POSP.Commands.InsertTrainingMaterialDetail;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Commands.InsertUserDeviceDetails
{
    public record InsertUserDeviceDetailsCommand : IRequest<HeroResult<bool>>
    {
        public string? UserId { get; set; }
        public string? MobileDeviceId { get; set; }
        public string? BrowserId { get; set; }
        public string? GfcToken { get; set; }

    }
    public class InsertUserDeviceDetailsCommandHandler : IRequestHandler<InsertUserDeviceDetailsCommand, HeroResult<bool>>
    {
        private readonly IPOSPRepository _pospRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// InsertUserDeviceDetailsCommandHandler
        /// </summary>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertUserDeviceDetailsCommandHandler(IPOSPRepository pospRepository, IMapper mapper)
        {
            _pospRepository = pospRepository ?? throw new ArgumentNullException(nameof(pospRepository));
            _mapper = mapper;
        }


        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="insertUserDeviceDetailsCommand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<bool>> Handle(InsertUserDeviceDetailsCommand insertUserDeviceDetailsCommand, CancellationToken cancellationToken)
        {
            var userDevice = _mapper.Map<InsertUserDeviceDetailsModel>(insertUserDeviceDetailsCommand);
            var result = await _pospRepository.InsertUserDeviceDetails(insertUserDeviceDetailsCommand, cancellationToken);
            return HeroResult<bool>.Success(result);
        }
    }
}
