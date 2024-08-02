using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.UnitedIndia.Queries.GetCKYCPOAStatus;
using Insurance.Core.Responses;
using Insurance.Domain.GoDigit;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.UnitedIndia.Queries.GetFinancierBranch;

public record GetFinancierBranchQuery : IRequest<HeroResult<IEnumerable<NameValueModel>>>
{
    public string financierCode { get; set; }
}
public class GetFinancierBranchQueryHandler : IRequestHandler<GetFinancierBranchQuery, HeroResult<IEnumerable<NameValueModel>>>
{
    private readonly IUnitedIndiaRepository _unitedIndiaRepository;
    public GetFinancierBranchQueryHandler(IUnitedIndiaRepository unitedIndiaRepository)
    {
        _unitedIndiaRepository = unitedIndiaRepository;
    }
    public async Task<HeroResult<IEnumerable<NameValueModel>>> Handle(GetFinancierBranchQuery request, CancellationToken cancellationToken)
    {
        var financierDetails = await _unitedIndiaRepository.GetFinancierBranchDetails(request.financierCode, cancellationToken);
        if(financierDetails != null) 
        {
            return HeroResult<IEnumerable<NameValueModel>>.Success(financierDetails);
        }
        return HeroResult<IEnumerable<NameValueModel>>.Fail("Fila to get financier Details");
        
    }
}
