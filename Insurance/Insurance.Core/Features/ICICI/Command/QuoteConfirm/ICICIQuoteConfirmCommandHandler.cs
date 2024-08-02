using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI.Response;
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

namespace Insurance.Core.Features.ICICI.Command.QuoteConfirm
{
    public class ICICIQuoteConfirmCommand : QuoteConfirmRequestModel, IRequest<HeroResult<QuoteConfirmDetailsResponseModel>>
    {

    }
    public class ICICIQuoteConfirmCommandHandler : IRequestHandler<ICICIQuoteConfirmCommand, HeroResult<QuoteConfirmDetailsResponseModel>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly IICICIRepository _iciciRepository;
        private readonly IApplicationClaims _applicationClaims;
        private readonly LogoConfig _logoConfig;
        private readonly PolicyTypeConfig _policyTypeConfig;
        private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
        public ICICIQuoteConfirmCommandHandler(IQuoteRepository quoteRepository, IMapper mapper, IICICIRepository iciciRepository, IApplicationClaims applicationClaims, IOptions<LogoConfig> options, IOptions<PolicyTypeConfig> policyTypeConfig)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
            _iciciRepository = iciciRepository;
            _applicationClaims = applicationClaims;
            _logoConfig = options.Value;
            _policyTypeConfig = policyTypeConfig.Value;
        }

        public async Task<HeroResult<QuoteConfirmDetailsResponseModel>> Handle(ICICIQuoteConfirmCommand request, CancellationToken cancellationToken)
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

                var quoteResponseDB = await _quoteRepository.GetQuoteConfirmDetailsDB(quoteConfirmCommand.QuoteTransactionId, quoteConfirmCommand, cancellationToken).ConfigureAwait(false);

                if (quoteResponseDB != null)
                {
                    var result = await _iciciRepository.QuoteConfirmDetails(quoteResponseDB, quoteConfirmCommand, cancellationToken);

                    var proposalResponse = JsonConvert.DeserializeObject<ICICIResponseDto>(result.Item4);

                    quoteconfirmModel = _mapper.Map<QuoteConfirmDataModel>(quoteConfirmCommand);
                    quoteconfirmModel.ConfirmCommand = quoteConfirmCommand;//_mapper.Map<QuoteConfirmDataModel>(quoteConfirmCommand);
                    quoteconfirmModel.UserId = _applicationClaims.GetUserId();
                    quoteconfirmModel.Stage = "QuoteConfirm";
                    quoteconfirmModel.LeadId = result.Item5;
                    quoteconfirmModel.RequestBody = result.Item3;
                    quoteconfirmModel.ResponseBody = result.Item4;
                    quoteconfirmModel.CommonResponse = JsonConvert.SerializeObject(result.Item1);
                    quoteconfirmModel.IsBreakin = result.Item1.IsBreakin;
                    quoteconfirmModel.IsSelfInspection = result.Item1.IsSelfInspection;
                    quoteconfirmModel.IsApprovalRequired = proposalResponse.isApprovalRequired;
                    quoteconfirmModel.IsQuoteDeviation = proposalResponse.isQuoteDeviation;
                    quoteconfirmModel.TransactionId = proposalResponse.correlationId;

                    string oldResponse = quoteResponseDB?.QuoteTransactionRequest.CommonResponse;
                    if (oldResponse != null)
                    {
                        QuoteResponseModel getOldResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(oldResponse);
                        result.Item1.NewPremium = Math.Round(Convert.ToDecimal(result.Item1?.NewPremium)).ToString();
                        result.Item1.OldPremium = Math.Round(Convert.ToDecimal(getOldResponse.GrossPremium)).ToString();

                        quoteconfirmModel.MinIDV = getOldResponse.MinIDV;
                        quoteconfirmModel.MaxIDV = getOldResponse.MaxIDV;
                        quoteconfirmModel.RecommendedIDV = getOldResponse.IDV;
                        result.Item1.IDV = Convert.ToInt32(getOldResponse.IDV);
                        result.Item1.MinIDV = Convert.ToInt32(getOldResponse.MinIDV);
                        result.Item1.MaxIDV = Convert.ToInt32(getOldResponse.MaxIDV);
                    }



                    var insertDataResponse = await _quoteRepository.QuoteConfirmTransaction(quoteconfirmModel, cancellationToken).ConfigureAwait(false);

                    result.Item1.Logo = _logoConfig.InsurerLogoURL + insertDataResponse.Logo;
                    result.Item1.TransactionId = insertDataResponse.QuoteTransactionId;
                    if (result != null && result.Item1?.InsurerStatusCode == 200)
                    {
                        return HeroResult<QuoteConfirmDetailsResponseModel>.Success(result.Item1);
                    }
                    else
                    {
                        return HeroResult<QuoteConfirmDetailsResponseModel>.Fail(result.Item1.ValidationMessage);
                    }
                }
                return HeroResult<QuoteConfirmDetailsResponseModel>.Fail("Fail to fetch data from DB");
            }
        }
    }
}
