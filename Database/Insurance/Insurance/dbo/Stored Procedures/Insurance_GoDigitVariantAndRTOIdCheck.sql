-- EXEC [dbo].[Insurance_GoDigitVariantAndRTOIdCheck] 'F9239C87-7F5A-483F-8368-3CC126CC0748', '98C1FA26-7E2E-4779-9B0E-E1E525530889'
CREATE   PROCEDURE [dbo].[Insurance_GoDigitVariantAndRTOIdCheck]
@VariantId VARCHAR(100) NULL,
@RTOId VARCHAR(100) NULL
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		SELECT RTOId AS RTOId
		FROM MOTOR.GoDigit_RTO WITH(NOLOCK) 
		WHERE RTOId = @RTOId

		SELECT VariantId AS VariantId
		FROM MOTOR.GoDigit_VehicleMaster WITH(NOLOCK) 
		WHERE VariantId = @VariantId

	END TRY                
	BEGIN CATCH          
			 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END