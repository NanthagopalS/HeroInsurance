-- =============================================              
-- Author:  <Author, Suraj>              
-- Description: <Description,Update Admin User Password>              
-- Identity_UpdateAdminUser '','XXXXXX'              
-- =============================================              
CREATE  
    
  
 PROCEDURE [dbo].[Identity_UpdateAdminUserPassword] (  
 @UserId VARCHAR(100) = NULL  
 ,@NewPassWord VARCHAR(MAX) = NULL  
 )  
AS  
BEGIN  
 BEGIN TRY  
  IF EXISTS (  
    SELECT TOP 1 UserId  
    FROM Identity_User WITH (NOLOCK)  
    WHERE UserId = @UserId  
     AND IsActive = 1  
    )  
  BEGIN  
   UPDATE Identity_User  
   SET [Password] = @NewPassWord  
    ,UpdatedOn = GETDATE()  
    ,IsPasswordResetRequired = 0  
   WHERE UserId = @UserId  
    AND IsActive = 1  
  
   SELECT UserId  
    ,1 AS IsPasswordUpdated  
    ,'Password updated successfully' AS [Message]  
   FROM Identity_User  
   WHERE UserId = @UserId  
    AND IsActive = 1  
  END  
  ELSE  
  BEGIN  
   SELECT NULL AS UserId  
    ,0 AS IsPasswordUpdated  
    ,'Failed to update password' AS [Message]  
  END  
 END TRY  
  
 BEGIN CATCH  
  IF @@TRANCOUNT > 0  
   ROLLBACK  
  
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