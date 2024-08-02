
CREATE PROCEDURE [dbo].[POSP_GetPOSPMessageDetail]
(
	@MessageKey VARCHAR(100)
)
AS
BEGIN
	BEGIN TRY

		SELECT Id, MessageValue FROM POSP_MessageConfiguration WITH(NOLOCK) WHERE MessageKey = @MessageKey AND IsActive = 1
	
	END TRY                
	BEGIN CATCH
		
		DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
		
	END CATCH
END

