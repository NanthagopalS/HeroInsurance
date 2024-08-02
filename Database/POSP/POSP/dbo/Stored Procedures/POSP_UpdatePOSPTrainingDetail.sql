
CREATE PROCEDURE [dbo].[POSP_UpdatePOSPTrainingDetail] 
(
	@UserId VARCHAR(500),
	@TrainingModuleType VARCHAR(50) --(General Insurance / Life Insurance)
)
AS
BEGIN

	BEGIN TRY
		
		If(@TrainingModuleType = 'General Insurance')
		BEGIN
		
			UPDATE [dbo].[POSP_Training] SET GeneralInsuranceEndDateTime = GETDATE(), LifeInsuranceStartDateTime =  GETDATE(), IsGeneralInsuranceCompleted = 1, UpdatedOn = GETDATE() WHERE IsActive = 1 AND UserId = @UserId
		
		END
		ELSE IF(@TrainingModuleType = 'Life Insurance')
		BEGIN
			
			UPDATE [dbo].[POSP_Training] SET LifeInsuranceEndDateTime = GETDATE(), IsLifeInsuranceCompleted = 1, IsTrainingCompleted = 1, UpdatedOn = GETDATE() WHERE IsActive = 1 AND UserId = @UserId
		
		END		

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
