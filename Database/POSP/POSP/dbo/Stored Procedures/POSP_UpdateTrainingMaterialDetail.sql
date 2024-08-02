
  
CREATE   PROCEDURE [dbo].[POSP_UpdateTrainingMaterialDetail]       
(      
 @Id varchar(200),  
 @TrainingModuleType varchar(500),
@MaterialFormatType varchar(10),
@VideoDuration varchar(20),
@LessonNumber varchar(500),
@LessonTitle varchar(500),
@DocumentFileName varchar(500),
@PriorityIndex varchar(500)
)      
AS      
BEGIN      
 BEGIN TRY    
   
   UPDATE [dbo].[POSP_TrainingMaterialDetail] SET       
  MaterialFormatType = @MaterialFormatType,
  VideoDuration = @VideoDuration,
  LessonNumber = @LessonNumber,
  LessonTitle = @LessonTitle,
  DocumentFileName = @DocumentFileName,
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
