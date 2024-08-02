-- EXEC [dbo].[Insurance_DoesTATAVariantAndRTOExists] 'F5D1533B-F19E-48D7-9DD8-45B554FA08C1', 'F20CB3D5-E3C6-4C8A-BDD0-DD4E6A190F18'
CREATE   PROCEDURE [dbo].[Insurance_DoesTATAVariantAndRTOExists]
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
		FROM MOTOR.TATA_RTOMaster WITH(NOLOCK) 
		WHERE RTOId = @RTOId)
		BEGIN
			SET @IsRTOExists = 1 
		END

		IF EXISTS(SELECT VarientId
		FROM MOTOR.TATA_VehicleMaster WITH(NOLOCK) 
		WHERE VarientId = @VariantId)
		BEGIN
			SET @IsVariantExists = 1 
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