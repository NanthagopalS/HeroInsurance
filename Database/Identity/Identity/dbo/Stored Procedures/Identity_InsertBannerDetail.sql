    
-- =============================================          
-- Author:  <Author, Harsh Patel >          
-- Create date: <Create Date,13-12-2022>          
-- Description: <Description,,InsertBannerDetail>          
-- =============================================          
CREATE          PROCEDURE [dbo].[Identity_InsertBannerDetail]           
(          
 --@Id VARCHAR(100) = NULL,            
 @BannerFileName VARCHAR(100) = NULL,          
 @BannerStoragePath VARCHAR(200) = NULL,  
 @BannerType VARCHAR(200) = NULL,
 @DocumentId VARCHAR(100) = NULL  
)          
AS          
BEGIN          
   BEGIN TRY          
    
   UPDATE BannerDetail SET IsActive  = 0, UpdatedOn= GETDATE() WHERE  BannerType = @BannerType
    
    INSERT INTO BannerDetail (BannerFileName,BannerStoragePath,DocumentId,BannerType)            
      VALUES(@BannerFileName,@BannerStoragePath,@DocumentId,@BannerType)        
    
 END TRY                          
 BEGIN CATCH                    
                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH         
          
END        


SELECT * FROM BannerDetail

