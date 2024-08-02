using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.Bajaj;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.HDFC.Queries.GetQuote;
public class GetHdfcQuoteQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{

}

public class GetQuoteQueryHandler : IRequestHandler<GetHdfcQuoteQuery, HeroResult<QuoteResponseModel>>
{
    private readonly IHDFCRepository _hdfcRepository;
    private readonly HDFCConfig _hdfcConfig;
    private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
    private readonly IInsurerCheck _insurerCheck;
	private readonly IQuoteRepository _quoteRepository;
	public GetQuoteQueryHandler(IHDFCRepository hdfcRepository, IOptions<HDFCConfig> options, IInsurerCheck insurerCheck, IQuoteRepository quoteRepository)
    {
        _hdfcRepository = hdfcRepository ?? throw new ArgumentNullException(nameof(hdfcRepository));
        _hdfcConfig = options.Value;
        _insurerCheck = insurerCheck ?? throw new ArgumentNullException(nameof(insurerCheck));
		_quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
	}

    public async Task<HeroResult<QuoteResponseModel>> Handle(GetHdfcQuoteQuery request, CancellationToken cancellationToken)
    {
        QuoteResponseModel quoteResponseModel = new QuoteResponseModel();
        string ValidationMassage = string.Empty;
        var variantAndRTOIdCheck = await _hdfcRepository.DoesHDFCVariantAndRTOExists(request.VariantId, request.RTOId, request.VehicleNumber,request.VehicleTypeId, cancellationToken);
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
            quoteResponseModel.InsurerId = _hdfcConfig.InsurerId;
            quoteResponseModel.InsurerLogo = _hdfcConfig.InsurerLogo;
            quoteResponseModel.InsurerName = _hdfcConfig.InsurerName;
            quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
			await _quoteRepository.InsertICLogs(_hdfcConfig.InsurerId, JsonConvert.SerializeObject(request), ValidationMassage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
			return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
        }
        else
        {
            bool insurerCheckFlag = _insurerCheck.CheckPreviousCurrentInsurer(request.IsBrandNewVehicle,
                                           request.PreviousPolicy.IsPreviousPolicy,
                                           request.PreviousPolicy.PreviousPolicyTypeId,
                                           _hdfcConfig.InsurerId,
                                           request.PreviousPolicy.SAODInsurer,
                                           request.PreviousPolicy.TPInsurer);
            if (insurerCheckFlag)
            {
                quoteResponseModel.InsurerId = _hdfcConfig.InsurerId;
                quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteResponseModel.ValidationMessage = IdentitalInsurerMessage;
                quoteResponseModel.InsurerLogo = _hdfcConfig.InsurerLogo;
				await _quoteRepository.InsertICLogs(_hdfcConfig.InsurerId, JsonConvert.SerializeObject(request), IdentitalInsurerMessage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
				return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            var quoteData = await _hdfcRepository.GetQuote(request, cancellationToken);
            if (quoteData != null)
            {
                return HeroResult<QuoteResponseModel>.Success(quoteData);
            }

            return HeroResult<QuoteResponseModel>.Fail("Failed to fetch the quotation");
        }
    }
}
