
CREATE   PROCEDURE [dbo].[POSP_GetPOSPMessageDetail]
(
	@MessageKey VARCHAR(100) = NULL
)
AS
BEGIN
	BEGIN TRY

		SELECT Id, TitleValue, Subtitle1, Subtitle2, Subtitle3 FROM POSP_MessageConfiguration WITH(NOLOCK) WHERE MessageKey = @MessageKey AND IsActive = 1
	
	END TRY                
	BEGIN CATCH
	
		DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
		
	END CATCH
END
