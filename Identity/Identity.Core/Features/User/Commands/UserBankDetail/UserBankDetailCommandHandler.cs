using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.User;
using Identity.Domain.UserBankDetail;
using Identity.Core.Responses;
using MediatR;

namespace Identity.Core.Features.User.Commands.UserBankDetail;

/// <summary>
/// Command for User Bank Detail
/// </summary>
public record UserBankDetailCommand : IRequest<HeroResult<UserBankDetailUpdateResponce>>
{
    /// <summary>
    /// User Id
    /// </summary>
    //[Required]
    public string UserId { get; set; }

    /// <summary>
    /// Bank Id
    /// </summary>
    //[Required]
    public string? BankId { get; set; }

    /// <summary>
    /// IFSC Code
    /// </summary>
    //[Required]
    public string? IFSC { get; set; }

    /// <summary>
    /// AccountHolderName
    /// </summary>
    //[Required]
    public string? AccountHolderName { get; set; }

    /// <summary>
    /// AccountNumber
    /// </summary>
    //[Required]
    public string? AccountNumber { get; set; }
    /// <summary>
    /// IsDraft
    /// </summary>
    //[Required]
    public bool IsDraft { get; set; } = false;
    public bool IsAdminUpdating { get; set; } = false;

}

/// <summary>
/// Handler for UserBankDetail
/// </summary>
public class UserBankDetailCommandHandler : IRequestHandler<UserBankDetailCommand, HeroResult<UserBankDetailUpdateResponce>>
{
    private readonly IUserRepository _userBankDetailRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userBankDetailRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserBankDetailCommandHandler(IUserRepository userBankDetailRepository, IMapper mapper)
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
    public async Task<HeroResult<UserBankDetailUpdateResponce>> Handle(UserBankDetailCommand userBankDetailCommand, CancellationToken cancellationToken)
    {
        var userBankDetailModel = _mapper.Map<UserBankDetailModel>(userBankDetailCommand);
        var result = await _userBankDetailRepository.UpdateUserBankDetail(userBankDetailModel, cancellationToken);

        return HeroResult<UserBankDetailUpdateResponce>.Success(result);
        //if(result.Equals("Bank Verification Succesfull"))
        //{
        //    return HeroResult<string>.Success(result);
        //}
        //return HeroResult<string>.Fail(result);
    }
}
