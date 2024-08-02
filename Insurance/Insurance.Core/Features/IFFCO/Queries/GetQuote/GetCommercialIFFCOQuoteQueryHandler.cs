using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.IFFCO;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.IFFCO.Queries.GetQuote;

public class GetCommercialIFFCOQuoteQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{

}
public class GetCommercialIFFCOQuoteQueryHandler : IRequestHandler<GetCommercialIFFCOQuoteQuery, HeroResult<QuoteResponseModel>>
{
    private readonly IIFFCORepository _iFFORepository;
    private readonly IMapper _mapper;
    private readonly IFFCOConfig _iffcoConfig;
    private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
    private const string BrandNewSATPValidationMessage = "Insurer currently not providing SATP policy for brand new vehicle";
    private readonly IInsurerCheck _insurerCheck;
    private readonly IQuoteRepository _quoteRepository;

    public GetCommercialIFFCOQuoteQueryHandler(IIFFCORepository iFFCORepository, IMapper mapper, IOptions<IFFCOConfig> options, IInsurerCheck insurerCheck, IQuoteRepository quoteRepository)
    {
        _iFFORepository = iFFCORepository ?? throw new ArgumentNullException(nameof(iFFCORepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _iffcoConfig = options.Value;
        _insurerCheck = insurerCheck ?? throw new ArgumentNullException(nameof(insurerCheck));
        _quoteRepository = quoteRepository;
    }
    public async Task<HeroResult<QuoteResponseModel>> Handle(GetCommercialIFFCOQuoteQuery request, CancellationToken cancellationToken)
    {
        QuoteResponseModel quoteResponseModel = new QuoteResponseModel();
        string ValidationMassage = string.Empty;
        var variantAndRTOIdCheck = await _iFFORepository.DoesIFFCOVariantAndRTOExists(request.VariantId, request.RTOId, request.VehicleNumber, cancellationToken);
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
            quoteResponseModel.InsurerId = _iffcoConfig.InsurerId;
            quoteResponseModel.InsurerLogo = _iffcoConfig.InsurerLogo;
            quoteResponseModel.InsurerName = _iffcoConfig.InsurerName;
            quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await _quoteRepository.InsertICLogs(_iffcoConfig.InsurerId, JsonConvert.SerializeObject(request), ValidationMassage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
            return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
        }
        else
        {
            bool insurerCheckFlag = _insurerCheck.CheckPreviousCurrentInsurer(request.IsBrandNewVehicle,
                                   request.PreviousPolicy.IsPreviousPolicy,
                                   request.PreviousPolicy.PreviousPolicyTypeId,
                                   _iffcoConfig.InsurerId,
                                   request.PreviousPolicy.SAODInsurer,
                                   request.PreviousPolicy.TPInsurer);

            if (insurerCheckFlag)
            {
                quoteResponseModel.InsurerId = _iffcoConfig.InsurerId;
                quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteResponseModel.ValidationMessage = IdentitalInsurerMessage;
                quoteResponseModel.InsurerLogo = _iffcoConfig.InsurerLogo;
                quoteResponseModel.InsurerName = _iffcoConfig.InsurerName;
                return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            var quoteRequest = _mapper.Map<GetIFFCOQuery>(request);
            if(quoteRequest.IsBrandNewVehicle && quoteRequest.PreviousPolicy.PreviousPolicyTypeId.Equals("2AA7FDCA-9E36-4A8D-9583-15ADA737574B"))
            {
                quoteResponseModel.InsurerId = _iffcoConfig.InsurerId;
                quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteResponseModel.ValidationMessage = BrandNewSATPValidationMessage;
                quoteResponseModel.InsurerLogo = _iffcoConfig.InsurerLogo;
                quoteResponseModel.InsurerName = _iffcoConfig.InsurerName;
                return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            else
            {
                var quoteData = await _iFFORepository.GetQuote(quoteRequest, cancellationToken);
                if (quoteData != null)
                {
                    return HeroResult<QuoteResponseModel>.Success(quoteData);
                }
                return HeroResult<QuoteResponseModel>.Fail("Failed to fetch the quotation");
            }
        }
    }
}

