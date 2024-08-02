-- =============================================          
-- Author:  <Author,HARSH PATEL>          
-- Create date: <Create Date,28-12-2022>          
-- Description: <Description,DeleteExamQuestionPaperOptionMaster>          
-- ============================================= 
CREATE    PROCEDURE [dbo].[POSP_DeleteExamQuestionPaperOptionMaster]    
(    
@Id varchar(200)  
)    
AS    
BEGIN    
 UPDATE [dbo].[POSP_ExamQuestionPaperOptionMaster]    
 SET     
  IsActive = 0,    
  UpdatedOn = GetDate()    
 WHERE Id = @Id    
END    
