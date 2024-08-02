      
-- =============================================          
-- Author:  <Author,VISHAL KANJARIYA>          
-- Create date: <Create Date,06-01-2023>          
-- Description: <Description,GetDocumentType>          
-- =============================================          
CREATE PROCEDURE [dbo].[POSP_GetExamBannerDetail]            
AS          
BEGIN          
 BEGIN TRY          
  SELECT Id, BannerFileName, BannerStoragePath, BannerType, DocumentId FROM ExamBannerDetail WITH(NOLOCK) WHERE IsActive = 1      
 END TRY                          
 BEGIN CATCH                    
  
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH          
END 


-----------------------
/****** Object:  StoredProcedure [dbo].[POSP_InsertExamBannerDetail]    Script Date: 01/06/2023 11:41:27 ******/
SET ANSI_NULLS ON
