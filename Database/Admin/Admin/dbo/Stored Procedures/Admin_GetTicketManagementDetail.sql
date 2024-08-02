-- =========================================================================================                 
-- Author:  <Author, Ankit>              
-- Create date: <Create Date,6-Apr-2023>              
-- Description: <Description, Admin_GetTicketManagementDetail>        
-- =========================================================================================     
-- exec [dbo].[Admin_GetTicketManagementDetail]
 CREATE   PROCEDURE [dbo].[Admin_GetTicketManagementDetail]               
 (              
 @TicketType VARCHAR(100) = null,  
 @SearchText VARCHAR(100) = null,  
 @RelationshipManagerIds VARCHAR(100) = null,  
 @PolicyType VARCHAR(100) = null,        
 @StartDate VARCHAR(100) = null,      
 @EndDate VARCHAR(100) = null,      
 @CurrentPageIndex INT = 1,      
 @CurrentPageSize INT = 10      
 )              
AS       
 DECLARE   
 @TicketType1 VARCHAR(100) = @TicketType,  
 @SearchText1 VARCHAR(100) = @SearchText,  
 @RelationshipManagerIds1 VARCHAR(100) = @RelationshipManagerIds,  
 @PolicyType1 VARCHAR(100) = @PolicyType,      
 @StartDate1 VARCHAR(100) = @StartDate,          
 @EndDate1 VARCHAR(100) = @EndDate,          
 @CurrentPageIndex1 INT = @CurrentPageIndex,      
 @CurrentPageSize1 INT = @CurrentPageSize      
      
 DECLARE @PreviousPageIndex1 INT = @CurrentPageIndex1 - 1,       
   @NextPageIndex1 INT =  @CurrentPageIndex1 + 1      
         
       
 DECLARE @TempDataTable TABLE(      
  TicketId VARCHAR(100),       
  POSPId VARCHAR(100),       
  POSPName VARCHAR(100),       
  MobileNumber VARCHAR(100),       
  RelationshipManager VARCHAR(100),       
  ConcernType VARCHAR(100),  
  Status VARCHAR(100),  
  CreatedOn VARCHAR(100),
  TicketAge INT
 )      
     
BEGIN              
 BEGIN TRY              
            
   IF(@CurrentPageIndex1 = -1)      
   BEGIN      
        
  INSERT @TempDataTable      
	Select HS.Id as TicketId, IU.POSPId, IU.UserName as POSPName, IU.MobileNo as MobileNumber, serUsr.UserName as RelationshipManager,
		CT.ConcernTypeName as ConcernType, HS.Status, HS.CreatedOn, DATEDIFF(day, HS.CreatedOn, GetDate()) as TicketAge
	 from [Admin_HelpAndSupport] as HS WITH(NOLOCK)  
	 LEFT Join [Admin_ConcernType] as CT WITH(NOLOCK) on CT.ConcernTypeId = HS.ConcernTypeId  
	 LEFT Join [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) on HS.UserId = IU.UserId
	 LEFT join [HeroIdentity].[dbo].[Identity_UserDetail] USD WITH(NOLOCK)ON USD.UserID = IU.UserId 
	 LEFT JOIN [HeroIdentity].[dbo].[Identity_User] serUsr WITH(NOLOCK)ON USD.ServicedByUserId = serUsr.UserId
  WHERE       
   (((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.UserName like '%' + @SearchText1 + '%'))      
   or      
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.MobileNo like '%' + @SearchText1 + '%'))
    OR ((@SearchText1 IS NULL OR @SearchText1 = '') OR (HS.Id like '%' + @SearchText1 + '%')))          
   AND       
    (      
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR       
     CAST(HS.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)      
    )  
	AND ((@RelationshipManagerIds1 IS NULL OR @RelationshipManagerIds1 = '') OR (USD.ServicedByUserId = @RelationshipManagerIds1))
 ORDER BY HS.CreatedOn DESC       
          
  SELECT TicketId, POSPId, POSPName, MobileNumber, RelationshipManager, ConcernType, Status, CreatedOn, TicketAge
  FROM @TempDataTable ORDER BY CreatedOn desc      
        
  SELECT 0 as CurrentPageIndex, 0 as PreviousPageIndex, 0 as NextPageIndex, 0 as CurrentPageSize, 0 as TotalRecord       
      
   END      
   ELSE      
   BEGIN      
        
  INSERT @TempDataTable      
  Select HS.Id as TicketId, IU.POSPId, IU.UserName as POSPName, IU.MobileNo as MobileNumber, serUsr.UserName as RelationshipManager,
		CT.ConcernTypeName as ConcernType, HS.Status, HS.CreatedOn, DATEDIFF(day, HS.CreatedOn, GetDate()) as TicketAge
	 from [Admin_HelpAndSupport] as HS WITH(NOLOCK)  
	  LEFT Join [Admin_ConcernType] as CT WITH(NOLOCK) on CT.ConcernTypeId = HS.ConcernTypeId  
	 LEFT Join [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) on HS.UserId = IU.UserId
	 LEFT join [HeroIdentity].[dbo].[Identity_UserDetail] USD WITH(NOLOCK)ON USD.UserID = IU.UserId 
	 LEFT JOIN [HeroIdentity].[dbo].[Identity_User] serUsr WITH(NOLOCK)ON USD.ServicedByUserId = serUsr.UserId
  WHERE       
   (((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.UserName like '%' + @SearchText1 + '%'))      
   or      
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.MobileNo like '%' + @SearchText1 + '%'))
    OR ((@SearchText1 IS NULL OR @SearchText1 = '') OR (HS.Id like '%' + @SearchText1 + '%')))       
   AND       
    (      
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR 
		CAST(HS.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)) 
	AND ((@RelationshipManagerIds1 IS NULL OR @RelationshipManagerIds1 = '') OR (USD.ServicedByUserId = @RelationshipManagerIds1))
   ORDER BY HS.CreatedOn DESC       
     
     
      
  Select HS.Id as TicketId, IU.POSPId, IU.UserName as POSPName, IU.MobileNo as MobileNumber, serUsr.UserName as RelationshipManager,
		CT.ConcernTypeName as ConcernType, HS.Status, HS.CreatedOn, DATEDIFF(day, HS.CreatedOn, GetDate()) as TicketAge
	 from [Admin_HelpAndSupport] as HS WITH(NOLOCK)  
	 LEFT Join [Admin_ConcernType] as CT WITH(NOLOCK) on CT.ConcernTypeId = HS.ConcernTypeId  
	 LEFT Join [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) on HS.UserId = IU.UserId
	 LEFT join [HeroIdentity].[dbo].[Identity_UserDetail] USD WITH(NOLOCK)ON USD.UserID = IU.UserId 
	 LEFT JOIN [HeroIdentity].[dbo].[Identity_User] serUsr WITH(NOLOCK)ON USD.ServicedByUserId = serUsr.UserId
  WHERE       
   (((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.UserName like '%' + @SearchText1 + '%'))      
   or      
   ((@SearchText1 IS NULL OR @SearchText1 = '') OR (IU.MobileNo like '%' + @SearchText1 + '%'))
   OR ((@SearchText1 IS NULL OR @SearchText1 = '') OR (HS.Id like '%' + @SearchText1 + '%')))     
   AND       
    (      
     ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR 
		CAST(HS.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date))
	AND ((@RelationshipManagerIds1 IS NULL OR @RelationshipManagerIds1 = '') OR (USD.ServicedByUserId = @RelationshipManagerIds1))
  ORDER BY HS.CreatedOn DESC  OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY      
        
      
  SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,      
      @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,      
      (SELECT COUNT(TicketId) from @TempDataTable) as TotalRecord      
      
   END        
 END TRY                              
 BEGIN CATCH                
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                          
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                          
  SET @ErrorDetail=ERROR_MESSAGE()                                          
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                           
 END CATCH              
           
END   