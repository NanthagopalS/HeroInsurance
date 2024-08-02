          
-- =============================================          
-- Author:  <Author, Suraj>          
-- Create date: <Create Date,21-June-2023>          
-- Description: <Description,Update User password>          
-- Identity_UpdateAdminUser '','XXXXXX','XXXXXX',''           
-- =============================================          
CREATE   PROCEDURE [dbo].[Identity_UpdateUserPasswordFromUserLink]           
(          
	 @UserId VARCHAR(100) = NULL,          
	 @NewPassWord VARCHAR(MAX) = NULL,        
	 @ConfirmPassWord VARCHAR(MAX) = NULL,        
	 @Guid VARCHAR(MAX) = NULL        
)          
AS         
BEGIN          
	DECLARE @OldPassword VARCHAR(MAX) = NULL
BEGIN TRY        
	IF(@NewPassWord = @ConfirmPassWord)
		BEGIN
			IF EXISTS(SELECT TOP 1 UserId FROM Identity_User WITH(NOLOCK) WHERE UserId = @UserId)            
				BEGIN         
					SET @OldPassword = (SELECT [Password] FROM Identity_User WHERE UserId = @UserId)
					IF EXISTS(SELECT * FROM HeroIdentity.dbo.Identity_ResetPasswordVerification WHERE UserId=@UserId and GuId=@Guid AND IsActive=1)        
						BEGIN			
									IF(@OldPassWord IS NOT NULL AND @OldPassWord != '')
										BEGIN
												Update Identity_ResetPasswordVerification SET IsActive = 0 WHERE UserId = @UserId
												Update Identity_ResetPasswordVerification SET IsVerify = 1 WHERE UserId = @UserId AND GuId = @Guid

												UPDATE Identity_User SET [Password] = @NewPassWord  WHERE UserId = @UserId              
										
												SELECT UserId, 1 as IsPasswordUpdated,'Password Updated Successfully.' as [Message] FROM Identity_User WITH(NOLOCK) WHERE UserId = @UserId  
										END
									ELSE
										BEGIN
												SELECT top 1 null as UserId, 0 as IsPasswordUpdated, 'Old password and New password can not be same.' as [Message] 
												FROM Identity_User WITH(NOLOCK)
										END
				
						END        
					ELSE        
						BEGIN
							SELECT top 1 null as UserId, 0 as IsPasswordUpdated, 'URL Not Valid.' as [Message] 
											FROM Identity_User WITH(NOLOCK)				
						END        
				END          
			ELSE          
				BEGIN          
					SELECT null as UserId, 0 as IsPasswordUpdated         
				END       
		END
	ELSE
		BEGIN
			SELECT top 1 null as UserId, 0 as IsPasswordUpdated, 'New Password And Confirm Password Doesn not match.' as [Message] 
											FROM Identity_User WITH(NOLOCK)	
		END
 END TRY        
         
 BEGIN CATCH                  
  IF @@TRANCOUNT > 0          
  ROLLBACK              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH          
END