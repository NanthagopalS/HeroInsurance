

CREATE    PROCEDURE [dbo].[POSP_GetTrainingInstructionsDetail]      
AS      
BEGIN      
 BEGIN TRY      
	SELECT Id, InstructionDetail, PriorityIndex, IsActive, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn
	FROM  [dbo].[POSP_TrainingInstructionsDetail] WITH(NOLOCK) ORDER BY PriorityIndex
  END TRY                      
 BEGIN CATCH                
             
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
END 
