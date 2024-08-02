using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using Insurance.Domain.UnitedIndia;
using MediatR;
using Microsoft.Extensions.Options;

namespace Insurance.Core.Features.UnitedIndia.Command
{
    public class UnitedIndiaCkycCommand : CKYCModel, IRequest<HeroResult<SaveCKYCResponse>>
    {
        public string QuoteTransactionId { get; set; }
    }
    public class UnitedIndiaCkycCommandHandler : IRequestHandler<UnitedIndiaCkycCommand, HeroResult<SaveCKYCResponse>>
    {
        private readonly IUnitedIndiaRepository _unitedIndiaRepository;
        private readonly UnitedIndiaConfig _unitedIndiaConfig;
        private readonly IQuoteRepository _quoteRepository;
        public UnitedIndiaCkycCommandHandler(IUnitedIndiaRepository unitedIndiaRepository, IQuoteRepository quoteRepository, IOptions<UnitedIndiaConfig> options)
        {
            _unitedIndiaRepository = unitedIndiaRepository;
            _quoteRepository = quoteRepository;
            _unitedIndiaConfig = options.Value;
        }
        public async Task<HeroResult<SaveCKYCResponse>> Handle(UnitedIndiaCkycCommand request, CancellationToken cancellationToken)
        {
            // Get LeadId From QuoteTransaction Id
            var leadDetailsrequest = new GetLeadDetailsByApplicationOrQuoteTransactionIdModel()
            {
                QuoteTransactionId = request.QuoteTransactionId,
                InsurerId = _unitedIndiaConfig.InsurerId
            };
            var leadDetails = await _quoteRepository.GetLeadDetailsByApplicationIdOrQuoteTransactionId(leadDetailsrequest, cancellationToken);
            request.LeadId = leadDetails?.LeadID;

            //var ckycResponse = await _unitedIndiaRepository.SaveCKYC(request, cancellationToken);

            //var dataSaveResponse = await _quoteRepository.SaveLeadDetails(_unitedIndiaConfig.InsurerId, request.QuoteTransactionId, ckycResponse.Item1, ckycResponse.Item2, "POI", ckycResponse.Item4, cancellationToken);

            //SaveCKYCResponse saveCKYCResponse = ckycResponse.Item3;
            //saveCKYCResponse.LeadID = dataSaveResponse.LeadID;
            //saveCKYCResponse.CKYCNumber = dataSaveResponse.CKYCNumber;
            //saveCKYCResponse.KYCId = dataSaveResponse.KYCId;

            //if (ckycResponse is null)
            //    return HeroResult<SaveCKYCResponse>.Fail("CKYC Failed");

            //return HeroResult<SaveCKYCResponse>.Success(saveCKYCResponse);
            return default;
        }
    }
}
