using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Chola;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Quote;
using MediatR;
using System.Net;

namespace Insurance.Core.Features.Chola.Queries.GetPaymentStatus
{
    public record GetCholaPaymentStatusQuery : IRequest<HeroResult<CholaPaymentDetailsVm>>
    {
        public string ApplicationId { get; set; }
    }
    public class GetCholaPaymentStatusQueryHandler : IRequestHandler<GetCholaPaymentStatusQuery, HeroResult<CholaPaymentDetailsVm>>
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly ICholaRepository _cholaRepository;

        public GetCholaPaymentStatusQueryHandler(IQuoteRepository quoteRepository, IMapper mapper, ICholaRepository cholaRepository)
        {
            _quoteRepository = quoteRepository;
            _mapper = mapper;
            _cholaRepository = cholaRepository;
        }

        public async Task<HeroResult<CholaPaymentDetailsVm>> Handle(GetCholaPaymentStatusQuery request, CancellationToken cancellationToken)
        {
            PaymentCKCYResponseModel paymentResponseModel = new PaymentCKCYResponseModel();
            CholaPaymentDetailsVm paymentDetailsVm = new CholaPaymentDetailsVm();
            var quoteResponse = _mapper.Map<QuoteResponseModel>(request);

            quoteResponse.PaymentStatus = "Payment Completed";
            quoteResponse.Type = "UPDATE";
            paymentResponseModel = await _quoteRepository.InsertPaymentTransaction(quoteResponse, cancellationToken).ConfigureAwait(false);

            if (paymentResponseModel != null)
            {
                paymentDetailsVm = _mapper.Map<CholaPaymentDetailsVm>(paymentResponseModel);
                paymentDetailsVm.ProposalNumber = request.ApplicationId;
                paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.OK;
                return HeroResult<CholaPaymentDetailsVm>.Success(paymentDetailsVm);
            }
            paymentDetailsVm.InsurerStatusCode = (int)HttpStatusCode.BadRequest;
            return HeroResult<CholaPaymentDetailsVm>.Fail("Failed to get the payment status");
        }
    }
}
