
CREATE   PROCEDURE [dbo].[POSP_GetPOSPExamQuetionNavigatorDetail] 
(
	@UserId VARCHAR(500),
	@ExamId VARCHAR(500)
)
AS
BEGIN

	DECLARE @Result TABLE(Id int, QuestionStatus varchar(100), QuestionCount int)

	DECLARE @SkippedCount int = 0,
			@AnsweredCount int = 0,
			@NotVisitedCount int = 0


	BEGIN TRY
		
		SET @SkippedCount = (SELECT SkippedAnswered FROM POSP_Exam WHERE UserId = @UserId AND Id = @ExamId AND IsActive = 1)

		SET @AnsweredCount = (SELECT (CorrectAnswered + InCorrectAnswered) as Answered FROM POSP_Exam WHERE UserId = @UserId AND Id = @ExamId AND IsActive = 1)

		SET @NotVisitedCount = ABS(20 - (@SkippedCount + @AnsweredCount))

		INSERT INTO @Result VALUES
		(1, 'Skipped', @SkippedCount),
		(2, 'Answered', @AnsweredCount),
		(3, 'Not Visited', @NotVisitedCount)

		SELECT QuestionStatus, QuestionCount from @Result order by Id

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
		
	END CATCH

END
