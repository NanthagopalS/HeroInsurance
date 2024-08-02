using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.HDFC.Queries.GetQuote;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Reliance;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;

namespace Insurance.Core.Features.Reliance.Command.GetQuote;

public class GetCVRelianceQuery : QuoteBaseCommand, IRequest<HeroResult<QuoteResponseModel>>
{

}

public class GetCVQuoteQueryHandler : IRequestHandler<GetCVRelianceQuery, HeroResult<QuoteResponseModel>>
{
    private readonly IRelianceRepository _relianceRepository;
    private readonly RelianceConfig _relianceConfig;
    private const string IdentitalInsurerMessage = "New insurer can not be same as previous insurer. Please Try with another Insurance company.";
    private readonly IInsurerCheck _insurerCheck;
    private readonly IQuoteRepository _quoteRepository;
    private readonly IMapper _mapper;
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="relianceRepository"></param>
    /// <param name="mapper"></param>
    public GetCVQuoteQueryHandler(IRelianceRepository relianceRepository, IOptions<RelianceConfig> options, IInsurerCheck insurerCheck, IQuoteRepository quoteRepository, IMapper mapper)
    {
        _relianceRepository = relianceRepository;
        _relianceConfig = options.Value;
        _insurerCheck = insurerCheck ?? throw new ArgumentNullException(nameof(insurerCheck));
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        _mapper = mapper;
    }

    public async Task<HeroResult<QuoteResponseModel>> Handle(GetCVRelianceQuery request, CancellationToken cancellationToken)
    {
        QuoteResponseModel quoteResponseModel = new QuoteResponseModel();
        string ValidationMassage = string.Empty;
        var variantAndRTOIdCheck = await _relianceRepository.DoesRelianceVariantAndRTOExists(request.VariantId, request.RTOId, request.VehicleNumber, request.VehicleTypeId,cancellationToken);
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
            quoteResponseModel.InsurerId = _relianceConfig.InsurerId;
            quoteResponseModel.InsurerLogo = _relianceConfig.InsurerLogo;
            quoteResponseModel.InsurerName = _relianceConfig.InsurerName;
            quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            await _quoteRepository.InsertICLogs(_relianceConfig.InsurerId, JsonConvert.SerializeObject(request), ValidationMassage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
            return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
        }
        else
        {
            bool insurerCheckFlag = _insurerCheck.CheckPreviousCurrentInsurer(request.IsBrandNewVehicle,
                                           request.PreviousPolicy.IsPreviousPolicy,
                                           request.PreviousPolicy.PreviousPolicyTypeId,
                                           _relianceConfig.InsurerId,
                                           request.PreviousPolicy.SAODInsurer,
                                           request.PreviousPolicy.TPInsurer);
            if (insurerCheckFlag)
            {
                quoteResponseModel.InsurerId = _relianceConfig.InsurerId;
                quoteResponseModel.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                quoteResponseModel.ValidationMessage = IdentitalInsurerMessage;
                quoteResponseModel.InsurerLogo = _relianceConfig.InsurerLogo;
				await _quoteRepository.InsertICLogs(_relianceConfig.InsurerId, JsonConvert.SerializeObject(request), IdentitalInsurerMessage, request.LeadId, string.Empty, string.Empty, string.Empty, "Quote");
				return HeroResult<QuoteResponseModel>.Success(quoteResponseModel);
            }
            // Mapped Reliance Commercial Query with Reliance Quote Query Handler for Quotation
            var quoteRequest = _mapper.Map<GetRelianceQuery>(request);
            var quoteData = await _relianceRepository.GetQuote(quoteRequest, cancellationToken);
            if (quoteData is not null)
            {
                if (!string.IsNullOrEmpty(quoteData.ValidationMessage))
                {
                    quoteData.InsurerId = _relianceConfig.InsurerId;
                    quoteData.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
                    quoteData.InsurerLogo = _relianceConfig.InsurerLogo;
                    quoteData.InsurerName = _relianceConfig.InsurerName;
                    quoteData.LeadId = request.LeadId;
                }
                return HeroResult<QuoteResponseModel>.Success(quoteData);
            }

            return HeroResult<QuoteResponseModel>.Fail("Failed to fetch the quotation");
        }
    }

}
