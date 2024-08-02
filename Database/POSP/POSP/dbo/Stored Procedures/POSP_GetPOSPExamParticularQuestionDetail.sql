
CREATE   PROCEDURE [dbo].[POSP_GetPOSPExamParticularQuestionDetail] 
(
	@UserId VARCHAR(500),
	@ExamId VARCHAR(500),
	@QuestionNo int
)
AS
BEGIN

	BEGIN TRY
		
		SELECT ep.Id, ep.ExamId, ep.QuestionNo, pm.ExamModuleType, ep.QuestionId, pm.QuestionValue, ep.StatusId, es.StatusValue, ep.AnswerOptionId, po.OptionIndex, po.OptionValue  
		FROM POSP_Exam as pe WITH(NOLOCK)
			INNER JOIN POSP_ExamPaperDetail as ep WITH(NOLOCK) ON ep.ExamId = pe.Id AND ep.IsActive = 1 AND ep.QuestionNo = @QuestionNo
			INNER JOIN POSP_ExamQuestionPaperMaster as pm WITH(NOLOCK) ON pm.Id = ep.QuestionId AND pm.IsActive = 1
			INNER JOIN POSP_ExamQuestionStatusMaster as es WITH(NOLOCK) ON es.Id = ep.StatusId AND es.IsActive = 1
			LEFT JOIN POSP_ExamQuestionPaperOptionMaster as po WITH(NOLOCK) ON po.Id = ep.AnswerOptionId AND po.QuestionId = pm.Id AND po.IsActive = 1
		WHERE pe.Id = @ExamId AND pe.UserId = @UserId AND pe.IsActive = 1
		
		----Find All Option...

		SELECT po.Id, po.OptionIndex, po.OptionValue
		FROM POSP_ExamPaperDetail as ep WITH(NOLOCK)
			INNER JOIN POSP_ExamQuestionPaperMaster as pm WITH(NOLOCK) ON pm.Id = ep.QuestionId AND pm.IsActive = 1
			INNER JOIN POSP_ExamQuestionPaperOptionMaster as po WITH(NOLOCK) ON po.QuestionId = pm.Id AND po.IsActive = 1
		WHERE ep.ExamId = @ExamId AND ep.QuestionNo = @QuestionNo AND ep.IsActive = 1
		ORDER BY po.OptionIndex 

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
		
	END CATCH

END
