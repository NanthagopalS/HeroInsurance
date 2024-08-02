

-- =============================================
-- Author:		<Author, AMBI GUPTA>
-- Create date: <Create Date,07-DEC-2022>
-- Description:	<Description,,Identity_ValidateUser>
--Identity_ValidateUser '9987848971'
-- =============================================
CREATE   PROCEDURE [dbo].[Identity_ValidateUser] 
(
	@MobileNo VARCHAR(10) = NULL
)
AS
BEGIN
	BEGIN TRY
		
		SELECT UserId FROM Identity_User WITH(NOLOCK) WHERE MOBILEnO = @MobileNo

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
