using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.Quote.Query.GetProposalField;
using Insurance.Core.Features.Quote.Query.GetProposalSummary;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;

namespace Insurance.Core.Features.Quote.Query.GetPaymentStatus
{
    public class GetPaymentStatusQuery : IRequest<HeroResult<PaymentDetailsVm>>
    {
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
    }
    public class GetPaymentStatusQueryHandler : IRequestHandler<GetPaymentStatusQuery, HeroResult<PaymentDetailsVm>>
    {
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;
        private readonly LogoConfig _logoConfig;

        public GetPaymentStatusQueryHandler(IMapper mapper, IQuoteRepository quoteRepository, IOptions<LogoConfig> options)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _logoConfig = options.Value;
        }

        public async Task<HeroResult<PaymentDetailsVm>> Handle(GetPaymentStatusQuery getPaymentStatusQuery, CancellationToken cancellationToken)
        {
            PaymentDetailsVm paymentDetailsVm = new PaymentDetailsVm();
            var paymentCKYCReqModel = _mapper.Map<PaymentStatusRequestModel>(getPaymentStatusQuery);
            var paymentCKCYResponseModel = await _quoteRepository.GetPaymentStatus(paymentCKYCReqModel, cancellationToken).ConfigureAwait(false);

            if(paymentCKCYResponseModel != null)
            {
                paymentDetailsVm = _mapper.Map<PaymentDetailsVm>(paymentCKCYResponseModel);
                paymentDetailsVm.Logo = _logoConfig.InsurerLogoURL + paymentDetailsVm.Logo;
                paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.OK;
                return HeroResult<PaymentDetailsVm>.Success(paymentDetailsVm);
            }
            paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.NotFound;
            return HeroResult<PaymentDetailsVm>.Fail("Data Not Found");
        }
    }
}

    


