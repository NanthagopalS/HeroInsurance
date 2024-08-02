
CREATE     PROCEDURE [dbo].[Admin_GetExamParticularQuestionDetail] 
(
	@QuestionId VARCHAR(100) = NULL
)
AS

BEGIN
 BEGIN TRY 

	SELECT DISTINCT Id AS OptionId, OptionIndex, OptionValue, IsCorrectAnswer
	FROM [HeroPOSP].[dbo].[POSP_ExamQuestionPaperOptionMaster] 
	WHERE QuestionId = @QuestionId
	ORDER BY OptionIndex

  END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END
