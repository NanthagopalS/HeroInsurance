using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Models.Email;

namespace Admin.Core.Features.TicketManagement.Queries.GetPOSPDetailsByDeactiveTicketId
{
    public class GetPOSPDetailsByDeactiveTicketIdQuery : IRequest<HeroResult<IEnumerable<GetPOSPDetailsByDeactiveTicketIdVm>>>
    {
        public string? POSPId { get; set; }
    }
    public class GetPOSPDetailsByDeactiveTicketIdQueryHandler : IRequestHandler<GetPOSPDetailsByDeactiveTicketIdQuery, HeroResult<IEnumerable<GetPOSPDetailsByDeactiveTicketIdVm>>>
    {


        private readonly ITicketManagementRepository _ticketManagement;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;


        public GetPOSPDetailsByDeactiveTicketIdQueryHandler(ITicketManagementRepository ticketManagement, IMapper mapper,IEmailService email)
        {
            _ticketManagement = ticketManagement;
            _mapper = mapper;
            _emailService = email;
        }

        public async Task<HeroResult<IEnumerable<GetPOSPDetailsByDeactiveTicketIdVm>>> Handle(GetPOSPDetailsByDeactiveTicketIdQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _ticketManagement.GetPOSPDetailsByDeactiveTicketId(request.POSPId, cancellationToken);
            if (modelResult.Any())
            {
                var listInsurerModel = _mapper.Map<IEnumerable<GetPOSPDetailsByDeactiveTicketIdVm>>(modelResult);
                
                    var record = listInsurerModel.FirstOrDefault();
                    string address = record.AddressLine1 + " " + record.AddressLine2 + ", " + record.City + ", " + record.State + ", " + record.City + ", " + record.Pincode;

                    DateTime releavingDate = record.releavingDate;
                    DateTime effectiveFrom = record.effectiveFrom;
                    EmailForNOCModel nocModelEmail = new EmailForNOCModel
                        {
                            ToEmailId = record.EmailId.ToString(),
                            Name = record.POSPName.ToString(),
                            PanNumber = record.PanNumber.ToString(),
                            Address = address.ToString(),
                            JoiningDate = record.JoinDate,
                            ReleavingData = releavingDate,
                            PospId = record.POSPId.ToString(),
                            EmailDate = releavingDate
                        };
                    _emailService.SendEmailForNOC(nocModelEmail, cancellationToken);

                return HeroResult<IEnumerable<GetPOSPDetailsByDeactiveTicketIdVm>>.Success(listInsurerModel);   
            }
            return HeroResult<IEnumerable<GetPOSPDetailsByDeactiveTicketIdVm>>.Fail("No Record Found");
        }
    }
}
