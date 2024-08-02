-- EXEC [dbo].[Insurance_DoesCholaVariantAndRTOExists] '10344191-EB76-467A-88CC-2F12CB2B376D', '98C1FA26-7E2E-4779-9B0E-E1E525530889', '517D8F9C-F532-4D45-8034-ABECE46693E3',''
CREATE   PROCEDURE [dbo].[Insurance_DoesCholaVariantAndRTOExists]
@VariantId VARCHAR(100) NULL,
@RTOId VARCHAR(100) NULL,
@PolicyTypeId VARCHAR(100) NULL,
@VehicleNumber VARCHAR(100) NULL
AS
BEGIN
	BEGIN TRY
		DECLARE @IsRTOExists BIT = 0, @IsVariantExists BIT = 0

		SET NOCOUNT ON;

		IF(ISNULL(@RTOId, '' ) = '') 
		BEGIN
			SELECT @RTOId = RTOId FROM Insurance_RTO WITH(NOLOCK)   
			WHERE RTOCode = LEFT(@VehicleNumber,4) 
		END

		IF EXISTS(SELECT RTOId
		FROM MOTOR.Chola_RTOMaster WITH(NOLOCK) 
		WHERE RTOId = @RTOId)
		BEGIN
			SET @IsRTOExists = 1 
		END

		IF @PolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B'
		BEGIN
			IF EXISTS(SELECT VarientId
			FROM MOTOR.Chola_VehicleMaster VM WITH(NOLOCK) 
			INNER JOIN MOTOR.Chola_RTOMaster RTO WITH(NOLOCK) 
			ON VM.NumStateCode = RTO.NumStateCode
			WHERE VM.VarientId = @VariantId AND VM.PolicyTypeId = '2AA7FDCA-9E36-4A8D-9583-15ADA737574B' AND RTO.RTOId = @RTOId)
			BEGIN
				SET @IsVariantExists = 1 
			END

			IF(@IsVariantExists = 0)
			BEGIN
				
				EXEC Insurance_GetVariantProbabilityForCHOLA @VariantId,@PolicyTypeId, @IsVariantMapped = @IsVariantExists OUTPUT;
				
			END
		END
		ELSE
		BEGIN
			IF EXISTS(SELECT VarientId
			FROM MOTOR.Chola_VehicleMaster VM WITH(NOLOCK) 
			INNER JOIN MOTOR.Chola_RTOMaster RTO WITH(NOLOCK) 
			ON VM.NumStateCode = RTO.NumStateCode
			WHERE VM.VarientId = @VariantId AND (VM.PolicyTypeId IS NULL OR VM.PolicyTypeId != '2AA7FDCA-9E36-4A8D-9583-15ADA737574B') AND RTO.RTOId = @RTOId)
			BEGIN
				SET @IsVariantExists = 1 
			END

			IF(@IsVariantExists = 0)
			BEGIN
				
				EXEC Insurance_GetVariantProbabilityForCHOLA @VariantId,@PolicyTypeId, @IsVariantMapped = @IsVariantExists OUTPUT;
				
			END
		END

		SELECT @IsRTOExists AS IsRTOExists, @IsVariantExists AS IsVariantExists
		
	END TRY                
	BEGIN CATCH          
			 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END