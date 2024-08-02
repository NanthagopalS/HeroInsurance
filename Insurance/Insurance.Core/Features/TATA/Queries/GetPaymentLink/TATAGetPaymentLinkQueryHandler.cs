using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using Insurance.Domain.TATA;
using MediatR;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.TATA.Queries.GetPaymentLink
{
    public class TATAGetPaymentLinkQuery : IRequest<HeroResult<string>>
    {
        public string InsurerId { get; set; }
        public string QuoteTransactionId { get; set; }
    }
    public class TATAGetPaymentLinkQueryHandler : IRequestHandler<TATAGetPaymentLinkQuery, HeroResult<string>>
    {
        private readonly IMapper _mapper;
        private readonly IQuoteRepository _quoteRepository;
        private readonly ITATARepository _tataRepository;
        private readonly TATAConfig _tataConfig;
        public TATAGetPaymentLinkQueryHandler(IMapper mapper, IQuoteRepository quoteRepository, ITATARepository tataRepository, IOptions<TATAConfig> options)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
            _tataRepository = tataRepository;
            _tataConfig = options.Value;
        }
        public async Task<HeroResult<string>> Handle(TATAGetPaymentLinkQuery request, CancellationToken cancellationToken)
        {
            var requestModel = _mapper.Map<PaymentStatusRequestModel>(request);
            var breakinPaymentDetails = await _quoteRepository.GetBreakInPaymentDetails(requestModel, cancellationToken);
            if (breakinPaymentDetails != null)
            {
                TATAPaymentRequestModel paymentRequestModel = new TATAPaymentRequestModel()
                {
                    QuoteTransactionId = request.QuoteTransactionId,
                    PAN = breakinPaymentDetails.PANNumber,
                    MobileNo = breakinPaymentDetails.PhoneNumber,
                    Email = breakinPaymentDetails.Email,
                    Name = breakinPaymentDetails.CustomerType.Equals("COMPANY") ? breakinPaymentDetails.CompanyName : breakinPaymentDetails.LeadFirstName + " " + breakinPaymentDetails.LeadLastName,
                    TATAPaymentId = breakinPaymentDetails.ApplicationId,
                    LeadId = breakinPaymentDetails.LeadId,
                    VehicleTypeId = breakinPaymentDetails.VehicleTypeId
                };

                var paymentResponse = await _tataRepository.GetPaymentLink(paymentRequestModel, cancellationToken);
                if (paymentResponse != null && paymentResponse.status.Equals(200) && !string.IsNullOrEmpty(paymentResponse.data?.paymentLink_web))
                {
                    var paymentLinkUpdate = await _quoteRepository.UpdateLeadPaymentLink(request.InsurerId, request.QuoteTransactionId, paymentResponse.data?.paymentLink_web, string.Empty, cancellationToken);
                    if (!string.IsNullOrEmpty(paymentLinkUpdate))
                    {
                        return HeroResult<string>.Success(paymentResponse.data?.paymentLink_web);
                    }
                }
            }
            return HeroResult<string>.Fail("Something went wrong, please try again");
        }
    }
}
