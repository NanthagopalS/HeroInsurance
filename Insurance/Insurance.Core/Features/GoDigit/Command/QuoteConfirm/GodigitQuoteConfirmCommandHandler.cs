using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.ICICI.Queries.ConfirmDetails;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.GoDigit.Request;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Core.Features.GoDigit.Command.QuoteConfirm
{
    public class GodigitQuoteConfirmCommand : QuoteConfirmRequestModel, IRequest<HeroResult<QuoteConfirmDetailsResponseModel>>
    {
    }
    public class GodigitQuoteConfirmCommandHandler : IRequestHandler<GodigitQuoteConfirmCommand, HeroResult<QuoteConfirmDetailsResponseModel>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly IGoDigitRepository _goDigitRepository;
        private readonly IApplicationClaims _applicationClaims;
        private readonly LogoConfig _logoConfig;
        public GodigitQuoteConfirmCommandHandler(IQuoteRepository quoteRepository, IMapper mapper, IGoDigitRepository goDigitRepository,IApplicationClaims applicationClaims, IOptions<LogoConfig> options)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
            _goDigitRepository = goDigitRepository;
            _applicationClaims = applicationClaims;
            _logoConfig = options.Value;
        }

        public async Task<HeroResult<QuoteConfirmDetailsResponseModel>> Handle(GodigitQuoteConfirmCommand request, CancellationToken cancellationToken)
        {
            var res = new QuoteConfirmDetailsResponseModel();
            var quoteConfirmCommand = _mapper.Map<QuoteConfirmRequestModel>(request);
            quoteConfirmCommand.ManufacturingMonthYear = string.IsNullOrEmpty(quoteConfirmCommand.ManufacturingMonthYear) ? null : Convert.ToDateTime(quoteConfirmCommand.ManufacturingMonthYear).ToString("yyyy-MM-dd");
            quoteConfirmCommand.RegistrationDate = string.IsNullOrEmpty(quoteConfirmCommand.RegistrationDate) ? null : Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("yyyy-MM-dd");
            quoteConfirmCommand.PolicyDates.PolicyStartDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.PolicyStartDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("yyyy-MM-dd");
            quoteConfirmCommand.PolicyDates.PolicyEndDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.PolicyEndDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyEndDate).ToString("yyyy-MM-dd");

            var quoteconfirmModel = new QuoteConfirmDataModel();

            var quoteResponseDB = await _quoteRepository.GetQuoteConfirmDetailsDB(quoteConfirmCommand.QuoteTransactionId, quoteConfirmCommand, cancellationToken).ConfigureAwait(false);
            if (quoteResponseDB != null)
            {
                var result = await _goDigitRepository.QuoteConfirmDetails(quoteResponseDB, quoteConfirmCommand, cancellationToken);
                var requestBody = JsonConvert.DeserializeObject<GoDigitRequestDto>(result.Item3);

                quoteconfirmModel = _mapper.Map<QuoteConfirmDataModel>(quoteConfirmCommand);
                quoteconfirmModel.ConfirmCommand = quoteConfirmCommand;
                quoteconfirmModel.UserId = _applicationClaims.GetUserId();
                quoteconfirmModel.Stage = "QuoteConfirm";
                quoteconfirmModel.LeadId = result.Item5;
                quoteconfirmModel.RequestBody = result.Item3;
                quoteconfirmModel.ResponseBody = result.Item4;
                quoteconfirmModel.CommonResponse = JsonConvert.SerializeObject(result.Item1);
                quoteconfirmModel.IsBreakin = result.Item1.IsBreakin;
                quoteconfirmModel.IsSelfInspection = result.Item1.IsSelfInspection;
                quoteconfirmModel.TransactionId = result.Item2.ApplicationId;
                quoteconfirmModel.IsPolicyExpired = request.PreviousPolicy != null ? request.PreviousPolicy.IsPolicyExpired : true;
                quoteconfirmModel.ConfirmCommand = quoteConfirmCommand;
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
                
                
                
                var insertDataResponse = await _quoteRepository.QuoteConfirmTransaction(quoteconfirmModel,cancellationToken).ConfigureAwait(false);
                
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
