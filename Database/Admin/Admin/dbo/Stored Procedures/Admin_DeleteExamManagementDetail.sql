

CREATE   PROCEDURE [dbo].[Admin_DeleteExamManagementDetail]       
(      
 @QuestionId varchar(100)   
)      
AS      
BEGIN      
 BEGIN TRY  

	DELETE FROM [HeroPOSP].[dbo].[POSP_ExamQuestionPaperOptionMaster]  WHERE QuestionId = @QuestionId;
	
	DELETE FROM [HeroPOSP].[dbo].[POSP_ExamQuestionPaperMaster]  WHERE Id = @QuestionId;
 
 END TRY                      


 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
      
END 
