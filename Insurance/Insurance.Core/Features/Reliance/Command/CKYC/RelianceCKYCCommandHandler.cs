using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.Reliance;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.Reliance.Command.CKYC
{
    public class RelianceCKYCCommand : CKYCModel, IRequest<HeroResult<SaveCKYCResponse>>
    {
        public string QuoteTransactionId { get; set; }
    }
    public class RelianceCKYCCommandHandler : IRequestHandler<RelianceCKYCCommand, HeroResult<SaveCKYCResponse>>
    {
        private readonly IRelianceRepository _relianceRepository;
        private readonly RelianceConfig _relianceConfig;
        private readonly IQuoteRepository _quoteRepository;
        public RelianceCKYCCommandHandler(IRelianceRepository relianceRepository, IQuoteRepository quoteRepository, IOptions<RelianceConfig> options)
        {
            _relianceRepository = relianceRepository;
            _quoteRepository = quoteRepository;
            _relianceConfig = options.Value;
        }
        public async Task<HeroResult<SaveCKYCResponse>> Handle(RelianceCKYCCommand request, CancellationToken cancellationToken)
        {
            // Get LeadId From QuoteTransaction Id
            var leadDetailsrequest = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
            {
                QuoteTransactionId = request.QuoteTransactionId,
                InsurerId = _relianceConfig.InsurerId
            };
            var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(leadDetailsrequest, cancellationToken);
            request.LeadId = leadDetails?.LeadID;
            request.TransactionId = leadDetails?.TransactionId;
            var response = await _relianceRepository.GetCKYCDetails(request, cancellationToken);
            if (response == null)
                return HeroResult<SaveCKYCResponse>.Fail("CKYC Failed");
            return HeroResult<SaveCKYCResponse>.Success(response);
        }
    }

}
