using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetAllBUDetail;
using Admin.Core.Features.User.Queries.GetAllUserRoleMappingDetailModel;
using Admin.Domain.Roles;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetAllPOSPDetailForIIBDashboard
{
    public class GetAllPOSPDetailForIIBDashboardQuery : IRequest<HeroResult<GetAllPOSPDetailForIIBDashboardVm>>
    {
        public string? Searchtext { get; set; }
        public string? CreatedBy { get; set; }
        public string? StatusType { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }

        public IEnumerable<AllPOSPDetailDataModel>? AllPOSPDetailDataModel { get; set; }
        public IEnumerable<AllPOSPDetailDataPaginationModel>? AllPOSPDetailDataPaginationModel { get; set; }
    }
    public class GetAllPOSPDetailForIIBDashboardQueryHandler : IRequestHandler<GetAllPOSPDetailForIIBDashboardQuery, HeroResult<GetAllPOSPDetailForIIBDashboardVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetAllPOSPDetailForIIBDashboardQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetAllPOSPDetailForIIBDashboardVm>> Handle(GetAllPOSPDetailForIIBDashboardQuery request, CancellationToken cancellationToken)
        {
            var roleDetailMapInput = _mapper.Map<AllPOSPDetailForIIBInputModel>(request);
            var modelResult = await _userRepository.GetAllPOSPDetailForIIBDashboard(roleDetailMapInput, cancellationToken).ConfigureAwait(false);
            if (modelResult != null)
            {
                var listInsurer = _mapper.Map<GetAllPOSPDetailForIIBDashboardVm>(modelResult);
                return HeroResult<GetAllPOSPDetailForIIBDashboardVm>.Success(listInsurer);
            }
            return HeroResult<GetAllPOSPDetailForIIBDashboardVm>.Fail("No Record Found");
        }
    }
}
