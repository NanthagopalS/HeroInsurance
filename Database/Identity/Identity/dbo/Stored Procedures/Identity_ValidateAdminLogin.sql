-- =============================================          
-- Author:  <Author, Girish>          
-- Create date: <Create Date,12-Dec-2022>          
-- Description: <Description,INSERT Admin>          
-- Identity_InsertLogUser 'GirishSF@Mantralabsglobal.com','XXXXXX'          
-- =============================================          
CREATE  
    
  
 PROCEDURE [dbo].[Identity_ValidateAdminLogin] (@EmailId VARCHAR(100) = NULL)  
AS  
BEGIN  
 BEGIN TRY  
  DECLARE @RoleId VARCHAR(100) = NULL  
  DECLARE @UserId VARCHAR(100) = NULL  
  
  SELECT TOP 1 @UserId = UserId  
   ,@RoleId = RoleId  
  FROM Identity_User WITH (NOLOCK)  
  WHERE EmailId = @EmailId  
   AND IsActive = 1  
  
  IF (ISNULL(@UserId, '') != '')  
  BEGIN  
   UPDATE Identity_UserLog  
   SET IsActive = 0  
   WHERE UserId = @UserId  
  
   INSERT INTO Identity_UserLog (UserId)  
   VALUES (@UserId)  
  
   SELECT U.USerID  
    ,U.EmailId AS EmailId  
    ,1 IsUserExists  
    ,RM.RoleId AS SuperRoleId  
    ,RM.RoleName AS SuperRoleName  
    ,ARM.RoleId  
    ,ARM.RoleName  
    ,U.[Password]  
    ,U.IsPasswordResetRequired AS CredentialsReseted  
   FROM Identity_User U WITH (NOLOCK)  
   INNER JOIN Identity_RoleMaster Rm WITH (NOLOCK) ON Rm.RoleId = u.RoleId  
   LEFT JOIN [HeroAdmin].[dbo].[Admin_UserRoleMapping] AURM WITH (NOLOCK) ON AURM.UserId = U.UserId  
   LEFT JOIN [HeroAdmin].[dbo].[Admin_RoleMaster] ARM WITH (NOLOCK) ON ARM.RoleId = AURM.RoleId  
   WHERE U.EmailId = @EmailId  
    AND U.IsActive = 1  
    AND AURM.IsActive = 1  
    AND ARM.IsActive = 1  
  END  
  ELSE  
  BEGIN  
   SELECT NULL AS EmailId  
    ,0 AS IsUserExists  
    ,NULL AS RoleName  
    ,NULL AS UserId  
    ,0 AS CredentialsReseted  
  END  
 END TRY  
  
 BEGIN CATCH  
  DECLARE @StrProcedure_Name VARCHAR(500)  
   ,@ErrorDetail VARCHAR(1000)  
   ,@ParameterList VARCHAR(2000)  
  
  SET @StrProcedure_Name = ERROR_PROCEDURE()  
  SET @ErrorDetail = ERROR_MESSAGE()  
  
  EXEC Identity_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name  
   ,@ErrorDetail = @ErrorDetail  
   ,@ParameterList = @ParameterList  
 END CATCH  
END