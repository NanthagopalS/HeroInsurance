using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.TATA;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.TATA.Queries.GetQuote;

public class GetTATAQuoteQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{
}

public class GetTATAQuoteQueryHandler : IRequestHandler<GetTATAQuoteQuery, HeroResult<QuoteResponseModel>>
{
    private readonly ITATARepository _tataRepository;
    private readonly IMapper _mapper;
    private readonly TATAConfig _tataConfig;
    private readonly PolicyTypeConfig _policyTypeConfig;
    private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
    private readonly IInsurerCheck _insurerCheck;
    private readonly IQuoteRepository _quoteRepository;
    public GetTATAQuoteQueryHandler(ITATARepository tataRepository, IMapper mapper, IOptions<TATAConfig> options, IOptions<PolicyTypeConfig> policyTypeConfig, IInsurerCheck insurerCheck, IQuoteRepository quoteRepository)
    {
        _tataRepository = tataRepository ?? throw new ArgumentNullException(nameof(tataRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _tataConfig = options.Value;
        _policyTypeConfig = policyTypeConfig.Value;
        _insurerCheck = insurerCheck ?? throw new ArgumentNullException(nameof(insurerCheck));
        _quoteRepository = quoteRepository;
    }

    public async Task<HeroResult<QuoteResponseModel>> Handle(GetTATAQuoteQuery request, CancellationToken cancellationToken)
    {
        QuoteResponseModel quoteResponseModel = new QuoteResponseModel();
        string ValidationMassage = string.Empty;
        var variantAndRTOIdCheck = await _tataRepository.DoesTATAVariantAndRTOExists(request.VariantId, request.RTOId, request.VehicleNumber, cancellationToken);
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
            quoteResponseModel.InsurerId = _tataConfig.InsurerId;
            quoteResponseModel.InsurerLogo = _tataConfig.InsurerLogo;
            quoteResponseModel.InsurerName = _tataConfig.InsurerName;
            quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await _quoteRepository.InsertICLogs(_tataConfig.InsurerId, JsonConvert.SerializeObject(request), ValidationMassage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
            return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
        }
        else
        {
            bool insurerCheckFlag = _insurerCheck.CheckPreviousCurrentInsurer(request.IsBrandNewVehicle,
                                           request.PreviousPolicy.IsPreviousPolicy,
                                           request.PreviousPolicy.PreviousPolicyTypeId,
                                           _tataConfig.InsurerId,
                                           request.PreviousPolicy.SAODInsurer,
                                           request.PreviousPolicy.TPInsurer);
            if (insurerCheckFlag)
            {
                quoteResponseModel.InsurerId = _tataConfig.InsurerId;
                quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteResponseModel.ValidationMessage = IdentitalInsurerMessage;
                quoteResponseModel.InsurerLogo = _tataConfig.InsurerLogo;
				await _quoteRepository.InsertICLogs(_tataConfig.InsurerId, JsonConvert.SerializeObject(request), IdentitalInsurerMessage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
				return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            var quoteData = await _tataRepository.GetQuote(request, cancellationToken);
            if (quoteData is not null)
            {
                return HeroResult<QuoteResponseModel>.Success(quoteData);
            }

            return HeroResult<QuoteResponseModel>.Fail("Failed to fetch the quotation");
        }
    }
}