
-- EXEC [dbo].[Insurance_DoesHDFCVariantAndRTOExists] '10344191-EB76-467A-88CC-2F12CB2B376D', '98C1FA26-7E2E-4779-9B0E-E1E25530889' , '', '2d566966-5525-4ed7-bd90-bb39e8418f39' 
CREATE PROCEDURE [dbo].[Insurance_DoesHDFCVariantAndRTOExists] @VariantId VARCHAR(50) = NULL
	,@RTOId VARCHAR(50) = NULL
	,@VehicleNumber VARCHAR(20) = NULL
	,@VehicleTypeId VARCHAR(50) = NULL
AS
BEGIN
	BEGIN TRY
		DECLARE @IsRTOExists BIT = 0
			,@IsVariantExists BIT = 0

		SET NOCOUNT ON;

		IF (ISNULL(@RTOId, '') = '')
		BEGIN
			SELECT @RTOId = RTOId
			FROM Insurance_RTO WITH (NOLOCK)
			WHERE RTOCode = LEFT(@VehicleNumber, 4)
		END

		IF (@VehicleTypeId = '6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056')
		BEGIN
			IF EXISTS (
					SELECT RTOId
					FROM MOTOR.HDFC_RTOMasterMotor WITH (NOLOCK)
					WHERE RTOId = @RTOId
					)
			BEGIN
				SET @IsRTOExists = 1
			END
		END
		ELSE IF (@VehicleTypeId = '2d566966-5525-4ed7-bd90-bb39e8418f39')
		BEGIN
			IF EXISTS (
					SELECT RTOId
					FROM MOTOR.HDFC_RTOMasterCar WITH (NOLOCK)
					WHERE RTOId = @RTOId
					)
			BEGIN
				SET @IsRTOExists = 1
			END
		END
		ELSE IF (@VehicleTypeId = '88a807b3-90e4-484b-b5d2-65059a8e1a91')
		BEGIN
			IF EXISTS (
					SELECT RTOId
					FROM MOTOR.HDFC_CV_RTOMaster WITH (NOLOCK)
					WHERE RTOId = @RTOId
					)
			BEGIN
				SET @IsRTOExists = 1
			END
		END

		IF EXISTS (
				SELECT VariantId
				FROM MOTOR.HDFC_VehicleMaster WITH (NOLOCK)
				WHERE VariantId = @VariantId
				)
		BEGIN
			SET @IsVariantExists = 1
		END

		IF (@IsVariantExists = 0)
		BEGIN
			EXEC Insurance_GetVariantProbabilityForHDFC @VariantId
				,@IsVariantMapped = @IsVariantExists OUTPUT;
		END

		SELECT @IsRTOExists AS IsRTOExists
			,@IsVariantExists AS IsVariantExists
	END TRY

	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500)
			,@ErrorDetail VARCHAR(1000)
			,@ParameterList VARCHAR(2000)

		SET @StrProcedure_Name = ERROR_PROCEDURE()
		SET @ErrorDetail = ERROR_MESSAGE()

		EXEC Insurance_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name
			,@ErrorDetail = @ErrorDetail
			,@ParameterList = @ParameterList
	END CATCH
END