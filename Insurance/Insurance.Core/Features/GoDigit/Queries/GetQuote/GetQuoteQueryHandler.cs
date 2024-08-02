using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.GoDigit.Queries.GetQuote;

public class GetGoDigitQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{

}

public class GetQuoteQueryHandler : IRequestHandler<GetGoDigitQuery, HeroResult<QuoteResponseModel>>
{
    private readonly IGoDigitRepository _goDigitRepository;
    private readonly IQuoteRepository _quoteRepository;
    private readonly GoDigitConfig _goDigitConfig;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="goDigitRepository"></param>
    /// <param name="mapper"></param>
    public GetQuoteQueryHandler(IGoDigitRepository goDigitRepository, IQuoteRepository quoteRepository, IOptions<GoDigitConfig> option)
    {
        _goDigitRepository = goDigitRepository ?? throw new ArgumentNullException(nameof(goDigitRepository));
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        _goDigitConfig = option.Value;
    }

    public async Task<HeroResult<QuoteResponseModel>> Handle(GetGoDigitQuery request, CancellationToken cancellationToken)
    {
        string ValidationMassage = string.Empty;
        var variantAndRTOIdCheck = await _goDigitRepository.DoesGoDigitVariantAndRTOExists(request.VariantId, request.RTOId, request.VehicleNumber, cancellationToken);
        if (!variantAndRTOIdCheck.IsRTOExists || !variantAndRTOIdCheck.IsVariantExists)
        {
            var quoteResponse = new QuoteResponseModel();
            if (!variantAndRTOIdCheck.IsRTOExists)
            {
                ValidationMassage = "RTO Id Not Mapped";
            }
            else
            {
                ValidationMassage = "Variant Id Not Mapped";
            }
            quoteResponse.ValidationMessage = ValidationMassage;
            quoteResponse.InsurerId = _goDigitConfig.InsurerId;
            quoteResponse.InsurerLogo = _goDigitConfig.InsurerLogo;
            quoteResponse.InsurerName = _goDigitConfig.InsurerName;
            quoteResponse.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await _quoteRepository.InsertICLogs(_goDigitConfig.InsurerId, JsonConvert.SerializeObject(request), ValidationMassage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
            return HeroResult<QuoteResponseModel>.Success(quoteResponse);
        }
        else
        {
            var validationResponse = await _goDigitRepository.ValidationCheck(request, cancellationToken).ConfigureAwait(false);
            if (validationResponse is not null)
            {
                if (!validationResponse.IsAddonIdValid)
                {
                    return HeroResult<QuoteResponseModel>.Fail("Addon Id Invalid");
                }
                if (!validationResponse.IsDiscountIdValid)
                {
                    return HeroResult<QuoteResponseModel>.Fail("Discount Id Invalid");
                }
                if (!validationResponse.ISPacoverIdValid)
                {
                    return HeroResult<QuoteResponseModel>.Fail("PACover Id Invalid");
                }
                if (!validationResponse.IsAccessoryIdValid)
                {
                    return HeroResult<QuoteResponseModel>.Fail("Accessory Id Invalid");
                }
                if (!validationResponse.IsVarientIdValid)
                {
                    return HeroResult<QuoteResponseModel>.Fail("Varient Id Invalid");
                }
                if (!validationResponse.IsVehicleTypeIdValid)
                {
                    return HeroResult<QuoteResponseModel>.Fail("Vehicel Type Id Invalid");
                }
                if (!validationResponse.IsPolicyTypeIdValid)
                {
                    return HeroResult<QuoteResponseModel>.Fail("Policy Type Id Invalid");
                }
                if (!validationResponse.IsRTOIdValid)
                {
                    return HeroResult<QuoteResponseModel>.Fail("RTO Id Invalid");
                }
            }
            var quoteResponse = await _goDigitRepository.GetQuote(request, cancellationToken);
            if (quoteResponse is not null)
            {
                return HeroResult<QuoteResponseModel>.Success(quoteResponse);
            }

            return HeroResult<QuoteResponseModel>.Fail("No Record Found");
        }
    }
}
