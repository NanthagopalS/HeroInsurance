using System.ComponentModel.DataAnnotations;

namespace Admin.Domain.UserCreation;
public class UserCreationModel
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
    public string MobileNo { get; set; }
}

public class UserCreateResponseModel
{
    public string UserId { get; set; }
    public bool IsUserExists { get; set; }
}