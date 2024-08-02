using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using Insurance.Domain.Quote;
using MediatR;

namespace Insurance.Core.Features.Quote.Query.GetCKYCField
{
    public record GetCKYCFieldsQuery : IRequest<HeroResult<List<ProposalFieldMasterModel>>>
    {
        public string InsurerID { get; set; }
        public bool IsPOI { get; set; }
        public bool IsCompany { get; set; }
        public bool IsDocumentUpload { get; set; }
    }
    public class GetCKYCFieldsQueryHandler : IRequestHandler<GetCKYCFieldsQuery, HeroResult<List<ProposalFieldMasterModel>>>
    {
        private readonly IQuoteRepository _quoteRepository;

        public GetCKYCFieldsQueryHandler(IQuoteRepository quoteRepository)
        {
            _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
        }

        public async Task<HeroResult<List<ProposalFieldMasterModel>>> Handle(GetCKYCFieldsQuery request, CancellationToken cancellationToken)
        {
            var result = await _quoteRepository.GetCKYCFields(request.InsurerID, request.IsPOI, request.IsCompany, request.IsDocumentUpload, cancellationToken);

            if (result != null && result.Any())
            {
                return HeroResult<List<ProposalFieldMasterModel>>.Success(result.ToList());
            }

            return HeroResult<List<ProposalFieldMasterModel>>.Fail("No record found");
        }
    }
}
