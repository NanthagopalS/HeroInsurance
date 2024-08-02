using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.UserInquiryDetail;
using Identity.Core.Responses;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Features.User.Commands.UserInquiryDetail;

/// <summary>
/// Command for UserInquiryDetail
/// </summary>
public record UserInquiryDetailCommand : IRequest<HeroResult<bool>>
{
    /// <summary>
    /// User Id
    /// </summary>
    //[Required]
    public string UserName { get; set; }

    /// <summary>
    /// Gender
    /// </summary>
    //[Required]
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Alternate Contact No
    /// </summary>
  
    public string InquiryDescription { get; set; }
}

/// <summary>
/// Handler for UserInquiryDetail
/// </summary>
public class UserInquiryDetailCommandHandler : IRequestHandler<UserInquiryDetailCommand, HeroResult<bool>>
{
    private readonly IUserRepository _userInquiryDetailRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userInquiryDetailRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserInquiryDetailCommandHandler(IUserRepository userInquiryDetailRepository, IMapper mapper)
    {
        _userInquiryDetailRepository = userInquiryDetailRepository ?? throw new ArgumentNullException(nameof(userInquiryDetailRepository));
        _mapper = mapper;
    }

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<HeroResult<bool>> Handle(UserInquiryDetailCommand userInquiryDetailCommand, CancellationToken cancellationToken)
    {
        var userInquiryDetailModel = _mapper.Map<UserInquiryDetailModel>(userInquiryDetailCommand);
        var result = await _userInquiryDetailRepository.InsertUserInquiryDetail(userInquiryDetailModel);

        return HeroResult<bool>.Success(result);
    }
}
