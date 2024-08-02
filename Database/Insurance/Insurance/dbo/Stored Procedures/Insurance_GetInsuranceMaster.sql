
CREATE  PROCEDURE [dbo].[Insurance_GetInsuranceMaster]    
AS    
BEGIN    
 BEGIN TRY    
  
  SET NOCOUNT ON;    
    
  SELECT CAST(InsurerId AS VARCHAR(50))InsurerId,InsurerName,InsurerCode,IsActive, InsurerType  
  FROM Insurance_Insurer WITH(NOLOCK)      
  ORDER BY InsurerName    

     
 END TRY                    
 BEGIN CATCH              
           
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
END