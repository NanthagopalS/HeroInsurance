using Admin.Domain.User;
using Admin.Domain.Roles;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Ocsp;
using Admin.Domain.Banners;

namespace Admin.Core.Contracts.Persistence;
public interface IBannerRepository
{
    Task<IEnumerable<GetBannerDetailModel>> GetBannerDetail(CancellationToken cancellationToken);


}
