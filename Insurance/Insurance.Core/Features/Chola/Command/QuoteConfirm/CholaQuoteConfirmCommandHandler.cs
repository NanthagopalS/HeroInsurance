using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.Chola.Command.QuoteConfirm
{
    public class CholaQuoteConfirmCommand : QuoteConfirmRequestModel, IRequest<HeroResult<QuoteConfirmDetailsResponseModel>>
    {
    }
    public class CholaQuoteConfirmCommandHandler : IRequestHandler<CholaQuoteConfirmCommand, HeroResult<QuoteConfirmDetailsResponseModel>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly ICholaRepository _cholaRepository;
        private readonly IApplicationClaims _applicationClaims;
        private readonly LogoConfig _logoConfig;
        private readonly PolicyTypeConfig _policyTypeConfig;
        private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";

        public CholaQuoteConfirmCommandHandler(IQuoteRepository quoteRepository, IMapper mapper, ICholaRepository cholaRepository, IApplicationClaims applicationClaims, IOptions<LogoConfig> options, IOptions<PolicyTypeConfig> policyTypeConfig)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
            _cholaRepository = cholaRepository;
            _applicationClaims = applicationClaims;
            _logoConfig = options.Value;
            _policyTypeConfig = policyTypeConfig.Value;
        }
        public async Task<HeroResult<QuoteConfirmDetailsResponseModel>> Handle(CholaQuoteConfirmCommand request, CancellationToken cancellationToken)
        {
            if (DoesTPInsurerIsSame(request))
            {
                return HeroResult<QuoteConfirmDetailsResponseModel>.Success(ReturnQuoteResponse());
            }

            if (DoesSAODInsurerIsSame(request))
            {
                return HeroResult<QuoteConfirmDetailsResponseModel>.Success(ReturnQuoteResponse());
            }

            var quoteConfirmCommand = _mapper.Map<QuoteConfirmRequestModel>(request);

            quoteConfirmCommand.ManufacturingMonthYear = string.IsNullOrEmpty(quoteConfirmCommand.ManufacturingMonthYear) ? null : Convert.ToDateTime(quoteConfirmCommand.ManufacturingMonthYear).ToString("yyyy-MM-dd");

            quoteConfirmCommand.PolicyDates.PolicyStartDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.PolicyStartDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("yyyy-MM-dd");

            quoteConfirmCommand.PolicyDates.PolicyEndDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.PolicyEndDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyEndDate).ToString("yyyy-MM-dd");

            var quoteResponseDB = await _quoteRepository.GetQuoteConfirmDetailsDB(quoteConfirmCommand.QuoteTransactionId, quoteConfirmCommand, cancellationToken).ConfigureAwait(false);

            if (quoteResponseDB != null)
            {
                var result = await _cholaRepository.QuoteConfirmDetails(quoteResponseDB, quoteConfirmCommand, cancellationToken);
                var quoteconfirmModel = _mapper.Map<QuoteConfirmDataModel>(quoteConfirmCommand);
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
                quoteconfirmModel.ProposalId = result.quoteResponse.ProposalId;
                quoteconfirmModel.PolicyId = result.quoteResponse.PolicyId;
                quoteconfirmModel.IsPolicyExpired = request.PreviousPolicy.IsPolicyExpired;
                quoteconfirmModel.ConfirmCommand = quoteConfirmCommand;
                string oldResponse = quoteResponseDB?.QuoteTransactionRequest.CommonResponse;
                if (oldResponse != null)
                {
                    QuoteResponseModel getOldResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(oldResponse);
                    result.quoteConfirmResponse.NewPremium = Math.Round(Convert.ToDecimal(result.quoteConfirmResponse?.NewPremium)).ToString();
                    result.quoteConfirmResponse.OldPremium = Math.Round(Convert.ToDecimal(getOldResponse.GrossPremium)).ToString();

                    quoteconfirmModel.MinIDV = getOldResponse.MinIDV;
                    quoteconfirmModel.MaxIDV = getOldResponse.MaxIDV;
                    quoteconfirmModel.RecommendedIDV = getOldResponse.IDV;
                    result.quoteConfirmResponse.IDV = Convert.ToInt32(getOldResponse.IDV);
                    result.quoteConfirmResponse.MinIDV = Convert.ToInt32(getOldResponse.MinIDV);
                    result.quoteConfirmResponse.MaxIDV = Convert.ToInt32(getOldResponse.MaxIDV);
                }

                var insertDataResponse = await _quoteRepository.QuoteConfirmTransaction(quoteconfirmModel, cancellationToken).ConfigureAwait(false);

                result.quoteConfirmResponse.Logo = _logoConfig.InsurerLogoURL + insertDataResponse.Logo;
                result.quoteConfirmResponse.TransactionId = insertDataResponse.QuoteTransactionId;
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

        private static QuoteConfirmDetailsResponseModel ReturnQuoteResponse()
        {
            return new QuoteConfirmDetailsResponseModel
            {
                InsurerStatusCode = (int)HttpStatusCode.OK,
                ValidationMessage = IdentitalInsurerMessage,
                CTA = "Retry",
                isNavigateToQuote = true
            };
        }

        private bool DoesSAODInsurerIsSame(CholaQuoteConfirmCommand request)
        {
            return !request.IsBrandNewVehicle &&
                            (request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive) || request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD)) &&
                            request.InsurerId.Equals(request.PreviousPolicy.SAODInsurer);
        }

        private bool DoesTPInsurerIsSame(CholaQuoteConfirmCommand request)
        {
            return !request.IsBrandNewVehicle &&
                            request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SATP) &&
                            request.InsurerId.Equals(request.PreviousPolicy.TPInsurer);
        }
    }
}
