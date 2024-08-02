-- EXEC [dbo].[Insurance_DoesIFFCOVariantAndRTOExists] '3884AB42-6B1F-465B-8EBF-0EE44CB8D720', 'F20CB3D5-E3C6-4C8A-BDD0-DD4E6A190F18',null
CREATE     PROCEDURE [dbo].[Insurance_DoesIFFCOVariantAndRTOExists]
@VariantId VARCHAR(100) NULL,
@RTOId VARCHAR(100) NULL,
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
		FROM MOTOR.ITGI_RTO_CityMaster WITH(NOLOCK) 
		WHERE RTOId = @RTOId)
		BEGIN
			SET @IsRTOExists = 1 
		END

		IF EXISTS(SELECT VariantId
		FROM MOTOR.ITGI_VehicleMaster WITH(NOLOCK) 
		WHERE VariantId = @VariantId)
		BEGIN
			SET @IsVariantExists = 1 
		END
		 IF(@IsVariantExists = 0)
			BEGIN
				
				EXEC Insurance_GetVariantProbabilityForIFFCO @VariantId, @IsVariantMapped = @IsVariantExists OUTPUT;
				
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