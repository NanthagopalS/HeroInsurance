using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.Bajaj;
using Insurance.Domain.GoDigit;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.Bajaj.Queries.GetQuote;
public class GetBajajQuoteQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{

}

public class GetQuoteQueryHandler : IRequestHandler<GetBajajQuoteQuery, HeroResult<QuoteResponseModel>>
{
    private readonly IBajajRepository _bajajRepository;
    private readonly BajajConfig _bajajConfig;
    private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
    private readonly IInsurerCheck _insurerCheck;
    private readonly IQuoteRepository _quoteRepository;

    public GetQuoteQueryHandler(IBajajRepository bajajRepository, IOptions<BajajConfig> options, IInsurerCheck insurerCheck, IQuoteRepository quoteRepository)
    {
        _bajajRepository = bajajRepository ?? throw new ArgumentNullException(nameof(bajajRepository));
        _bajajConfig = options.Value;
        _insurerCheck = insurerCheck ?? throw new ArgumentNullException(nameof(insurerCheck));
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
    }

    public async Task<HeroResult<QuoteResponseModel>> Handle(GetBajajQuoteQuery request, CancellationToken cancellationToken)
    {
        string ValidationMassage = string.Empty;
        QuoteResponseModel quoteResponseModel = new QuoteResponseModel();
        var variantAndRTOIdCheck = await _bajajRepository.DoesBajajVariantAndRTOExists(request.VariantId, request.RTOId, request.VehicleNumber, cancellationToken);
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
            quoteResponseModel.InsurerId = _bajajConfig.InsurerId;
            quoteResponseModel.InsurerLogo = _bajajConfig.InsurerLogo;
            quoteResponseModel.InsurerName = _bajajConfig.InsurerName;
            quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await _quoteRepository.InsertICLogs(_bajajConfig.InsurerId, JsonConvert.SerializeObject(request), ValidationMassage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
            return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
        }
        else
        {
            bool insurerCheckFlag = _insurerCheck.CheckPreviousCurrentInsurer(request.IsBrandNewVehicle,
                                               request.PreviousPolicy.IsPreviousPolicy,
                                               request.PreviousPolicy.PreviousPolicyTypeId,
                                               _bajajConfig.InsurerId,
                                               request.PreviousPolicy.SAODInsurer,
                                               request.PreviousPolicy.TPInsurer);
            if (insurerCheckFlag)
            {
                quoteResponseModel.InsurerId = _bajajConfig.InsurerId;
                quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteResponseModel.ValidationMessage = IdentitalInsurerMessage;
                quoteResponseModel.InsurerLogo = _bajajConfig.InsurerLogo;
                await _quoteRepository.InsertICLogs(_bajajConfig.InsurerId, JsonConvert.SerializeObject(request), IdentitalInsurerMessage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
                return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            var quoteData = await _bajajRepository.GetQuote(request, cancellationToken);
            if (quoteData != null)
            {
                return HeroResult<QuoteResponseModel>.Success(quoteData);
            }

            return HeroResult<QuoteResponseModel>.Fail("Failed to fetch the quotation");
        }
    }
}
