-- =============================================        
-- Author:  <Author, Parth>        
-- Create date: <Create Date,10,Mar-2022>        
-- Description: <Description,Admin_InsertTariningMaterialDetail>        
-- =============================================        
CREATE   PROCEDURE [dbo].[Admin_InsertTariningMaterialDetail]         
(        
 @TrainingModuleType varchar(100),          
 @LessionTitle varchar(100),    
 @FileName varchar(100),     
 @DocumentId varchar(100),  
 @MaterialFormatType varchar(100),  
 @VideoDuration varchar(100),  
 @LessonNumber varchar(100),  
 @CreatedBy varchar(100)  
)        
AS        
BEGIN        
 BEGIN TRY        
  BEGIN TRANSACTION        
 DECLARE @Index INT    
 IF(@MaterialFormatType IS NOT NULL)  
 BEGIN  
 SET @MaterialFormatType = LOWER(@MaterialFormatType)  
 END  
   
 SELECT @Index = MAX(PrioRityIndex) from [HeroPOSP].[dbo].[POSP_TrainingMaterialDetail]    
   INSERT INTO [HeroPOSP].[dbo].[POSP_TrainingMaterialDetail]        
           ([TrainingModuleType],[MaterialFormatType],[VideoDuration],[LessonNumber],[LessonTitle],[DocumentFileName],[DocumentId],  
     [PriorityIndex],[IsActive],[CreatedBy])                  
     VALUES (@TrainingModuleType, @MaterialFormatType, @VideoDuration,@LessonNumber,@LessionTitle,@FileName,@DocumentId,  
   @Index + 1,1,@CreatedBy)        
   IF @@TRANCOUNT > 0        
            COMMIT        
 END TRY                        
 BEGIN CATCH                  
 IF @@TRANCOUNT > 0        
        ROLLBACK          
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
        
END  
