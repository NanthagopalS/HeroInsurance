  
CREATE    PROCEDURE [dbo].[Insurance_GetRelianceStateCityId]          
@State VARCHAR(50) = NULL,     
@City VARCHAR(20) = NULL,   
@PinCode VARCHAR(50) = NULL  
AS          
BEGIN          
 BEGIN TRY       
  SET NOCOUNT ON;  
    
  --SELECT DISTINCT StateId, CityOrVillageID as CityId, DistrictID as DistrictId  
  --FROM  MOTOR.Reliance_PincodeMaster WITH(NOLOCK)   
  --WHERE StateName = @State AND CityOrVillageName = @City
    SELECT TOP 1  StateId,CityOrVillageID as CityId, DistrictID as DistrictId FROM  MOTOR.Reliance_PincodeMaster WITH(NOLOCK)   
	WHERE StateName = @State AND PinCode = @PinCode

 END TRY          
 BEGIN CATCH                     
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                  
 END CATCH          
END