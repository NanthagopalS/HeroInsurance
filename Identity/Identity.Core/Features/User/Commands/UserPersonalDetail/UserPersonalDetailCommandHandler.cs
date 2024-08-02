using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Domain.UserPersonalDetail;
using Identity.Core.Responses;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Features.User.Commands.UserPersonalDetail;

/// <summary>
/// Command for UserPersonalDetail
/// </summary>
public record UserPersonalDetailCommand : IRequest<HeroResult<bool>>
{
    /// <summary>
    /// User Id
    /// </summary>
    //[Required]
    public string UserId { get; set; }


    /// <summary>
    /// DOB
    /// </summary>
    public string DOB { get; set; }


    /// <summary>
    /// Gender
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// FatherName
    /// </summary>
    public string FatherName { get; set; }

    /// <summary>
    /// FatherName
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Alternate Contact No
    /// </summary>
    public string AlternateContactNo { get; set; }

    /// <summary>
    /// Aadhaar Number
    /// </summary>
    public string AadhaarNumber { get; set; }


    /// <summary>
    /// Is Name Different On Document
    /// </summary>
    public bool? IsNameDifferentOnDocument { get; set; }

    /// <summary>
    /// Name Different On Document
    /// </summary>
    public string NameDifferentOnDocument { get; set; }

    /// <summary>
    /// Name Different On Document File Path
    /// </summary>
    public string NameDifferentOnDocumentFilePath { get; set; }

    /// <summary>
    /// Alias Name
    /// </summary>
    public string AliasName { get; set; }

    /// <summary>
    /// Address Line1
    /// </summary>
    public string AddressLine1 { get; set; }

    /// <summary>
    /// Address Line2
    /// </summary>
    public string AddressLine2 { get; set; }

    /// <summary>
    /// Pincode
    /// </summary>
    public int? Pincode { get; set; }


    /// <summary>
    /// City Id
    /// </summary>
    public string CityId { get; set; }

    /// <summary>
    /// State Id
    /// </summary>
    public string StateId { get; set; }

    /// <summary>
    /// Education Qualification Type Id
    /// </summary>
    public string EducationQualificationTypeId { get; set; }

    /// <summary>
    /// Insurance Selling Experience Range Id
    /// </summary>
    public string InsuranceSellingExperienceRangeId { get; set; }

    /// <summary>
    /// Insurance Products of Interest Type Id
    /// </summary>
    public string InsuranceProductsofInterestTypeId { get; set; }

    /// <summary>
    /// Insurance Products of Interest Type Id
    /// </summary>
    public string InsuranceCompanyofInterestTypeId { get; set; }

    /// <summary>
    /// POSP Source Mode
    /// </summary>
    public bool? POSPSourceMode { get; set; }

    /// <summary>
    /// POSP Source Type Id
    /// </summary>
    public string POSPSourceTypeId { get; set; }

    /// <summary>
    /// Sourced By User Id
    /// </summary>
    public string SourcedByUserId { get; set; }

    /// <summary>
    /// Serviced By User Id
    /// </summary>
    public string ServicedByUserId { get; set; }

    /// <summary>
    /// NOC Available
    /// </summary>
    public string NOCAvailable { get; set; }


    /// <summary>
    /// Is Selling
    /// </summary>
    public string IsSelling { get; set; }

    /// <summary>
    /// Userdocument
    /// </summary>
    public byte[] Userdocument { get; set; }
    public bool? IsDraft { get; set; }
    public string? AssistedBUId { get; set; }
    public string? CreatedBy { get; set; }
    public bool? IsAdminUpdating { get; set; } = false;
   // public string ICName { get; set; }
   // public string PremiumSold { get; set; }
    //public string PolicyTagged { get; set; }
    //public string StampNumber { get; set; }
    //public string RelationshipManagerId { get; set; }
    //public string SourcedBy { get; set; }
    //public string CreatedBy { get; set; }
    //public string ServicedBy { get; set; }
    //public string ProductTeam { get; set; }
    //public string Tagging { get; set; }
    //public string OnboardingDate { get; set; }
    //public string PreSaleUserId { get; set; }
    //public string PostSaleUserId { get; set; }
    //public string MarketingUserId { get; set; }
    //public string ClaimUserId { get; set; }

}

/// <summary>
/// Handler for UserPersonalDetail
/// </summary>
public class UserPersonalDetailCommandHandler : IRequestHandler<UserPersonalDetailCommand, HeroResult<bool>>
{
    private readonly IUserRepository _userPersonalDetailRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="userPersonalDetailRepository"></param>
    /// <param name="mapper"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserPersonalDetailCommandHandler(IUserRepository userPersonalDetailRepository, IMapper mapper)
    {
        _userPersonalDetailRepository = userPersonalDetailRepository ?? throw new ArgumentNullException(nameof(userPersonalDetailRepository));
        _mapper = mapper;
    }

    /// <summary>
    /// Handler
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<HeroResult<bool>> Handle(UserPersonalDetailCommand userPersonalDetailCommand, CancellationToken cancellationToken)
    {
        var userPersonalDetailModel = _mapper.Map<UserPersonalDetailModel>(userPersonalDetailCommand);
        var result = await _userPersonalDetailRepository.UpdateUserPersonalDetail(userPersonalDetailModel);

        return HeroResult<bool>.Success(result);
    }
}
