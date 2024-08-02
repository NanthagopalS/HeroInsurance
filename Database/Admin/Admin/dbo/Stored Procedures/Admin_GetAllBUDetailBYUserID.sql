-- exec [Admin_GetAllBUDetailBYUserID] '66AC7FEA-E99B-4B01-BD4E-BD0A39821FEB' Create     PROCEDURE [dbo].[Admin_GetAllBUDetailBYUserID] (@UserID VARCHAR(100) = NULL)ASDECLARE @TempDataTable TABLE (	BuId VARCHAR(100)	,BuName VARCHAR(100)	,HierarchyLevelId VARCHAR(100)	,HierarchyLevelName VARCHAR(100)	,CreatedOn VARCHAR(100)	,IsActive BIT	,SalesandSupport BIT	)BEGIN	BEGIN TRY	DECLARE @RoleType VARCHAR(100) = NULL      
	 DECLARE @UserProfile VARCHAR(100) = NULL      
	 DECLARE @BUID VARCHAR(100) = NULL      
	 DECLARE @UsersList TABLE (Userid VARCHAR(100))	if exists (Select BuHeadId from Admin_BU where BUHeadId = @UserID)		BEGIN 		SELECT distinct bu.BuId			,bu.BuName			,bul.BULevelId AS HierarchyLevelId			,bul.BULevelName AS HierarchyLevelName			,bu.CreatedOn			,bu.IsActive			,bu.IsSales as SalesandSupport		FROM [Admin_Bu](NOLOCK) Bu		LEFT JOIN [Admin_UserRoleMapping](NOLOCK) URM ON URM.BUId = bu.BUId		LEFT JOIN [Admin_BuLevel](NOLOCK) bul ON bul.BuLevelId = bu.BuLevelId		WHERE Bu.BUHeadId = @UserID and BU.IsActive =1					END	else 	BEGIN		  SELECT RT.RoleTypeID      
			   ,UC.UserCategoryId      
			   ,URM.BUId      
			  INTO #TEMP_USER_ID_DATA      
			  FROM HeroIdentity.dbo.Identity_User IUSER WITH (NOLOCK)      
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


	 -- Central User BU or Shared Product
	 if (@RoleType = '2BA70571-63B3-4193-9F31-BDBCC59ED08B' or (@RoleType = 'D1E03B48-C313-4BB6-928C-E62C44E1DBDE' and @UserProfile='34431C50-DC32-476E-96D9-A896CD248357' ) )
		
		BEGIN 
			SELECT distinct bu.BuId			,bu.BuName			,bu.CreatedOn			,bu.IsActive			,bu.IsSales as SalesandSupport		FROM [Admin_Bu](NOLOCK) Bu		WHERE BU.IsActive =1

		END

	-- BU Specific
	else if (@RoleType = 'C924DA2C-B0DA-40A5-B1F7-99DD6A859E16')
		BEGIN

			SELECT distinct bu.BuId			,bu.BuName			,bul.BULevelId AS HierarchyLevelId			,bul.BULevelName AS HierarchyLevelName			,bu.CreatedOn			,bu.IsActive			,bu.IsSales as SalesandSupport		FROM [Admin_Bu](NOLOCK) Bu		LEFT JOIN [Admin_UserRoleMapping](NOLOCK) URM ON URM.BUId = bu.BUId		LEFT JOIN [Admin_BuLevel](NOLOCK) bul ON bul.BuLevelId = bu.BuLevelId		WHERE URM.UserId = @UserID and BU.IsActive =1

		END

	-- shared - sales or support
	else if (@RoleType = 'D1E03B48-C313-4BB6-928C-E62C44E1DBDE' and (@UserProfile ='B9D2CB66-56E3-4665-B26C-D32948ABA25C' or @UserProfile = '43B21452-64F8-4301-83D2-BC0F429282E0'))
		BEGIN

				SELECT distinct bu.BuId				,bu.BuName				,bu.CreatedOn				,bu.IsActive				,bu.IsSales as SalesandSupport			FROM [Admin_Bu](NOLOCK) Bu			WHERE BU.IsActive =1 and BU.IsSales = 1
		END


	 ENDEND Try	BEGIN CATCH		DECLARE @StrProcedure_Name VARCHAR(500)			,@ErrorDetail VARCHAR(1000)			,@ParameterList VARCHAR(2000)		SET @StrProcedure_Name = ERROR_PROCEDURE()		SET @ErrorDetail = ERROR_MESSAGE()		EXEC Admin_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name			,@ErrorDetail = @ErrorDetail			,@ParameterList = @ParameterList	END CATCHEND