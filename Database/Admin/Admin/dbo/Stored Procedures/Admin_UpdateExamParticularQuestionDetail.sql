
CREATE       PROCEDURE [dbo].[Admin_UpdateExamParticularQuestionDetail] 
(
	@QuestionId VARCHAR(100) = NULL,
	@OptionId1 VARCHAR(100) = NULL,
	@OptionValue1 VARCHAR(MAX) = NULL,
	@OptionId2 VARCHAR(100) = NULL,
	@OptionValue2 VARCHAR(MAX) = NULL,
	@OptionId3 VARCHAR(100) = NULL,
	@OptionValue3 VARCHAR(MAX) = NULL,
	@OptionId4 VARCHAR(100) = NULL,
	@OptionValue4 VARCHAR(MAX) = NULL,
	@CorrectAnswerIndex INT = 1
)
AS

BEGIN
 BEGIN TRY 

	UPDATE [HeroPOSP].[dbo].[POSP_ExamQuestionPaperOptionMaster] SET OptionValue = @OptionValue1,CreatedOn= GETDATE() WHERE QuestionId = @QuestionId AND Id = @OptionId1

	UPDATE [HeroPOSP].[dbo].[POSP_ExamQuestionPaperOptionMaster] SET OptionValue = @OptionValue2, CreatedOn= GETDATE() WHERE QuestionId = @QuestionId AND Id = @OptionId2

	UPDATE [HeroPOSP].[dbo].[POSP_ExamQuestionPaperOptionMaster] SET OptionValue = @OptionValue3, CreatedOn= GETDATE() WHERE QuestionId = @QuestionId AND Id = @OptionId3

	UPDATE [HeroPOSP].[dbo].[POSP_ExamQuestionPaperOptionMaster] SET OptionValue = @OptionValue4, CreatedOn= GETDATE() WHERE QuestionId = @QuestionId AND Id = @OptionId4

	UPDATE [HeroPOSP].[dbo].[POSP_ExamQuestionPaperOptionMaster] SET IsCorrectAnswer = 0, CreatedOn= GETDATE() WHERE QuestionId = @QuestionId 
	
	UPDATE [HeroPOSP].[dbo].[POSP_ExamQuestionPaperOptionMaster] SET IsCorrectAnswer = 1, CreatedOn= GETDATE() WHERE QuestionId = @QuestionId AND OptionIndex = @CorrectAnswerIndex 
	

  END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END
