using AutoMapper;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.ICICI.Response;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Quote;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Core.Features.Quote.Command.SaveForLater;

public class SaveForLaterCommand : QuoteConfirmRequestModel, IRequest<HeroResult<string>>
{
}
public class SaveForLaterCommandHandler : IRequestHandler<SaveForLaterCommand, HeroResult<string>>
{
    private readonly IMapper _mapper;
    private readonly IQuoteRepository _quoteRepository;
    private readonly IApplicationClaims _applicationClaims;
    public SaveForLaterCommandHandler(IMapper mapper,
        IQuoteRepository quoteRepository,
        IApplicationClaims applicationClaims)
    {
        _mapper = mapper;
        _quoteRepository = quoteRepository;
        _applicationClaims = applicationClaims;
    }
    public async Task<HeroResult<string>> Handle(SaveForLaterCommand request, CancellationToken cancellationToken)
    {
        var commonResponse = new QuoteConfirmDetailsResponseModel();
        var quoteConfirmCommand = _mapper.Map<QuoteConfirmRequestModel>(request);
        quoteConfirmCommand.ManufacturingMonthYear = string.IsNullOrEmpty(quoteConfirmCommand.ManufacturingMonthYear) ? null : Convert.ToDateTime(quoteConfirmCommand.ManufacturingMonthYear).ToString("yyyy-MM-dd");
        quoteConfirmCommand.RegistrationDate = string.IsNullOrEmpty(quoteConfirmCommand.RegistrationDate) ? null : Convert.ToDateTime(quoteConfirmCommand.RegistrationDate).ToString("yyyy-MM-dd");
        quoteConfirmCommand.PolicyDates.PolicyStartDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.PolicyStartDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyStartDate).ToString("yyyy-MM-dd");
        quoteConfirmCommand.PolicyDates.PolicyEndDate = string.IsNullOrEmpty(quoteConfirmCommand.PolicyDates.PolicyEndDate) ? null : Convert.ToDateTime(quoteConfirmCommand.PolicyDates.PolicyEndDate).ToString("yyyy-MM-dd");
        var quoteconfirmModel = new QuoteConfirmDataModel();

        var quoteResponseDB = await _quoteRepository.GetQuoteConfirmDetailsDB(quoteConfirmCommand.QuoteTransactionId, quoteConfirmCommand, cancellationToken).ConfigureAwait(false);

        if (quoteResponseDB != null)
        {
            quoteconfirmModel = _mapper.Map<QuoteConfirmDataModel>(quoteConfirmCommand);
            quoteconfirmModel.ConfirmCommand = quoteConfirmCommand;
            quoteconfirmModel.UserId = _applicationClaims.GetUserId();
            quoteconfirmModel.Stage = "QuoteConfirm";

            string oldResponse = quoteResponseDB?.QuoteTransactionRequest.CommonResponse;
            if (oldResponse != null)
            {
                var tax = new ServiceTaxModel();
                QuoteResponseModel getOldResponse = JsonConvert.DeserializeObject<QuoteResponseModel>(oldResponse);
                if (getOldResponse.Tax != null)
                {
                    tax.cgst = getOldResponse.Tax.cgst;
                    tax.sgst = getOldResponse.Tax.sgst;
                    tax.igst = getOldResponse.Tax.igst;
                    tax.utgst = getOldResponse.Tax.utgst;
                    tax.totalTax = getOldResponse.Tax.totalTax;
                    tax.taxType = getOldResponse.Tax.taxType;
                }
                quoteconfirmModel.MinIDV = getOldResponse.MinIDV;
                quoteconfirmModel.MaxIDV = getOldResponse.MaxIDV;
                quoteconfirmModel.RecommendedIDV = getOldResponse.IDV;
                commonResponse.GrossPremium = getOldResponse.GrossPremium;
                commonResponse.TotalPremium = getOldResponse.TotalPremium;
                commonResponse.NCB = getOldResponse.NCB;
                commonResponse.Tax = tax;
                quoteconfirmModel.CommonResponse = JsonConvert.SerializeObject(commonResponse);
            }
            var insertDataResponse = await _quoteRepository.QuoteConfirmTransaction(quoteconfirmModel, cancellationToken).ConfigureAwait(false);

            if (insertDataResponse != null)
            {
                return HeroResult<string>.Success("Data stored successfully");
            }
            else
            {
                return HeroResult<string>.Fail("Fail to store data");
            }
        }
        return HeroResult<string>.Fail("Fail to fetch data from DB");
    }
}

