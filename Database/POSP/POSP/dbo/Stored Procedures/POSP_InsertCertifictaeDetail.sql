      
-- =============================================            
-- Author:  <Author,Harsh Patel>            
-- Create date: <Create Date,24-01-2023>            
-- Description: <Description,,InsertCertifictaeDetail>            
-- =============================================            
CREATE   PROCEDURE [dbo].[POSP_InsertCertifictaeDetail]             
(                        
 @ConfigurationKey VARCHAR(500) = NULL,  
 @ConfigurationValue VARCHAR(500) = NULL    
)            
AS            
BEGIN            
   BEGIN TRY            
      
   --UPDATE ExamBannerDetail SET IsActive  = 0      
      
    INSERT INTO [HeroIdentity].[dbo].[Identity_Configuration] (ConfigurationKey,ConfigurationValue)              
      VALUES(@ConfigurationKey,@ConfigurationValue)          
      
 END TRY                            
 BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH  
   
END          
