        
-- =============================================              
-- Author:  <Author,Harsh Patel>              
-- Create date: <Create Date,24-01-2023>              
-- Description: <Description,,InsertCertificateDocument>              
-- =============================================              
CREATE    PROCEDURE [dbo].[POSP_InsertCertificateDocument] --'A0881237-2665-47FD-9F5F-81B3222C20BA','63d103fc2d2c0a009d7fbac1'              
(                          
 @UserId VARCHAR(500) = NULL,    
 @DocumentId VARCHAR(500) = NULL      
)              
AS              
BEGIN              
   BEGIN TRY              

   UPDATE POSP_Exam
   SET DocumentId =@DocumentId, UpdatedOn = GETDATE()
     WHERE UserId=@UserId

	 SELECT DocumentId FROM  POSP_Exam WITH(NOLOCK) WHERE UserId = @UserId


 END TRY                              
 BEGIN CATCH                        
                     
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                          
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                          
  SET @ErrorDetail=ERROR_MESSAGE()                                          
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                           
 END CATCH    
     
END 
