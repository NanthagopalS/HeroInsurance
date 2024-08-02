
CREATE    PROCEDURE [dbo].[Identity_GetStateCitybyPincode]
@Pincode VARCHAR(10) = NULL 
AS
BEGIN
   BEGIN TRY  
	  
	  select StateId, StateName from Identity_Pincode WITH(NOLOCK) WHERE Pincode = @Pincode;
	  
	  SELECT CAST(CityId as varchar(50)) as CityId, CityName  FROM Identity_City WITH(NOLOCK) 
	  WHERE StateId IN (select StateId from Identity_Pincode WITH(NOLOCK) WHERE Pincode = @Pincode)
	  ORDER BY CityName
	  
 END TRY                  
 BEGIN CATCH            
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END
