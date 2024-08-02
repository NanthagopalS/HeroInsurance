using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetAllTrainingManagementDetails;
using Admin.Domain.HelpAndSupport;
using Admin.Domain.User;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.HelpAndSupport.Queries.GetAllHelpAndSupport
{
    public class GetAllHelpAndSupportQuery : IRequest<HeroResult<GetAllHelpAndSupportVm>>
    {
        public string? Searchtext { get; set; }
        public string? UserId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? CurrentPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
    }
        
    internal class GetAllHelpAndSupportQueryHandler : IRequestHandler<GetAllHelpAndSupportQuery, HeroResult<GetAllHelpAndSupportVm>>
    {
        private readonly IHelpAndSupportRepository _helpAndSupportRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public GetAllHelpAndSupportQueryHandler(IHelpAndSupportRepository helpAndSupportRepository, IMapper mapper)
        {
            _helpAndSupportRepository = helpAndSupportRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<GetAllHelpAndSupportVm>> Handle(GetAllHelpAndSupportQuery request, CancellationToken cancellationToken)
        {
            var allHelpAndRequest = await _helpAndSupportRepository.GetAllHelpAndSupport(request.Searchtext, request.UserId, request.StartDate, request.EndDate, request.CurrentPageIndex, request.CurrentPageSize, cancellationToken);
            if (allHelpAndRequest != null)
            {
                var listInsurer = _mapper.Map<GetAllHelpAndSupportVm>(allHelpAndRequest);
                return HeroResult<GetAllHelpAndSupportVm>.Success(listInsurer);
            }
            return HeroResult<GetAllHelpAndSupportVm>.Fail("No Record Found");
        }

    }
}
