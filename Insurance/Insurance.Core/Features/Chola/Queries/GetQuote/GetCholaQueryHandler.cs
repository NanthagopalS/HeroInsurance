using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.Chola.Queries.GetQuote;

public class GetCholaQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{

}

public class GetCholaQueryHandler : IRequestHandler<GetCholaQuery, HeroResult<QuoteResponseModel>>
{
    private readonly ICholaRepository _cholaRepository;
    private readonly IMapper _mapper;
    private readonly CholaConfig _cholaConfig;
    private readonly PolicyTypeConfig _policyTypeConfig;
    private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
    private readonly IInsurerCheck _insurerCheck;
    private readonly IQuoteRepository _quoteRepository;

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="CholaRepository"></param>
    /// <param name="mapper"></param>
    public GetCholaQueryHandler(ICholaRepository cholaRepository, IMapper mapper, IOptions<CholaConfig> options, IOptions<PolicyTypeConfig> policyTypeConfig, IInsurerCheck insurerCheck, IQuoteRepository quoteRepository)
    {
        _cholaRepository = cholaRepository ?? throw new ArgumentNullException(nameof(cholaRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cholaConfig = options.Value;
        _policyTypeConfig = policyTypeConfig.Value;
        _insurerCheck = insurerCheck ?? throw new ArgumentNullException(nameof(insurerCheck));
        _quoteRepository = quoteRepository;
    }

    public async Task<HeroResult<QuoteResponseModel>> Handle(GetCholaQuery request, CancellationToken cancellationToken)
    {
        QuoteResponseModel quoteResponseModel = new QuoteResponseModel();
        string ValidationMassage = string.Empty;
        var variantAndRTOIdCheck = await _cholaRepository.DoesCholaVariantAndRTOExists(request.VariantId, request.RTOId, request.PreviousPolicy.PreviousPolicyTypeId, request.VehicleNumber, cancellationToken);
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
            quoteResponseModel.InsurerId = _cholaConfig.InsurerId;
            quoteResponseModel.InsurerLogo = _cholaConfig.InsurerLogo;
            quoteResponseModel.InsurerName = _cholaConfig.InsurerName;
            quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await _quoteRepository.InsertICLogs(_cholaConfig.InsurerId, JsonConvert.SerializeObject(request), ValidationMassage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
            return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
        }
        else
        {
            bool insurerCheckFlag = _insurerCheck.CheckPreviousCurrentInsurer(request.IsBrandNewVehicle,
                                           request.PreviousPolicy.IsPreviousPolicy,
                                           request.PreviousPolicy.PreviousPolicyTypeId,
                                           _cholaConfig.InsurerId,
                                           request.PreviousPolicy.SAODInsurer,
                                           request.PreviousPolicy.TPInsurer);
            if (insurerCheckFlag)
            {
                quoteResponseModel.InsurerId = _cholaConfig.InsurerId;
                quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteResponseModel.ValidationMessage = IdentitalInsurerMessage;
                quoteResponseModel.InsurerLogo = _cholaConfig.InsurerLogo;
				await _quoteRepository.InsertICLogs(_cholaConfig.InsurerId, JsonConvert.SerializeObject(request), IdentitalInsurerMessage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
				return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            var quoteData = await _cholaRepository.GetQuote(request, cancellationToken);
            if (quoteData != null)
            {
                return HeroResult<QuoteResponseModel>.Success(quoteData);
            }

            return HeroResult<QuoteResponseModel>.Fail("Failed to fetch the quotation");
        }
    }
}


