using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.UnitedIndia;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Core.Features.UnitedIndia.Command.CreateProposal;

public class UIICCreateProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
{
}
public class UIICCreateProposalCommandHandler : IRequestHandler<UIICCreateProposalCommand, HeroResult<QuoteResponseModel>>
{
    private readonly IMapper _mapper;
    private readonly IUnitedIndiaRepository _unitedIndiaRepository;
    private readonly IQuoteRepository _quoteRepository;
    private readonly UnitedIndiaConfig _unitedIndiaConfig;
    public UIICCreateProposalCommandHandler(IMapper mapper,
    IUnitedIndiaRepository unitedIndiaRepository,
    IQuoteRepository quoteRepository,
    IOptions<UnitedIndiaConfig> unitedIndiaConfig)
    {
        _mapper = mapper;
        _unitedIndiaRepository = unitedIndiaRepository;
        _quoteRepository = quoteRepository;
        _unitedIndiaConfig = unitedIndiaConfig.Value;
    }
    public async Task<HeroResult<QuoteResponseModel>> Handle(UIICCreateProposalCommand request, CancellationToken cancellationToken)
    {
        var paymentTransactionId = (dynamic)null;
        var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(request.QuoteTransactionID, cancellationToken);
        if (_quotedetails != null)
        {
            QuoteTransactionRequest _QuoteConfirmResponse = _quotedetails.QuoteTransactionRequest;
            CreateLeadModel _leadDetails = (_quotedetails.LeadDetail);
            UnitedProposalDynamicDetail unitedProposalDynamicDetail = JsonConvert.DeserializeObject<UnitedProposalDynamicDetail>(_quotedetails.ProposalRequestBody);
            InitiatePaymentRequestDto initiatePaymentRequestDto = new InitiatePaymentRequestDto();
            CKYCModel cKYCModel = new CKYCModel();
            var proposalResponseModel = new QuoteResponseModel();

            if (_QuoteConfirmResponse != null && _leadDetails != null && unitedProposalDynamicDetail != null)
            {
                //Calling Quote API to Append User Details
                var quoteResponse = await _unitedIndiaRepository.GetQuoteUpdate(_QuoteConfirmResponse, _leadDetails, unitedProposalDynamicDetail, cancellationToken);
                if (quoteResponse.InsurerStatusCode == 200 && quoteResponse.ValidationMessage == null && !string.IsNullOrEmpty(quoteResponse.TransactionId))
                {
                    var proposalResponse = await _unitedIndiaRepository.CreateProposal(quoteResponse.RequestBody, quoteResponse.TransactionId, _leadDetails.LeadID, cancellationToken);

                    proposalResponse.QuoteTransactionId = request.QuoteTransactionID;
                    proposalResponse.IsSharePaymentLink = request.IsSharePaymentLink;
                    proposalResponse.InsurerId = _unitedIndiaConfig.InsurerId;

                    await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken);

                    if (proposalResponse?.quoteResponseModel?.InsurerStatusCode == 200)
                    {
                        // Once Breakin API Received the we have to update
                        proposalResponse.quoteResponseModel.IsBreakIn = false;
                        proposalResponse.quoteResponseModel.IsSelfInspection = false;

                        proposalResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse.quoteResponseModel);
                        proposalResponseModel.InsurerId = _unitedIndiaConfig.InsurerId;
                        proposalResponseModel.Type = "INSERT";
                        proposalResponseModel.PaymentCorrelationId = quoteResponse.OEMUniqId;
                        proposalResponseModel.CustomerId = proposalResponse?.quoteResponseModel?.CustomerId;
                        cKYCModel.CustomerType = _leadDetails.CarOwnedBy.Equals("INDIVIDUAL") ? "I" : "C";
                        cKYCModel.TransactionId = quoteResponse.OEMUniqId;
                        cKYCModel.QuotetransactionId = proposalResponse.quoteResponseModel.TransactionID;
                        cKYCModel.VariantId = _leadDetails.VariantId;
                        var ckycResponse = await _unitedIndiaRepository.SaveCKYC(cKYCModel, unitedProposalDynamicDetail, cancellationToken);

                        var dataSaveResponse = await _quoteRepository.SaveLeadDetails(_unitedIndiaConfig.InsurerId, proposalResponse.quoteResponseModel.TransactionID, ckycResponse.Item1, ckycResponse.Item2, "POI", ckycResponse.Item4, cancellationToken);

                        if (ckycResponse != null && ckycResponse.Item3.KYC_Status.Equals("KYC_SUCCESS"))
                        {
                            proposalResponseModel.CKYCStatus = ckycResponse.Item3.KYC_Status;
                            proposalResponseModel.IsPOARedirectionURL = false;

                            initiatePaymentRequestDto.QuoteTransactionId = proposalResponse.quoteResponseModel.TransactionID;
                            initiatePaymentRequestDto.LeadId = _leadDetails.LeadID;
                            initiatePaymentRequestDto.orderId = proposalResponse.TransactionId;
                            initiatePaymentRequestDto.txnAmount = proposalResponseModel.GrossPremium;
                            initiatePaymentRequestDto.userInfo = new Userinfo()
                            {
                                custId = proposalResponse?.quoteResponseModel?.CustomerId,
                                mobile = _leadDetails.PhoneNumber
                            };
                            initiatePaymentRequestDto.num_reference_number = proposalResponse.quoteResponseModel.ProposalNumber;
                            var paymentURLResponse = await _unitedIndiaRepository.InitiatePayment(initiatePaymentRequestDto, cancellationToken);
                            if (paymentURLResponse != null)
                            {
                                proposalResponseModel.PaymentURLLink = paymentURLResponse;
                                paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken);
                                if (paymentTransactionId != null)
                                {
                                    return HeroResult<QuoteResponseModel>.Success(proposalResponseModel);
                                }
                                return HeroResult<QuoteResponseModel>.Fail("Insert Payment Transaction Failed");
                            }
                            paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken);
                            return HeroResult<QuoteResponseModel>.Fail("Create Payment Link Failed");
                        }
                        else if (ckycResponse != null && ckycResponse.Item3.KYC_Status.Equals("POA_REQUIRED"))
                        {
                            proposalResponseModel.CKYCStatus = ckycResponse.Item3.KYC_Status;
                            proposalResponseModel.IsPOARedirectionURL = true;

                            proposalResponseModel.CKYCLink = ckycResponse.Item3.redirect_link;
                            proposalResponseModel.IsDocumentUpload = true;
                            paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken);
                            if (paymentTransactionId != null)
                            {
                                return HeroResult<QuoteResponseModel>.Success(proposalResponseModel);
                            }
                            return HeroResult<QuoteResponseModel>.Fail("Insert Payment Transaction Failed");
                        }
                        paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken);
                        return HeroResult<QuoteResponseModel>.Fail("CKYC API Failed");
                    }
                    return HeroResult<QuoteResponseModel>.Fail(proposalResponse?.quoteResponseModel?.ValidationMessage);
                }
                return HeroResult<QuoteResponseModel>.Fail(quoteResponse.ValidationMessage);
            }
            return HeroResult<QuoteResponseModel>.Fail("Fail to get details to proceed for proposal");
        }
        return HeroResult<QuoteResponseModel>.Fail("Fail to fetch data from database");
    }
}
