namespace Identity.Domain.UserPersonalDetail;
public class UserPersonalDetailModel
{
    /// <summary>
    /// User Id
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Gender
    /// </summary>
    public string Gender { get; set; }

    /// <summary>
    /// FatherName
    /// </summary>
    public string FatherName { get; set; }

    /// <summary>
    /// DOB
    /// </summary>
    public string DOB { get; set; }
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
    public bool IsNameDifferentOnDocument { get; set; }

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
    public int Pincode { get; set; }


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
    /// POSP Source Mode
    /// </summary>
    public bool POSPSourceMode { get; set; }

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

    //public byte[] UserImage { get; set; }
    /// <summary>
    /// Userdocument
    /// </summary>
    public byte[] Userdocument { get; set; }


    public string DocumentId { get; set; }

    public string Image64 { get; set; }
    public string IsDraft { get; set; }
    public string InsuranceCompanyofInterestTypeId { get; set; }
    public string AssistedBUId { get; set; }
    public string CreatedBy { get; set; }
    public bool IsAdminUpdating { get; set; }
    //public string ICName { get; set; }
    //public string PremiumSold { get; set; }
    //public string PolicyTagged { get; set; }
    //public string StampNumber { get; set; }
    //public string RelationshipManagerId { get; set; }
    //public string SourcedBy { get; set; }
    //public string CreatedBy { get; set; }
    //public string ServicedBy { get; set; }
    //public string ProductTeam { get; set; }
    //public string OnboardingDate { get; set; }
    //public string PreSaleUserId { get; set; }
    //public string PostSaleUserId { get; set; }
    //public string MarketingUserId { get; set; }
    //public string ClaimUserId { get; set; }

}
