
CREATE PROCEDURE [dbo].[POSP_InsertPOSPTrainingProgressDetail] 
(
	@UserId VARCHAR(500),
	@TrainingMaterialId VARCHAR(500)
)
AS
BEGIN

	BEGIN TRY
		
		DECLARE @TrainingId VARCHAR(500) = NULL
		
		SET @TrainingId = (SELECT TOP(1) Id FROM [dbo].[POSP_Training] WITH(NOLOCK) WHERE UserId = @UserId AND IsActive = 1 ORDER BY GeneralInsuranceStartDateTime DESC)
		
		IF EXISTS(SELECT TOP(1) Id FROM [dbo].[POSP_TrainingProgressDetail] WITH(NOLOCK) WHERE IsActive = 1 AND TrainingId = @TrainingId AND TrainingMaterialId = @TrainingMaterialId) 
		BEGIN
		
			UPDATE [dbo].[POSP_TrainingProgressDetail] SET UpdatedOn = GETDATE() WHERE IsActive = 1 AND TrainingId = @TrainingId AND TrainingMaterialId = @TrainingMaterialId
		
		END
		ELSE
		BEGIN
		
			INSERT INTO [dbo].[POSP_TrainingProgressDetail] (TrainingId, TrainingMaterialId) VALUES (@TrainingId, @TrainingMaterialId)
		END
		
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
