namespace Admin.Domain.UserLogin;
public class UserLoginModel
{
    /// <summary>
    /// Mobile No
    /// </summary>
    public string MobileNo { get; set; }
}

//public class UserLoginResponseModel
//{
//    public string UserId { get; set; }
//    public bool IsUserExists { get; set; }
//}

public class UserLoginResponseModel
{
    public string UserId { get; set; }
    public bool IsUserExists { get; set; }
    public int WrongOtpCount { get; set; }
    public bool IsAccountLock { get; set; }
}
