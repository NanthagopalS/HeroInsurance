-- =========================================================================================             
-- Author:  <Author, Ankit>          
-- Create date: <Create Date,23-Feb-2023>          
-- Description: <Description, Admin_GetAllTrainingManagementDetails>    
-- =========================================================================================             
 CREATE       PROCEDURE [dbo].[Admin_GetAllTrainingManagementDetails]           
 (          
   @SearchText VARCHAR(100) = Null,          
   @Category VARCHAR(100) = Null,    
   @StartDate VARCHAR(100) = Null,      
   @EndDate VARCHAR(100) = Null,      
   @CurrentPageIndex INT = 1,    
   @CurrentPageSize INT = 10    
 )          
AS          
 DECLARE @SearchText1 VARCHAR(100) = @SearchText,    
   @Category1 VARCHAR(100) = @Category,    
   @StartDate1 VARCHAR(100) = @StartDate,        
   @EndDate1 VARCHAR(100) = @EndDate,        
   @CurrentPageIndex1 INT = @CurrentPageIndex,    
   @CurrentPageSize1 INT = @CurrentPageSize    
    
 DECLARE @PreviousPageIndex1 INT = @CurrentPageIndex1 - 1,     
   @NextPageIndex1 INT =  @CurrentPageIndex1 + 1    
       
     
 DECLARE @TempDataTable TABLE(    
  TrainingMaterialId VARCHAR(100),     
  Category VARCHAR(100),     
  DocumentType VARCHAR(100),     
  Duration VARCHAR(100),     
  LessonNumber VARCHAR(100),    
  LessonTitle VARCHAR(100),     
  Content VARCHAR(100),    
  PriorityIndex VARCHAR(100),    
  IsActive bit,    
  CreatedOn VARCHAR(100)    
 )    
    
BEGIN          
 BEGIN TRY     
    
 IF(@CurrentPageIndex1 = -1)    
 BEGIN    
    
  INSERT @TempDataTable    
  SELECT Id as TrainingMaterialId, TrainingModuleType as Category, MaterialFormatType as DocumentType, VideoDuration as Duration, LessonNumber, LessonTitle, DocumentFileName as Content, PriorityIndex, IsActive  
  , CASE     
     WHEN UpdatedOn IS NULL    
      THEN CreatedOn    
     ELSE UpdatedOn    
     END AS CreatedOn    
  FROM [HeroPOSP].[dbo].[POSP_TrainingMaterialDetail] (NOLOCK)    
  WHERE    
  ((@SearchText1 IS NULL OR @SearchText1 = '') OR (DocumentFileName like '%' + @SearchText1 + '%'))     
  AND ((@Category1 IS NULL OR @Category1 = '' OR @Category1 = 'All') OR (TrainingModuleType = @Category1))      
  AND     
  (    
   ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
   CAST(CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
  )       
  ORDER BY CreatedOn DESC      
    
  SELECT TrainingMaterialId, Category, DocumentType, Duration, LessonNumber, LessonTitle, Content, PriorityIndex, IsActive, CreatedOn FROM @TempDataTable ORDER BY CreatedOn desc    
      
  SELECT 0 as CurrentPageIndex, 0 as PreviousPageIndex, 0 as NextPageIndex, 0 as CurrentPageSize, 0 as TotalRecord    
    
 END    
 ELSE    
 BEGIN    
    
  INSERT @TempDataTable    
  SELECT Id as TrainingMaterialId, TrainingModuleType as Category, MaterialFormatType as DocumentType, VideoDuration as Duration, LessonNumber, LessonTitle, DocumentFileName as Content, PriorityIndex, IsActive,  
  CASE     
     WHEN UpdatedOn IS NULL    
      THEN CreatedOn    
     ELSE UpdatedOn    
     END AS CreatedOn    
  FROM [HeroPOSP].[dbo].[POSP_TrainingMaterialDetail] (NOLOCK)    
  WHERE    
  ((@SearchText1 IS NULL OR @SearchText1 = '') OR (DocumentFileName like '%' + @SearchText1 + '%'))     
  AND ((@Category1 IS NULL OR @Category1 = '' OR @Category1 = 'All') OR (TrainingModuleType = @Category1))      
  AND     
  (    
   ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
   CAST(CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
  )       
  ORDER BY CreatedOn DESC    
    
    
  SELECT Id as TrainingMaterialId, TrainingModuleType as Category, MaterialFormatType as DocumentType, VideoDuration as Duration, LessonNumber, LessonTitle, DocumentFileName as Content, PriorityIndex, IsActive  
   ,CASE     
     WHEN UpdatedOn IS NULL    
      THEN CreatedOn    
     ELSE UpdatedOn    
     END AS CreatedOn    
  FROM [HeroPOSP].[dbo].[POSP_TrainingMaterialDetail] (NOLOCK)    
  WHERE    
  ((@SearchText1 IS NULL OR @SearchText1 = '') OR (DocumentFileName like '%' + @SearchText1 + '%'))     
  AND ((@Category1 IS NULL OR @Category1 = '' OR @Category1 = 'All') OR (TrainingModuleType = @Category1))      
  AND     
  (    
   ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
   CAST(CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
  )       
  ORDER BY CreatedOn DESC OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY    
      
    
  SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,    
      @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,    
      (SELECT COUNT(TrainingMaterialId) from @TempDataTable) as TotalRecord    
    
 END    
     
 END TRY                          
 BEGIN CATCH            
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                                       
 END CATCH          
       
END    
