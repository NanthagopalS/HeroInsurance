using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.UnitedIndia;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.UnitedIndia.Queries.GetQuote;

public class GetUnitedIndiaQuoteQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{

}
public class GetUnitedIndiaQuoteQueryHandler : IRequestHandler<GetUnitedIndiaQuoteQuery, HeroResult<QuoteResponseModel>>
{
    private readonly IUnitedIndiaRepository _unitedIndiaRepository;
    private readonly UnitedIndiaConfig _unitedIndiaConfig;
    private readonly IQuoteRepository _quoteRepository;
    private readonly IInsurerCheck _insurerCheck;
    private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
    public GetUnitedIndiaQuoteQueryHandler(IUnitedIndiaRepository unitedIndiaRepository,
        IOptions<UnitedIndiaConfig> unitedIndiaConfig,
        IQuoteRepository quoteRepository,
        IInsurerCheck insurerCheck)
    {
        _unitedIndiaRepository = unitedIndiaRepository;
        _unitedIndiaConfig = unitedIndiaConfig.Value;
        _quoteRepository = quoteRepository;
        _insurerCheck = insurerCheck ?? throw new ArgumentNullException(nameof(insurerCheck));
    }
    public async Task<HeroResult<QuoteResponseModel>> Handle(GetUnitedIndiaQuoteQuery request, CancellationToken cancellationToken)
    {
        QuoteResponseModel quoteResponseModel = new QuoteResponseModel();
        string ValidationMassage = string.Empty;
        var variantAndRTOIdCheck = await _unitedIndiaRepository.DoesUnitedIndiaVariantAndRTOExists(request.VariantId, request.RTOId, request.VehicleNumber, cancellationToken);
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
            quoteResponseModel.InsurerId = _unitedIndiaConfig.InsurerId;
            quoteResponseModel.InsurerLogo = _unitedIndiaConfig.InsurerLogo;
            quoteResponseModel.InsurerName = _unitedIndiaConfig.InsurerName;
            quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await _quoteRepository.InsertICLogs(_unitedIndiaConfig.InsurerId, JsonConvert.SerializeObject(request), ValidationMassage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
            return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
        }
        else
        {
            bool insurerCheckFlag = _insurerCheck.CheckPreviousCurrentInsurer(request.IsBrandNewVehicle,
                                   request.PreviousPolicy.IsPreviousPolicy,
                                   request.PreviousPolicy.PreviousPolicyTypeId,
                                   _unitedIndiaConfig.InsurerId,
                                   request.PreviousPolicy.SAODInsurer,
                                   request.PreviousPolicy.TPInsurer);

            if (insurerCheckFlag)
            {
                quoteResponseModel.InsurerId = _unitedIndiaConfig.InsurerId;
                quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteResponseModel.ValidationMessage = IdentitalInsurerMessage;
                quoteResponseModel.InsurerLogo = _unitedIndiaConfig.InsurerLogo;
                await _quoteRepository.InsertICLogs(_unitedIndiaConfig.InsurerId, JsonConvert.SerializeObject(request), IdentitalInsurerMessage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
                return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            var quoteData = await _unitedIndiaRepository.GetQuote(request, cancellationToken);
            if (quoteData != null)
            {
                return HeroResult<QuoteResponseModel>.Success(quoteData);
            }
            return HeroResult<QuoteResponseModel>.Fail("Failed to fetch the quotation");
        }
    }
}
