using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.UserAddressDetail;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Commands.UserAddressDetail;

/// <summary>
/// Command for User Address Detail
/// </summary>
public record UserAddressDetailCommand : IRequest<HeroResult<bool>>
{
    /// <summary>
    /// User Id
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// AddressLine1
    /// </summary>
    public string AddressLine1 { get; set; }

    /// <summary>
    /// AddressLine2
    /// </summary>
    public string AddressLine2 { get; set; }

    /// <summary>
    /// Pincode
    /// </summary>
    public string Pincode { get; set; }

    /// <summary>
    /// CityId
    /// </summary>
    public string CityId { get; set; }

    /// <summary>
    /// StateId
    /// </summary>
    public string StateId { get; set; }


}

/// <summary>
/// Handler for UserAddressDetail
/// </summary>
public class UserAddressDetailCommandHandler : IRequestHandler<UserAddressDetailCommand, HeroResult<bool>>
{
    private readonly IUserRepository _userAddressDetailRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userAddressDetailRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserAddressDetailCommandHandler(IUserRepository userAddressDetailRepository, IMapper mapper)
    {
        _userAddressDetailRepository = userAddressDetailRepository ?? throw new ArgumentNullException(nameof(userAddressDetailRepository));
        _mapper = mapper;
    }

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<HeroResult<bool>> Handle(UserAddressDetailCommand userAddressDetailCommand, CancellationToken cancellationToken)
    {
        var userAddressDetailModel = _mapper.Map<UserAddressDetailModel>(userAddressDetailCommand);
        var result = await _userAddressDetailRepository.UpdateUserAddressDetail(userAddressDetailModel, cancellationToken);
        return HeroResult<bool>.Success(result);
    }
}
