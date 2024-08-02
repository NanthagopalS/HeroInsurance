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

namespace Insurance.Core.Features.IFFCO.Command.QuoteConfirm
{
    public class IffcoQuoteConfirmCommand : QuoteConfirmRequestModel, IRequest<HeroResult<QuoteConfirmDetailsResponseModel>>
    {
    }
    public class IffcoQuoteConfirmCommandHandler : IRequestHandler<IffcoQuoteConfirmCommand, HeroResult<QuoteConfirmDetailsResponseModel>>
    {
        private readonly PolicyTypeConfig _policyTypeConfig;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly IIFFCORepository _iFFCORepository;
        private readonly IApplicationClaims _applicationClaims;
        private readonly LogoConfig _logoConfig;
        private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";

        public IffcoQuoteConfirmCommandHandler(IQuoteRepository quoteRepository, IMapper mapper, IOptions<PolicyTypeConfig> policyTypeConfig,IIFFCORepository iFFCORepository, IApplicationClaims applicationClaims, IOptions<LogoConfig> options)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
            _policyTypeConfig = policyTypeConfig.Value;
            _iFFCORepository = iFFCORepository;
            _applicationClaims = applicationClaims;
            _logoConfig = options.Value;
        }
        public async Task<HeroResult<QuoteConfirmDetailsResponseModel>> Handle(IffcoQuoteConfirmCommand request, CancellationToken cancellationToken)
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
                var result = await _iFFCORepository.QuoteConfirmDetails(quoteResponseDB, quoteConfirmCommand, cancellationToken);

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
                quoteconfirmModel.IsPolicyExpired = request.PreviousPolicy.IsPolicyExpired;
                quoteconfirmModel.ConfirmCommand = quoteConfirmCommand;
                quoteconfirmModel.VehicleNumber = request.VehicleNumber;
                quoteconfirmModel.ResponseReferanceFlag = result.ResponseReferanceFlag;
                string oldResponse = quoteResponseDB?.QuoteTransactionRequest.CommonResponse;
                if (oldResponse != null)
                {
                    QuoteResponseModel getOldResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(oldResponse);
                    result.quoteConfirmResponse.NewPremium = Math.Round(Convert.ToDecimal(result.quoteConfirmResponse?.NewPremium)).ToString();
                    result.quoteConfirmResponse.OldPremium = Math.Round(Convert.ToDecimal(getOldResponse.GrossPremium)).ToString();

                    quoteconfirmModel.MinIDV = getOldResponse.MinIDV;
                    quoteconfirmModel.MaxIDV = getOldResponse.MaxIDV;
                    quoteconfirmModel.RecommendedIDV = getOldResponse.IDV;
                }

                var tpStartDate = !string.IsNullOrEmpty(quoteconfirmModel.PolicyDates.TPPolicyStartDate) ? Convert.ToDateTime(quoteconfirmModel.PolicyDates.TPPolicyStartDate).ToString("yyyy-MM-dd") : quoteconfirmModel.PolicyDates.TPPolicyStartDate;
                var odStartDate = !string.IsNullOrEmpty(quoteconfirmModel.PolicyDates.ODPolicyStartDate) ? Convert.ToDateTime(quoteconfirmModel.PolicyDates.ODPolicyStartDate).ToString("yyyy-MM-dd") : quoteconfirmModel.PolicyDates.ODPolicyStartDate;

                quoteconfirmModel.PolicyDates.ODPolicyStartDate = odStartDate;
                quoteconfirmModel.PolicyDates.TPPolicyStartDate = tpStartDate;

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

        private bool DoesSAODInsurerIsSame(IffcoQuoteConfirmCommand request)
        {
            return !request.IsBrandNewVehicle &&
            (request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.PackageComprehensive) || request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SAOD)) &&
            request.InsurerId.Equals(request.PreviousPolicy.SAODInsurer);
        }

        private bool DoesTPInsurerIsSame(IffcoQuoteConfirmCommand request)
        {
            return !request.IsBrandNewVehicle &&
                            request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SATP) &&
                            request.InsurerId.Equals(request.PreviousPolicy.TPInsurer);
        }
    }
}
