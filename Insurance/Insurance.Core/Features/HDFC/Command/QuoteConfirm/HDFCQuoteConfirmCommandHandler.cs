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

namespace Insurance.Core.Features.HDFC.Command.QuoteConfirm
{
    public class HDFCQuoteConfirmCommand : QuoteConfirmRequestModel, IRequest<HeroResult<QuoteConfirmDetailsResponseModel>>
    {
    }
    public class HDFCQuoteConfirmCommandHandler : IRequestHandler<HDFCQuoteConfirmCommand, HeroResult<QuoteConfirmDetailsResponseModel>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly IHDFCRepository _hdfcRepository;
        private readonly IApplicationClaims _applicationClaims;
        private readonly LogoConfig _logoConfig;
        private readonly PolicyTypeConfig _policyTypeConfig;
        private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";

        public HDFCQuoteConfirmCommandHandler(IQuoteRepository quoteRepository, IMapper mapper, IHDFCRepository hdfcRepository, IApplicationClaims applicationClaims, IOptions<LogoConfig> options, IOptions<PolicyTypeConfig> policyTypeConfig)
        {
            _quoteRepository= quoteRepository;
            _mapper= mapper;
            _hdfcRepository= hdfcRepository;
            _applicationClaims= applicationClaims;
            _logoConfig = options.Value;
            _policyTypeConfig = policyTypeConfig.Value;
        }
        public async Task<HeroResult<QuoteConfirmDetailsResponseModel>> Handle(HDFCQuoteConfirmCommand request, CancellationToken cancellationToken)
        {
            var res = new QuoteConfirmDetailsResponseModel();
            if (request != null && !request.IsBrandNewVehicle && request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SATP) && request.InsurerId.Equals(request.PreviousPolicy.TPInsurer))
            {
                res.InsurerStatusCode = (int)HttpStatusCode.OK;
                res.ValidationMessage = IdentitalInsurerMessage;
                res.CTA = "Retry";
                res.isNavigateToQuote = true;
                return HeroResult<QuoteConfirmDetailsResponseModel>.Success(res);
            }
            else if (request != null && !request.IsBrandNewVehicle && (request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive) || request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD)) && request.InsurerId.Equals(request.PreviousPolicy.SAODInsurer))
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
                quoteConfirmCommand.ManufacturingMonthYear = string.IsNullOrEmpty(quoteConfirmCommand.ManufacturingMonthYear) ? null : Convert.ToDateTime(quoteConfirmCommand.ManufacturingMonthYear).ToString("yyyy-MM-dd");
                quoteConfirmCommand.RegistrationDate = string.IsNullOrEmpty(quoteConfirmCommand.RegistrationDate) ? null : Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("yyyy-MM-dd");
                quoteConfirmCommand.PolicyDates.PolicyStartDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.PolicyStartDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("yyyy-MM-dd");
                quoteConfirmCommand.PolicyDates.PolicyEndDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.PolicyEndDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyEndDate).ToString("yyyy-MM-dd");

                var quoteconfirmModel = new QuoteConfirmDataModel();

                var quoteResponseDB = await _quoteRepository.GetQuoteConfirmDetailsDB(quoteConfirmCommand.QuoteTransactionId, quoteConfirmCommand, cancellationToken);
                if (quoteResponseDB != null)
                {
                    var result = await _hdfcRepository.QuoteConfirmDetails(quoteResponseDB, quoteConfirmCommand, cancellationToken);
                    var requestBody = JsonConvert.DeserializeObject<HDFCServiceRequestModel>(result.RequestBody);

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
                    quoteconfirmModel.TransactionId = result.TransactionId;
                    quoteconfirmModel.IsPolicyExpired = request.PreviousPolicy != null ? request.PreviousPolicy.IsPolicyExpired : true;
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
                        result.quoteConfirmResponse.IDV = Convert.ToInt32(getOldResponse.IDV);
                        result.quoteConfirmResponse.MinIDV = Convert.ToInt32(getOldResponse.MinIDV);
                        result.quoteConfirmResponse.MaxIDV = Convert.ToInt32(getOldResponse.MaxIDV);
                    }


                    var insertDataResponse = await _quoteRepository.QuoteConfirmTransaction(quoteconfirmModel, cancellationToken);

                    result.quoteConfirmResponse.Logo = _logoConfig.InsurerLogoURL + insertDataResponse.Logo;
                    result.quoteConfirmResponse.TransactionId = insertDataResponse.QuoteTransactionId;
                    if (result != null && result.quoteConfirmResponse?.InsurerStatusCode == 200)
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
