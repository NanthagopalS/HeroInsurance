




CREATE   PROCEDURE [dbo].[POSP_InsertTrainingMaterialDetail] 
(
@TrainingModuleType varchar(500),
@MaterialFormatType varchar(10),
@VideoDuration varchar(20),
@LessonNumber varchar(500),
@LessonTitle varchar(500),
@DocumentFileName varchar(500),
@PriorityIndex int
)
AS
BEGIN
	BEGIN TRY

			INSERT INTO POSP_TrainingMaterialDetail (TrainingModuleType, MaterialFormatType, VideoDuration, LessonNumber, LessonTitle, DocumentFileName, PriorityIndex, CreatedOn)
			VALUES(@TrainingModuleType, @MaterialFormatType, @VideoDuration, @LessonNumber, @LessonTitle, @DocumentFileName, @PriorityIndex, GETDATE() )
		

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
