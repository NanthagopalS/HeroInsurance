namespace Identity.Domain.Authentication;

/// <summary>
/// Authentication Response
/// </summary>
public class AuthenticationResponse
{
    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; }   
    


    /// <summary>
    /// UserId
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// RoleId
    /// </summary>
    public string RoleId { get; set; }

    /// <summary>
    /// RoleName
    /// </summary>
    public string RoleName { get; set; }

    /// <summary>
    /// FullName
    /// </summary>
    public string FullName { get; set; }

    /// <summary>
    /// Menu Details
    /// </summary>
    public IEnumerable<PageAccessModel> LstPageAccess { get; set; }

    public int UserProfileStage { get; set; }
    public int WrongOtpCount { get; set; }
    public bool IsAccountLock { get; set; }

}

/// <summary>
/// Menu details
/// </summary>
public class PageAccessModel
{
    /// <summary>
    /// Link Name
    /// </summary>
    public string LinkName { get; set; }

    /// <summary>
    /// Page Name
    /// </summary>
    public string PageName { get; set; }

    /// <summary>
    /// Parent Link Id
    /// </summary>
    public int ParentLinkId { get; set; }

    /// <summary>
    /// PagelinkId
    /// </summary>
    public int PageLinkId { get; set; }

    /// <summary>
    /// Icons
    /// </summary>
    public string Icons { get; set; }

    /// <summary>
    /// LinkPageId
    /// </summary>
    public string LinkPageId { get; set; }
}