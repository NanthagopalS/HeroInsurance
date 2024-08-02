    
CREATE PROCEDURE [dbo].[Identity_CheckMobileExists]     
(    
 @MobileNo VARCHAR(100)  
)    
AS    
BEGIN    
 BEGIN TRY    
  IF EXISTS(SELECT TOP 1 MobileNumber FROM TestMobile WITH(NOLOCK) WHERE MobileNumber = @MobileNo)  
  BEGIN  
 SELECT 1 AS IsExists  
  END  
  ELSE  
  BEGIN  
 SELECT 0 AS IsExists   
  END  
 END TRY                    
 BEGIN CATCH      
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
    
END 
