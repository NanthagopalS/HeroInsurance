using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.Reliance;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using static Google.Apis.Requests.BatchRequest;

namespace Insurance.Core.Features.Reliance.Command.CreateProposal
{
    public class RelianceCreateProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
    {

    }
    public class CreateProposalCommandHandler : IRequestHandler<RelianceCreateProposalCommand, HeroResult<QuoteResponseModel>>
    {
        private readonly IRelianceRepository _relianceRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly RelianceConfig _relianceConfig;
        private readonly PolicyTypeConfig _policyTypeConfig;
        private readonly IApplicationClaims _applicationClaims;
        private readonly VehicleTypeConfig VehicleType;
        public CreateProposalCommandHandler(IMapper mapper, IRelianceRepository relianceRepository, IQuoteRepository quoteRepository, IOptions<RelianceConfig> options, IApplicationClaims applicationClaims, IOptions<PolicyTypeConfig> policyTypeConfig, IOptions<VehicleTypeConfig> vehicleType)
        {
            _relianceRepository = relianceRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository;
            _relianceConfig = options.Value;
            _policyTypeConfig = policyTypeConfig.Value;
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
            VehicleType = vehicleType?.Value;
        }
        public async Task<HeroResult<QuoteResponseModel>> Handle(RelianceCreateProposalCommand request, CancellationToken cancellationToken)
        {
            var proposalRequest = _mapper.Map<ProposalRequestModel>(request);
            SaveQuoteTransactionModel proposalResponse = new SaveQuoteTransactionModel();
            var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(proposalRequest.QuoteTransactionID, cancellationToken);
            if (_quotedetails != null)
            {
                QuoteResponseModel quoteResponseModel;
                var relianceCkycDetails = new RelianceCKYCRequestModel();
                RelianceRequestDto _relianceProposal = JsonConvert.DeserializeObject<RelianceRequestDto>(_quotedetails.QuoteTransactionRequest?.RequestBody);
                RelianceQuoteResponseDto quoteResponse = JsonConvert.DeserializeObject<RelianceQuoteResponseDto>(_quotedetails.QuoteTransactionRequest?.ResponseBody);
                proposalResponse.TransactionId = _quotedetails.QuoteTransactionRequest?.TransactionId;
                proposalResponse.PolicyNumber = _quotedetails.QuoteTransactionRequest?.ProposalId;
                proposalResponse.PolicyId = _quotedetails.QuoteTransactionRequest?.PolicyId;
                CreateLeadModel _leadDetails = (_quotedetails.LeadDetail);
                RelianceProposalRequest _relianceProposalRequest = JsonConvert.DeserializeObject<RelianceProposalRequest>(_quotedetails.ProposalRequestBody);
                if (!string.IsNullOrWhiteSpace(_quotedetails.CKYCRequestBody))
                {
                    relianceCkycDetails = JsonConvert.DeserializeObject<RelianceCKYCRequestModel>(_quotedetails.CKYCRequestBody);
                }

                _relianceProposal.PolicyDetails.Risk.IDV = quoteResponse.MotorPolicy.IDV;
                proposalResponse = await _relianceRepository.CreateProposal(_relianceProposal, _relianceProposalRequest, _leadDetails, relianceCkycDetails, cancellationToken);

                proposalResponse.QuoteTransactionId = proposalRequest.QuoteTransactionID;
                proposalResponse.IsSharePaymentLink = request.IsSharePaymentLink;
                // Save or Update in Quote Transaction Table
                await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken);
                if (proposalResponse != null && proposalResponse.quoteResponseModel.InsurerStatusCode == 200)
                {
                    if (proposalResponse.quoteResponseModel.IsBreakIn || proposalResponse.quoteResponseModel.IsSelfInspection)
                    {
                        InsertBreakInDetailsModel breakInModel = new InsertBreakInDetailsModel()
                        {
                            LeadId = _leadDetails.LeadID,
                            IsBreakIn = true,
                            PolicyNumber = proposalResponse.PolicyNumber,
                            BreakInId = proposalResponse.quoteResponseModel.BreakinId,
                            BreakinInspectionURL = proposalResponse.quoteResponseModel.BreakinInspectionURL,
                            BreakInInspectionAgency = string.Empty
                        };
                        var res = _quoteRepository.InsertBreakInDetails(breakInModel, cancellationToken);
                    }
                    proposalResponse.quoteResponseModel.IsBreakIn = false;
                    proposalResponse.quoteResponseModel.IsSelfInspection = false;
                    quoteResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse.quoteResponseModel);
                    quoteResponseModel.ProposalNumber = !string.IsNullOrEmpty(_quotedetails.QuoteTransactionRequest?.PolicyId) ? _quotedetails.QuoteTransactionRequest?.PolicyId : proposalResponse.PolicyNumber;

                    var paymentURL = _relianceRepository.PaymentURLGeneration(proposalResponse.quoteResponseModel.ApplicationId, Convert.ToString(proposalResponse.quoteResponseModel.GrossPremium), proposalResponse.QuoteTransactionId, relianceCkycDetails.PAN, proposalResponse.quoteResponseModel?.ApplicationId, relianceCkycDetails.UNIQUEID, _relianceProposal.PolicyDetails.ProductCode);

                    quoteResponseModel.PaymentURLLink = paymentURL;
                    quoteResponseModel.ApplicationId = proposalResponse.quoteResponseModel.ApplicationId;
                    quoteResponseModel.InsurerId = _relianceConfig.InsurerId;
                    quoteResponseModel.CKYCStatus = "Success";
                    quoteResponseModel.Type = "INSERT";
                    quoteResponseModel.CKYCStatus = "KYC_SUCCESS";
                    var paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(quoteResponseModel, cancellationToken).ConfigureAwait(false);
                    if (paymentTransactionId != null)
                    {
                        return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
                    }
                    return HeroResult<QuoteResponseModel>.Fail("Insert Payment Transaction Failed");
                }
                else
                {
                    return HeroResult<QuoteResponseModel>.Fail(proposalResponse.quoteResponseModel.ValidationMessage);
                }

            }
            return HeroResult<QuoteResponseModel>.Fail("Fail to fetch data from database");
        }
    }
}
