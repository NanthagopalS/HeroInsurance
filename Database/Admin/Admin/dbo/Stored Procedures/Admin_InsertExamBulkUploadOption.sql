-- =============================================        
-- Author:  <Author, Parth>    
-- Create date: <Create Date,03-Mar-2023>    
-- Description: <Description,Insert Exam Options>          
-- =============================================        
CREATE PROCEDURE [dbo].[Admin_InsertExamBulkUploadOption]         
(           
 @QuestionId varchar(100),   
 @OptionValue varchar(100),    
 @IsCorrectAnswer bit       
)        
AS        
BEGIN        
 BEGIN TRY        
  BEGIN TRANSACTION      
  
   DECLARE @oIndex int = 0
   SELECT @oIndex =  ISNULL(MAX([OptionIndex]),0) from  [HeroPOSP].[dbo].[POSP_ExamQuestionPaperOptionMaster] WITH(NOLOCK) where QuestionId = @QuestionId

   INSERT INTO [HeroPOSP].[dbo].[POSP_ExamQuestionPaperOptionMaster]([QuestionId],[OptionIndex],[OptionValue],[IsCorrectAnswer],
			   [IsActive],[CreatedOn])        
   VALUES(@QuestionId, @oIndex + 1,@OptionValue,@IsCorrectAnswer,1,GETDATE())        
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
