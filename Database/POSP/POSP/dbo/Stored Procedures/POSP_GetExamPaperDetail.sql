-- =============================================
-- Author: <Author, ANKIT GHOSH>
-- Create date: <Create Date, 29-12-2022>
-- Description:	<Description,POSP_GetExamPaperDetail>
--POSP_ExamPaperDetail
-- =============================================
CREATE     PROCEDURE [dbo].[POSP_GetExamPaperDetail]

AS
BEGIN
	BEGIN TRY
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		SELECT Id,ExamId, QuestionNo, QuestionId, StatusId, AnswerOptionId, IsActive, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn
		FROM [dbo].[POSP_ExamPaperDetail] WITH(NOLOCK)
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
	END CATCH
END
