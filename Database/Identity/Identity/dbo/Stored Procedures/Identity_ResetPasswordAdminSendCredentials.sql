      
-- exec [Identity_ResetPasswordAdmin] '7B6ECCA2-1639-4C7C-BD41-88E728A19CA5','Abcd@1234'      
CREATE        PROCEDURE [dbo].[Identity_ResetPasswordAdminSendCredentials]        
 (      
  @UserId VARCHAR(100),      
  @Password VARCHAR(MAX)      
 )       
AS        
BEGIN        
 BEGIN TRY        
            
  Update Identity_User Set Password = @Password, UpdatedOn = GETDATE(),IsPasswordResetRequired = 1 where UserId = @UserId      
      
  select EmailId as Email, Password from Identity_User where UserId = @UserId      
         
 END TRY                        
 BEGIN CATCH        
      
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
END