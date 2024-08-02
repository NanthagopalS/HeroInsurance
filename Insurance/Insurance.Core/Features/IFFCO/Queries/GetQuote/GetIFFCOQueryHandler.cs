using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Reliance;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.IFFCO.Queries.GetQuote;

public class GetIFFCOQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{

}
public class GetIFFCOQueryHandler : IRequestHandler<GetIFFCOQuery, HeroResult<QuoteResponseModel>>
{
    private readonly IIFFCORepository _iFFORepository;
    private readonly IMapper _mapper;
    private readonly IFFCOConfig _iffcoConfig;
    private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
    private readonly IInsurerCheck _insurerCheck;
    private readonly IQuoteRepository _quoteRepository;

    public GetIFFCOQueryHandler(IIFFCORepository iFFCORepository, IMapper mapper, IOptions<IFFCOConfig> options, IInsurerCheck insurerCheck, IQuoteRepository quoteRepository)
    {
        _iFFORepository = iFFCORepository ?? throw new ArgumentNullException(nameof(iFFCORepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _iffcoConfig = options.Value;
        _insurerCheck = insurerCheck ?? throw new ArgumentNullException(nameof(insurerCheck));
        _quoteRepository = quoteRepository;
    }
    public async Task<HeroResult<QuoteResponseModel>> Handle(GetIFFCOQuery request, CancellationToken cancellationToken)
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
				await _quoteRepository.InsertICLogs(_iffcoConfig.InsurerId, JsonConvert.SerializeObject(request), IdentitalInsurerMessage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
				return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            var quoteData = await _iFFORepository.GetQuote(request, cancellationToken);
            if (quoteData != null)
            {
                return HeroResult<QuoteResponseModel>.Success(quoteData);
            }
            return HeroResult<QuoteResponseModel>.Fail("Failed to fetch the quotation");
        }
    }
}

