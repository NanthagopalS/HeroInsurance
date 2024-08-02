  
CREATE PROCEDURE [dbo].[POSP_UpdateExamInstructionsDetail]       
(      
 @Id varchar(200),  
 @InstructionDetail varchar(200),  
 @PriorityIndex varchar(200)
)      
AS      
BEGIN      
 BEGIN TRY    
   
   UPDATE [dbo].[POSP_ExamInstructionsDetail] SET       
  InstructionDetail = @InstructionDetail,  
  PriorityIndex = @PriorityIndex,
  UpdatedOn = GetDate() 
 WHERE Id = @Id   
    
 END TRY                      
 BEGIN CATCH                
             
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList      
        
 END CATCH      
      
END 
