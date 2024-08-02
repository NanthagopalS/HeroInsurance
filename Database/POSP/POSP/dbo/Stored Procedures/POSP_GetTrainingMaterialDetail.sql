      
-- =============================================            
-- Author:  <Author,HARSH PATEL>            
-- Create date: <Create Date,27-12-2022>            
-- Description: <Description,GetTrainingMaterialDetail>            
-- =============================================            
CREATE      PROCEDURE [dbo].[POSP_GetTrainingMaterialDetail]  
	@ModuleType VARCHAR(50),
	@TrainingId VARCHAR(50)
AS            
BEGIN            
 BEGIN TRY

	IF ISNULL(@TrainingId, '') <> ''
	BEGIN
  
	  SELECT tm.Id, tm.TrainingModuleType, tm.MaterialFormatType, tm.VideoDuration, tm.LessonNumber, tm.LessonTitle, tm.DocumentFileName, tm.DocumentId, tm.PriorityIndex, tm.IsActive, tm.CreatedBy, tm.CreatedOn, tm.UpdatedBy, tm.UpdatedOn, tp.IsTrainingCompleted
	  FROM POSP_TrainingMaterialDetail as tm WITH(NOLOCK) 
		LEFT JOIN POSP_TrainingProgressDetail as tp WITH(NOLOCK) ON tp.TrainingMaterialId = tm.Id AND tp.TrainingId = @TrainingId AND tp.IsActive = 1
	  WHERE tm.TrainingModuleType = @ModuleType AND tm.IsActive = 1
	  ORDER BY tm.PriorityIndex ASC

	END
	ELSE
	BEGIN
		SELECT Id, TrainingModuleType, MaterialFormatType, VideoDuration, LessonNumber, LessonTitle, DocumentFileName, DocumentId, PriorityIndex, IsActive, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn, '0' as IsTrainingCompleted
	  FROM POSP_TrainingMaterialDetail WITH(NOLOCK) WHERE TrainingModuleType = @ModuleType  ORDER BY PriorityIndex ASC  
	END  
  END TRY                            
 BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH            
END   
