using Google.Apis.Http;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.Chola.Command.CKYC;
using Insurance.Core.Responses;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;

namespace Insurance.Core.Features.IFFCO.Command.CKYC
{
    public class IFFCOCKYCCommand : CKYCModel, IRequest<HeroResult<SaveCKYCResponse>>
    {
        public string QuoteTransactionId { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string LeadId { get; set; }
        public string VehicleTypeId { get; set; }
    }

    public class IFFCOCKYCCommandHandler : IRequestHandler<IFFCOCKYCCommand, HeroResult<SaveCKYCResponse>>
    {
        private readonly IIFFCORepository _iFFCORepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IFFCOConfig _iFFCOConfig;
        public IFFCOCKYCCommandHandler(IIFFCORepository iFFCORepository,IOptions<IFFCOConfig> option, IQuoteRepository quoteRepository)
        {
            _iFFCORepository = iFFCORepository;
            _iFFCOConfig = option.Value;
            _quoteRepository = quoteRepository;
        }
        public async Task<HeroResult<SaveCKYCResponse>> Handle(IFFCOCKYCCommand request, CancellationToken cancellationToken)
        {
            var leadDetailsrequest = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
            {
                QuoteTransactionId = request.QuoteTransactionId,
                InsurerId = _iFFCOConfig.InsurerId
            };
            var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(leadDetailsrequest, cancellationToken);
            request.LeadId = leadDetails?.LeadID;
            request.VehicleTypeId = leadDetails?.VehicleTypeId;

            var response = await _iFFCORepository.GetCKYCDetails(request, cancellationToken);
            if (response == null)
            {
                return HeroResult<SaveCKYCResponse>.Fail("CKYC Failed");
            }
            return HeroResult<SaveCKYCResponse>.Success(response);
        }
    }
}
