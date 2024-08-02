using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.ICICI;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.ICICI.Queries.GetQuote;

public class GetIciciQuoteQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{
}

public class GetQuoteQueryHandler : IRequestHandler<GetIciciQuoteQuery, HeroResult<QuoteResponseModel>>
{
    private readonly IICICIRepository _iCICIRepository;
    private readonly ICICIConfig _iciciConfig;
    private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
    private readonly IInsurerCheck _insurerCheck;
    private readonly IQuoteRepository _quoteRepository;

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="iCICIRepository"></param>
    /// <param name="mapper"></param>
    public GetQuoteQueryHandler(IICICIRepository iCICIRepository, IOptions<ICICIConfig> options, IInsurerCheck insurerCheck, IQuoteRepository quoteRepository)
    {
        _iCICIRepository = iCICIRepository ?? throw new ArgumentNullException(nameof(iCICIRepository));
        _iciciConfig = options.Value;
        _insurerCheck = insurerCheck ?? throw new ArgumentNullException(nameof(insurerCheck));
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
    }

    public async Task<HeroResult<QuoteResponseModel>> Handle(GetIciciQuoteQuery request, CancellationToken cancellationToken)
    {
        QuoteResponseModel quoteResponseModel = new QuoteResponseModel();
        string ValidationMassage = string.Empty;
        var variantAndRTOIdCheck = await _iCICIRepository.DoesICICIVariantAndRTOExists(request.VariantId, request.RTOId, request.VehicleNumber, cancellationToken);
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
            quoteResponseModel.InsurerId = _iciciConfig.InsurerId;
            quoteResponseModel.InsurerLogo = _iciciConfig.InsurerLogo;
            quoteResponseModel.InsurerName = _iciciConfig.InsurerName;
            quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await _quoteRepository.InsertICLogs(_iciciConfig.InsurerId, JsonConvert.SerializeObject(request), ValidationMassage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
            return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
        }
        else
        {
            bool insurerCheckFlag = _insurerCheck.CheckPreviousCurrentInsurer(request.IsBrandNewVehicle,
                                           request.PreviousPolicy.IsPreviousPolicy,
                                           request.PreviousPolicy.PreviousPolicyTypeId,
                                           _iciciConfig.InsurerId,
                                           request.PreviousPolicy.SAODInsurer,
                                           request.PreviousPolicy.TPInsurer);
            if (insurerCheckFlag)
            {
                quoteResponseModel.InsurerId = _iciciConfig.InsurerId;
                quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteResponseModel.ValidationMessage = IdentitalInsurerMessage;
                quoteResponseModel.InsurerLogo = _iciciConfig.InsurerLogo;
				await _quoteRepository.InsertICLogs(_iciciConfig.InsurerId, JsonConvert.SerializeObject(request), IdentitalInsurerMessage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
				return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            var quoteData = await _iCICIRepository.GetQuote(request, cancellationToken);
            if (quoteData != null)
            {
                return HeroResult<QuoteResponseModel>.Success(quoteData);
            }

            return HeroResult<QuoteResponseModel>.Fail("Failed to fetch the quotation");
        }
    }
}