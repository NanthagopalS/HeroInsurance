
CREATE   PROCEDURE [dbo].[POSP_UpdatePOSPExamQuestionAsweredDetail] 
(
	@UserId VARCHAR(500),
	@ExamId VARCHAR(500),
	@QuestionNo int,
	@QuestionId VARCHAR(500),
	@AnswerOptionId VARCHAR(500)
)
AS
BEGIN

	SET NOCOUNT ON;
	
	DECLARE @CorrectAnswered int, @InCorrectAnswered int, @SkippedAnswered int
	DECLARE @FinalResult float
	DECLARE @IsCleared bit = 0
	
	BEGIN TRY		
		
		--1. update in POSP_ExamPaperDetail		
		UPDATE POSP_ExamPaperDetail SET AnswerOptionId = @AnswerOptionId , UpdatedOn = GETDATE() WHERE ExamId = @ExamId AND QuestionNo = @QuestionNo AND QuestionId = @QuestionId AND IsActive = 1
		
		--Update Status based on AnswerOptionId
		IF(@AnswerOptionId IS NULL OR @AnswerOptionId = '' OR @AnswerOptionId = 'null')
		BEGIN
			
			UPDATE POSP_ExamPaperDetail SET StatusId = (SELECT Id FROM POSP_ExamQuestionStatusMaster WITH(NOLOCK) WHERE StatusValue = 'Skipped'), UpdatedOn = GETDATE() WHERE ExamId = @ExamId AND QuestionNo = @QuestionNo AND QuestionId = @QuestionId AND IsActive = 1			
			
		END
		ELSE
		BEGIN
			
			UPDATE POSP_ExamPaperDetail SET StatusId = (SELECT Id FROM POSP_ExamQuestionStatusMaster WITH(NOLOCK) WHERE StatusValue = 'Answered') , UpdatedOn = GETDATE() WHERE ExamId = @ExamId AND QuestionNo = @QuestionNo AND QuestionId = @QuestionId AND IsActive = 1
			
		END
		
		--Update in final Result...
		--SET @CorrectAnswered = (SELECT CorrectAnswered FROM POSP_Exam WHERE Id = @ExamId AND UserId = @UserId AND IsActive = 1)
		SET @CorrectAnswered = (SELECT COUNT(Id) as CorrectCount FROM POSP_ExamQuestionPaperOptionMaster WITH(NOLOCK) WHERE Id IN (select distinct AnswerOptionId from POSP_ExamPaperDetail WHERE ExamId = @ExamId AND IsActive = 1) AND IsActive = 1 AND IsCorrectAnswer = 1)
		
		--SET @InCorrectAnswered = (SELECT InCorrectAnswered FROM POSP_Exam WHERE Id = @ExamId AND UserId = @UserId AND IsActive = 1)

		SET @InCorrectAnswered = (SELECT COUNT(Id) as InCorrectCount FROM POSP_ExamQuestionPaperOptionMaster WITH(NOLOCK) WHERE Id IN (select distinct AnswerOptionId from POSP_ExamPaperDetail WHERE ExamId = @ExamId AND IsActive = 1) AND IsActive = 1 AND IsCorrectAnswer = 0)
		
		--SET @SkippedAnswered = (SELECT SkippedAnswered FROM POSP_Exam WHERE Id = @ExamId AND UserId = @UserId AND IsActive = 1)
		
		SET @SkippedAnswered =(select COUNT(Id) as SkippedCount from POSP_ExamPaperDetail WITH(NOLOCK) WHERE ExamId = @ExamId AND IsActive = 1 AND StatusId IN (SELECT Id FROM POSP_ExamQuestionStatusMaster WHERE StatusValue = 'Skipped'))
		/*
		IF(@AnswerOptionId IS NULL OR @AnswerOptionId = '' OR @AnswerOptionId = 'null')
		BEGIN
			SET @SkippedAnswered = @SkippedAnswered + 1
		END
		ELSE
		BEGIN
			
			IF((SELECT IsCorrectAnswer FROM POSP_ExamQuestionPaperOptionMaster WHERE QuestionId = @QuestionId AND Id = @AnswerOptionId AND IsActive = 1) = 1)
			BEGIN
				SET @CorrectAnswered = @CorrectAnswered + 1
			END
			ELSE
			BEGIN
				SET @InCorrectAnswered = @InCorrectAnswered + 1
			END
		END		
		*/

		SET @FinalResult = ((@CorrectAnswered * 100.0)/20.0)
		
		SET @IsCleared = 0

		IF(@FinalResult >= 30.0)
		BEGIN
			SET @IsCleared = 1
		END
		DELETE FROM [dbo].[POSP_Exam] WHERE IsActive = 0 AND IsCleared = 0 AND UserId = @UserId
		UPDATE POSP_Exam SET CorrectAnswered = @CorrectAnswered, InCorrectAnswered = @InCorrectAnswered, SkippedAnswered = @SkippedAnswered, FinalResult = @FinalResult, IsCleared = @IsCleared, UpdatedOn = GETDATE() WHERE Id = @ExamId AND UserId = @UserId AND IsActive = 1

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
		
	END CATCH

END
