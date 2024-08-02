using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Oriental;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.Oriental.Queries.GetQuote;

public class GetOrientalQuoteQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{

}
public class GetOrientalQuoteQueryHandler : IRequestHandler<GetOrientalQuoteQuery, HeroResult<QuoteResponseModel>>
{
    private readonly IOrientalRepository _orientalRepository;
    private readonly IInsurerCheck _insurerCheck;
    private readonly OrientalConfig _orientalConfig;
    private readonly PolicyTypeConfig _policyTypeConfig;
    private readonly IQuoteRepository _quoteRepository;
    private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
    private const string PolicyExpiredMessage = "Previous Policy Expired, Unable to proceed to further";
    public GetOrientalQuoteQueryHandler(IOrientalRepository orientalRepository,
        IInsurerCheck insurerCheck,
        IOptions<OrientalConfig> options,
        IQuoteRepository quoteRepository,
        IOptions<PolicyTypeConfig> policyTypeConfig)
    {
        _orientalRepository = orientalRepository;
        _insurerCheck = insurerCheck;
        _orientalConfig = options.Value;
        _quoteRepository = quoteRepository;
        _policyTypeConfig = policyTypeConfig.Value;
    }
    public async Task<HeroResult<QuoteResponseModel>> Handle(GetOrientalQuoteQuery request, CancellationToken cancellationToken)
    {
        QuoteResponseModel quoteResponseModel = new QuoteResponseModel();
        string ValidationMassage = string.Empty;
        var rtoCode = string.Empty;
        var isPolicyExpired = false;
        var todayDate = DateTime.Now.ToString("yyyy-MM-dd");
        if (!request.PreviousPolicy.PreviousPolicyTypeId.Equals(_policyTypeConfig.SATP))
        {
            if (!request.IsBrandNewVehicle && request.PreviousPolicy.IsPreviousPolicy)
            {
                if (Convert.ToDateTime(request.PreviousPolicy.SAODPolicyExpiryDate) < Convert.ToDateTime(todayDate))
                {
                    isPolicyExpired = true;
                }
            }
            else if (!request.IsBrandNewVehicle && !request.PreviousPolicy.IsPreviousPolicy)
            {
                isPolicyExpired = true;
            }
        }

        if (isPolicyExpired)
        {
            quoteResponseModel.InsurerId = _orientalConfig.InsurerId;
            quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            quoteResponseModel.ValidationMessage = PolicyExpiredMessage;
            quoteResponseModel.InsurerLogo = _orientalConfig.InsurerLogo;
            return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
        }
        if (!string.IsNullOrEmpty(request.VehicleNumber))
        {
            var vehicleNumber = await _quoteRepository.VehicleNumberSplit(request.VehicleNumber);
            if (vehicleNumber.Any())
            {
                if (vehicleNumber[1].Length == 1)
                {
                    rtoCode = $"{vehicleNumber[0]}0{vehicleNumber[1]}";
                }
                else
                {
                    rtoCode = $"{vehicleNumber[0]}{vehicleNumber[1]}";
                }
            }
        }
        var variantAndRTOIdCheck = await _orientalRepository.DoesOrientalVariantAndRTOExists(request.VariantId, request.RTOId, rtoCode, cancellationToken);
        if (!variantAndRTOIdCheck.IsRTOExists || !variantAndRTOIdCheck.IsVariantExists)
        {
            if (!variantAndRTOIdCheck.IsRTOExists)
            {
                ValidationMassage = "RTO Id Not Mapped";
            }
            else
            {
                ValidationMassage = "Variant Id Not Mapped";
            }
            quoteResponseModel.ValidationMessage = ValidationMassage;
            quoteResponseModel.InsurerId = _orientalConfig.InsurerId;
            quoteResponseModel.InsurerLogo = _orientalConfig.InsurerLogo;
            quoteResponseModel.InsurerName = _orientalConfig.InsurerName;
            quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await _quoteRepository.InsertICLogs(_orientalConfig.InsurerId, JsonConvert.SerializeObject(request), ValidationMassage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
            return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
        }
        else
        {
            bool insurerCheckFlag = _insurerCheck.CheckPreviousCurrentInsurer(request.IsBrandNewVehicle,
                                   request.PreviousPolicy.IsPreviousPolicy,
            request.PreviousPolicy.PreviousPolicyTypeId,
                                   _orientalConfig.InsurerId,
                                   request.PreviousPolicy.SAODInsurer,
                                   request.PreviousPolicy.TPInsurer);

            if (insurerCheckFlag)
            {
                quoteResponseModel.InsurerId = _orientalConfig.InsurerId;
                quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteResponseModel.ValidationMessage = IdentitalInsurerMessage;
                quoteResponseModel.InsurerLogo = _orientalConfig.InsurerLogo;
				await _quoteRepository.InsertICLogs(_orientalConfig.InsurerId, JsonConvert.SerializeObject(request), IdentitalInsurerMessage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
				return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            var quoteData = await _orientalRepository.GetQuote(request, cancellationToken);
            if (quoteData != null)
            {
                return HeroResult<QuoteResponseModel>.Success(quoteData);
            }
            return HeroResult<QuoteResponseModel>.Fail("Failed to fetch the quotation");
        }
    }
}
