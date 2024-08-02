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

namespace Admin.Core.Features.User.Queries.GetAllExamManagementDetail
{
    public record GetAllExamManagementDetailQuery : IRequest<HeroResult<GetAllExamManagementDetailVm>>
    {
        public string? Searchtext { get; set; }
        public string? Category { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public IEnumerable<ExamManagementDetailModel>? ExamManagementDetailModel { get; set; }
        public IEnumerable<ExamManagementDetailPagingModel>? ExamManagementDetailPagingModel { get; set; }
    }
    public class GetAllExamManagementDetailQueryHandler : IRequestHandler<GetAllExamManagementDetailQuery, HeroResult<GetAllExamManagementDetailVm>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetAllExamManagementDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetAllExamManagementDetailVm>> Handle(GetAllExamManagementDetailQuery request, CancellationToken cancellationToken)
        {
            var getLeadManagementDetail = await _userRepository.GetAllExamManagementDetail(request.Searchtext, request.Category, request.StartDate, request.EndDate, request.PageIndex,request.PageSize, cancellationToken).ConfigureAwait(false);
            if (getLeadManagementDetail != null)
            {
                var listInsurer = _mapper.Map<GetAllExamManagementDetailVm>(getLeadManagementDetail);
                return HeroResult<GetAllExamManagementDetailVm>.Success(listInsurer);
            }
            return HeroResult<GetAllExamManagementDetailVm>.Fail("No Record Found");
        }
    }
}
