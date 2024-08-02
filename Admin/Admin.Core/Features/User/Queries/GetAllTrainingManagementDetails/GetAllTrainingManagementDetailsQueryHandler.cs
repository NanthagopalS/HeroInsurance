using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetAllPOSPDetailForIIBDashboard;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetAllTrainingManagementDetails
{
    public class GetAllTrainingManagementDetailsQuery : IRequest<HeroResult<GetAllTrainingManagementDetailsVm>>
    {
        public string? Searchtext { get; set; }
        public string? Category { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public IEnumerable<AllTrainingManagementDetails>? AllTrainingManagementDetails { get; set; }
        public IEnumerable<TrainingManagementDetailsPagingModel>? TrainingManagementDetailsPagingModel { get; set; }
    }
    internal class GetAllTrainingManagementDetailsQueryHandler : IRequestHandler<GetAllTrainingManagementDetailsQuery, HeroResult<GetAllTrainingManagementDetailsVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetAllTrainingManagementDetailsQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetAllTrainingManagementDetailsVm>> Handle(GetAllTrainingManagementDetailsQuery request, CancellationToken cancellationToken)
        {
            var allTrainingManagement = await _userRepository.GetAllTrainingManagementDetails(request.Searchtext, request.Category, request.StartDate, request.EndDate, request.PageIndex,request.PageSize, cancellationToken).ConfigureAwait(false);
            if (allTrainingManagement != null)
            {
                var listInsurer = _mapper.Map<GetAllTrainingManagementDetailsVm>(allTrainingManagement);
                return HeroResult<GetAllTrainingManagementDetailsVm>.Success(listInsurer);
            }
            return HeroResult<GetAllTrainingManagementDetailsVm>.Fail("No Record Found");
        }

    }
}
