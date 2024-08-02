using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;

namespace Insurance.Core.Features.Quote.Query.GetPreviousCoverMaster
{
	public class GetPreviousCoverMasterQuery : PreviousCoverVm, IRequest<HeroResult<PreviousCoverVm>>
	{
		public string InsurerID { get; set; }
		public string VehicalTypeId { get; set; }
		public string PolicyTypeId { get; set; }
	}
	internal class GetPreviousCoverMasterQueryHandler : IRequestHandler<GetPreviousCoverMasterQuery, HeroResult<PreviousCoverVm>>
	{
		private readonly IQuoteRepository _quoteRepository;
		private readonly IMapper _mapper;

		public GetPreviousCoverMasterQueryHandler(IQuoteRepository quoteRepository, IMapper mapper)
		{
			_quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
			_mapper = mapper;
		}

		public async Task<HeroResult<PreviousCoverVm>> Handle(GetPreviousCoverMasterQuery request, CancellationToken cancellationToken)
		{
			var result = await _quoteRepository.GetPreviousCoverMaster(request.InsurerID, request.VehicalTypeId, request.PolicyTypeId, cancellationToken);


			if (result is not null )
			{
				var listCover = _mapper.Map<PreviousCoverVm>(result);
				return HeroResult<PreviousCoverVm>.Success(listCover);
			}
			else
				return HeroResult<PreviousCoverVm>.Fail("No record found");
		}

		
	}
	
}
