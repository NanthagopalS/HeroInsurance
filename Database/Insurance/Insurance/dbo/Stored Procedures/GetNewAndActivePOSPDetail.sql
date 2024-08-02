CREATE  PROCEDURE [dbo].[GetNewAndActivePOSPDetail]
AS                  
BEGIN
	SELECT IU.UserId,IU.UserName, IU.EmailId, IU.MobileNo, IU.RoleId,        
	IU.POSPLeadId, IU.POSPId, IU.UserProfileStage,IR.RoleName,  IU.CreatedBy as CreatedById,         
    CASE WHEN IUE.IsVerify IS NULL THEN 0               
    ELSE IUE.IsVerify                   
    END AS IsEmailVerified , IU.IsActive                  
    FROM HeroIdentity.dbo.Identity_User IU WITH(NOLOCK)                 
    LEFT JOIN HeroIdentity.dbo.Identity_EmailVerification IUE WITH(NOLOCK) ON IUE.UserId = IU.UserId AND IUE.IsActive = 1         
	LEFT JOIN HeroIdentity.dbo.Identity_RoleMaster IR WITH(NOLOCK) ON IR.RoleId = IU.RoleId


END