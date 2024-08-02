      
 -- exec [Admin_GetAllUserRoleMappingDetail]  '','','','2023-07-01','2023-07-20','1','10'         
 CREATE   PROCEDURE [dbo].[Admin_GetAllUserRoleMappingDetail]             
 (            
   @EmployeeIdorName VARCHAR(100) = NULL, --change           
   @RoleTypeId VARCHAR(100) = NULL,       
   @StatusId bit = 1,    --change        
   @StartDate VARCHAR(100) = NULL,        
   @EndDate VARCHAR(100) = NULL,        
   @CurrentPageIndex INT = 1,    
   @CurrentPageSize INT = 10        
 )            
AS            
 DECLARE @EmployeeIdorName1 VARCHAR(100) = @EmployeeIdorName,    
   @RoleTypeId1 VARCHAR(100) = @RoleTypeId,       
   @StatusId1 bit = @StatusId,    
   @StartDate1 VARCHAR(100) = @StartDate,        
   @EndDate1 VARCHAR(100) = @EndDate,        
   @CurrentPageIndex1 INT = @CurrentPageIndex,    
   @CurrentPageSize1 INT = @CurrentPageSize    
    
 DECLARE @PreviousPageIndex1 INT = @CurrentPageIndex1 - 1,     
   @NextPageIndex1 INT =  @CurrentPageIndex1 + 1    
       
     
 DECLARE @TempDataTable TABLE(    
   UserRoleMappingId VARCHAR(100), 
   UserId VARCHAR(100),
   EmpID VARCHAR(100),     
   EmployeeName VARCHAR(100),     
   Password VARCHAR(MAX),     
   MobileNo VARCHAR(100),     
   EmailId VARCHAR(100),    
   RoleTypeId VARCHAR(100),    
   RoleTypeName VARCHAR(100),    
   RoleId VARCHAR(100),    
   RoleName VARCHAR(100),    
   StatusId bit,    
   StatusName VARCHAR(100)
 )    
      
BEGIN            
 BEGIN TRY        
     
   IF(@CurrentPageIndex1 = -1)    
   BEGIN    
  
   INSERT @TempDataTable    
   SELECT  U.UserName AS EmployeeName, U.UserId, U.EmpID, U.Password, U.MobileNo, U.EmailId, RT.RoleTypeName,  
   UM.UserRoleMappingId, RM.RoleName, RT.RoleTypeId, RM.RoleId, UM.IsActive as StatusId,       
   CASE WHEN UM.IsActive = 1 THEN 'ACTIVE' ELSE 'DEACTIVATED' END as StatusName    
   FROM [HeroAdmin].[dbo].[Admin_UserRoleMapping] (NOLOCK) UM       
    INNER JOIN [HeroIdentity].[dbo].[Identity_User] (NOLOCK) U ON UM.UserId = U.UserId       
    INNER JOIN [HeroAdmin].[dbo].[Admin_RoleType] (NOLOCK) RT ON UM.RoleTypeID = RT.RoleTypeID       
    INNER JOIN [HeroAdmin].[dbo].[Admin_RoleMaster] (NOLOCK) RM on UM.RoleId = RM.RoleId      
   WHERE      
  (  
   ((@EmployeeIdorName1 IS NULL OR @EmployeeIdorName1 = '') OR U.EmpID LIKE @EmployeeIdorName1 + '%') OR      
   ((@EmployeeIdorName1 IS NULL OR @EmployeeIdorName1 = '') OR U.UserName LIKE '%' + @EmployeeIdorName1 + '%')  
  )  
  AND ((@RoleTypeId1 IS NULL OR @RoleTypeId1 = '') OR RT.RoleTypeID = @RoleTypeId1)      
  AND   
  (  
   ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR   
   (CAST(UM.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date))  
  )   
  AND ((@StatusId1 IS NULL OR @StatusId1 = null) OR UM.IsActive = @StatusId1)  
   
    ORDER BY UM.CreatedOn DESC   
  
    SELECT UserRoleMappingId,UserId, EmpID, EmployeeName, [Password], MobileNo, EmailId, RoleTypeId, RoleTypeName, RoleId, RoleName, StatusId, StatusName FROM @TempDataTable ORDER BY EmployeeName    
  
   SELECT 0 as CurrentPageIndex, 0 as PreviousPageIndex, 0 as NextPageIndex, 0 as CurrentPageSize, 0 as TotalRecord     
    
   END    
   ELSE    
   BEGIN    
  
  INSERT @TempDataTable   
   SELECT UM.UserRoleMappingId, U.UserId, U.EmpID, U.UserName AS EmployeeName, U.Password, U.MobileNo, U.EmailId,       
   RT.RoleTypeId, RT.RoleTypeName, RM.RoleId, RM.RoleName, UM.IsActive as StatusId,       
   CASE WHEN UM.IsActive = 1 THEN 'ACTIVE' ELSE 'DEACTIVATED' END as StatusName       
   FROM [HeroAdmin].[dbo].[Admin_UserRoleMapping] (NOLOCK) UM       
    INNER JOIN [HeroIdentity].[dbo].[Identity_User] (NOLOCK) U ON UM.UserId = U.UserId       
    left JOIN [HeroAdmin].[dbo].[Admin_RoleType] (NOLOCK) RT ON UM.RoleTypeID = RT.RoleTypeID       
    left JOIN [HeroAdmin].[dbo].[Admin_RoleMaster] (NOLOCK) RM on UM.RoleId = RM.RoleId      
   WHERE      
  (  
   ((@EmployeeIdorName1 IS NULL OR @EmployeeIdorName1 = '') OR U.EmpID LIKE @EmployeeIdorName1 + '%') OR      
   ((@EmployeeIdorName1 IS NULL OR @EmployeeIdorName1 = '') OR U.UserName LIKE '%' + @EmployeeIdorName1 + '%')  
  )  
  AND ((@RoleTypeId1 IS NULL OR @RoleTypeId1 = '') OR RT.RoleTypeID = @RoleTypeId1)      
  AND   
  (  
   ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR   
   (CAST(UM.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date))  
  )     
  AND ((@StatusId1 IS NULL OR @StatusId1 = null) OR UM.IsActive = @StatusId1)  
    ORDER BY UM.CreatedOn DESC  
    
   SELECT  UM.UserRoleMappingId, U.UserId, U.EmpID, U.UserName AS EmployeeName, U.Password, U.MobileNo, U.EmailId,       
   RT.RoleTypeId, RT.RoleTypeName, RM.RoleId, RM.RoleName, UM.IsActive as StatusId,       
   CASE WHEN UM.IsActive = 1 THEN 'ACTIVE' ELSE 'DEACTIVATED' END as StatusName       
   FROM [HeroAdmin].[dbo].[Admin_UserRoleMapping] (NOLOCK) UM       
    INNER JOIN [HeroIdentity].[dbo].[Identity_User] (NOLOCK) U ON UM.UserId = U.UserId       
    left JOIN [HeroAdmin].[dbo].[Admin_RoleType] (NOLOCK) RT ON UM.RoleTypeID = RT.RoleTypeID       
    left JOIN [HeroAdmin].[dbo].[Admin_RoleMaster] (NOLOCK) RM on UM.RoleId = RM.RoleId      
   WHERE      
  (  
   ((@EmployeeIdorName1 IS NULL OR @EmployeeIdorName1 = '') OR U.EmpID LIKE @EmployeeIdorName1 + '%') OR      
   ((@EmployeeIdorName1 IS NULL OR @EmployeeIdorName1 = '') OR U.UserName LIKE '%' + @EmployeeIdorName1 + '%')      
  )  
  AND ((@RoleTypeId1 IS NULL OR @RoleTypeId1 = '') OR RT.RoleTypeID = @RoleTypeId1)      
  AND   
  (  
   ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR   
   (CAST(UM.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date))  
  )      
  AND ((@StatusId1 IS NULL OR @StatusId1 = null) OR UM.IsActive = @StatusId1)  
   ORDER BY UM.CreatedOn DESC OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY    
    
    
    SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,    
    @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,    
    (SELECT COUNT(UserRoleMappingId) from @TempDataTable) as TotalRecord    
    
   END    
       
 END TRY                            
 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH            
         
END 
