using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.TATA;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;

namespace Insurance.Core.Features.TATA.Command.CreateProposal
{
    public class TATACreateProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
    {

    }
    public class CreateProposalCommandHandler : IRequestHandler<TATACreateProposalCommand, HeroResult<QuoteResponseModel>>
    {
        private readonly ITATARepository _TATARepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly TATAConfig _TATAConfig;
        private readonly IApplicationClaims _applicationClaims;
        public CreateProposalCommandHandler(IMapper mapper, ITATARepository TATARepository, IQuoteRepository quoteRepository, IOptions<TATAConfig> options, IApplicationClaims applicationClaims)
        {
            _TATARepository = TATARepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository;
            _TATAConfig = options.Value;
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        }
        public async Task<HeroResult<QuoteResponseModel>> Handle(TATACreateProposalCommand request, CancellationToken cancellationToken)
        {
            var proposalRequest = _mapper.Map<ProposalRequestModel>(request);
            SaveQuoteTransactionModel proposalResponse = new SaveQuoteTransactionModel();
            QuoteResponseModel quoteResponseModel = new QuoteResponseModel();
            TATAPaymentResponseDataDto paymentResponse = new TATAPaymentResponseDataDto();

            var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(proposalRequest.QuoteTransactionID, cancellationToken);
            if (_quotedetails != null)
            {
                QuoteTransactionRequest _QuoteConfirmResponse = _quotedetails.QuoteTransactionRequest;
                CreateLeadModel _leadDetails = (_quotedetails.LeadDetail);
                QuoteConfirmDetailsModel _quoteConfirmDetails = _quotedetails.QuoteConfirmDetailsModel;
                TATAProposalRequest _TATAProposalRequest = JsonConvert.DeserializeObject<TATAProposalRequest>(_quotedetails.ProposalRequestBody);

                if (_QuoteConfirmResponse != null && _leadDetails != null && _TATAProposalRequest != null)
                {
                    //Calling Quote API to Append Pinocode
                    var quoteRes = await _TATARepository.GetQuoteToAppendPincode(proposalRequest.VehicleTypeId, _QuoteConfirmResponse.RequestBody, _TATAProposalRequest.AddressDetails.pincode, _leadDetails.LeadID, cancellationToken);
                    if(quoteRes != null && quoteRes.InsurerStatusCode.Equals(200))
                    {
                        //Need to manipulate leaddetails as per new quote response
                        _leadDetails.TotalPremium = quoteRes.GrossPremium;
                        _leadDetails.Tax = JsonConvert.SerializeObject(quoteRes.ServiceTax);
                        _QuoteConfirmResponse.TransactionId = quoteRes.TransactionId;
                        _QuoteConfirmResponse.ProposalId = quoteRes.ProposalId;
                    }
                    else 
                    {
                        return HeroResult<QuoteResponseModel>.Fail(quoteRes.ValidationMessage);
                    }

                    proposalResponse = await _TATARepository.CreateProposal(_QuoteConfirmResponse, _quoteConfirmDetails, _TATAProposalRequest, _leadDetails, cancellationToken);
                   
                    proposalResponse.QuoteTransactionId = proposalRequest.QuoteTransactionID;
                    proposalResponse.IsSharePaymentLink = request.IsSharePaymentLink;
                    proposalResponse.InsurerId = _TATAConfig.InsurerId;
                    proposalResponse.TotalPremium = quoteRes.TotalPremium;
                    proposalResponse.GrossPremiume = quoteRes.GrossPremium;
                    proposalResponse.Tax = JsonConvert.SerializeObject(quoteRes.ServiceTax);

                    await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken);

                    if (proposalResponse?.quoteResponseModel?.InsurerStatusCode == 200)
                    {
                        if (proposalResponse.quoteResponseModel.IsBreakIn)
                        {
                            if (string.IsNullOrEmpty(proposalResponse?.quoteResponseModel?.BreakinId))
                            {
                                return HeroResult<QuoteResponseModel>.Fail("BreakIn Id not created");
                            }

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

                            var ckcyResponse = await GetCKYCDetails(proposalResponse.TransactionId, _leadDetails.LeadID, _TATAProposalRequest.PersonalDetails.panNumber,
                                    proposalResponse.quoteResponseModel.ProposalNumber, cancellationToken);

                            proposalResponse.quoteResponseModel.CKYCStatus = ckcyResponse?.tATACKYCStatusResponseModel?.SaveCKYCResponse?.KYC_Status;
                            proposalResponse.quoteResponseModel.CKYCFailReason = ckcyResponse?.tATACKYCStatusResponseModel?.SaveCKYCResponse?.Message;

                            proposalResponse.quoteResponseModel.BreakinStatus = "Initiated";
                            proposalResponse.quoteResponseModel.IsSelfInspection = true;
                            proposalResponse.quoteResponseModel.ValidationMessage = "Vehicle Inspection is Initiated," + _TATAConfig.InsurerName + " team will reach out for conducting inspection. Inspection ID: " + proposalResponse.quoteResponseModel?.BreakinId + " Please save it for future reference.";

                            if (res != null)
                            {
                                return HeroResult<QuoteResponseModel>.Success(proposalResponse.quoteResponseModel);
                            }
                            return HeroResult<QuoteResponseModel>.Fail("BreakIn Details failed to Insert in DB");
                        }
                        else
                        {
                            proposalResponse.quoteResponseModel.IsBreakIn = false;
                            proposalResponse.quoteResponseModel.IsSelfInspection = false;

                            var proposalResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse.quoteResponseModel);
                            proposalResponseModel.InsurerId = _TATAConfig.InsurerId;
                            proposalResponseModel.Type = "INSERT";

                            var ckcyResponse = await GetCKYCDetails(proposalResponseModel.TransactionID, _leadDetails.LeadID, _TATAProposalRequest.PersonalDetails.panNumber,
                                    proposalResponse.quoteResponseModel.ProposalNumber, cancellationToken);

                            proposalResponseModel.CKYCStatus = ckcyResponse?.tATACKYCStatusResponseModel?.SaveCKYCResponse?.KYC_Status;

                            if (ckcyResponse != null && ckcyResponse?.tATACKYCStatusResponseModel != null  && 
                                ckcyResponse.tATACKYCStatusResponseModel.SaveCKYCResponse.InsurerStatusCode.Equals(200) && 
                                ckcyResponse.tATACKYCStatusResponseModel.SaveCKYCResponse.KYC_Status.Equals("KYC_SUCCESS"))
                            {
                                TATAPaymentRequestModel paymentRequestModel = new TATAPaymentRequestModel()
                                {
                                    QuoteTransactionId = proposalResponseModel.TransactionID,
                                    PAN = _TATAProposalRequest.PersonalDetails.panNumber,
                                    MobileNo = _TATAProposalRequest.PersonalDetails.mobile,
                                    Email = _TATAProposalRequest.PersonalDetails.emailId,
                                    Name = _leadDetails.CarOwnedBy.Equals("COMPANY") ? _TATAProposalRequest.PersonalDetails.companyName : _TATAProposalRequest.PersonalDetails.firstName+ " "+_TATAProposalRequest.PersonalDetails.lastName,
                                    TATAPaymentId = proposalResponseModel.ApplicationId,
                                    LeadId = _leadDetails.LeadID,
                                    VehicleTypeId = proposalRequest.VehicleTypeId
                                };
                                paymentResponse = await _TATARepository.GetPaymentLink(paymentRequestModel, cancellationToken);
                                proposalResponseModel.PaymentURLLink = paymentResponse.data?.paymentLink_web;

                            }
                            proposalResponseModel.CKYCStatus = ckcyResponse?.tATACKYCStatusResponseModel?.SaveCKYCResponse.KYC_Status;
                            proposalResponseModel.CKYCFailReason = ckcyResponse?.tATACKYCStatusResponseModel?.SaveCKYCResponse?.Message;

                            var paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken);

                            if (paymentResponse != null && paymentResponse.status.Equals(200) && !string.IsNullOrEmpty(paymentResponse.data?.paymentLink_web))
                            {
                                if (paymentTransactionId != null)
                                {
                                    return HeroResult<QuoteResponseModel>.Success(proposalResponseModel);
                                }
                                return HeroResult<QuoteResponseModel>.Fail("Insert Payment Transaction Failed");
                            }
                            return HeroResult<QuoteResponseModel>.Fail("PaymentLink Creation Fail");
                        }
                    }
                    return HeroResult<QuoteResponseModel>.Fail(proposalResponse.quoteResponseModel.ValidationMessage);
                }
                return HeroResult<QuoteResponseModel>.Fail("Fail to get details to proceed for proposal");
            }
            return HeroResult<QuoteResponseModel>.Fail("Fail to fetch data from database");
        }
        public async Task<TATAGetCKYCResponseModel> GetCKYCDetails(string quoteTransactionId, string LeadId, string panNumber, string proposalNumber,CancellationToken cancellationToken)
        {
            TATACKYCRequestModel tATCKYCRequestModel = new TATACKYCRequestModel()
            {
                LeadId = LeadId,
                IDType = "PAN",
                IdNumber = panNumber,
                ProposalNumber = proposalNumber,
                ReqId = string.Empty
            };

            var kycResponse = await _TATARepository.PanCKYCVerification(tATCKYCRequestModel, cancellationToken);

            KYCDetailsModel kycDetailsModel = new KYCDetailsModel()
            {
                LeadId = LeadId,
                InsurerId = _TATAConfig.InsurerId,
                QuoteTransactionId = quoteTransactionId,
                Stage = "POI",
                RequestBody = kycResponse?.RequestBody,
                ResponseBody = kycResponse?.ResponseBody,
                PhotoId = kycResponse?.SaveCKYCResponse?.PhotoId,
                KYCId = kycResponse?.SaveCKYCResponse?.KYCId,
                CKYCNumber = kycResponse?.SaveCKYCResponse?.CKYCNumber,
                CKYCStatus = kycResponse?.SaveCKYCResponse?.KYC_Status
            };

            var kycInsertDetails = await _quoteRepository.InsertKYCDetailsAfterProposal(kycDetailsModel, cancellationToken);
            TATAGetCKYCResponseModel tATAGetCKYCResponseModel = new TATAGetCKYCResponseModel()
            {
                cKYCStatusModel = kycInsertDetails,
                tATACKYCStatusResponseModel = kycResponse,
            };
            return tATAGetCKYCResponseModel;
        }
    }
}
