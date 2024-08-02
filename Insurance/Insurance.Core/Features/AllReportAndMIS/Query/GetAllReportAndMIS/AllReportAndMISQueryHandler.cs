using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Customer.Queries.GetCustomersList;
using Insurance.Core.Responses;
using Insurance.Domain.AllReportAndMIS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.AllReportAndMIS.Query.GetAllReportAndMIS
{
    public class AllReportAndMISQuery : IRequest<HeroResult<AllReportAndMISVm>>
    {
        public string EnquiryId { get; set; }
        public string ProductType { get; set; }
        public string Insurertype { get; set; }
        public string Stage { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int CurrentPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
    }
    public class AllReportAndMISQueryHandler : IRequestHandler<AllReportAndMISQuery, HeroResult<AllReportAndMISVm>>
    {
        private readonly IReportAndMISRepository _reportAndMISRepository;
        private readonly IMapper _mapper;


        public AllReportAndMISQueryHandler(IReportAndMISRepository reportAndMISRepository, IMapper mapper)
        {
            _reportAndMISRepository = reportAndMISRepository ?? throw new ArgumentNullException(nameof(reportAndMISRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<AllReportAndMISVm>> Handle(AllReportAndMISQuery request, CancellationToken cancellationToken)
        {
            var report = _mapper.Map<AllReportAndMISRequestModel>(request);
            var reportResult = await _reportAndMISRepository.AllReportAndMIS(report, cancellationToken).ConfigureAwait(false);
            if (reportResult != null)
            {
                var listUserRoleModel = _mapper.Map<AllReportAndMISVm>(reportResult);
                return HeroResult<AllReportAndMISVm>.Success(listUserRoleModel);
            }
            return HeroResult<AllReportAndMISVm>.Fail("No Record Found");
        }
        
    }
}
