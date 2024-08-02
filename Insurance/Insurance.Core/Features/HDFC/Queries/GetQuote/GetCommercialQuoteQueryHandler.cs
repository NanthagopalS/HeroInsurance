using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using System.Net;
using ThirdPartyUtilities.Helpers;
namespace Insurance.Core.Features.HDFC.Queries.GetQuote;
public class GetCommercialHDFCQuoteQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{
    public string ManufacturingDate { get; set; }
}

public class GetCommercialQuoteQueryHandler : IRequestHandler<GetCommercialHDFCQuoteQuery, HeroResult<QuoteResponseModel>>
{
    private readonly IHDFCRepository _hdfcRepository;
    private readonly HDFCConfig _hdfcConfig;
    private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
    private readonly IInsurerCheck _insurerCheck;
    private readonly PolicyTypeConfig _policyType;
    private readonly IMapper _mapper;
    public GetCommercialQuoteQueryHandler(IHDFCRepository hdfcRepository, IOptions<HDFCConfig> options, IInsurerCheck insurerCheck, IOptions<PolicyTypeConfig> policyType, IMapper mapper)
    {
        _hdfcRepository = hdfcRepository ?? throw new ArgumentNullException(nameof(hdfcRepository));
        _hdfcConfig = options.Value;
        _insurerCheck = insurerCheck ?? throw new ArgumentNullException(nameof(insurerCheck));
        _policyType = policyType?.Value;
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<HeroResult<QuoteResponseModel>> Handle(GetCommercialHDFCQuoteQuery request, CancellationToken cancellationToken)
    {
        QuoteResponseModel quoteResponseModel = new QuoteResponseModel();
        string validationMassage = string.Empty;
        var variantAndRTOIdCheck = await _hdfcRepository.DoesHDFCVariantAndRTOExists(request.VariantId, request.RTOId, request.VehicleNumber,request.VehicleTypeId, cancellationToken);
        if (!variantAndRTOIdCheck.IsRTOExists || !variantAndRTOIdCheck.IsVariantExists)
        {
            validationMassage = !variantAndRTOIdCheck.IsRTOExists ? "RTO Id Not Mapped" : "Variant Id Not Mapped";
            quoteResponseModel.ValidationMessage = validationMassage;
            quoteResponseModel.InsurerId = _hdfcConfig.InsurerId;
            quoteResponseModel.InsurerLogo = _hdfcConfig.InsurerLogo;
            quoteResponseModel.InsurerName = _hdfcConfig.InsurerName;
            quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
        }
        else
        {
            bool insurerCheckFlag = _insurerCheck.CheckPreviousCurrentInsurer(request.IsBrandNewVehicle,request.PreviousPolicy.IsPreviousPolicy,
                    request.PreviousPolicy.PreviousPolicyTypeId,_hdfcConfig.InsurerId,request.PreviousPolicy.SAODInsurer,request.PreviousPolicy.TPInsurer);
            if (insurerCheckFlag)
            {
                quoteResponseModel.InsurerId = _hdfcConfig.InsurerId;
                quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteResponseModel.ValidationMessage = IdentitalInsurerMessage;
                quoteResponseModel.InsurerLogo = _hdfcConfig.InsurerLogo;
                return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            // Mapped HDFC Commercial Query with HDFC Quote Query Handler for Quotation
            var quoteRequest = _mapper.Map<GetHdfcQuoteQuery>(request);
            var quoteData = await _hdfcRepository.GetQuote(quoteRequest, cancellationToken);

            if (quoteData != null)
            {
                return HeroResult<QuoteResponseModel>.Success(quoteData);
            }
            return HeroResult<QuoteResponseModel>.Fail("Failed to fetch the quotation");
        }
    }
}
