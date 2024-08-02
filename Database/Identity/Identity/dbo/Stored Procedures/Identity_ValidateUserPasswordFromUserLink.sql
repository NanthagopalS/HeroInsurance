          
-- =============================================          
-- Author:  <Author, Suraj>          
-- Description: <Description,Validate password update request from link>          
-- Identity_ValidateUserPasswordFromUserLink '45BBA540-07C5-4D4C-BEFF-771AE2FC32B0','94026b93-e4c3-439f-a2fa-961a486d3ac4'           
-- =============================================       
-- exec [dbo].[Identity_ValidateUserPasswordFromUserLink] '45BBA540-07C5-4D4C-BEFF-771AE2FC32B0','ABCd@1234'


CREATE     PROCEDURE [dbo].[Identity_ValidateUserPasswordFromUserLink]           
(          
	 @UserId VARCHAR(100) = NULL,        
	 @Guid VARCHAR(MAX) = NULL        
)          
AS         
BEGIN          
BEGIN TRY        
	IF EXISTS(select * from Identity_User U INNER JOIN Identity_ResetPasswordVerification R
ON U.UserId = R.UserId WHERE U.UserId=@UserId AND U.IsActive = 1 AND R.IsActive=1 AND R.[GuId] = @Guid)
		BEGIN
                 select 1 as IsValid,U.UserId,R.[GuId],'Valid link' as [Message] from Identity_User U INNER JOIN Identity_ResetPasswordVerification R
ON U.UserId = R.UserId WHERE U.UserId=@UserId AND U.IsActive = 1 AND R.IsActive=1 AND R.[GuId] = @Guid
		END
	ELSE
		BEGIN
			SELECT 0 as IsValid, null as UserId, 'Invalid link' as [Message]
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