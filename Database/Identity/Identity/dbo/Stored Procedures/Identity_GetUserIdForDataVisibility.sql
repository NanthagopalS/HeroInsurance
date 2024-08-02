    
-- =========================================================================================                     
-- Author:  <Author, Suraj>                  
-- [dbo].[Identity_GetUserIdForDataVisibility] 'F24C9610-CF2D-4D95-A11D-F1B736475BAA'      
-- =========================================================================================        
-- EXEC [Identity_GetUserIdForDataVisibility] '1ABF6E56-FF59-4813-83B1-84F26473B99B'   
CREATE    PROCEDURE [dbo].[Identity_GetUserIdForDataVisibility] (@UserId VARCHAR(100))    
AS    
BEGIN    
 DECLARE @RoleType VARCHAR(100) = NULL    
 DECLARE @UserProfile VARCHAR(100) = NULL    
 DECLARE @BUID VARCHAR(100) = NULL    
 DECLARE @UsersList TABLE (Userid VARCHAR(100))    
    
 BEGIN TRY    
  SELECT RT.RoleTypeID    
   ,UC.UserCategoryId    
   ,URM.BUId    
  INTO #TEMP_USER_ID_DATA    
  FROM Identity_User IUSER WITH (NOLOCK)    
  LEFT JOIN HeroAdmin.dbo.Admin_UserRoleMapping URM WITH (NOLOCK) ON IUSER.UserId = URM.UserId    
  LEFT JOIN HeroAdmin.dbo.Admin_RoleType RT WITH (NOLOCK) ON URM.RoleTypeId = RT.RoleTypeID    
  LEFT JOIN HeroAdmin.dbo.Admin_UserCategory UC WITH (NOLOCK) ON URM.CategoryId = UC.UserCategoryId    
  WHERE IUSER.UserId = @UserId    
    
  SET @RoleType = (    
    SELECT RoleTypeID    
    FROM #TEMP_USER_ID_DATA    
    )    
  SET @UserProfile = (    
    SELECT UserCategoryId    
    FROM #TEMP_USER_ID_DATA    
    )    
  SET @BUID = (    
    SELECT BUId    
    FROM #TEMP_USER_ID_DATA    
    )    
   
   IF NOT EXISTS(SELECT * FROM HeroAdmin.dbo.Admin_UserRoleMapping WHERE UserId = @UserId) -- if POSP then it will not have record
	BEGIN
		SELECT UserId from dbo.[Func_GetUsersByHierarchy](@UserId,null)
	END
	ELSE
	BEGIN

  IF (    
    (    
     @RoleType = '2BA70571-63B3-4193-9F31-BDBCC59ED08B'    
     AND @UserProfile = '34431C50-DC32-476E-96D9-A896CD248357'    
     ) -- Central Product      
    OR (    
     @RoleType = '2BA70571-63B3-4193-9F31-BDBCC59ED08B'    
     AND @UserProfile = '43B21452-64F8-4301-83D2-BC0F429282E0'    
     ) -- Central Support      
    OR (    
     @RoleType = 'D1E03B48-C313-4BB6-928C-E62C44E1DBDE'    
     AND @UserProfile = '34431C50-DC32-476E-96D9-A896CD248357'    
     ) -- Shared Product      
    )    
  BEGIN    
   SELECT 0 AS UserId    
  END    
  ELSE    
  BEGIN    
   IF (    
     @RoleType = '2BA70571-63B3-4193-9F31-BDBCC59ED08B'    
     AND @UserProfile = 'B9D2CB66-56E3-4665-B26C-D32948ABA25C'    
     ) -- Central Sales      
   BEGIN    
    SELECT UserId    
    FROM dbo.[Func_GetUsersByHierarchy](@UserId, NULL)    
   END    
   ELSE    
   BEGIN    
    IF (    
      @RoleType = 'D1E03B48-C313-4BB6-928C-E62C44E1DBDE'    
      AND @UserProfile = 'B9D2CB66-56E3-4665-B26C-D32948ABA25C'    
      ) --Shared Sales      
    BEGIN    
     SELECT UserId    
     INTO #SSTEMP    
     FROM dbo.[Func_GetUsersByHierarchy](@UserId, @BUID)    
  
     SELECT TMP.UserId    
     FROM #SSTEMP AS TMP    
     LEFT JOIN HeroAdmin.dbo.Admin_UserRoleMapping URM ON TMP.UserId = URM.UserId    
     LEFT JOIN HeroAdmin.dbo.Admin_BU BU ON BU.BUId = URM.BUId    
     WHERE BU.IsSales = 1 OR URM.CategoryId = 'B9D2CB66-56E3-4665-B26C-D32948ABA25C'    
    
     IF OBJECT_ID(N'#SSTEMP') IS NOT NULL    
     BEGIN    
      DROP TABLE #LocalCustomer    
     END    
    END    
    ELSE IF (    
      @RoleType = 'C924DA2C-B0DA-40A5-B1F7-99DD6A859E16'    
      AND @UserProfile = 'B9D2CB66-56E3-4665-B26C-D32948ABA25C'    
      ) --BU Specific Sales      
    BEGIN    
     SELECT UserId    
     FROM dbo.[Func_GetUsersByHierarchy](@UserId, @BUID)    
    END    
    ELSE    
    BEGIN -- for Shared Support,BU Specific Product,BU Specific Support    
     IF (    
       @RoleType = 'D1E03B48-C313-4BB6-928C-E62C44E1DBDE'    
       AND @UserProfile = '43B21452-64F8-4301-83D2-BC0F429282E0'    
       ) -- Shared Support  
     BEGIN    
      SELECT UserId    
      INTO #SSUTEMP    
      FROM dbo.[Func_GetUsersOfBU]('', @UserId )   
    
      SELECT TMP.UserId    
      FROM #SSUTEMP AS TMP    
      LEFT JOIN HeroAdmin.dbo.Admin_UserRoleMapping URM ON TMP.UserId = URM.UserId    
      LEFT JOIN HeroAdmin.dbo.Admin_BU BU ON BU.BUId = URM.BUId    
      WHERE BU.IsSales = 1 OR URM.CategoryId = 'B9D2CB66-56E3-4665-B26C-D32948ABA25C'   
    
      IF OBJECT_ID(N'#SSUTEMP') IS NOT NULL    
      BEGIN    
       DROP TABLE #LocalCustomer    
      END    
     END    
     ELSE -- for BU Specific Product,BU Specific Support    
     BEGIN    
      SELECT UserId    
      FROM dbo.[Func_GetUsersOfBU](@BUID, @UserId)    
     END    
    END    
   END    
  END    
 
 END
 END TRY    
    
 BEGIN CATCH    
  DECLARE @StrProcedure_Name VARCHAR(500)    
   ,@ErrorDetail VARCHAR(1000)    
   ,@ParameterList VARCHAR(2000)    
    
  SET @StrProcedure_Name = ERROR_PROCEDURE()    
  SET @ErrorDetail = ERROR_MESSAGE()    
    
  EXEC Identity_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name    
   ,@ErrorDetail = @ErrorDetail    
   ,@ParameterList = @ParameterList    
 END CATCH    
END