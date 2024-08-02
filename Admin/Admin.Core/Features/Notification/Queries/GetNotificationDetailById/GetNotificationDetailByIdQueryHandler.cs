using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.Notification.Queries.GetNotificationDetailById
{
    public record GetNotificationDetailByIdQuery : IRequest<HeroResult<IEnumerable<GetNotificationDetailByIdVm>>> 
    {
        public string? NotificationId { get; set; }

    }
    internal class GetNotificationDetailByIdQueryHandler : IRequestHandler<GetNotificationDetailByIdQuery, HeroResult<IEnumerable<GetNotificationDetailByIdVm>>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mapper"></param>
        public GetNotificationDetailByIdQueryHandler(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetNotificationDetailByIdVm>>> Handle(GetNotificationDetailByIdQuery request, CancellationToken cancellationToken)
        {
            var leadType = await _notificationRepository.GetNotificationDetailById(request.NotificationId, cancellationToken);
            if (leadType.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetNotificationDetailByIdVm>>(leadType);
                return HeroResult<IEnumerable<GetNotificationDetailByIdVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetNotificationDetailByIdVm>>.Fail("No Record Found");
        }
    }
}
