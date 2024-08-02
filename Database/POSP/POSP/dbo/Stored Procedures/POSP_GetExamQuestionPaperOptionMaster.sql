      
-- =============================================            
-- Author:  <Author,HARSH PATEL>            
-- Create date: <Create Date,27-12-2022>            
-- Description: <Description,GetExamQuestionPaperOption>            
-- =============================================            
CREATE        PROCEDURE [dbo].[POSP_GetExamQuestionPaperOptionMaster]              
AS            
BEGIN            
 BEGIN TRY            
  SELECT [Id],[QuestionId],[OptionIndex],[OptionValue],[IsCorrectAnswer],[IsActive],[CreatedBy],[CreatedOn],[UpdatedBy],[UpdatedOn]  FROM  [dbo].[POSP_ExamQuestionPaperOptionMaster] WITH(NOLOCK)       
  END TRY                            
 BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH            
END       

