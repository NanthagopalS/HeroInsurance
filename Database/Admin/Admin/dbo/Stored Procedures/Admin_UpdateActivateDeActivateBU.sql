
CREATE   PROCEDURE [dbo].[Admin_UpdateActivateDeActivateBU]       
(      
 @BUId varchar(100),        
 @IsActive bit  
)      
AS      
BEGIN      
 BEGIN TRY      
 Update [HeroAdmin].[dbo].[Admin_BU] SET IsActive = @IsActive, CreatedOn= GETDATE() WHERE BUId = @BUId        
 END TRY                      


 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
      
END 
