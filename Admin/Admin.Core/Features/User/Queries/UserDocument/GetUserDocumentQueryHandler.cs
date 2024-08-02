using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.User;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.UserDocument;



/// <summary>
/// Query for Get Metals
/// </summary>
public record GetDocumentTypeQuery : IRequest<HeroResult<IEnumerable<UserDocumentTypeModel>>>;
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
        var result = await _userBankDetailRepository.GetUserDocumentType(cancellationToken).ConfigureAwait(false);

        return HeroResult<IEnumerable<UserDocumentTypeModel>>.Success(result);
    }
}

