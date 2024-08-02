using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Models;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.ICICI.Queries.ConfirmDetails;
//public class GodigitQuoteConfirmCommand : QuoteConfirmRequestModel, IRequest<HeroResult<QuoteConfirmDetailsResponseModel>>
public class QuoteConfirmCommand : QuoteConfirmRequestModel, IRequest<HeroResult<QuoteConfirmDetailsResponseModel>>
{
    //public string QuoteTransactionId { get; set; }
    //public string VehicleNumber { get; set; }
    //public string ManufacturingMonthYear { get; set; }
    //public string RegistrationDate { get; set; }
    //public string Customertype { get; set; }
    //public PreviousPolicyModel PreviousPolicy { get; set; }
    //public bool IsPACover { get; set; }
    //public string PACoverTenure { get; set; }
    //public bool IsHavePACover { get; set; }
    //public PolicyDatesResponse PolicyDates { get; set; }
    //public string VehicleTypeId { get; set; }
    //public bool IsBrandNewVehicle { get; set; }
    //public string CompanyName { get; set; }
    //public string DOI { get; set; }
    //public string GSTNo { get; set; }
}
public class QuoteConfirmCommandHandler : IRequestHandler<QuoteConfirmCommand, HeroResult<QuoteConfirmDetailsResponseModel>>
{
    private readonly IQuoteRepository _quoteRepository;
    private readonly IMapper _mapper;
    private readonly IGoDigitRepository _goDigitRepository;
    public QuoteConfirmCommandHandler(IQuoteRepository quoteRepository, IMapper mapper, IGoDigitRepository goDigitRepository)
    {
        _quoteRepository = quoteRepository;
        _mapper = mapper;
        _goDigitRepository = goDigitRepository;
    }

    public async Task<HeroResult<QuoteConfirmDetailsResponseModel>> Handle(QuoteConfirmCommand request, CancellationToken cancellationToken)
    {
        var quoteConfirmCommand = _mapper.Map<QuoteConfirmRequestModel>(request);
        var quoteResponse = await _quoteRepository.GetQuoteConfirmDetails(quoteConfirmCommand.QuoteTransactionId, quoteConfirmCommand, cancellationToken).ConfigureAwait(false);
        if (quoteResponse != null)
        {
            await _goDigitRepository.QuoteTransaction(quoteResponse.Item2, quoteResponse.Item3, quoteResponse.Item4, "Quote", quoteResponse.Item1.InsurerId, quoteResponse.Item5, Convert.ToDecimal(quoteResponse.Item2.MaxIDV), Convert.ToDecimal(quoteResponse.Item2.MinIDV), Convert.ToDecimal(quoteResponse.Item2.IDV), quoteResponse.Item6);
            quoteResponse.Item1.Logo = quoteResponse.Item2.InsurerLogo;
            quoteResponse.Item1.TransactionId = quoteResponse.Item2.TransactionID;
            return HeroResult<QuoteConfirmDetailsResponseModel>.Success(quoteResponse.Item1);
        }
        return HeroResult<QuoteConfirmDetailsResponseModel>.Fail(quoteResponse.Item1.ValidationMessage);
    }
}
