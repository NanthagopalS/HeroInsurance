using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Core.Features.HDFC.Command.CreateProposal
{
    public class HDFCCreateProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
    {

    }
    public class CreateProposalCommandHandler : IRequestHandler<HDFCCreateProposalCommand, HeroResult<QuoteResponseModel>>
    {
        private readonly IHDFCRepository _hdfcRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly HDFCConfig _hdfcConfig;
        private readonly IApplicationClaims _applicationClaims;
        private readonly VehicleTypeConfig _vehicleTypeConfig;
        public CreateProposalCommandHandler(IMapper mapper, IHDFCRepository hdfcRepository, IQuoteRepository quoteRepository, IOptions<HDFCConfig> options, IApplicationClaims applicationClaims, IOptions<VehicleTypeConfig> option)
        {
            _hdfcRepository = hdfcRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository;
            _hdfcConfig = options.Value;
            _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
            _vehicleTypeConfig = option.Value;
        }
        public async Task<HeroResult<QuoteResponseModel>> Handle(HDFCCreateProposalCommand request, CancellationToken cancellationToken)
        {
            var proposalRequest = _mapper.Map<ProposalRequestModel>(request);
            SaveQuoteTransactionModel proposalResponse = new SaveQuoteTransactionModel();
            QuoteResponseModel quoteResponseModel = new QuoteResponseModel();

            var _quotedetails = await _quoteRepository.GetQuoteTransactionDetails(proposalRequest.QuoteTransactionID, cancellationToken);
            if(_quotedetails != null)
            {
                Random random = new Random();
                int num = random.Next(0, 100000);
                string paymentTransactionNumber = "1063" + DateTime.Now.ToString("yyMMdd") + num;

                HDFCServiceRequestModel _hdfcProposal = JsonConvert.DeserializeObject<HDFCServiceRequestModel>(_quotedetails.QuoteTransactionRequest?.RequestBody);
                CreateLeadModel _leadDetails = (_quotedetails.LeadDetail);
                HDFCProposalRequest _hdfcProposalRequest = JsonConvert.DeserializeObject<HDFCProposalRequest>(_quotedetails.ProposalRequestBody);
                
                if(_hdfcProposal != null && _leadDetails != null && _hdfcProposalRequest != null)
                {
                    if(proposalRequest.VehicleTypeId.Equals(_vehicleTypeConfig.Commerical))
                    {
                        _hdfcProposal.Payment_Details = new Payment_Details()
                        {
                            INSTRUMENT_NUMBER = paymentTransactionNumber
                        };
                    }

                    proposalResponse = await _hdfcRepository.CreateProposal(_hdfcProposal, _hdfcProposalRequest, _leadDetails, cancellationToken);
                    proposalResponse.QuoteTransactionId = proposalRequest.QuoteTransactionID;
                    proposalResponse.IsSharePaymentLink = request.IsSharePaymentLink;
                    await _quoteRepository.SaveQuoteTransaction(proposalResponse, cancellationToken);

                    if (proposalResponse != null && proposalResponse.quoteResponseModel.InsurerStatusCode == 200)
                    {
                        
                        proposalResponse.quoteResponseModel.IsBreakIn = false;
                        proposalResponse.quoteResponseModel.IsSelfInspection = false;
                        
                        quoteResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse.quoteResponseModel);
                        quoteResponseModel.InsurerId = _hdfcConfig.InsurerId;
                        quoteResponseModel.CKYCStatus = "KYC_SUCCESS";
                        quoteResponseModel.Type = "INSERT";

                        

                        string checkSumId = await _hdfcRepository.GeneratePaymentCheckSum(paymentTransactionNumber, _leadDetails.GrossPremium, $"{_hdfcConfig.PGStatusRedirectionURL}{quoteResponseModel.TransactionID}/{_applicationClaims.GetUserId()}", _leadDetails.LeadID,cancellationToken);
                        if(checkSumId != null)
                        {
                            string paymentLink = $"{_hdfcConfig.PGSubmitPayment}{quoteResponseModel.TransactionID}/{_applicationClaims.GetUserId()}?Trnsno={paymentTransactionNumber}&Amt={_leadDetails.GrossPremium}&Chksum={checkSumId}";
                            
                           
                            quoteResponseModel.PaymentURLLink = paymentLink;
                            quoteResponseModel.IsHtml= true;
                            quoteResponseModel.ApplicationId = paymentTransactionNumber;

                            var paymentTransactionId = await _quoteRepository.InsertPaymentTransaction(quoteResponseModel, cancellationToken);
                            if (paymentTransactionId != null)
                            {
                                return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
                            }
                            return HeroResult<QuoteResponseModel>.Fail("Insert Payment Transaction Failed");
                        }
                        return HeroResult<QuoteResponseModel>.Fail("Payment CheckSum Creation Failed");
                    }
                    return HeroResult<QuoteResponseModel>.Fail(proposalResponse.quoteResponseModel.ValidationMessage);
                }
                return HeroResult<QuoteResponseModel>.Fail("Fail to get details to proceed for proposal");
            }
            return HeroResult<QuoteResponseModel>.Fail("Fail to fetch data from database");
        }
    }
}
