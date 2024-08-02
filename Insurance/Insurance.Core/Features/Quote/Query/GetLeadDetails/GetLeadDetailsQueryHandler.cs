using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Quote.Query.GetLeadDetails;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetailsQueryHandler
{
    public record GetLeadDetailsQuery : IRequest<HeroResult<GetLeadDetailsVm>>
    {
        public string LeadId { get; set; }
        public string StageId { get; set; }
    }
    public class GetLeadDetailsQueryHandler : IRequestHandler<GetLeadDetailsQuery, HeroResult<GetLeadDetailsVm>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly LogoConfig _logoConfig;

        public GetLeadDetailsQueryHandler(IQuoteRepository quoteRepository, IMapper mapper, IOptions<LogoConfig> options)
        {
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _mapper = mapper;
            _logoConfig = options.Value;
        }
        public async Task<HeroResult<GetLeadDetailsVm>> Handle(GetLeadDetailsQuery getLeadDetailsQuery, CancellationToken cancellationToken)
        {
            var result = await _quoteRepository.GetLeadDetails(getLeadDetailsQuery.LeadId, getLeadDetailsQuery.StageId, cancellationToken).ConfigureAwait(false);

            if (result != null)
            {
                var response = _mapper.Map<GetLeadDetailsVm>(result);

                response.Logo = _logoConfig.InsurerLogoURL + result.Logo;
                response.IsPrevPolicyClaim = !string.IsNullOrWhiteSpace(response.PrevPolicyClaims) ? (response.PrevPolicyClaims.ToLower().Equals("yes") ? true : false) : false;
                response.IsVehicleNumberPresent = !string.IsNullOrWhiteSpace(response.VehicleNumber) ? (response.VehicleNumber.Length > 4 ? true : false) : false;
                response.RegistrationYear = !string.IsNullOrWhiteSpace(response.RegistrationDate) ? response.RegistrationDate.Substring(6, 4) : response.RegistrationYear;

                response.QuoteResponse = !string.IsNullOrWhiteSpace(response.CommonResponse) ? JsonConvert.DeserializeObject<QuoteResponseModel>(response.CommonResponse) : null;

                response.QuoteBaseRequest = !string.IsNullOrWhiteSpace(result.QuoteRequest) ? JsonConvert.DeserializeObject<QuoteBaseCommand>(result.QuoteRequest) : null;

                response.IDV = string.IsNullOrEmpty(response.IDV) ? Math.Round(Convert.ToDecimal(response.QuoteResponse?.IDV)).ToString() : Math.Round(Convert.ToDecimal(response.IDV)).ToString();
                response.MaxIDV = string.IsNullOrEmpty(response.MaxIDV) ? Math.Round(Convert.ToDecimal(response.QuoteResponse?.MaxIDV)).ToString() : Math.Round(Convert.ToDecimal(response.MaxIDV)).ToString();
                response.MinIDV = string.IsNullOrEmpty(response.MinIDV) ? Math.Round(Convert.ToDecimal(response.QuoteResponse?.MinIDV)).ToString() : Math.Round(Convert.ToDecimal(response.MinIDV)).ToString();

                response.TaxResponse = response.TaxResponse != null ? response.TaxResponse : response.QuoteResponse?.Tax;
                response.GrossPremium = string.IsNullOrEmpty(response.GrossPremium) ? response.QuoteResponse?.GrossPremium : response.GrossPremium;
                response.TotalPremium = string.IsNullOrEmpty(response.TotalPremium) ? response.QuoteResponse?.TotalPremium : response.TotalPremium;
                response.NCBPercentage = string.IsNullOrEmpty(response.NCBPercentage) ? response.QuoteResponse?.NCB : response.NCBPercentage;

                return HeroResult<GetLeadDetailsVm>.Success(response);
            }
            return HeroResult<GetLeadDetailsVm>.Fail("Data Not Found");
        }
    }
}

