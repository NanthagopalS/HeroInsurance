using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Customer.Queries.GetCustomersList;
using Insurance.Core.Responses;
using Insurance.Domain.Customer;
using Insurance.Domain.ReportAndMIS;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.ReportAndMIS.Queries
{
    public record GetAllReportQuery : IRequest<HeroResult<GetAllReportVm>>
    {
        public string EnquirId { get; set; }
        public string ProductType { get; set; }
        public string Insurertype { get; set; }
        public string Stage { get; set; }
        public string Startdate { get; set; }
        public string EndDate { get; set; }
        public int CurrentPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
        public string? CreatedBy { get; set; }
    }
    public class GetAllReportQueryHandler : IRequestHandler<GetAllReportQuery, HeroResult<GetAllReportVm>>
    {
        private readonly IReportAndMISRepository _reportAndMISRepository;
        private readonly IMapper _mapper;


        public GetAllReportQueryHandler(IReportAndMISRepository reportAndMISRepository, IMapper mapper)
        {
            _reportAndMISRepository = reportAndMISRepository ?? throw new ArgumentNullException(nameof(reportAndMISRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<GetAllReportVm>> Handle(GetAllReportQuery request, CancellationToken cancellationToken)
        {
            //var reportReq = _mapper.Map<GetAllReportRequestModel>(request);
            //var reportResponseModel = await _reportAndMISRepository.GetAllReport(reportReq, cancellationToken);

            //if (reportResponseModel != null)
            //{
            //    var listCustomer = _mapper.Map<GetAllReportVm>(reportResponseModel);
            //    return HeroResult<GetAllReportVm>.Success(listCustomer);
            //}
            return HeroResult<GetAllReportVm>.Fail("No Record Found");
        }

    }
}
