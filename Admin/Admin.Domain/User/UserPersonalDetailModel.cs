namespace Admin.Domain.UserPersonalDetail;
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
    /// DOB
    /// </summary>
    public string DOB { get; set; }

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

}
