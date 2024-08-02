using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.Banners.Queries.GetBannerDetail
{
    public class GetBannerDetailQuery : IRequest<HeroResult<IEnumerable<GetBannerDetailVm>>>
    {
    }
    public class GetBannerDetailQueryHandler : IRequestHandler<GetBannerDetailQuery, HeroResult<IEnumerable<GetBannerDetailVm>>>
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IMapper _mapper;
        public GetBannerDetailQueryHandler(IBannerRepository bannerRepository, IMapper mapper)
        {
            _bannerRepository = bannerRepository ?? throw new ArgumentNullException(nameof(bannerRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<HeroResult<IEnumerable<GetBannerDetailVm>>> Handle(GetBannerDetailQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _bannerRepository.GetBannerDetail(cancellationToken);
            if (modelResult.Any())
            {
                var listInsurerModel = _mapper.Map<IEnumerable<GetBannerDetailVm>>(modelResult);
                return HeroResult<IEnumerable<GetBannerDetailVm>>.Success(listInsurerModel);
            }
            return HeroResult<IEnumerable<GetBannerDetailVm>>.Fail("No Record Found");
        }
    }
}
