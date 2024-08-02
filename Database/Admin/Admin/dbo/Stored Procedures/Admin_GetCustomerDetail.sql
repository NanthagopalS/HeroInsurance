
           
CREATE  PROCEDURE [dbo].[Admin_GetCustomerDetail]           
(      
 @CustomerType VARCHAR(100) = 'All Customers',     
 @SearchText VARCHAR(100) = NULL,        
 @PolicyType VARCHAR(100) = NULL,       
 @StartDate VARCHAR(100) = NULL,      
 @EndDate VARCHAR(100) = NULL,    
 @CurrentPageIndex INT = 1,    
 @CurrentPageSize INT = 10    
 )     
AS          
 DECLARE 
   @CustomerType1 VARCHAR(100) = @CustomerType,     
   @SearchText1 VARCHAR(100) = @SearchText,     
   @PolicyType1 VARCHAR(100) = @PolicyType,        
   @StartDate1 VARCHAR(100) = @StartDate,      
   @EndDate1 VARCHAR(100) = @EndDate,    
   @CurrentPageIndex1 INT = @CurrentPageIndex,    
   @CurrentPageSize1 INT = @CurrentPageSize    
    
 DECLARE @PreviousPageIndex1 INT = @CurrentPageIndex1 - 1,     
   @NextPageIndex1 INT =  @CurrentPageIndex1 + 1    
    
 DECLARE @TempDataTable TABLE(    
   UserId VARCHAR(100), 
   CustomerName VARCHAR(100),     
   MobileNo VARCHAR(100),     
   EmailId VARCHAR(100),
   PolicyNo VARCHAR(100),
   PolicyType VARCHAR(100),
   PolicyIssueDate VARCHAR(100),
   Premium VARCHAR(100),
   GeneratedOn VARCHAR(100),    
   ExpiringOn VARCHAR(100),    
   RequestStatus VARCHAR(100),     
   CreatedBy  VARCHAR(100) -- Work Pending....    
  )    
     
BEGIN          
 BEGIN TRY      
    
 IF(@CustomerType = 'All Customers')    
 BEGIN    
      
  INSERT @TempDataTable    
  SELECT IU.UserId, IL.LeadName as CustomerName,IL.PhoneNumber AS MobileNo, IL.Email AS EmailId,      
   '' as PolicyNo, '' as PolicyType, '' as PolicyIssueDate, 
   '' as Premium,  IL.CreatedOn as GeneratedOn, '' AS 'ExpiringOn',  '' as RequestStatus,
   CASE    
    WHEN IU.UserId IS NOT NULL THEN IU.UserId + '-' + IU.UserName    
    WHEN IU.EmpID IS NOT NULL THEN IU.EmpID + '-' + IU.UserName    
   END     
   AS CreatedBy    
  FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] (NOLOCK) IL    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] (NOLOCK) IU ON IU.UserId = IL.CreatedBy    
      
  WHERE     
   (              
     ((@Searchtext IS NULL OR @Searchtext = '') OR (IL.LeadName like '%' + @Searchtext + '%'))               
   )     
   AND     
   (    
    ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(IL.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
   )     
      
  ORDER BY IL.CreatedOn DESC     
    
  SELECT IU.UserId, IL.LeadName as CustomerName, IL.PhoneNumber AS MobileNo, IL.Email AS EmailId,        
   '' as PolicyNo, '' as PolicyType, '' as PolicyIssueDate, 
   '' as Premium,  IL.CreatedOn as GeneratedOn, '' AS 'ExpiringOn',  '' as RequestStatus, 
   CASE    
    WHEN IU.UserId IS NOT NULL THEN IU.UserId + '-' + IU.UserName    
    WHEN IU.EmpID IS NOT NULL THEN IU.EmpID + '-' + IU.UserName    
   END     
   AS CreatedBy    
  FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] (NOLOCK) IL    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] (NOLOCK) IU ON IU.UserId = IL.CreatedBy    
      
  WHERE     
   (              
     ((@Searchtext IS NULL OR @Searchtext = '') OR (IL.LeadName like '%' + @Searchtext + '%'))               
   )     
   AND     
   (    
    ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(IL.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
   )      
     
  ORDER BY IL.CreatedOn DESC  OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY    
      
      
  SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,    
      @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,    
      (SELECT COUNT(UserId) from @TempDataTable) as TotalRecord    
    
 END    
 ELSE IF(@CustomerType = 'All Renewals')    
 BEGIN    
      
  INSERT @TempDataTable    
 SELECT IU.UserId, IL.LeadName as CustomerName, IL.PhoneNumber AS MobileNo, IL.Email AS EmailId,       
   '' as PolicyNo, '' as PolicyType, '' as PolicyIssueDate, 
   '' as Premium,  IL.CreatedOn as GeneratedOn, '' AS 'ExpiringOn',  '' as RequestStatus,  
         
   CASE    
    WHEN IU.UserId IS NOT NULL THEN IU.UserId + '-' + IU.UserName    
    WHEN IU.EmpID IS NOT NULL THEN IU.EmpID + '-' + IU.UserName    
   END     
   AS CreatedBy    
  FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] (NOLOCK) IL    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] (NOLOCK) IU ON IU.UserId = IL.CreatedBy    
   
  WHERE     
   (               
     ((@Searchtext IS NULL OR @Searchtext = '') OR (IL.LeadName like '%' + @Searchtext + '%'))                    
   )     
   AND     
   (    
    ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(IL.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
   )     
    
  ORDER BY IL.CreatedOn DESC     
    
 SELECT IU.UserId, IL.LeadName as CustomerName, IL.PhoneNumber AS MobileNo, IL.Email AS EmailId,      
   '' as PolicyNo, '' as PolicyType, '' as PolicyIssueDate, 
   '' as Premium,  IL.CreatedOn as GeneratedOn, '' AS 'ExpiringOn',  '' as RequestStatus,    
   CASE    
    WHEN IU.UserId IS NOT NULL THEN IU.UserId + '-' + IU.UserName    
    WHEN IU.EmpID IS NOT NULL THEN IU.EmpID + '-' + IU.UserName    
   END     
   AS CreatedBy    
  FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] (NOLOCK) IL    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] (NOLOCK) IU ON IU.UserId = IL.CreatedBy    
   
  WHERE     
   (               
     ((@Searchtext IS NULL OR @Searchtext = '') OR (IL.LeadName like '%' + @Searchtext + '%'))                    
   )     
   AND     
   (    
    ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(IL.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
   )
     
  ORDER BY IL.CreatedOn DESC  OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY    
      
  SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,    
      @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,    
      (SELECT COUNT(UserId) from @TempDataTable) as TotalRecord    
    
 END    
     

	 ELSE IF(@CustomerType = 'All Policies')    
 BEGIN    
      
  INSERT @TempDataTable    
 SELECT IU.UserId,IL.LeadName as CustomerName, IL.PhoneNumber AS MobileNo, IL.Email AS EmailId,       
   '' as PolicyNo, '' as PolicyType, '' as PolicyIssueDate, 
   '' as Premium,  IL.CreatedOn as GeneratedOn, '' AS 'ExpiringOn',  '' as RequestStatus,
   CASE    
    WHEN IU.UserId IS NOT NULL THEN IU.UserId + '-' + IU.UserName    
    WHEN IU.EmpID IS NOT NULL THEN IU.EmpID + '-' + IU.UserName    
   END     
   AS CreatedBy    
  FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] IL    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] IU ON IU.UserId = IL.CreatedBy    
   
  WHERE     
   (               
     ((@Searchtext IS NULL OR @Searchtext = '') OR (IL.LeadName like '%' + @Searchtext + '%'))                    
   )     
   AND     
   (    
    ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(IL.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
   )     
    
  ORDER BY IL.CreatedOn DESC     
    
  SELECT IU.UserId, IL.LeadName as CustomerName, IL.PhoneNumber AS MobileNo,IL.Email AS EmailId,      
   '' as PolicyNo, '' as PolicyType, '' as PolicyIssueDate, 
   '' as Premium,  IL.CreatedOn as GeneratedOn, '' AS 'ExpiringOn',  '' as RequestStatus,    
   CASE    
    WHEN IU.UserId IS NOT NULL THEN IU.UserId + '-' + IU.UserName    
    WHEN IU.EmpID IS NOT NULL THEN IU.EmpID + '-' + IU.UserName    
   END     
   AS CreatedBy    
  FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] IL    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] IU ON IU.UserId = IL.CreatedBy    
   
  WHERE     
   (               
     ((@Searchtext IS NULL OR @Searchtext = '') OR (IL.LeadName like '%' + @Searchtext + '%'))                    
   )     
   AND     
   (    
    ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(IL.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
   )
     
  ORDER BY IL.CreatedOn DESC  OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY    
      
  SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,    
      @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,    
      (SELECT COUNT(UserId) from @TempDataTable) as TotalRecord    

END

ELSE     
 BEGIN    
      
  INSERT @TempDataTable    
 SELECT IU.UserId,IL.LeadName as CustomerName,IL.PhoneNumber AS MobileNo,IL.Email AS EmailId,        
   '' as PolicyNo, '' as PolicyType, '' as PolicyIssueDate, 
   '' as Premium,  IL.CreatedOn as GeneratedOn, '' AS 'ExpiringOn',  '' as RequestStatus,
   CASE    
    WHEN IU.UserId IS NOT NULL THEN IU.UserId + '-' + IU.UserName    
    WHEN IU.EmpID IS NOT NULL THEN IU.EmpID + '-' + IU.UserName    
   END     
   AS CreatedBy    
  FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] IL    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] IU ON IU.UserId = IL.CreatedBy    
   
  WHERE     
   (               
     ((@Searchtext IS NULL OR @Searchtext = '') OR (IL.LeadName like '%' + @Searchtext + '%'))                    
   )     
   AND     
   (    
    ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(IL.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
   )     
    
  ORDER BY IL.CreatedOn DESC     
    
  SELECT IU.UserId, IL.LeadName as CustomerName,IL.PhoneNumber AS MobileNo,IL.Email AS EmailId,       
      '' as PolicyNo, '' as PolicyType, '' as PolicyIssueDate, 
   '' as Premium,  IL.CreatedOn as GeneratedOn, '' AS 'ExpiringOn',  '' as RequestStatus,    
   CASE    
    WHEN IU.UserId IS NOT NULL THEN IU.UserId + '-' + IU.UserName    
    WHEN IU.EmpID IS NOT NULL THEN IU.EmpID + '-' + IU.UserName    
   END     
   AS CreatedBy    
  FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] IL    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] IU ON IU.UserId = IL.CreatedBy    
   
  WHERE     
   (               
     ((@Searchtext IS NULL OR @Searchtext = '') OR (IL.LeadName like '%' + @Searchtext + '%'))                    
   )     
   AND     
   (    
    ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR     
     CAST(IL.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)    
   )
     
  ORDER BY IL.CreatedOn DESC  OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY    
      
  SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,    
      @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,    
      (SELECT COUNT(UserId) from @TempDataTable) as TotalRecord    

END




 END TRY                          
 BEGIN CATCH            
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH          
       
END 
