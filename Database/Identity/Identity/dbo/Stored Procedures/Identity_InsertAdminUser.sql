    
-- =============================================    
-- Author:  <Author, Girish>    
-- Create date: <Create Date,12-Dec-2022>    
-- Description: <Description,INSERT Admin>    
-- Identity_InsertLogUser 'GirishSF@Mantralabsglobal.com','XXXXXX'    
-- =============================================    
CREATE PROCEDURE [dbo].[Identity_InsertAdminUser]     
(    
	 @EmailId VARCHAR(100) = NULL,    
	 @Password VARCHAR(MAX) = NULL    
)    
AS    
BEGIN    
 BEGIN TRY    
  
  DECLARE @RoleId VARCHAR(100) =NULL    
  DECLARE @UserId VARCHAR(100) =NULL    
    
  IF EXISTS(SELECT TOP 1 EmailId FROM Identity_User WITH(NOLOCK) WHERE EmailId=@EmailId AND [Password] = @Password AND IsActive = 1)    
  BEGIN    
       
		SET @UserId = (SELECT UserId FROM Identity_User WITH(NOLOCK) WHERE EmailId = @EmailId AND [Password]  COLLATE Latin1_General_BIN = @Password  COLLATE Latin1_General_BIN AND IsActive = 1)    
    
		--LogIn Entry    
    
	   Update Identity_UserLog SET IsActive = 0 WHERE UserId = @UserId    
    
	   INSERT INTO Identity_UserLog (UserId) VALUES (@UserId)    
   
	   SELECT TOP 1 U.USerID, U.EmailId AS EmailId, 1 IsUserExists, RM.RoleId as SuperRoleId, RM.RoleName as SuperRoleName, ARM.RoleId, ARM.RoleName    
	   FROM Identity_User U WITH(NOLOCK)  
	   INNER JOIN Identity_RoleMaster Rm WITH(NOLOCK) on Rm.RoleId = u.RoleId  
	   LEFT JOIN [HeroAdmin].[dbo].[Admin_UserRoleMapping] AURM WITH(NOLOCK) ON AURM.UserId = U.UserId
	   LEFT JOIN [HeroAdmin].[dbo].[Admin_RoleMaster] ARM WITH(NOLOCK) ON ARM.RoleId = AURM.RoleId
	   WHERE U.EmailId = @EmailId AND U.[Password] = @Password AND U.IsActive = 1 AND AURM.IsActive = 1 AND ARM.IsActive = 1

  END    
  ELSE    
  BEGIN    
   SELECT TOP 1 null as EmailId, 0 as IsUserExists, null as RoleName, null as UserId  
   FROM Identity_User u WITH(NOLOCK)     
   --LEFT JOIN Identity_RoleMaster Rm on Rm.RoleId = u.RoleId  
   --WHERE EmailId=@EmailId AND [Password] = @Password AND IsActive = 1  
  END    
    
 END TRY                    
 BEGIN CATCH              
      
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
    
END 
