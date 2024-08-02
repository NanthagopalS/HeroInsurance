    
-- =============================================          
-- Author:  <Author,VISHAL KANJARIYA>          
-- Create date: <Create Date,06-01-2023>          
-- Description: <Description,,InsertExamBannerDetail>          
-- =============================================          
CREATE PROCEDURE [dbo].[POSP_InsertExamBannerDetail]           
(          
 --@Id VARCHAR(100) = NULL,            
 @BannerFileName VARCHAR(500) = NULL,          
 @BannerStoragePath VARCHAR(500) = NULL,  
 @BannerType VARCHAR(500) = NULL,
 @DocumentId VARCHAR(500) = NULL  
)          
AS          
BEGIN          
   BEGIN TRY          
    
   UPDATE ExamBannerDetail SET IsActive  = 0 , UpdatedOn = GETDATE()   
    
    INSERT INTO ExamBannerDetail (BannerFileName,BannerStoragePath,DocumentId,BannerType)            
      VALUES(@BannerFileName,@BannerStoragePath,@DocumentId,@BannerType)        
    
 END TRY                          
 BEGIN CATCH                    
                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH
 
END        
