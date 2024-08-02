using Identity.Domain.Banner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Contracts.Persistence;

public interface IBannerRepository
{
    Task<bool> BannerUpload(BannerDetailModel bannerdetailmodel);
    Task<IEnumerable<BannerDetailModel>> GetBannerDetail(CancellationToken cancellationToken);
}

