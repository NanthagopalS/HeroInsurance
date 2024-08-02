
-- =============================================
-- Author:		<Author,,Harsh Patel >
-- Create date: <Create Date,,12/19/2022>
-- Description:	<Description,GetCityByStateId>
-- =============================================
CREATE    PROCEDURE [dbo].[Identity_GetCityByStateId]
@StateId VARCHAR(100) = NULL 
AS
BEGIN
   BEGIN TRY  
  SELECT CAST(CityId as varchar(50)) as CityId, CityName FROM Identity_City  WITH(NOLOCK) 
  WHERE StateId =@StateId
  ORDER BY CityName 
 END TRY                  
 BEGIN CATCH            
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
END
