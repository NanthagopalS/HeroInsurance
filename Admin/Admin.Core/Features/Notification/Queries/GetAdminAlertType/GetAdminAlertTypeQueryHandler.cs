using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.Notification.Queries.GetAdminAlertType
{
    public record GetAdminAlertTypeQuery : IRequest<HeroResult<IEnumerable<GetAdminAlertTypeVm>>> { }
    internal class GetAdminAlertTypeQueryQueryHandler : IRequestHandler<GetAdminAlertTypeQuery, HeroResult<IEnumerable<GetAdminAlertTypeVm>>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mapper"></param>
        public GetAdminAlertTypeQueryQueryHandler(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetAdminAlertTypeVm>>> Handle(GetAdminAlertTypeQuery request, CancellationToken cancellationToken)
        {
            var leadType = await _notificationRepository.GetAdminAlertType(cancellationToken);
            if (leadType.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetAdminAlertTypeVm>>(leadType);
                return HeroResult<IEnumerable<GetAdminAlertTypeVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetAdminAlertTypeVm>>.Fail("No Record Found");
        }
    }
}
