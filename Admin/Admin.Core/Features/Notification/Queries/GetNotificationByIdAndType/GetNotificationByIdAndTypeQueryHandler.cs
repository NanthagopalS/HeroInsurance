using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.Notification.Queries.GetNotificationByIdAndType;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.Notification.Queries.GetNotificationByIdAndType
{
    public record GetNotificationByIdAndTypeQuery : IRequest<HeroResult<GetNotificationByIdAndTypeVm>>
    {
        public string? UserId { get; set; }
        public string? NotificationCategory { get; set; }

    }
    internal class GetNotificationByIdAndTypeQueryHandler : IRequestHandler<GetNotificationByIdAndTypeQuery, HeroResult<GetNotificationByIdAndTypeVm>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mapper"></param>
        public GetNotificationByIdAndTypeQueryHandler(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetNotificationByIdAndTypeVm>> Handle(GetNotificationByIdAndTypeQuery request, CancellationToken cancellationToken)
        {
            var leadType = await _notificationRepository.GetNotificationByIdAndType(request.UserId, request.NotificationCategory, cancellationToken);
            if (leadType != null)
            {
                var listInsurer = _mapper.Map<GetNotificationByIdAndTypeVm>(leadType);
                return HeroResult<GetNotificationByIdAndTypeVm>.Success(listInsurer);
            }

            return HeroResult<GetNotificationByIdAndTypeVm>.Fail("No Record Found");
        }
    }
}
