using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Command.QuoteConfirm;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Request;
using Insurance.Domain.HDFC;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.TATA.Command.QuoteConfirm
{
    public class TATAQuoteConfirmCommand : QuoteConfirmRequestModel, IRequest<HeroResult<QuoteConfirmDetailsResponseModel>>
    {
    }
    public class TATAQuoteConfirmCommandHandler : IRequestHandler<TATAQuoteConfirmCommand, HeroResult<QuoteConfirmDetailsResponseModel>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly ITATARepository _tataRepository;
        private readonly IApplicationClaims _applicationClaims;
        private readonly LogoConfig _logoConfig;
        private readonly PolicyTypeConfig _policyTypeConfig;
        private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";

        public TATAQuoteConfirmCommandHandler(IQuoteRepository quoteRepository, IMapper mapper, ITATARepository tataRepository, IApplicationClaims applicationClaims, IOptions<LogoConfig> options, IOptions<PolicyTypeConfig> policyTypeConfig)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
            _tataRepository = tataRepository;
            _applicationClaims = applicationClaims;
            _logoConfig = options.Value;
            _policyTypeConfig = policyTypeConfig.Value;
        }
        public async Task<HeroResult<QuoteConfirmDetailsResponseModel>> Handle(TATAQuoteConfirmCommand request, CancellationToken cancellationToken)
        {
            var res = new QuoteConfirmDetailsResponseModel();
            if (request != null && !request.IsBrandNewVehicle && 
                ((request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SATP) && request.InsurerId.Equals(request.PreviousPolicy.TPInsurer)) ||
                ((request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive) || request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD)) && request.InsurerId.Equals(request.PreviousPolicy.SAODInsurer))))
            {
                res.InsurerStatusCode = (int)HttpStatusCode.OK;
                res.ValidationMessage = IdentitalInsurerMessage;
                res.CTA = "Retry";
                res.isNavigateToQuote = true;
                return HeroResult<QuoteConfirmDetailsResponseModel>.Success(res);
            }
            else
            {
                var quoteConfirmCommand = _mapper.Map<QuoteConfirmRequestModel>(request);
                quoteConfirmCommand.PolicyDates.ManufacturingDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.ManufacturingDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ManufacturingDate).ToString("yyyy-MM-dd");
                quoteConfirmCommand.PolicyDates.RegistrationDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.RegistrationDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.RegistrationDate).ToString("yyyy-MM-dd");
                quoteConfirmCommand.PolicyDates.PolicyStartDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.PolicyStartDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("yyyy-MM-dd");
                quoteConfirmCommand.PolicyDates.PolicyEndDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.PolicyEndDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyEndDate).ToString("yyyy-MM-dd");
                quoteConfirmCommand.PolicyDates.ODPolicyStartDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.ODPolicyStartDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd");
                quoteConfirmCommand.PolicyDates.ODPolicyEndDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.ODPolicyEndDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd");
                quoteConfirmCommand.PolicyDates.TPPolicyStartDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.TPPolicyStartDate) ? Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd")
                    : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd");
                quoteConfirmCommand.PolicyDates.TPPolicyEndDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.TPPolicyEndDate) ? Convert.ToDateTime(quoteConfirmCommand.PolicyDates.ODPolicyEndDate).ToString("yyyy-MM-dd")
                    : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.TPPolicyEndDate).ToString("yyyy-MM-dd");
                quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP = string.IsNullOrEmpty(quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP) ? quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumber
                    : quoteConfirmCommand.PreviousPolicy.PreviousPolicyNumberSATP;
                quoteConfirmCommand.PreviousPolicy.TPInsurer = string.IsNullOrEmpty(quoteConfirmCommand.PreviousPolicy.TPInsurer) ? quoteConfirmCommand.PreviousPolicy.SAODInsurer
                    : quoteConfirmCommand.PreviousPolicy.TPInsurer;
                quoteConfirmCommand.PreviousPolicy.TPPolicyExpiryDate = string.IsNullOrEmpty(quoteConfirmCommand.PreviousPolicy.TPPolicyExpiryDate) ? quoteConfirmCommand.PreviousPolicy.SAODPolicyExpiryDate
                    : quoteConfirmCommand.PreviousPolicy.TPPolicyExpiryDate;

                QuoteConfirmDataModel quoteconfirmModel = new();

                var quoteResponseDB = await _quoteRepository.GetQuoteConfirmDetailsDB(quoteConfirmCommand.QuoteTransactionId, quoteConfirmCommand, cancellationToken);
                if (quoteResponseDB != null)
                {
                    var result = await _tataRepository.QuoteConfirmDetails(quoteResponseDB, quoteConfirmCommand, cancellationToken);

                    quoteconfirmModel = _mapper.Map<QuoteConfirmDataModel>(quoteConfirmCommand);
                    quoteconfirmModel.ConfirmCommand = quoteConfirmCommand;
                    quoteconfirmModel.UserId = _applicationClaims.GetUserId();
                    quoteconfirmModel.Stage = "QuoteConfirm";
                    quoteconfirmModel.LeadId = result.LeadId;
                    quoteconfirmModel.RequestBody = result.RequestBody;
                    quoteconfirmModel.ResponseBody = result.ResponseBody;
                    quoteconfirmModel.CommonResponse = JsonConvert.SerializeObject(result.quoteConfirmResponse);
                    quoteconfirmModel.IsBreakin = result.quoteConfirmResponse.IsBreakin;
                    quoteconfirmModel.IsSelfInspection = result.quoteConfirmResponse.IsSelfInspection;
                    quoteconfirmModel.TransactionId = result.quoteResponse.TransactionID;
                    quoteconfirmModel.PolicyId = result.quoteResponse.PolicyId;
                    quoteconfirmModel.ProposalId = result.quoteResponse.ProposalId;
                    quoteconfirmModel.IsPolicyExpired = request?.PreviousPolicy != null ? request.PreviousPolicy.IsPolicyExpired : true;
                    quoteconfirmModel.ConfirmCommand = quoteConfirmCommand;
                    string oldResponse = quoteResponseDB?.QuoteTransactionRequest.CommonResponse;
                    if (oldResponse != null)
                    {
                        QuoteResponseModel getOldResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(oldResponse);
                        result.quoteConfirmResponse.NewPremium = Convert.ToString(Math.Round(Convert.ToDecimal(result.quoteConfirmResponse?.NewPremium)));
                        result.quoteConfirmResponse.OldPremium = Convert.ToString(Math.Round(Convert.ToDecimal(getOldResponse.GrossPremium)));

                        quoteconfirmModel.MinIDV = getOldResponse.MinIDV;
                        quoteconfirmModel.MaxIDV = getOldResponse.MaxIDV;
                        quoteconfirmModel.RecommendedIDV = getOldResponse.IDV;
                    }


                    var insertDataResponse = await _quoteRepository.QuoteConfirmTransaction(quoteconfirmModel, cancellationToken);

                    result.quoteConfirmResponse.Logo = _logoConfig.InsurerLogoURL + insertDataResponse.Logo;
                    result.quoteConfirmResponse.TransactionId = insertDataResponse.QuoteTransactionId;
                    result.quoteConfirmResponse.IsSkipKYC = true;//to skip the kyc after quoteconfirm and to land on proposal dynamic fields
                    if (result.quoteConfirmResponse?.InsurerStatusCode == 200)
                    {
                        return HeroResult<QuoteConfirmDetailsResponseModel>.Success(result.quoteConfirmResponse);
                    }
                    else
                    {
                        return HeroResult<QuoteConfirmDetailsResponseModel>.Fail(result.quoteConfirmResponse.ValidationMessage);
                    }
                }
                return HeroResult<QuoteConfirmDetailsResponseModel>.Fail("Fail to fetch data from DB");
            }
        }
    }
}
