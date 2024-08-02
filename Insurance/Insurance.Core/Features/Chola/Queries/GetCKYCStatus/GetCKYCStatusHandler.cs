using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Quote.Query.GetCKYCField;
using Insurance.Core.Responses;
using Insurance.Domain.Chola;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ThirdPartyUtilities.Helpers;

namespace Insurance.Core.Features.Chola.Queries.GetCKYCStatus
{
    
    public record GetCholaCKYCStatusQuery : IRequest<HeroResult<string>>
    {
        public string TransactionID { get; set; }
        public string AppRefNo { get; set; }
        public string QuoteTransactionId { get; set; }
        public string LeadId { get; set; }
    }
    public class GetCKYCStatusHandler : IRequestHandler<GetCholaCKYCStatusQuery, HeroResult<string>>
    {
        private readonly ICholaRepository _cholaRepository;
        private readonly CholaConfig _cholaConfig;
        private readonly IQuoteRepository _quoteRepository;

        public GetCKYCStatusHandler(ICholaRepository cholaRepository, IOptions<CholaConfig> options, IQuoteRepository quoteRepository)
        {
            _cholaRepository = cholaRepository;
            _cholaConfig = options.Value;
            _quoteRepository = quoteRepository;
        }
        public async Task<HeroResult<string>> Handle(GetCholaCKYCStatusQuery cholaCKYCStatusQuery, CancellationToken cancellationToken)
        {

            var request = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
            {
                QuoteTransactionId = cholaCKYCStatusQuery.QuoteTransactionId,
                InsurerId = _cholaConfig.InsurerId
            };
            var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(request, cancellationToken);
            cholaCKYCStatusQuery.LeadId = leadDetails?.LeadID;

            var cKycStatusResponse = await _cholaRepository.GetCKYCStatus(cholaCKYCStatusQuery, cancellationToken);

            if (!string.IsNullOrEmpty(cKycStatusResponse.CreateLeadModel.CKYCstatus))
            {
                    CreateLeadModel createLeadModelObject = cKycStatusResponse.CreateLeadModel;
                    var response = await _quoteRepository.SaveLeadDetails(_cholaConfig.InsurerId, cholaCKYCStatusQuery.QuoteTransactionId, cKycStatusResponse.RequestBody, cKycStatusResponse.ResponseBody, "POI", createLeadModelObject, cancellationToken);
                return HeroResult<string>.Success(cKycStatusResponse.CreateLeadModel.CKYCstatus.ToString());
            }
            return HeroResult<string>.Fail("Failed");
        }
    }
}
