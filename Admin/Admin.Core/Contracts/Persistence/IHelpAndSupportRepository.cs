using Admin.Domain.User;
using Admin.Domain.Roles;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Ocsp;
using Admin.Domain.Banners;
using Admin.Core.Features.HelpAndSupport.Queries.GetConcernType;
using Admin.Core.Features.HelpAndSupport.Queries.GetSubConcernType;
using Admin.Core.Features.User.Commands.InsertNotification;
using Admin.Core.Features.HelpAndSupport.RaiseRequest;
using Admin.Domain.HelpAndSupport;
using Microsoft.AspNetCore.Http;
using Admin.Core.Features.HelpAndSupport.InsertDeactivatePospDetails;

namespace Admin.Core.Contracts.Persistence;
public interface IHelpAndSupportRepository
{
    Task<IEnumerable<GetConcernTypeResponseModel>> GetConcernType(CancellationToken cancellationToken);
    Task<IEnumerable<GetSubConcernTypeResponseModel>> GetSubConcernType(string?  concernTypeId,CancellationToken cancellationToken);
    Task<GetAllHelpAndSupportResponseModel> GetAllHelpAndSupport(string? searchtext, string? UserId, string? startDate, string? endDate, int? currentpageIndex, int? currentpageSize, CancellationToken cancellationToken);

    Task<bool> InsertRaiseRequest(RaiseRequestCommand cmd, CancellationToken cancellationToken);
    Task<IEnumerable<ParticularHelpAndSupportModel>> GetParticularHelpAndSupport(string requestId, CancellationToken cancellationToken);
    Task<bool> DeleteHelpAndSupport(string requestId, CancellationToken cancellationToken);
    Task<bool> InsertDeactivatePospDetails(InsertDeactivatePospDetailsCommand command, CancellationToken cancellationToken);
}
