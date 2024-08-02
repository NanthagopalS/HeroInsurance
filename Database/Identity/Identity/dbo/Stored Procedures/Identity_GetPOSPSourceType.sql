    
CREATE   PROCEDURE [dbo].[Identity_GetPOSPSourceType]        
AS      
BEGIN      
 BEGIN TRY       
  
  SELECT Id as POSPSourceTypeId, POSPSourceType FROM [dbo].[Identity_POSPSourceTypeMaster] WITH(NOLOCK) ORDER BY POSPSourceType
 
 
 END TRY                      
 BEGIN CATCH                
             
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                   
 END CATCH      
END 
