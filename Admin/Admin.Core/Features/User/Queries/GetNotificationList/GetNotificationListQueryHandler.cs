using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetRecipientList;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetNotificationList
{
    public class GetNotificationListQuery : IRequest<HeroResult<GetNotificationListVm>>
    {
        public string? SearchText { get; set; }
        public string? RecipientTypeId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? CurrentPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
        public IEnumerable<NotificationListModel>? NotificationListModel { get; set; }
        public IEnumerable<NotificationPagingModel>? NotificationPagingModel { get; set; }
    }
    public class GetNotificationListQueryHandler : IRequestHandler<GetNotificationListQuery, HeroResult<GetNotificationListVm>>
    {


        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetNotificationListQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<GetNotificationListVm>> Handle(GetNotificationListQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _userRepository.GetNotificationList(request.SearchText,
                request.RecipientTypeId, request.StartDate, request.EndDate, request.CurrentPageIndex, request.CurrentPageSize, cancellationToken).ConfigureAwait(false);
            if (modelResult != null)
            {
                var listInsurerModel = _mapper.Map<GetNotificationListVm>(modelResult);
                return HeroResult<GetNotificationListVm>.Success(listInsurerModel);
            }
            return HeroResult<GetNotificationListVm>.Fail("No Record Found");
        }
    }
}
