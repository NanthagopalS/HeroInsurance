using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using ThirdPartyUtilities.Abstraction;

namespace Admin.Core.Features.TicketManagement.Queries.GetPOSPDetailsByIDToDeActivate
{
    public class GetPOSPDetailsByIDToDeActivateQuery : IRequest<HeroResult<IEnumerable<GetPOSPDetailsByIDToDeActivateVm>>>
    {
        public string? POSPId { get; set; }
    }
    public class GetPOSPDetailsByIDToDeActivateQueryHandler : IRequestHandler<GetPOSPDetailsByIDToDeActivateQuery, HeroResult<IEnumerable<GetPOSPDetailsByIDToDeActivateVm>>>
    {


        private readonly ITicketManagementRepository _ticketManagement;
        private readonly IMapper _mapper;
        private readonly IMongoDBService _mongodbService;


        public GetPOSPDetailsByIDToDeActivateQueryHandler(ITicketManagementRepository ticketManagement, IMapper mapper, IMongoDBService mongoDB)
        {
            _ticketManagement = ticketManagement;
            _mapper = mapper;
            _mongodbService = mongoDB;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPDetailsByIDToDeActivateVm>>> Handle(GetPOSPDetailsByIDToDeActivateQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _ticketManagement.GetPOSPDetailsByIDToDeActivate(request.POSPId, cancellationToken);
            if (modelResult.Any())
            {
                if (modelResult.FirstOrDefault().BusinessTeamApprovalAttachmentDocumentId != null && modelResult.FirstOrDefault().BusinessTeamApprovalAttachmentDocumentId != "")
                {
                    modelResult.FirstOrDefault().BusinessTeamApprovalAttachment = await _mongodbService.MongoDownloadForPOSP(modelResult.FirstOrDefault().BusinessTeamApprovalAttachmentDocumentId);
                }
                if(modelResult.FirstOrDefault().EmailAttachmentDocumentId != null && modelResult.FirstOrDefault().EmailAttachmentDocumentId != "")
                {
                    modelResult.FirstOrDefault().EmailAttachment = await _mongodbService.MongoDownloadForPOSP(modelResult.FirstOrDefault().EmailAttachmentDocumentId);
                }
                var listInsurerModel = _mapper.Map<IEnumerable<GetPOSPDetailsByIDToDeActivateVm>>(modelResult);
                return HeroResult<IEnumerable<GetPOSPDetailsByIDToDeActivateVm>>.Success(listInsurerModel);
            }
            return HeroResult<IEnumerable<GetPOSPDetailsByIDToDeActivateVm>>.Fail("No Record Found");
        }
    }
}
