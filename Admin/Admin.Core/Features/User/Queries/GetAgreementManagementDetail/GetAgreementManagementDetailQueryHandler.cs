using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetAgreementManagementDetail
{
    public record GetAgreementManagementDetailQuery : IRequest<HeroResult<GetAgreementManagementDetailVm>>
    {
        public string? SearchText { get; set; }
        public string? StatusId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<AgreementDetailListModel>? AgreementDetailListModel { get; set; }
        public IEnumerable<AgreementDetailPagingModel>? AgreementDetailPagingModel { get; set; }

    }
    public class GetAgreementManagementDetailQueryHandler : IRequestHandler<GetAgreementManagementDetailQuery, HeroResult<GetAgreementManagementDetailVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetAgreementManagementDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetAgreementManagementDetailVm>> Handle(GetAgreementManagementDetailQuery request, CancellationToken cancellationToken)
        {
            var getLeadManagementDetail = await _userRepository.GetAgreementManagementDetail(request.SearchText, request.StatusId, request.StartDate, request.EndDate, request.PageIndex,request.PageSize, cancellationToken).ConfigureAwait(false);
            if (getLeadManagementDetail != null)
            {
                var listInsurer = _mapper.Map<GetAgreementManagementDetailVm>(getLeadManagementDetail);
                return HeroResult<GetAgreementManagementDetailVm>.Success(listInsurer);
            }
            return HeroResult<GetAgreementManagementDetailVm>.Fail("No Record Found");
        }
    }
}
