using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.Notification.Queries.GetAdminAlertType;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.Notification.Queries.GetAdminRecipientType
{
    public record GetAdminRecipientTypeQuery : IRequest<HeroResult<IEnumerable<GetAdminRecipientTypeVm>>> { }
    internal class GetAdminRecipientTypeQueryHandler : IRequestHandler<GetAdminRecipientTypeQuery, HeroResult<IEnumerable<GetAdminRecipientTypeVm>>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mapper"></param>
        public GetAdminRecipientTypeQueryHandler(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetAdminRecipientTypeVm>>> Handle(GetAdminRecipientTypeQuery request, CancellationToken cancellationToken)
        {
            var leadType = await _notificationRepository.GetAdminRecipientType(cancellationToken);
            if (leadType.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetAdminRecipientTypeVm>>(leadType);
                return HeroResult<IEnumerable<GetAdminRecipientTypeVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetAdminRecipientTypeVm>>.Fail("No Record Found");
        }
    }
}
