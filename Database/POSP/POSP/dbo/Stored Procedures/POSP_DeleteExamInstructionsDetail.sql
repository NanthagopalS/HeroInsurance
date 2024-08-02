
CREATE    PROCEDURE [dbo].[POSP_DeleteExamInstructionsDetail]    
(    
@Id varchar(200)  
)    
AS    
BEGIN    
 UPDATE [dbo].[POSP_ExamInstructionsDetail]    
 SET     
  IsActive = 0,    
  UpdatedOn = GetDate()    
 WHERE Id = @Id    
END    
