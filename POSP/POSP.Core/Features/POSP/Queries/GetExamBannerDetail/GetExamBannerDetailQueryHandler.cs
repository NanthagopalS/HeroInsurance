using AutoMapper;
using MediatR;
using POSP.Core.Contracts.Persistence;
using POSP.Core.Responses;
using POSP.Domain.POSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Core.Features.POSP.Queries.GetExamBannerDetail;

public record GetExamBannerDetailQuery : IRequest<HeroResult<IEnumerable<GetExamBannerDetailVm>>>;

public class GetExamBannerDetailQueryHandler : IRequestHandler<GetExamBannerDetailQuery, HeroResult<IEnumerable<GetExamBannerDetailVm>>>
{ 

    private readonly IPOSPRepository _posprepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userBankDetailRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GetExamBannerDetailQueryHandler(IPOSPRepository posprepository, IMapper mapper)
    { 
    _posprepository = posprepository ?? throw new ArgumentNullException(nameof(posprepository));
        _mapper = mapper;
    }

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<HeroResult<IEnumerable<GetExamBannerDetailVm>>> Handle(GetExamBannerDetailQuery getbannerdetailquery, CancellationToken cancellationToken)
    {
        var result = await _posprepository.GetExamBannerDetail(cancellationToken).ConfigureAwait(false);

        if (result.Any())
        {
            var listInsurer = _mapper.Map<IEnumerable<GetExamBannerDetailVm>>(result);
            return HeroResult<IEnumerable<GetExamBannerDetailVm>>.Success(listInsurer);
        }

        return HeroResult<IEnumerable<GetExamBannerDetailVm>>.Fail("No Record Found");

   
    }
}
