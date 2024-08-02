using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.User;
using Identity.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.Features.User.Queries.UserDocument;



/// <summary>
/// Query for Get Metals
/// </summary>
public record GetDocumentTypeQuery : IRequest<HeroResult<IEnumerable<UserDocumentTypeModel>>>
{
    public string UserId { get; set; }
}
internal class GetUserDocumentQueryHandler : IRequestHandler<GetDocumentTypeQuery, HeroResult<IEnumerable<UserDocumentTypeModel>>>
{
    private readonly IUserRepository _userBankDetailRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userBankDetailRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public GetUserDocumentQueryHandler(IUserRepository userBankDetailRepository, IMapper mapper)
    {
        _userBankDetailRepository = userBankDetailRepository ?? throw new ArgumentNullException(nameof(userBankDetailRepository));
        _mapper = mapper;
    }

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<HeroResult<IEnumerable<UserDocumentTypeModel>>> Handle(GetDocumentTypeQuery userBankDetailCommand, CancellationToken cancellationToken)
    {
        var result = await _userBankDetailRepository.GetUserDocumentType(userBankDetailCommand.UserId,cancellationToken).ConfigureAwait(false);

        return HeroResult<IEnumerable<UserDocumentTypeModel>>.Success(result);
    }
}

