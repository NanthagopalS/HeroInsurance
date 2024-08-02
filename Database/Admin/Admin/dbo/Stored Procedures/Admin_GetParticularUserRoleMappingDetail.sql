      
-- [HeroAdmin].[dbo].[Admin_GetParticularUserRoleMappingDetail] 'AF9EC336-E48C-4EA2-AD8B-D448216DFC2E'
              
 CREATE PROCEDURE [dbo].[Admin_GetParticularUserRoleMappingDetail]             
 (            
 @UserRoleMappingId VARCHAR(100) = NULL      
 )            
AS      
      
BEGIN            
 BEGIN TRY            
          
 SELECT TOP 1
 CASE WHEN HEADBu.BUId IS NULL THEN 0 ELSE 1 END as IsHead,     
 CASE WHEN HEADBu.BUId IS NULL THEN ISNULL(bu.BUName,'') ELSE ISNULL(HEADBu.BUName,'') END as DisplayBUName,     
 HEADBu.BUId  as HEADBuId,HEADBu.BUName as HEADBuName,
 urm.UserRoleMappingId, urm.UserId, iu.[Password], urm.RoleTypeId, rt.RoleTypeName, urm.BuId, bu.BUName,
 urm.RoleId, rm.RoleName, urm.ReportingIdentityRoleId, irm.RoleName as ReportingIdentityRoleName, urm.ReportingUserId,     
 iru.UserName as ReportingUserName, urm.CategoryId, uc.UserCategoryName as CategoryName, urm.CreatedBy,      
 iu.UserName, iu.EmpId, iu.MobileNo, iu.EmailId, iu.Gender, iu.DOB, iud.ProfilePictureId,      
 iud.DocumentId, urm.IsActive as IsActiveStatus, CASE WHEN urm.IsActive = 1 THEN 'ACTIVE' ELSE 'DEACTIVATED' END as StatusName    
 FROM [HeroAdmin].[dbo].[Admin_UserRoleMapping] urm WITH(NOLOCK)     
  LEFT JOIN [HeroAdmin].[dbo].[Admin_RoleType] rt WITH(NOLOCK) ON rt.RoleTypeID = urm.RoleTypeId      
  LEFT JOIN [HeroAdmin].[dbo].[Admin_BU] bu WITH(NOLOCK) ON bu.BUId = urm.BUId      
  LEFT JOIN [HeroAdmin].[dbo].[Admin_RoleMaster] rm WITH(NOLOCK) ON rm.RoleId = urm.RoleId      
  LEFT JOIN [HeroAdmin].[dbo].[Admin_RoleMaster] irm WITH(NOLOCK) ON irm.RoleId = urm.ReportingIdentityRoleId      
  LEFT JOIN [HeroIdentity].[dbo].[Identity_User] iu WITH(NOLOCK) ON iu.UserId = urm.UserId      
  LEFT JOIN [HeroIdentity].[dbo].[Identity_User] iru WITH(NOLOCK) ON iru.UserId = urm.ReportingUserId      
  LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] iud WITH(NOLOCK) ON iud.UserId = urm.UserId      
  LEFT JOIN [HeroAdmin].[dbo].[Admin_UserCategory] uc WITH(NOLOCK) ON uc.UserCategoryId = urm.CategoryId    
  LEFT JOIN [HeroAdmin].[dbo].[Admin_BU] HEADBu WITH(NOLOCK) ON HEADBu.BUHeadId = urm.UserId
 WHERE urm.UserRoleMappingId = @UserRoleMappingId      
 ORDER BY urm.CreatedOn DESC      
      
 END TRY                            
 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                         
 END CATCH            
         
END 
