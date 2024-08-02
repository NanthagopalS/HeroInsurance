using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
using MediatR;
using Microsoft.Extensions.Options;
using static Google.Apis.Requests.BatchRequest;

namespace Insurance.Core.Features.TATA.Queries.GetBreakIn;

public class GetTATABreakInVerificationQuery : IRequest<HeroResult<string>>
{
	public string ProposalNo { get; set; }
	public string TicketId { get; set; }
	public string LeadId { get; set; }
    public string VehicleTypeId { get; set; }
    public string QuoteTransactionId { get; set; }
}

public class GetTATABreakInVerificationQueryHandler : IRequestHandler<GetTATABreakInVerificationQuery, HeroResult<string>>
{
    private readonly IQuoteRepository _quoteRepository;
    private readonly ITATARepository _tataRepository;
    private readonly TATAConfig _tataConfig;
    private readonly IMapper _mapper;
    private readonly IApplicationClaims _applicationClaims;
    public GetTATABreakInVerificationQueryHandler(IQuoteRepository quoteRepository, ITATARepository tataRepository, IMapper mapper, IOptions<TATAConfig> tataConfig, IApplicationClaims applicationClaims)
    {
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        _tataRepository = tataRepository ?? throw new ArgumentNullException(nameof(tataRepository));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		_tataConfig = tataConfig.Value;
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
    }
    public async Task<HeroResult<string>> Handle(GetTATABreakInVerificationQuery request, CancellationToken cancellationToken)
	{
		TATABreakInPaymentRequestModel tataBreakInRequestModel = new()
		{
			ProposalNo = request.ProposalNo,
			TicketId = request.TicketId,
            LeadId = request.LeadId,
		};
		var getTataBreakInResponse = await _tataRepository.VerifyBreakIn(tataBreakInRequestModel,cancellationToken);
		if (getTataBreakInResponse != null && getTataBreakInResponse.InsurerStatusCode.Equals(200))
		{
			var data = getTataBreakInResponse.TATABreakInPesponseDto?.data[0];
            if (data != null && data.inspection_status.Equals("Approved"))
			{
                //Updated the breakIn details
                await UpdateBreakInStatus(request.LeadId, request.QuoteTransactionId, request.ProposalNo, null, "Proposal", true, cancellationToken);
                //Getting lead details to generate paymentlink
                var leadDetails = await _quoteRepository.GetLeadDetails(request.LeadId, "ADB9EB9C-CB73-4DE3-BAF7-151F90C2A6F2", cancellationToken);

                if (!string.IsNullOrEmpty(data.policy[0].payment_id))
                {
                    TATAPaymentRequestModel paymentRequestModel = new()
                    {
                        PAN = leadDetails.PanNumber,
                        MobileNo = leadDetails.PhoneNumber,
                        Email = leadDetails.Email,
                        Name = leadDetails.CustomerType.Equals("COMPANY") ? leadDetails.CompanyName : leadDetails.FirstName + " " + leadDetails.LastName,
                        TATAPaymentId = data.policy[0].payment_id,
                        LeadId = request.LeadId,
                        VehicleTypeId = leadDetails.VehicleTypeId
                    };
                    var paymentResponse = await _tataRepository.GetPaymentLink(paymentRequestModel, cancellationToken);
                    
                    QuoteResponseModel quoteResponse = new()
                    {
                        Type = "INSERT",
                        BreakinId = request.TicketId,
                        PaymentURLLink = paymentResponse.data?.paymentLink_web,
                        TransactionID = leadDetails.QuoteTransactionID,
                        InsurerId = _tataConfig.InsurerId,
                        ApplicationId = data.policy[0].payment_id,
                        ProposalNumber = data.policy[0].proposal_no,
                        GrossPremium = Convert.ToString(data.policy[0].premium_value),
                        PolicyNumber = data.policy[0].policy_id,
                    };
                    var paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(quoteResponse, cancellationToken);
                    
                }
                return HeroResult<string>.Success($"Vehicle Inspection is Completed Successfully with {_tataConfig.InsurerName}.Inspection ID: {request?.TicketId}");
            }
			else if (data != null && data.inspection_status.Equals("Rejected"))
            {
                await UpdateBreakInStatus(request.LeadId, request.QuoteTransactionId, request.ProposalNo, null, "BreakIn", false, cancellationToken);
                return HeroResult<string>.Success($"Vehicle Inspection is Not Successful with {_tataConfig.InsurerName}.Inspection ID: {request?.TicketId}.Please retry with other insurer or reach out to us for assistance.");
            }
            return HeroResult<string>.Success($"Vehicle Inspection is In Progress,{_tataConfig.InsurerName} team will reach out for conducting inspection. Inspection ID: {request?.TicketId} Please save it for future reference.");
        }
        return HeroResult<string>.Fail($"Vehicle Inspection is In Progress,{_tataConfig.InsurerName} team will reach out for conducting inspection. Inspection ID: {request?.TicketId} Please save it for future reference.");
    }
    private async Task UpdateBreakInStatus(string leadId, string quoteTransactionId, string proposalNumber, string paymentURL, string stage, bool breakInStatus, CancellationToken cancellationToken)
    {
        CreateLeadModel createLeadModel = new CreateLeadModel()
        {
            LeadID = leadId,
            QuoteTransactionID = quoteTransactionId,
            PolicyNumber = proposalNumber,
            PaymentLink = paymentURL,
            Stage = stage,
            BreakInStatus = breakInStatus,
            InsurerId = _tataConfig.InsurerId
        };
        await _quoteRepository.UpdateLeadDetails(createLeadModel, cancellationToken);
    }
}
