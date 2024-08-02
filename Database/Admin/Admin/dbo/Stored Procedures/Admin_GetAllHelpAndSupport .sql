-- =========================================================================================                 
-- Author:  <Author, Ankit>              
-- Create date: <Create Date,2-Feb-2023>              
-- Description: <Description, Admin_GetAllHelpAndSupport >        
-- =========================================================================================                 
 CREATE     PROCEDURE [dbo].[Admin_GetAllHelpAndSupport ]               
 (              
   @SearchText VARCHAR(100) = null,  
   @UserId VARCHAR(100) = null,
   @StartDate VARCHAR(100) = null,      
   @EndDate VARCHAR(100) = null,      
   @CurrentPageIndex INT = 1,      
   @CurrentPageSize INT = 10      
 )              
AS       
 DECLARE @SearchText1 VARCHAR(100) = @SearchText,      
   @StartDate1 VARCHAR(100) = @StartDate,          
   @EndDate1 VARCHAR(100) = @EndDate,          
   @CurrentPageIndex1 INT = @CurrentPageIndex,      
   @CurrentPageSize1 INT = @CurrentPageSize      
      
 DECLARE @PreviousPageIndex1 INT = @CurrentPageIndex1 - 1,       
   @NextPageIndex1 INT =  @CurrentPageIndex1 + 1      
         
       
 DECLARE @TempDataTable TABLE(      
  Id VARCHAR(100),      
  ConcernTypeId VARCHAR(100),      
  ConcernType VARCHAR(100),      
  SubConcernTypeId VARCHAR(100),      
  SubConcernType VARCHAR(100),        
  SubjectText VARCHAR(500),       
  DetailText VARCHAR(MAX),       
  Status VARCHAR(100),       
  IsActive bit,      
  CreatedOn VARCHAR(100),    
  CreatedOnDate VARCHAR(100),    
  CreatedOnTime VARCHAR(100)    
 )      
      
BEGIN              
 BEGIN TRY              
            
   IF(@CurrentPageIndex1 = -1)      
   BEGIN      
        
  INSERT @TempDataTable      
  Select Hs.Id, HS.ConcernTypeId, CT.ConcernTypeName as ConcernType, HS.SubConcernTypeId, SCT.SubConcernTypeName as SubConcernType,    
   HS.SubjectText, HS.DetailText,HS.Status,HS.IsActive, HS.CreatedOn,
   Case when HS.UpdatedOn is null then (convert(date, HS.CreatedOn))
   else (convert(date, HS.UpdatedOn)) END as CreatedOnDate,
   Case when HS.UpdatedOn is null then convert(varchar(8), convert(time, HS.CreatedOn))
   else convert(varchar(8), convert(time, HS.UpdatedOn)) END as CreatedOnTime    
   from [HeroAdmin].[dbo].[Admin_HelpAndSupport] (NOLOCK) as HS      
   left join [HeroAdmin].[dbo].[Admin_ConcernType] (NOLOCK) as CT on CT.ConcernTypeId = HS.ConcernTypeId      
   left join [HeroAdmin].[dbo].[Admin_SubConcernType] (NOLOCK) as SCT on SCT.SubConcernTypeId = HS.SubConcernTypeId       
   WHERE        
    ((@SearchText1 IS NULL OR @SearchText1 = '') OR (CT.ConcernTypeName like '%' + @SearchText1 + '%')) 
	AND
	((@UserId IS NULL OR @UserId = '') OR (HS.UserId = @UserId))
   AND       
    (      
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR       
     CAST(HS.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)      
    )         
  ORDER BY HS.CreatedOn DESC       
          
  SELECT Id, ConcernTypeId, ConcernType, SubConcernTypeId, SubConcernType, SubjectText, DetailText, CreatedOn, Status, IsActive,    
   CreatedOnDate,CreatedOnTime    
  FROM @TempDataTable ORDER BY CreatedOn desc      
        
  SELECT 0 as CurrentPageIndex, 0 as PreviousPageIndex, 0 as NextPageIndex, 0 as CurrentPageSize, 0 as TotalRecord       
      
   END      
   ELSE      
   BEGIN      
      
  INSERT @TempDataTable      
  Select Hs.Id, HS.ConcernTypeId, CT.ConcernTypeName as ConcernType, HS.SubConcernTypeId, SCT.SubConcernTypeName as SubConcernType,    
   HS.SubjectText, HS.DetailText,HS.Status,HS.IsActive, HS.CreatedOn,
   Case when HS.UpdatedOn is null then (convert(date, HS.CreatedOn))
   else (convert(date, HS.UpdatedOn)) END as CreatedOnDate,
   Case when HS.UpdatedOn is null then convert(varchar(8), convert(time, HS.CreatedOn))
   else convert(varchar(8), convert(time, HS.UpdatedOn)) END as CreatedOnTime   
   from [HeroAdmin].[dbo].[Admin_HelpAndSupport] (NOLOCK) as HS      
   left join [HeroAdmin].[dbo].[Admin_ConcernType] (NOLOCK) as CT on CT.ConcernTypeId = HS.ConcernTypeId      
   left join [HeroAdmin].[dbo].[Admin_SubConcernType] (NOLOCK) as SCT on SCT.SubConcernTypeId = HS.SubConcernTypeId      
   WHERE         
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (CT.ConcernTypeName like '%' + @SearchText1 + '%')) 
   AND
	((@UserId IS NULL OR @UserId = '') OR (HS.UserId = @UserId))
   AND       
    (      
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR       
     CAST(HS.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)      
    )         
  ORDER BY HS.CreatedOn DESC       
      
     
  Select Hs.Id, HS.ConcernTypeId, CT.ConcernTypeName as ConcernType, HS.SubConcernTypeId, SCT.SubConcernTypeName as SubConcernType,    
   HS.SubjectText, HS.DetailText,HS.Status,HS.IsActive, HS.CreatedOn,
   Case when HS.UpdatedOn is null then (convert(date, HS.CreatedOn))
   else (convert(date, HS.UpdatedOn)) END as CreatedOnDate,
   Case when HS.UpdatedOn is null then convert(varchar(8), convert(time, HS.CreatedOn))
   else convert(varchar(8), convert(time, HS.UpdatedOn)) END as CreatedOnTime    
   from [HeroAdmin].[dbo].[Admin_HelpAndSupport] (NOLOCK) as HS      
   left join [HeroAdmin].[dbo].[Admin_ConcernType] (NOLOCK) as CT on CT.ConcernTypeId = HS.ConcernTypeId      
   left join [HeroAdmin].[dbo].[Admin_SubConcernType] (NOLOCK) as SCT on SCT.SubConcernTypeId = HS.SubConcernTypeId      
  WHERE        
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (CT.ConcernTypeName like '%' + @SearchText1 + '%'))  
   AND
	((@UserId IS NULL OR @UserId = '') OR (HS.UserId = @UserId))
   AND       
    (      
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR       
     CAST(HS.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)      
    )         
  ORDER BY HS.CreatedOn DESC OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY      
        
      
  SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,      
      @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,      
      (SELECT COUNT(Id) from @TempDataTable) as TotalRecord      
      
   END        
 END TRY                              
 BEGIN CATCH                
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                          
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                          
  SET @ErrorDetail=ERROR_MESSAGE()                                          
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                           
 END CATCH              
           
END   