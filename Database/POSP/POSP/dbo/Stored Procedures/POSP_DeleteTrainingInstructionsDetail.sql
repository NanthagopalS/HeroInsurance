

CREATE   PROCEDURE [dbo].[POSP_DeleteTrainingInstructionsDetail]    
(    
@Id varchar(200)  
)    
AS    
BEGIN    
 UPDATE [dbo].[POSP_TrainingInstructionsDetail]    
 SET     
  IsActive = 0,    
  UpdatedOn = GetDate()    
 WHERE Id = @Id    
END    
