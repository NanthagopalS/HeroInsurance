using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Bajaj;
using Insurance.Domain.ICICI;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.Bajaj.Command.CheckBreakinPinStatus;

public class BajajBreakinPinStatusCommand : IRequest<HeroResult<string>>
{
    public string QuotetransactionId { get; set; }
    public string VehicleNumber { get; set; }
    public string LeadId { get; set; }
}
internal class BajajBreakinPinStatusCommandHandler : IRequestHandler<BajajBreakinPinStatusCommand, HeroResult<string>>
{
    private readonly IBajajRepository _bajajRepository;
    private readonly IMapper _mapper;
    private readonly IQuoteRepository _quoteRepositor;
    private readonly BajajConfig _bajajConfig;

    public BajajBreakinPinStatusCommandHandler(IMapper mapper, IBajajRepository bajajRepository, IQuoteRepository quoteRepository, IOptions<BajajConfig> options)
    {
        _bajajRepository = bajajRepository;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _quoteRepositor = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        _bajajConfig = options.Value;
    }
    public async Task<HeroResult<string>> Handle(BajajBreakinPinStatusCommand request, CancellationToken cancellationToken)
    {
        var response = await _bajajRepository.GetBreakinPinStatus(request.LeadId, request.VehicleNumber, cancellationToken).ConfigureAwait(false);
        if (response != null && response.Item1.pinList != null)
        {
            if (response.Item1.pinList[response.Item1.pinList.Length - 1].stringval2.Equals("PIN_APPRD"))
            {
                //var quoteId = await _bajajRepository.GetQuoteTransactionId(request.VehicleNumber, request.QuotetransactionId, cancellationToken);
                //ProposalRequestModel proposalRequestModel = new ProposalRequestModel()
                //{
                //    QuoteTransactionID = quoteId
                //};
                //var proposalResponse = await _bajajRepository.CreateProposal(proposalRequestModel, cancellationToken);

                //if (proposalResponse.quoteResponseModel.PaymentURLLink != null)
                //{
                //    response.Item2.QuoteTransactionID = proposalResponse.quoteResponseModel.TransactionID;
                //    response.Item2.PaymentLink = proposalResponse.quoteResponseModel.PaymentURLLink;
                //    var updateLead = _quoteRepositor.UpdateLeadDetails(response.Item2, cancellationToken);
                //    return HeroResult<string>.Success($"Vehicle Inspection is Completed Successfully with {_bajajConfig.InsurerName}.Inspection ID: {response.Item1.pinList[response.Item1.pinList.Length - 1].stringval1}");
                //}
                //response.Item2.PaymentLink = proposalResponse.quoteResponseModel.PaymentURLLink;

                response.Item2.QuoteTransactionID = request.QuotetransactionId;
                response.Item2.InsurerId = _bajajConfig.InsurerId;
                var updateLead = _quoteRepositor.UpdateLeadDetails(response.Item2, cancellationToken);
                return HeroResult<string>.Success($"Vehicle Inspection is Completed Successfully with {_bajajConfig.InsurerName}.Inspection ID: {response.Item1.pinList[response.Item1.pinList.Length - 1].stringval1}");

                //return HeroResult<string>.Fail("Issue With Creating Proposal");
            }
            else if (response.Item1.pinList[response.Item1.pinList.Length - 1].stringval2.Equals("PGNR_ALTD"))
            {
                return HeroResult<string>.Fail($"Vehicle Inspection is In Progress,{_bajajConfig.InsurerName} team will reach out for conducting inspection.");
            }
            else //if (response.Item1.pinList[response.Item1.pinList.Length - 1].stringval2.Equals(""))
            {
                var updateLead = _quoteRepositor.UpdateLeadDetails(response.Item2, cancellationToken);
                return HeroResult<string>.Fail($"Vehicle Inspection is Not Successful with {_bajajConfig.InsurerName}.Inspection ID: {response.Item1.pinList[0].stringval1}.Please retry with other insurer or reach out to us for assistance.");
            }
        }
        return HeroResult<string>.Fail("No Record Found");
    }
}
