using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using AutoMapper;
using Admin.Core.Responses;
using MailKit.Search;
using MediatR;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetLeadManagementDetail
{
    public record GetLeadManagementDetailQuery : IRequest<HeroResult<GetLeadManagementDetailVm>>
    {
        public string? ViewLeadsType { get; set; }
        public string? POSPId { get; set; }
        public string? SearchText { get; set; }
        public string? LeadType { get; set; }
        public string? PolicyType { get; set; }
        public string? PreQuote { get; set; }
        public string? AllStatus { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? UserId { get; set; }

        public bool isMask { get; set; } 
        public IEnumerable<LeadDetailModelList>? LeadDetailModelList { get; set; }
        public IEnumerable<LeadDetailPagingModel>? LeadDetailPagingModel { get; set; }

    }
    public class GetLeadManagementDetailQueryHandler : IRequestHandler<GetLeadManagementDetailQuery, HeroResult<GetLeadManagementDetailVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetLeadManagementDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetLeadManagementDetailVm>> Handle(GetLeadManagementDetailQuery request, CancellationToken cancellationToken)
        {
            var getLeadManagementDetail = await _userRepository.GetLeadManagementDetail(request, cancellationToken).ConfigureAwait(false);
            if (getLeadManagementDetail != null)
            {
                var listInsurer = _mapper.Map<GetLeadManagementDetailVm>(getLeadManagementDetail);
                return HeroResult<GetLeadManagementDetailVm>.Success(listInsurer);
            }
            return HeroResult<GetLeadManagementDetailVm>.Fail("No Record Found");
        }
    }
}
