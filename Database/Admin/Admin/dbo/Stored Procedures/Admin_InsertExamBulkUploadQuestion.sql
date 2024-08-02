-- =============================================      
-- Author:  <Author, Parth>      
-- Create date: <Create Date,03-Mar-2023>      
-- Description: <Description,Insert Exam Questions>      
-- =============================================      
CREATE PROCEDURE [dbo].[Admin_InsertExamBulkUploadQuestion]       
(         
 @ExamModuleType varchar(100),    
 @QuestionValue varchar(500),    
 @IsActive varchar(100),    
 @CreatedBy varchar(100)      
)      
AS      
BEGIN      
 BEGIN TRY      
  BEGIN TRANSACTION     
  DECLARE @SNumber int  
  SELECT @SNumber =  MAX([SequenceNo]) from  [HeroPOSP].[dbo].[POSP_ExamQuestionPaperMaster] WITH(NOLOCK)  
  IF(@SNumber IS NULL)
  BEGIN
    SET @SNumber = 0
  END
  SET IDENTITY_INSERT [HeroPOSP].[dbo].[POSP_ExamQuestionPaperMaster] ON  
    INSERT INTO [HeroPOSP].[dbo].[POSP_ExamQuestionPaperMaster]       
           ([SequenceNo],[ExamModuleType],[QuestionValue],[IsActive],[CreatedBy],[CreatedOn])    
     VALUES (@SNumber + 1,@ExamModuleType,@QuestionValue,@IsActive,@CreatedBy,GETDATE())      
   SELECT TOP 1 Id AS [IdentityRoleId] FROM [HeroPOSP].[dbo].[POSP_ExamQuestionPaperMaster]  
   ORDER BY CreatedOn DESC;      
         
   IF @@TRANCOUNT > 0      
            COMMIT      
 END TRY                      
 BEGIN CATCH                
 IF @@TRANCOUNT > 0      
        ROLLBACK        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
      
END      
--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- 
