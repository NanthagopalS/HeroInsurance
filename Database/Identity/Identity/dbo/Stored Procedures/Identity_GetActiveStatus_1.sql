

Create   PROCEDURE [dbo].[Identity_GetActiveStatus] 
(
	@UserId VARCHAR(50)= NULL
)
AS
BEGIN
	BEGIN TRY
	
		Select IsActive from [dbo].[Identity_User] where UserId = @UserId
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END