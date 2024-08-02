-- EXEC [dbo].[Insurance_DoesGoDigitVariantAndRTOExists] '8ca35d32-bf95-4962-a3d7-89fce2dbd7eb', '','MH01DB3014'  
CREATE   PROCEDURE [dbo].[Insurance_DoesGoDigitVariantAndRTOExists]  
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
   SELECT @RTOId = RTOId FROM Insurance_RTO WITH(NOLOCK)  WHERE RTOCode = LEFT(@VehicleNumber,4)  
  END  
  
  IF EXISTS(SELECT RTOId  
  FROM MOTOR.GoDigit_RTO WITH(NOLOCK)   
  WHERE RTOId = @RTOId)  
  BEGIN  
   SET @IsRTOExists = 1   
  END  
  
  IF EXISTS(SELECT VariantId  
  FROM MOTOR.GoDigit_VehicleMaster WITH(NOLOCK)   
  WHERE VariantId = @VariantId)  
  BEGIN  
   SET @IsVariantExists = 1   
  END  
  
  IF(@IsVariantExists = 0)
			BEGIN
				
				EXEC Insurance_GetVariantProbabilityforGoDigit @VariantId, @IsVariantMapped = @IsVariantExists OUTPUT;
				
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