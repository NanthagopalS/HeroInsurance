
CREATE PROCEDURE [dbo].[POSP_DeleteTrainingMaterialDetail]    
(    
@Id varchar(200)  
)    
AS    
BEGIN    
 UPDATE [dbo].[POSP_TrainingMaterialDetail]    
 SET     
  IsActive = 0,    
  UpdatedOn = GetDate()    
 WHERE Id = @Id    
END    
