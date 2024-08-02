

Create   PROCEDURE [dbo].[POSP_InsertTrainingInstructionsDetail] 
(
@InstructionDetail varchar(200),
@PriorityIndex varchar(200) = NULL
)
AS
BEGIN
	BEGIN TRY

			INSERT INTO POSP_TrainingInstructionsDetail (InstructionDetail, PriorityIndex)
			VALUES(@InstructionDetail, @PriorityIndex)
		

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
