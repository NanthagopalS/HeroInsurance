-- =========================================================================================               
-- Author:  <Author, Ankit>            
-- Create date: <Create Date,2-Feb-2023>            
-- Description: <Description, Admin_GetAllBUDetail>      
-- =========================================================================================               
 CREATE    PROCEDURE [dbo].[Admin_GetAllBUDetail]             
 (            
   @BuName VARCHAR(100) = null,    
   @StartDate VARCHAR(100) = null,    
   @EndDate VARCHAR(100) = null,    
   @IsActive bit = NULL,    
   @CurrentPageIndex INT = 1,    
   @CurrentPageSize INT = 10    
 )            
AS     
 DECLARE @BuName1 VARCHAR(100) = @BuName,    
   @StartDate1 VARCHAR(100) = @StartDate,        
   @EndDate1 VARCHAR(100) = @EndDate,        
   @CurrentPageIndex1 INT = @CurrentPageIndex,    
   @CurrentPageSize1 INT = @CurrentPageSize    
    
 DECLARE @PreviousPageIndex1 INT = @CurrentPageIndex1 - 1,     
   @NextPageIndex1 INT =  @CurrentPageIndex1 + 1    
       
     
 DECLARE @TempDataTable TABLE(    
  BuId VARCHAR(100),     
  BuName VARCHAR(100),     
  HierarchyLevelId VARCHAR(100),     
  HierarchyLevelName VARCHAR(100),     
  CreatedOn VARCHAR(100),     
  IsActive bit,    
  SalesandSupport bit    
 )    
    
BEGIN            
 BEGIN TRY            
          
   IF(@CurrentPageIndex1 = -1)    
   BEGIN    
      
  INSERT @TempDataTable    
  SELECT bu.BuId, bu.BuName, bul.BULevelId as HierarchyLevelId, bul.BULevelName as HierarchyLevelName,bu.CreatedOn, bu.IsActive, bu.SalesandSupport      
  FROM [Admin_Bu] (NOLOCK) bu    
   INNER JOIN [Admin_BuLevel] (NOLOCK) bul on bul.BuLevelId = bu.BuLevelId      
  WHERE      
   ((@BUName1 IS NULL OR @BuName1 = '') OR (bu.BuName like '%' + @BuName1 + '%'))     
   AND     
    (    
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(bu.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
    )       
   AND ((@IsActive IS NULL OR @IsActive = '' OR @IsActive = null) OR bu.IsActive = @IsActive)    
  ORDER BY bu.CreatedOn DESC     
        
  SELECT BuId, BuName, HierarchyLevelId, HierarchyLevelName, CreatedOn, IsActive, SalesandSupport FROM @TempDataTable ORDER BY CreatedOn desc    
      
  SELECT 0 as CurrentPageIndex, 0 as PreviousPageIndex, 0 as NextPageIndex, 0 as CurrentPageSize, 0 as TotalRecord     
    
   END    
   ELSE    
   BEGIN    
      
  INSERT @TempDataTable    
  SELECT bu.BuId, bu.BuName, bul.BULevelId as HierarchyLevelId, bul.BULevelName as HierarchyLevelName,bu.CreatedOn, bu.IsActive, bu.SalesandSupport      
  From [Admin_Bu] (NOLOCK) bu    
   INNER JOIN [Admin_BuLevel] (NOLOCK) bul on bul.BuLevelId = bu.BuLevelId      
  WHERE      
   ((@BUName1 IS NULL OR @BuName1 = '') OR (bu.BuName like '%' + @BuName1 + '%'))     
   AND     
    (    
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(bu.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
    )       
   AND ((@IsActive IS NULL OR @IsActive = '' OR @IsActive = null) OR bu.IsActive = @IsActive)    
  ORDER BY bu.CreatedOn DESC     
    
    
  SELECT bu.BuId, bu.BuName, bul.BULevelId as HierarchyLevelId, bul.BULevelName as HierarchyLevelName,bu.CreatedOn, bu.IsActive, bu.SalesandSupport      
  From [Admin_Bu] (NOLOCK) bu    
   INNER JOIN [Admin_BuLevel] (NOLOCK) bul on bul.BuLevelId = bu.BuLevelId      
  WHERE      
   ((@BUName1 IS NULL OR @BuName1 = '') OR (bu.BuName like '%' + @BuName1 + '%'))     
   AND     
    (    
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(bu.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
    )       
   AND ((@IsActive IS NULL OR @IsActive = '' OR @IsActive = null) OR bu.IsActive = @IsActive)    
  ORDER BY bu.CreatedOn DESC OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY    
      
    
  SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,    
      @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,    
      (SELECT COUNT(BuId) from @TempDataTable) as TotalRecord    
    
   END      
 END TRY                            
 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH            
         
END 
