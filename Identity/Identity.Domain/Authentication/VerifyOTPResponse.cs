namespace Identity.Domain.Authentication;
public class VerifyOTPResponse
{
    public bool IsValidOTP { get; set; }
    public string  MobileNumber { get; set; }
    public int UserProfileStage { get; set; }
    public string wrongOtpCount { get; set; }
}

public class VerifyAdminUserResponse
{
    public string EmailId { get; set; }
    public string HashPassword { get; set; }
    public string RoleId { get; set; }
    public string RoleName { get; set; }
}