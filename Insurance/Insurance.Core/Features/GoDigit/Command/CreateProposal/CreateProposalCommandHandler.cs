using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.GoDigit.Command.CreateProposal
{
    public class GodigitCreateProposalCommand : ProposalRequestModel, IRequest<HeroResult<QuoteResponseModel>>
    {

    }
    public class CreateProposalCommandHandler : IRequestHandler<GodigitCreateProposalCommand, HeroResult<QuoteResponseModel>>
    {
        private readonly IGoDigitRepository _goDigitRepository;
        private readonly IQuoteRepository _quoteRepository;
        private readonly IMapper _mapper;
        private readonly GoDigitConfig _goDigitConfig;

        public CreateProposalCommandHandler(IMapper mapper, IGoDigitRepository goDigitRepository, IQuoteRepository quoteRepository, GoDigitConfig goDigitConfig)
        {
            _goDigitRepository = goDigitRepository;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _quoteRepository = quoteRepository;
            _goDigitConfig = goDigitConfig;
        }
        public async Task<HeroResult<QuoteResponseModel>> Handle(GodigitCreateProposalCommand request, CancellationToken cancellationToken)
        {
            var createLeadModel = _mapper.Map<ProposalRequestModel>(request);
            var proposalResponse = await _goDigitRepository.CreateProposal(createLeadModel, cancellationToken).ConfigureAwait(false);
            
            if (proposalResponse != null && proposalResponse.InsurerStatusCode == 200)
            {
                var proposalResponseModel = _mapper.Map<QuoteResponseModel>(proposalResponse);
                proposalResponseModel.Type = "INSERT";
                var resInsertPayment = await _quoteRepository.InsertPaymentTransaction(proposalResponseModel, cancellationToken).ConfigureAwait(false);
                string cancelURL = _goDigitConfig.PGCancelURL + proposalResponseModel.ApplicationId;
                string successURL = _goDigitConfig.PGSuccessURL + proposalResponseModel.ApplicationId;

                if (resInsertPayment != null)
                {
                    var paymentURLResponse = await _goDigitRepository.CreatePaymentLink(proposalResponseModel.ApplicationId, cancelURL, successURL, cancellationToken).ConfigureAwait(false);
                    if (paymentURLResponse != null)
                    {
                        proposalResponse.PaymentURLLink = paymentURLResponse.PaymentURL;
                        return HeroResult<QuoteResponseModel>.Success(proposalResponse);
                    }
                    return HeroResult<QuoteResponseModel>.Fail("No Record Found");
                }
                return HeroResult<QuoteResponseModel>.Fail("No Record Found");
            }
            return HeroResult<QuoteResponseModel>.Fail("No Record Found");
        }
    }
}
