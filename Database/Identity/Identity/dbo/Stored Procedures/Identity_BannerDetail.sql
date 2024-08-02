      
-- =============================================          
-- Author:  <Author,HARSH PATEL>          
-- Create date: <Create Date,13-12-2022>          
-- Description: <Description,GetDocumentType>          
-- =============================================          
CREATE        PROCEDURE [dbo].[Identity_BannerDetail]            
AS          
BEGIN          
 BEGIN TRY          
  SELECT Id,BannerFileName,BannerStoragePath,BannerType,DocumentId FROM BannerDetail WITH(NOLOCK)     WHERE IsActive =1      
 END TRY                          
 BEGIN CATCH                    
                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH          
END 
