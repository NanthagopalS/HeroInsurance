using Identity.Core.Contracts.Persistence;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.GetRejectedDocument;

public record GetRejectedDocumentQuery : IRequest<HeroResult<IEnumerable<UserDocumentDetailModel>>>
{
    public string UserId { get; set; }
}

public class GetRejectedDocumentQueryHandler : IRequestHandler<GetRejectedDocumentQuery, HeroResult<IEnumerable<UserDocumentDetailModel>>>
{
    private readonly IUserRepository _userRepository;

    public GetRejectedDocumentQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<HeroResult<IEnumerable<UserDocumentDetailModel>>> Handle(GetRejectedDocumentQuery request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetUserDocumentDetail(request.UserId, cancellationToken);
        if (result.Any())
        {
            return HeroResult<IEnumerable<UserDocumentDetailModel>>.Success(result);
        }

        return HeroResult<IEnumerable<UserDocumentDetailModel>>.Fail("No Record Found");
    }
}
