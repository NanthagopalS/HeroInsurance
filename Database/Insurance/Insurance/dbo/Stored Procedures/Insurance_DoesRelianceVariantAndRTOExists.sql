  
-- EXEC [dbo].[Insurance_DoesRelianceVariantAndRTOExists] '0E23DAB0-E8EB-4A99-B6D7-855050B4A558', 'F20CB3D5-E3C6-4C8A-BDD0-DD4E6A190F18','',''    
CREATE    PROCEDURE [dbo].[Insurance_DoesRelianceVariantAndRTOExists] 
	@VariantId VARCHAR(100) NULL,  
    @RTOId VARCHAR(100) NULL,  
    @VehicleNumber VARCHAR(100) = '',
	@VehicleTypeId VARCHAR(50) = NULL
AS  
BEGIN  
    BEGIN TRY  
        DECLARE @IsRTOExists BIT = 0,  
            @IsVariantExists BIT =0  
        SET NOCOUNT ON;  
        IF (ISNULL(@RTOId, '') = '')  
        BEGIN  
            SELECT @RTOId = RTOId  
            FROM Insurance_RTO WITH (NOLOCK)  
            WHERE RTOCode = LEFT(@VehicleNumber, 4)  
        END
		
		-- For Commercial Wheeler
		IF(@VehicleTypeId IS NOT NULL AND @VehicleTypeId = '88a807b3-90e4-484b-b5d2-65059a8e1a91')
		BEGIN		
			IF EXISTS(SELECT RTOId FROM MOTOR.Reliance_CV_RTOMaster WITH (NOLOCK)  WHERE RTOId = @RTOId)  
			BEGIN  
				SET @IsRTOExists = 1  
			END  
  
			IF EXISTS (SELECT VarientId FROM MOTOR.Reliance_CV_VehicleMaster WITH (NOLOCK) WHERE VarientId = @VariantId)  
			BEGIN  
				SET @IsVariantExists = 1  
			END  
		END
		ELSE
		BEGIN
		   -- For Two/Four Wheeler
			IF EXISTS (SELECT RTOId FROM MOTOR.Reliance_RTOMaster WITH (NOLOCK) WHERE RTOId = @RTOId)  
			BEGIN  
				SET @IsRTOExists = 1  
			END  
  			IF EXISTS (SELECT VarientId FROM MOTOR.Reliance_VehicleMaster WITH (NOLOCK) WHERE VarientId = @VariantId)  
			BEGIN  
				SET @IsVariantExists = 1  
			END  
       END
  
    IF(@IsVariantExists = 0)  
    BEGIN  
	  EXEC Insurance_GetVariantProbabilityForReliance @VariantId, @IsVariantMapped = @IsVariantExists OUTPUT;  
    END  
    SELECT @IsRTOExists AS IsRTOExists,@IsVariantExists AS IsVariantExists  
	END TRY  
  
    BEGIN CATCH  
        DECLARE @StrProcedure_Name VARCHAR(500),  
            @ErrorDetail VARCHAR(1000),  
            @ParameterList VARCHAR(2000)  
  
        SET @StrProcedure_Name = ERROR_PROCEDURE()  
        SET @ErrorDetail = ERROR_MESSAGE()  
  
        EXEC Insurance_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name,  
            @ErrorDetail = @ErrorDetail,  
            @ParameterList = @ParameterList  
    END CATCH  
END