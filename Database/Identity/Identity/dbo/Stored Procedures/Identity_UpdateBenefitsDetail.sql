    
      
      
      
-- =============================================      
-- Author:  <Author,,VISHAL KANJARIYA>      
-- Create date: <Create Date,,30-NOV-2022>      
-- Description: <Description,,UPDATE USER BENEFIT DETAIL>      
-- =============================================      
CREATE    PROCEDURE [dbo].[Identity_UpdateBenefitsDetail]       
(      
 @Id varchar(200),  
 @BenefitsTitle varchar(200),  
 @BenefitsDescription varchar(200)
)      
AS      
BEGIN      
 BEGIN TRY    
   
   UPDATE [dbo].[Identity_BenefitsDetail] SET       
  BenefitsTitle = @BenefitsTitle,  
  BenefitsDescription = @BenefitsDescription,
  UpdatedOn = GetDate()  
 WHERE Id = @Id   
    
 END TRY                      
 BEGIN CATCH                
             
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList      
        
 END CATCH      
      
END 
