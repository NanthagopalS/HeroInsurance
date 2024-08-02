using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Bajaj.Command;
using Insurance.Core.Responses;
using Insurance.Domain.Bajaj;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using static Google.Apis.Requests.BatchRequest;
using static MongoDB.Driver.WriteConcern;

namespace Insurance.Core.Features.Chola.Command.CreateProposal
{
    public class CholaCreateProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
    {

    }
    public class CreateProposalCommandHandler : IRequestHandler<CholaCreateProposalCommand, HeroResult<QuoteResponseModel>>
    {
        private readonly ICholaRepository _cholaRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly CholaConfig _cholaConfig;
        private readonly IApplicationClaims _applicationClaims;
        private readonly PolicyTypeConfig _policyTypeConfig;
        public CreateProposalCommandHandler(IMapper mapper, ICholaRepository cholaRepository, IQuoteRepository quoteRepository, IOptions<CholaConfig> options, IApplicationClaims applicationClaims, IOptions<PolicyTypeConfig> policyTypeConfig)
        {
            _cholaRepository = cholaRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository;
            _cholaConfig = options.Value;
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
            _policyTypeConfig = policyTypeConfig.Value;
        }
        public async Task<HeroResult<QuoteResponseModel>> Handle(CholaCreateProposalCommand request, CancellationToken cancellationToken)
        {
            var proposalRequest = _mapper.Map<ProposalRequestModel>(request);
            SaveQuoteTransactionModel proposalResponse = new SaveQuoteTransactionModel();

            var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(proposalRequest.QuoteTransactionID, cancellationToken);
            if (_quotedetails != null)
            {
                QuoteResponseModel quoteResponseModel;
                var cholaCkycDetails = new CholaCKYCRequestModel();
                CholaServiceRequestModel _cholaProposal = JsonConvert.DeserializeObject<CholaServiceRequestModel>(_quotedetails.QuoteTransactionRequest?.RequestBody);
                _cholaProposal.quote_id = _quotedetails.QuoteTransactionRequest?.TransactionId;
                _cholaProposal.proposal_id = _quotedetails.QuoteTransactionRequest?.ProposalId;

                proposalResponse.TransactionId = _quotedetails.QuoteTransactionRequest?.TransactionId;
                proposalResponse.PolicyNumber = _quotedetails.QuoteTransactionRequest?.ProposalId;
                proposalResponse.PolicyId = _quotedetails.QuoteTransactionRequest?.PolicyId;

                CreateLeadModel _leadDetails = (_quotedetails.LeadDetail);
                CholaProposalRequest _cholaProposalRequest = JsonConvert.DeserializeObject<CholaProposalRequest>(_quotedetails.ProposalRequestBody);

                if (!string.IsNullOrWhiteSpace(_quotedetails.CKYCRequestBody))
                {
                    cholaCkycDetails = JsonConvert.DeserializeObject<CholaCKYCRequestModel>(_quotedetails.CKYCRequestBody);
                }

                if (_leadDetails != null && _leadDetails.IsBreakin && !_leadDetails.IsBreakinApproved && !_leadDetails.PolicyTypeId.Equals(_policyTypeConfig.SATP))
                {
                    proposalResponse = await _cholaRepository.CreateBreakIn(_quotedetails, cancellationToken);

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
					proposalResponse.quoteResponseModel.BreakinStatus = "Initiated";
                    if (!string.IsNullOrEmpty(proposalResponse.quoteResponseModel.BreakinId))
                    {
                        proposalResponse.quoteResponseModel.ValidationMessage = $"Vehicle Inspection is Initiated,{_cholaConfig.InsurerName} team will reach out for conducting inspection. Inspection ID: {proposalResponse.quoteResponseModel.BreakinId} Please save it for future reference.";
                    }
                    else
                    {
                        return HeroResult<QuoteResponseModel>.Fail(proposalResponse.quoteResponseModel.ValidationMessage);
                    }
                    quoteResponseModel = proposalResponse.quoteResponseModel;
                    quoteResponseModel.TransactionID = proposalRequest.QuoteTransactionID;
                    quoteResponseModel.BreakinInspectionURL = proposalResponse.BreakinInspectionURL;
                    proposalResponse.QuoteTransactionId = proposalRequest.QuoteTransactionID;
                    proposalResponse.IsSharePaymentLink = request.IsSharePaymentLink;
                    quoteResponseModel.Type = "INSERT";
                    quoteResponseModel.CKYCStatus = "KYC_SUCCESS";
                    quoteResponseModel.ProposalNumber = _quotedetails.QuoteTransactionRequest.PolicyId;
                    var paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(quoteResponseModel, cancellationToken).ConfigureAwait(false);
                    if(paymentTransactionId != null)
                    {
                        var smsResponse = await _cholaRepository.SendBreakinNotification(quoteResponseModel.BreakinId, _cholaProposalRequest.PersonalDetails.mobile, proposalResponse.BreakinInspectionURL, cancellationToken);
                        if(smsResponse != null && smsResponse.Equals("success"))
                        {
                            return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
                        }
                        return HeroResult<QuoteResponseModel>.Fail("Failed to send notification");
                    }
                    return HeroResult<QuoteResponseModel>.Fail("Insert Payment Transaction Failed");
                }
                else
                {
                    proposalResponse = await _cholaRepository.CreateProposal(_cholaProposal, _cholaProposalRequest, _leadDetails, cholaCkycDetails, cancellationToken);
                    proposalResponse.QuoteTransactionId = proposalRequest.QuoteTransactionID;
                    proposalResponse.IsSharePaymentLink = request.IsSharePaymentLink;

                    if (proposalResponse != null && proposalResponse.quoteResponseModel.InsurerStatusCode == 200)
                    {
                        await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken);

                        proposalResponse.quoteResponseModel.IsBreakIn = false;
                        proposalResponse.quoteResponseModel.IsSelfInspection = false;

                        quoteResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse.quoteResponseModel);
                        quoteResponseModel.ProposalNumber = _quotedetails.QuoteTransactionRequest?.PolicyId;
                        var checksumID = _cholaRepository.PaymentURLGeneration(proposalResponse.quoteResponseModel.ApplicationId, Convert.ToString(proposalResponse.quoteResponseModel.GrossPremium), proposalResponse.quoteResponseModel.TransactionID);

                        string paymentURL = $"{_cholaConfig.PaymentGenerationURLLink}msg={_cholaConfig.MerchantId}|{proposalResponse.quoteResponseModel.ApplicationId}|NA|{Convert.ToString(proposalResponse.quoteResponseModel.GrossPremium)}|NA|NA|NA|{_cholaConfig.Currency}|NA|{_cholaConfig.Code1}|{_cholaConfig.SecurityId}|NA|NA|{_cholaConfig.Code2}|NA|{_cholaConfig.AdditionalInformation2}|NA|NA|NA|NA|NA|{_cholaConfig.PGStatusRedirectionURL}{proposalResponse.quoteResponseModel.TransactionID}/{_applicationClaims.GetUserId()}|{checksumID}";

                        quoteResponseModel.PaymentURLLink = paymentURL;
                        quoteResponseModel.ApplicationId = proposalResponse.quoteResponseModel.ApplicationId;

                        quoteResponseModel.InsurerId = _cholaConfig.InsurerId;
                        quoteResponseModel.CKYCStatus = "KYC_SUCCESS";
                        quoteResponseModel.Type = "INSERT";
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
            }
            return HeroResult<QuoteResponseModel>.Fail("Fail to fetch data from database");
        }
    }
}


