-- =========================================================================================               
-- Author:  <Author, Ankit>            
-- Create date: <Create Date,6-Apr-2023>            
-- Description: <Description, Admin_GetNotificationList>      
-- =========================================================================================               
 CREATE PROCEDURE [dbo].[Admin_GetNotificationList]             
 (            
   @SearchText VARCHAR(100) = null,    
   @RecipientTypeId VARCHAR(MAX) = null,   
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
  NotificationId VARCHAR(100),     
  Title VARCHAR(100),     
  RecipientType VARCHAR(100),     
  AlertType VARCHAR(100),     
  CreatedOn VARCHAR(100),     
  IsPublished bit,
  Description VARCHAR(MAX),
  UserIds VARCHAR(MAX)
 )    
  
  
DECLARE @SpitTempTable TABLE(  
 RecipientId VARCHAR(MAX)  
)  
IF (@RecipientTypeId IS NOT NULL OR @RecipientTypeId <> '' OR @RecipientTypeId != '')  
BEGIN  
 INSERT INTO @SpitTempTable(RecipientId) SELECT value FROM STRING_SPLIT(@RecipientTypeId,',')  
END  
--select * from @SpitTempTable  
BEGIN            
 BEGIN TRY            
          
   IF(@CurrentPageIndex1 = -1)    
   BEGIN    
      
  INSERT @TempDataTable    
  Select ND.NotificationId, ND.NotificationTitle as Title, Rt.RecipientType, AT.AlertType, ND.CreatedOn, ND.IsPublished,
  ND.Description as Description, Nd.RecipientUserids as UserIds
  from Admin_Notification as ND WITH(NOLOCK)   
  Left Join Admin_AlertType as AT WITH(NOLOCK) on AT.AlertTypeId = ND.AlertTypeId    
  Left Join Admin_RecipientType as Rt WITH(NOLOCK) on RT.RecipientTypeId = Nd.RecipientId    
  WHERE     
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (Nd.NotificationTitle like '%' + @SearchText1 + '%'))    
   or    
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (AT.AlertType like '%' + @SearchText1 + '%'))    
   or    
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (Rt.RecipientType like '%' + @SearchText1 + '%'))    
   AND     
    (    
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(ND.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
    )   
 AND ((@RecipientTypeId IS NULL OR @RecipientTypeId = '') OR (ND.RecipientId in (SELECT RecipientId from @SpitTempTable)))    
  ORDER BY ND.CreatedOn DESC     
        
  SELECT NotificationId, Title, RecipientType, AlertType, CreatedOn, IsPublished, Description, UserIds
  FROM @TempDataTable ORDER BY CreatedOn desc    
      
  SELECT 0 as CurrentPageIndex, 0 as PreviousPageIndex, 0 as NextPageIndex, 0 as CurrentPageSize, 0 as TotalRecord     
    
   END    
   ELSE    
   BEGIN    
      
  INSERT @TempDataTable    
  Select ND.NotificationId, ND.NotificationTitle as Title, Rt.RecipientType, AT.AlertType, ND.CreatedOn, ND.IsPublished,
		 ND.Description as Description, Nd.RecipientUserids as UserIds
  from Admin_Notification as ND WITH(NOLOCK)   
  Left Join Admin_AlertType as AT WITH(NOLOCK) on AT.AlertTypeId = ND.AlertTypeId    
  Left Join Admin_RecipientType as Rt WITH(NOLOCK) on RT.RecipientTypeId = Nd.RecipientId    
  WHERE     
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (Nd.NotificationTitle like '%' + @SearchText1 + '%'))    
   or    
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (AT.AlertType like '%' + @SearchText1 + '%'))    
   or    
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (Rt.RecipientType like '%' + @SearchText1 + '%'))    
   AND     
    (    
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(ND.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
    )    
 AND ((@RecipientTypeId IS NULL OR @RecipientTypeId = '') OR (ND.RecipientId in (SELECT RecipientId from @SpitTempTable)))   
  ORDER BY ND.CreatedOn DESC     
        
  ---SELECT NotificationId, Title, RecipientType, AlertType, CreatedOn, IsPublished FROM @TempDataTable ORDER BY CreatedOn desc    
      
    
    
  Select ND.NotificationId, ND.NotificationTitle as Title, Rt.RecipientType, AT.AlertType, ND.CreatedOn, ND.IsPublished,
		 ND.Description as Description, Nd.RecipientUserids as UserIds
  from Admin_Notification as ND WITH(NOLOCK)   
  Left Join Admin_AlertType as AT WITH(NOLOCK) on AT.AlertTypeId = ND.AlertTypeId    
  Left Join Admin_RecipientType as Rt WITH(NOLOCK) on RT.RecipientTypeId = Nd.RecipientId    
  WHERE     
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (Nd.NotificationTitle like '%' + @SearchText1 + '%'))    
   or    
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (AT.AlertType like '%' + @SearchText1 + '%'))    
   or    
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (Rt.RecipientType like '%' + @SearchText1 + '%'))    
   AND     
    (    
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(ND.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
    )   
 AND ((@RecipientTypeId IS NULL OR @RecipientTypeId = '') OR (ND.RecipientId in (SELECT RecipientId from @SpitTempTable)))   
  ORDER BY ND.CreatedOn DESC  OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY    
      
    
  SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,    
      @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,    
      (SELECT COUNT(NotificationId) from @TempDataTable) as TotalRecord    
    
   END      
 END TRY                            
 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH            
         
END 
