
CREATE   PROCEDURE [dbo].[POSP_GetPOSPExamParticularQuestionStatus] 
(
	@ExamId VARCHAR(500)	
)
AS
BEGIN

	BEGIN TRY		
		
		SELECT pd.Id as QuestionId, pd.QuestionNo, pd.StatusId, sm.StatusValue
		FROM POSP_ExamPaperDetail as pd WITH(NOLOCK), POSP_ExamQuestionStatusMaster as sm WITH(NOLOCK)
		WHERE pd.StatusId = sm.Id AND pd.ExamId = @ExamId AND pd.IsActive = 1 AND sm.IsActive = 1
		ORDER BY pd.QuestionNo

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail  @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
		
	END CATCH

END
