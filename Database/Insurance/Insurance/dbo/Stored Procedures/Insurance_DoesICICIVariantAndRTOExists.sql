-- EXEC [dbo].[Insurance_DoesICICIVariantAndRTOExists] '5081F93F-EF49-4A3B-B446-002AB3C97E3A', '2B8B0522-468B-4C03-918B-53C1376FDE17'
CREATE   PROCEDURE [dbo].[Insurance_DoesICICIVariantAndRTOExists]
@VariantId VARCHAR(100) NULL,
@RTOId VARCHAR(100) NULL,
@VehicleNumber VARCHAR(100) NULL
AS
BEGIN
	BEGIN TRY
		DECLARE @IsRTOExists BIT = 0, @IsVariantExists BIT = 0, @VehicleClass VARCHAR(50), @VehicleTypeId VARCHAR(50)

		SET NOCOUNT ON;

		IF(ISNULL(@RTOId, '' ) = '') 
		BEGIN
			SELECT @RTOId = RTOId FROM Insurance_RTO WITH(NOLOCK)   
			WHERE RTOCode = LEFT(@VehicleNumber,4) 
		END

		SELECT @VehicleTypeId = VehicleTypeId
		FROM Insurance_Variant WITH(NOLOCK) 
		WHERE VariantId = @VariantId

		IF @VehicleTypeId = '80981D72-4C05-465E-B554-20B64BEF7536'
		BEGIN
			SET @VehicleClass = 45
		END
		ELSE
		BEGIN
			SET @VehicleClass = 37
		END

	    IF EXISTS(SELECT VariantId, VehicleClassCode
		FROM MOTOR.ICICI_VehicleMaster WITH(NOLOCK) 
		WHERE VariantId = @VariantId)
		BEGIN
			SET @IsVariantExists = 1 
		END
		IF EXISTS(SELECT RTOId
			FROM MOTOR.ICICI_RTOMaster WITH(NOLOCK) 
			WHERE RTOId = @RTOId AND VehicleClassCode = @VehicleClass)
			BEGIN
				SET @IsRTOExists = 1 
			END

		 IF(@IsVariantExists = 0)
			BEGIN
				
				EXEC Insurance_GetVariantProbabilityForICICI @VariantId, @IsVariantMapped = @IsVariantExists OUTPUT;
				
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