-- =========================================================================================               
-- Author:  <Author, Parth>            
-- Create date: <Create Date,31-Jan-2023>            
-- Description: <Description, Admin_GetRoleDetail>         
-- exec Admin_GetRoleDetail null,'2BA70571-63B3-4193-9F31-BDBCC59ED08B',null,null,1       
-- =========================================================================================               
 CREATE PROCEDURE [dbo].[Admin_GetRoleDetail]             
 (            
   @RoleName VARCHAR(100) = NULL,            
   @RoleTypeId VARCHAR(100) = NULL,          
   @StartDate VARCHAR(100) = NULL,        
   @EndDate VARCHAR(100) = NULL,  
   @IsActive bit = 1,  
   @CurrentPageIndex INT = 1,    
   @CurrentPageSize INT = 10    
 )  
AS  
     
 DECLARE @RoleName1 VARCHAR(100) = @RoleName,    
	   @RoleTypeId1 VARCHAR(100) = @RoleTypeId,    
	   @StartDate1 VARCHAR(100) = @StartDate,        
	   @EndDate1 VARCHAR(100) = @EndDate,        
	   @CurrentPageIndex1 INT = @CurrentPageIndex,    
	   @CurrentPageSize1 INT = @CurrentPageSize,
	   @IsActive1 bit = @IsActive
    
 DECLARE @PreviousPageIndex1 INT = @CurrentPageIndex1 - 1,     
	@NextPageIndex1 INT =  @CurrentPageIndex1 + 1    
       
     
 DECLARE @TempDataTable TABLE(    
  RoleId VARCHAR(100),     
  RoleName VARCHAR(100),   
  RoleTypeId VARCHAR(100),  
  RoleTypeName VARCHAR(100),  
  RoleLevelId  VARCHAR(100),  
  RoleLevelName  VARCHAR(100),  
  LevelNo int,  
  CreatedBy  VARCHAR(100),  
  CreatedOn VARCHAR(100),     
  UpdatedBy  VARCHAR(100),  
  UpdatedOn  VARCHAR(100),  
  IsActive bit,
  BUID VARCHAR(100),
  BUName VARCHAR(100)
 )    
    
     
BEGIN            
 BEGIN TRY            
    IF(@CurrentPageIndex1 = -1)    
 BEGIN    
      
  INSERT @TempDataTable    
  SELECT rm.RoleId, rm.RoleName, rm.RoleTypeId, rt.RoleTypeName, rm.RoleLevelID, rl.RoleLevelName, rl.PriorityIndex as LevelNo,
  IUC.UserId as CreatedBy,  rm.CreatedOn, IUU.UserId as UpdatedBy, rm.UpdatedOn, rm.IsActive, ISNULL(rm.BUID,'') as BUID,
  ISNULL(bu.BUName,'') as BUName       
  From Admin_RoleMaster rm WITH(NOLOCK)         
   LEFT JOIN Admin_RoleType rt WITH(NOLOCK) on rt.RoleTypeID = rm.RoleTypeID  
   LEFT JOIN Admin_RoleLevel rl WITH(NOLOCK) ON rl.RoleLevelId = rm.RoleLevelID  
   LEFT JOIN HeroIdentity.dbo.Identity_User IUC WITH(NOLOCK) ON IUC.UserId = rm.CreatedBy  
   LEFT JOIN HeroIdentity.dbo.Identity_User IUU WITH(NOLOCK) ON IUU.UserId = rm.UpdatedBy
   LEFT JOIN Admin_BU bu ON rm.BUID = bu.BUId
  WHERE      
     ((@RoleName1 IS NULL OR @RoleName1 = '') OR rm.RoleName like '%' + @RoleName1 + '%')      
     AND ((@RoleTypeId1 IS NULL OR @RoleTypeId1 = '') OR rt.RoleTypeID = @RoleTypeId1)      
     AND (  
   ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR   
   CAST(rm.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)  
  )   
  AND  
  ((@IsActive1 IS NULL OR @IsActive1 = null) OR rm.IsActive = @IsActive1)  
       ORDER BY rm.CreatedOn DESC     
        
     SELECT RoleId, RoleName, RoleTypeId, RoleTypeName, RoleLevelId, RoleLevelName, LevelNo, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn, IsActive, BuId, BUName FROM @TempDataTable ORDER BY CreatedOn DESC    
      
  SELECT 0 as CurrentPageIndex, 0 as PreviousPageIndex, 0 as NextPageIndex, 0 as CurrentPageSize, 0 as TotalRecord     
    
 END    
 ELSE    
 BEGIN    
      
  INSERT @TempDataTable    
  SELECT rm.RoleId, rm.RoleName, rm.RoleTypeId, rt.RoleTypeName, rm.RoleLevelID, rl.RoleLevelName, rl.PriorityIndex as LevelNo,
  IUC.UserId as CreatedBy,  rm.CreatedOn, IUU.UserId as UpdatedBy, rm.UpdatedOn, rm.IsActive,ISNULL(rm.BUID,'') as BUID,
  ISNULL(bu.BUName,'') as BUName
  From Admin_RoleMaster rm WITH(NOLOCK)         
   LEFT JOIN Admin_RoleType rt WITH(NOLOCK) on rt.RoleTypeID = rm.RoleTypeID  
   LEFT JOIN Admin_RoleLevel rl WITH(NOLOCK) ON rl.RoleLevelId = rm.RoleLevelID  
   LEFT JOIN HeroIdentity.dbo.Identity_User IUC WITH(NOLOCK) ON IUC.UserId = rm.CreatedBy  
   LEFT JOIN HeroIdentity.dbo.Identity_User IUU WITH(NOLOCK) ON IUU.UserId = rm.UpdatedBy 
   LEFT JOIN Admin_BU bu WITH(NOLOCK) ON rm.BUID = bu.BUId
  WHERE      
    ((@RoleName1 IS NULL OR @RoleName1 = '') OR rm.RoleName like '%' + @RoleName1 + '%')      
     AND ((@RoleTypeId1 IS NULL OR @RoleTypeId1 = '') OR rt.RoleTypeID = @RoleTypeId1)      
     AND (  
   ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR   
   CAST(rm.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)  
  )   
  AND  
  ((@IsActive1 IS NULL OR @IsActive1 = null) OR rm.IsActive = @IsActive1)  
   ORDER BY rm.CreatedOn DESC     
    
    
   SELECT rm.RoleId, rm.RoleName, rm.RoleTypeId, rt.RoleTypeName, rm.RoleLevelID, rl.RoleLevelName, rl.PriorityIndex as LevelNo, 
   IUC.UserId as CreatedBy,  rm.CreatedOn, IUU.UserId as UpdatedBy, rm.UpdatedOn, rm.IsActive,ISNULL(rm.BUID,'') as BUID,
   ISNULL(bu.BUName,'') as BUName
  From Admin_RoleMaster rm WITH(NOLOCK)         
   LEFT JOIN Admin_RoleType rt WITH(NOLOCK) on rt.RoleTypeID = rm.RoleTypeID  
   LEFT JOIN Admin_RoleLevel rl WITH(NOLOCK) ON rl.RoleLevelId = rm.RoleLevelID  
   LEFT JOIN HeroIdentity.dbo.Identity_User IUC WITH(NOLOCK) ON IUC.UserId = rm.CreatedBy  
   LEFT JOIN HeroIdentity.dbo.Identity_User IUU WITH(NOLOCK) ON IUU.UserId = rm.UpdatedBy
   LEFT JOIN Admin_BU bu WITH(NOLOCK) ON rm.BUID = bu.BUId
  WHERE      
    ((@RoleName1 IS NULL OR @RoleName1 = '') OR rm.RoleName like '%' + @RoleName1 + '%')      
     AND ((@RoleTypeId1 IS NULL OR @RoleTypeId1 = '') OR rt.RoleTypeID = @RoleTypeId1)      
     AND (  
   ((@StartDate1 IS NULL OR @StartDate1 = '') AND (@EndDate1 IS NULL OR @EndDate1 = '')) OR   
   CAST(rm.CreatedOn as date) BETWEEN CAST(@StartDate1 as date) and CAST(@EndDate1 as date)  
  )   
  AND  
  ((@IsActive1 IS NULL OR @IsActive1 = null) OR rm.IsActive = @IsActive1)  
   ORDER BY rm.CreatedOn DESC  OFFSET (@CurrentPageIndex1-1)*@CurrentPageSize1 ROWS FETCH NEXT @CurrentPageSize1 ROWS ONLY     
    
   SELECT @CurrentPageIndex1 as CurrentPageIndex, @PreviousPageIndex1 as PreviousPageIndex,    
      @NextPageIndex1 as NextPageIndex, @CurrentPageSize1 as CurrentPageSize,    
      (SELECT COUNT(RoleId) from @TempDataTable) as TotalRecord    
    
 END    
 END TRY                            
 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH            
         
END 
