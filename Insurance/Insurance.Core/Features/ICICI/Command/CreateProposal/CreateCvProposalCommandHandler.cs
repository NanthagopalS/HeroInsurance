using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
using Insurance.Domain.ICICI.Response;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Core.Features.ICICI.Command.Cv
{
    public class IciciCreateCvProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
    {
    }
    public class IciciCreateCvProposalCommandHandler : IRequestHandler<IciciCreateCvProposalCommand, HeroResult<QuoteResponseModel>>
    {
        private readonly IICICIRepository _iciciRepository;
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;
        private readonly ICICIConfig _iCICIConfig;
        public IciciCreateCvProposalCommandHandler(IMapper mapper, IICICIRepository iciciRepository, IQuoteRepository quoteRepository, IOptions<ICICIConfig> options)
        {
            _iciciRepository = iciciRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository;
            _iCICIConfig = options.Value;
        }
        public async Task<HeroResult<QuoteResponseModel>> Handle(IciciCreateCvProposalCommand request, CancellationToken cancellationToken)
        {
            var paymentTransactionId = (dynamic)null;
            var createLeadModel = _mapper.Map<ProposalRequestModel>(request);
            var proposalResponse = await _iciciRepository.CreateCvProposal(createLeadModel, cancellationToken);
            proposalResponse.IsSharePaymentLink = request.IsSharePaymentLink;
            await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken);
            createLeadModel.LeadID = proposalResponse.LeadId;
            if (proposalResponse is not null)
            {
                proposalResponse.quoteResponseModel.Type = "INSERT";
                if (proposalResponse.quoteResponseModel.IsBreakIn || proposalResponse.quoteResponseModel.isApprovalRequired || proposalResponse.quoteResponseModel.isQuoteDeviation || proposalResponse.Stage.Equals("BreakIn"))
                {
                    var breakinId = await _iciciRepository.CreateBreakinId(createLeadModel.LeadID, proposalResponse.RequestBody, proposalResponse.ResponseBody, request.VehicleTypeId, cancellationToken);
                    proposalResponse.quoteResponseModel.CKYCStatus = "KYC_SUCCESS";
                    proposalResponse.quoteResponseModel.BreakinId = breakinId;
                    proposalResponse.quoteResponseModel.BreakinStatus = "Initiated";
                    proposalResponse.quoteResponseModel.ValidationMessage = "Vehicle Inspection is Initiated," + _iCICIConfig.InsurerName + " team will reach out for conducting inspection. Inspection ID: " + breakinId + " Please save it for future reference.";
                    paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponse.quoteResponseModel, cancellationToken).ConfigureAwait(false);
                    if (paymentTransactionId != null)
                    {
                        return HeroResult<QuoteResponseModel>.Success(proposalResponse.quoteResponseModel);
                    }
                }
                else
                {
                    ICICICVResponseDto mICICICVResponseDto = JsonConvert.DeserializeObject<ICICICVResponseDto>(proposalResponse.ResponseBody);
                    var mResponseBody = _mapper.Map<ICICIResponseDto>(mICICICVResponseDto);// proposalResponse.ResponseBody
                    //string ddd = JsonConvert.SerializeObject(mResponseBody);
                    var paymentLink = await _iciciRepository.CreatePaymentLink(createLeadModel.LeadID, proposalResponse.RequestBody, JsonConvert.SerializeObject(mResponseBody), cancellationToken);
                    if (paymentLink != null)
                    {
                        proposalResponse.quoteResponseModel.PaymentURLLink = paymentLink.Item1;
                        var proposalResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse.quoteResponseModel);
                        proposalResponseModel.CKYCStatus = "KYC_SUCCESS";
                        proposalResponseModel.PaymentURLLink = paymentLink.Item1;
                        proposalResponseModel.PaymentCorrelationId = paymentLink.Item2;
                        paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken).ConfigureAwait(false);
                        if (paymentTransactionId is not null)
                        {
                            return HeroResult<QuoteResponseModel>.Success(proposalResponse.quoteResponseModel);
                        }
                        return HeroResult<QuoteResponseModel>.Fail("Insert Payment Transaction Failed");
                    }
                    return HeroResult<QuoteResponseModel>.Fail("Create PaymentLink Failed");
                }
            }
            return HeroResult<QuoteResponseModel>.Fail("No Record Found");
        }
    }
}
