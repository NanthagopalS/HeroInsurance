
CREATE   PROCEDURE [dbo].[Identity_Logout] 
(
	@UserId VARCHAR(100) = NULL
)
AS
BEGIN
	BEGIN TRY
		
		--LogIn Entry
		UPDATE Identity_UserLog SET LogOutDateTime = GETDATE() WHERE UserId = @UserId AND IsActive = 1
		
		SELECT 1 as LogoutStatus

	END TRY                
	BEGIN CATCH          
		
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
