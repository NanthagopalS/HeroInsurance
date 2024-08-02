using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
using Insurance.Domain.ICICI.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.ICICI.Command
{
    public class ICICICreateProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
    {
    }
    public class CreateProposalCommandHandler : IRequestHandler<ICICICreateProposalCommand, HeroResult<QuoteResponseModel>>
    {
        private readonly IICICIRepository _iciciRepository;
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;
        private readonly ICICIConfig _iCICIConfig;
        private readonly VehicleTypeConfig _vehicleTypeConfig;

        public CreateProposalCommandHandler(IMapper mapper, IICICIRepository iciciRepository, IQuoteRepository quoteRepository, IOptions<ICICIConfig> options, IOptions<VehicleTypeConfig> vehicleTypeConfig)
        {
            _iciciRepository = iciciRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository;
            _iCICIConfig = options.Value;
            _vehicleTypeConfig = vehicleTypeConfig.Value;
        }
        public async Task<HeroResult<QuoteResponseModel>> Handle(ICICICreateProposalCommand request, CancellationToken cancellationToken)
        {
            var paymentTransactionId = (dynamic)null;
            var createLeadModel = _mapper.Map<ProposalRequestModel>(request);
            var proposalResponse = new SaveQuoteTransactionModel();
            if (request.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
            {
                proposalResponse = await _iciciRepository.CreateCvProposal(createLeadModel, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                proposalResponse = await _iciciRepository.CreateProposal(createLeadModel, cancellationToken).ConfigureAwait(false);
            }
            proposalResponse.IsSharePaymentLink = request.IsSharePaymentLink;
            createLeadModel.LeadID = proposalResponse.LeadId;
            await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken);

            if (proposalResponse != null && proposalResponse.quoteResponseModel.InsurerStatusCode == (int)HttpStatusCode.OK)
            {
                proposalResponse.quoteResponseModel.Type = "INSERT";
                if (proposalResponse.quoteResponseModel.IsBreakIn || proposalResponse.quoteResponseModel.isApprovalRequired || proposalResponse.quoteResponseModel.isQuoteDeviation || proposalResponse.Stage.Equals("BreakIn"))
                {
                    var breakinData = await _iciciRepository.CreateBreakinId(createLeadModel.LeadID, proposalResponse.RequestBody, proposalResponse, request.VehicleTypeId, cancellationToken);
                    if (string.Equals(breakinData?.Item2, "Success", StringComparison.OrdinalIgnoreCase))
                    {
                        InsertBreakInDetailsModel breakInModel = new InsertBreakInDetailsModel()
                        {
                            LeadId = createLeadModel.LeadID,
                            IsBreakIn = true,
                            PolicyNumber = proposalResponse.PolicyNumber,
                            BreakInId = proposalResponse.quoteResponseModel.BreakinId,
                            BreakinInspectionURL = proposalResponse.quoteResponseModel.BreakinInspectionURL,
                            BreakInInspectionAgency = string.Empty
                        };
                        var res = _quoteRepository.InsertBreakInDetails(breakInModel, cancellationToken);

                        proposalResponse.quoteResponseModel.CKYCStatus = "KYC_SUCCESS";
                        proposalResponse.quoteResponseModel.BreakinId = breakinData.Item1;//breakinId;
                        proposalResponse.quoteResponseModel.BreakinStatus = "Initiated";
                        proposalResponse.quoteResponseModel.ValidationMessage = "Vehicle Inspection is Initiated," + _iCICIConfig.InsurerName + " team will reach out for conducting inspection. Inspection ID: " + breakinData.Item1 + " Please save it for future reference.";
                        paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponse.quoteResponseModel, cancellationToken).ConfigureAwait(false);
                        if (paymentTransactionId != null)
                        {
                            return HeroResult<QuoteResponseModel>.Success(proposalResponse.quoteResponseModel);
                        }
                    }
                    else
                    {
                        proposalResponse.quoteResponseModel.BreakinStatus = "Initiated";
                        proposalResponse.quoteResponseModel.ValidationMessage = breakinData?.Item3;
                        proposalResponse.quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                        return HeroResult<QuoteResponseModel>.Fail(proposalResponse.quoteResponseModel.ValidationMessage);
                    }
                }
                else
                {
                    string ResponseBody = string.Empty;
                    if (request.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                    {
                        ICICICVResponseDto mICICICVResponseDto = JsonConvert.DeserializeObject<ICICICVResponseDto>(proposalResponse.ResponseBody);
                        var mResponse = _mapper.Map<ICICIResponseDto>(mICICICVResponseDto);// proposalResponse.ResponseBody
                        ResponseBody = JsonConvert.SerializeObject(mResponse);
                    }
                    else
                    {
                        ResponseBody = proposalResponse.ResponseBody;
                    }
                    //var paymentLink = await _iciciRepository.CreatePaymentLink(createLeadModel.LeadID, proposalResponse.RequestBody, proposalResponse.ResponseBody, cancellationToken);
                    var paymentLink = await _iciciRepository.CreatePaymentLink(createLeadModel.LeadID, proposalResponse.RequestBody, ResponseBody, cancellationToken);

                    if (paymentLink != null)
                    {
                        proposalResponse.quoteResponseModel.PaymentURLLink = paymentLink.Item1;
                        var proposalResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse.quoteResponseModel);
                        proposalResponseModel.CKYCStatus = "KYC_SUCCESS";
                        proposalResponseModel.PaymentURLLink = paymentLink.Item1;
                        proposalResponseModel.PaymentCorrelationId = paymentLink.Item2;
                        paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken).ConfigureAwait(false);
                        if (paymentTransactionId != null)
                        {
                            return HeroResult<QuoteResponseModel>.Success(proposalResponse.quoteResponseModel);
                        }
                        return HeroResult<QuoteResponseModel>.Fail("Insert Payment Transaction Failed");
                    }
                    return HeroResult<QuoteResponseModel>.Fail("Create PaymentLink Failed");
                }
            }
            else
            {
                if (proposalResponse.quoteResponseModel.InsurerStatusCode != (int)HttpStatusCode.OK)
                {
                    proposalResponse.quoteResponseModel.CKYCStatus = "KYC_SUCCESS";
                    proposalResponse.quoteResponseModel.BreakinId = "";//breakinId;
                    proposalResponse.quoteResponseModel.BreakinStatus = "Initiated";
                    proposalResponse.quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    proposalResponse.quoteResponseModel.ValidationMessage = proposalResponse.quoteResponseModel.ValidationMessage;
                    return HeroResult<QuoteResponseModel>.Fail(proposalResponse.quoteResponseModel.ValidationMessage);
                }
            }
            return HeroResult<QuoteResponseModel>.Fail("No Record Found");
        }
    }
}
