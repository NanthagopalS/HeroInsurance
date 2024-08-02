


-- =============================================
-- Author:		<Author, Girish SF>
-- Create date: <Create Date,22-DEC-2022>
-- Description:	<Description,,INSERT USER EMAIL LINK DETAIL>
-- =============================================
CREATE  PROCEDURE [dbo].[Identity_AdminInsertUserEmailVerificationLinkDetail] 
(
	@EmailId VARCHAR(100) = NULL,
	@GuId VARCHAR(100) = NULL,
	@UserId VARCHAR(50) = NULL
	--@Condition VARCHAR(50) = NULL
)
AS
BEGIN
	BEGIN TRY
		

			IF EXISTS(SELECT TOP 1 GuId FROM [Identity_EmailVerification] WITH(NOLOCK) WHERE IsActive = 1 AND [GuId] = @GuId AND UserId = @UserId AND DATEDIFF(MM,LinkSendDateTime,GETDATE()) <=10)
			BEGIN
				
				UPDATE [Identity_EmailVerification] SET IsVerify = 1, UpdatedOn= GETDATE() WHERE [GuId] = @GuId AND UserId = @UserId

				SELECT 1 IsValid
			END
	     ELSE
		
			BEGIN
			
			UPDATE [dbo].[Identity_EmailVerification] SET IsActive = 0, UpdatedOn= GETDATE() WHERE UserId = @UserId AND @EmailId = @EmailId
			
			INSERT INTO [dbo].[Identity_EmailVerification] (GuId, UserId, EmailId, CreatedBy) 
			VALUES (@GuId, @UserId, @EmailId, @UserId)
		
				SELECT 0 IsValid
			END
		
		
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
