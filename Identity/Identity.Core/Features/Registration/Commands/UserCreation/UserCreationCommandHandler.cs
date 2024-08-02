using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.UserCreation;
using Identity.Core.Responses;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Features.User.Commands.UserCreation;

/// <summary>
/// Command for UserCreation
/// </summary>
public record UserCreationCommand : IRequest<HeroResult<UserCreateResponseModel>>
{
    /// <summary>
    /// User Name
    /// </summary>
    //[Required] 
    public string UserName { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    //[Required] 
    public string EmailId { get; set; }

    /// <summary>
    /// Mobile No
    /// </summary>
    //[Required] 
    public string MobileNo { get; set; }

    /// <summary>
    /// BackOfficeUserId
    /// </summary>
    //[Required] 
    public string BackOfficeUserId { get; set; }
    public string Environment { get; set; }
    public string ReferralUserId { get; set; }

}

/// <summary>
/// Handler for UserCreation
/// </summary>
public class UserCreationCommandHandler : IRequestHandler<UserCreationCommand, HeroResult<UserCreateResponseModel>>
{
    private readonly IUserRepository _userCreationRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userCreationRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserCreationCommandHandler(IUserRepository userCreationRepository, IMapper mapper)
    {
        _userCreationRepository = userCreationRepository ?? throw new ArgumentNullException(nameof(userCreationRepository));
        _mapper = mapper;
    }

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<HeroResult<UserCreateResponseModel>> Handle(UserCreationCommand userCreationCommand, CancellationToken cancellationToken)
    {
        var userCreationModel = _mapper.Map<UserCreationModel>(userCreationCommand);
        var result = await _userCreationRepository.InsertUserCreationDetail(userCreationModel);
        if (result != null)
        {
            return HeroResult<UserCreateResponseModel>.Success(result);
        }
        return HeroResult<UserCreateResponseModel>.Fail("Failed to create user");
    }
}
