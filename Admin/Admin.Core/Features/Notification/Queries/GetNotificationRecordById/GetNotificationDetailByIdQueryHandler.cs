using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.Notification.Queries.GetNotificationRecordById
{
    public record GetNotificationRecordByIdQuery : IRequest<HeroResult<IEnumerable<GetNotificationRecordByIdVm>>> 
    {
        public string? NotificationId { get; set; }

    }
    internal class GetNotificationRecordByIdQueryHandler : IRequestHandler<GetNotificationRecordByIdQuery, HeroResult<IEnumerable<GetNotificationRecordByIdVm>>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="mapper"></param>
        public GetNotificationRecordByIdQueryHandler(INotificationRepository notificationRepository, IMapper mapper)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<GetNotificationRecordByIdVm>>> Handle(GetNotificationRecordByIdQuery request, CancellationToken cancellationToken)
        {
            var leadType = await _notificationRepository.GetNotificationRecordById(request.NotificationId, cancellationToken);
            if (leadType.Any())
            {
                var listInsurer = _mapper.Map<IEnumerable<GetNotificationRecordByIdVm>>(leadType);
                return HeroResult<IEnumerable<GetNotificationRecordByIdVm>>.Success(listInsurer);
            }

            return HeroResult<IEnumerable<GetNotificationRecordByIdVm>>.Fail("No Record Found");
        }
    }
}
