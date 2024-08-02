namespace Insurance.Core.Contracts.Common;

public interface IApplicationClaims
{
    string EmailId();
    string GetAadhaarNumber();
    string GetDOB();
    string GetMobileNo();
    string GetPAN();
    string GetPOSPId();
    string GetUserId();
    string GetUserName();
    string GetLocation();
    string GetRole();
}