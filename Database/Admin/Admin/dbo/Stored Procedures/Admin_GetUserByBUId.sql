-- exec[Admin_GetUserByBUId] 'CC7AED84-3664-4C3F-85D6-7CE435B96D4C','45BBA540-07C5-4D4C-BEFF-771AE2FC32B0',''
CREATE
	

 PROCEDURE [dbo].[Admin_GetUserByBUId] (
	@BUId VARCHAR(100)
	,@UserId VARCHAR(100) = NULL
	,@SearchText VARCHAR(100) = NULL
	)
AS
BEGIN
	BEGIN TRY
		DECLARE @RoleType VARCHAR(100) = NULL
		DECLARE @UserProfile VARCHAR(100) = NULL

		SELECT RT.RoleTypeID
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

		DECLARE @USERASPER_BUID TABLE (
			UserId VARCHAR(100)
			,UserName VARCHAR(100)
			)
		DECLARE @BuUsers TABLE (
			UserId VARCHAR(100)
			,UserName VARCHAR(100)
			)

		INSERT INTO @BuUsers (
			UserId
			,UserName
			)
		SELECT U.UserId
			,U.UserName
		FROM [HeroIdentity].[dbo].[Identity_User](NOLOCK) U
		WHERE U.UserId IN (
				SELECT UserId
				FROM HeroIdentity.dbo.[Func_GetUsersOfBU](@BUId, @UserId)
				)
			AND U.IsActive = 1

		DECLARE @HeirarchyUsers TABLE (
			UserId VARCHAR(100)
			,UserName VARCHAR(100)
			)

		INSERT INTO @HeirarchyUsers (
			UserId
			,UserName
			)
		SELECT U.UserId
			,U.UserName
		FROM [HeroIdentity].[dbo].[Identity_User](NOLOCK) U
		WHERE U.UserId IN (
				SELECT UserId
				FROM HeroIdentity.dbo.[Func_GetUsersByHierarchy](@UserId, '')
				)
			AND U.IsActive = 1

		Declare @CommonUserTable Table(
				UserId VARCHAR(100),
				UserName VARCHAR(100)
			)

		-- BU Head Central role
		IF EXISTS (
				SELECT BuHeadId
				FROM Admin_BU
				WHERE BUHeadId = @UserID
				)
		BEGIN
			SELECT UserId
				,UserName
			FROM @BuUsers
			WHERE UserId != @UserId
				AND (UserName LIKE '%' + ISNULL(@SearchText, '') + '%')
			ORDER BY UserName ASC
		END
				-- Central Sales
		ELSE IF (
				@RoleType = '2BA70571-63B3-4193-9F31-BDBCC59ED08B'
					AND @UserProfile = 'B9D2CB66-56E3-4665-B26C-D32948ABA25C'
				)
		BEGIN
			Insert into @CommonUserTable (UserId, UserName)
			SELECT UserId
				,UserName
			FROM @BuUsers

			Insert into @CommonUserTable (UserId, UserName)
			SELECT BUHead.UserId
				,BUHead.UserName
			FROM [HeroIdentity].[dbo].[Identity_User](NOLOCK) BUHead
			INNER JOIN [HeroAdmin].[dbo].[Admin_BU](NOLOCK) BU ON BU.BUHeadId = BUHead.UserId
			WHERE BUHead.IsActive = 1
				AND Bu.BUId = @BUId
				AND BUHead.IsActive= 1


			SELECT distinct UserId
				,UserName
			FROM @CommonUserTable
			WHERE UserId != @UserId
				AND (UserName LIKE '%' + ISNULL(@SearchText, '') + '%')
			ORDER BY UserName ASC 
		END

			-- Central - (Support and Product) and Shared Product
		ELSE IF (
				@RoleType = '2BA70571-63B3-4193-9F31-BDBCC59ED08B'
				OR (
					@RoleType = 'D1E03B48-C313-4BB6-928C-E62C44E1DBDE'
					AND @UserProfile = '34431C50-DC32-476E-96D9-A896CD248357'
					)
				)
		BEGIN
			Insert into @CommonUserTable (UserId, UserName)
			SELECT UserId
				,UserName
			FROM @BuUsers

			Insert into @CommonUserTable (UserId, UserName)
			SELECT BUHead.UserId
				,BUHead.UserName
			FROM [HeroIdentity].[dbo].[Identity_User](NOLOCK) BUHead
			INNER JOIN [HeroAdmin].[dbo].[Admin_BU](NOLOCK) BU ON BU.BUHeadId = BUHead.UserId
			WHERE BUHead.IsActive = 1
				AND Bu.BUId = @BUId
				AND BUHead.IsActive = 1

			SELECT distinct UserId
				,UserName
			FROM @CommonUserTable
			WHERE UserId != @UserId
				AND (UserName LIKE '%' + ISNULL(@SearchText, '') + '%')
			ORDER BY UserName ASC 
		END
				-- BU Specific - Product or Support
		ELSE IF (
				@RoleType = 'C924DA2C-B0DA-40A5-B1F7-99DD6A859E16'
				AND (
					@UserProfile = '34431C50-DC32-476E-96D9-A896CD248357'
					OR @UserProfile = '43B21452-64F8-4301-83D2-BC0F429282E0'
					)
				)
		BEGIN
			Insert into @CommonUserTable (UserId, UserName)
			SELECT UserId
				,UserName
			FROM @BuUsers

			Insert into @CommonUserTable (UserId, UserName)
			SELECT BUHead.UserId
				,BUHead.UserName
			FROM [HeroIdentity].[dbo].[Identity_User](NOLOCK) BUHead
			INNER JOIN [HeroAdmin].[dbo].[Admin_BU](NOLOCK) BU ON BU.BUHeadId = BUHead.UserId
			WHERE BUHead.IsActive = 1
				AND Bu.BUId = @BUId
				AND BUHead.IsActive = 1

			SELECT distinct UserId
				,UserName
			FROM @CommonUserTable
			WHERE UserId != @UserId
				AND (UserName LIKE '%' + ISNULL(@SearchText, '') + '%')
			ORDER BY UserName ASC 
		END
				-- BU Specific Sales
		ELSE IF (
				@RoleType = 'C924DA2C-B0DA-40A5-B1F7-99DD6A859E16'
				AND @UserProfile = 'B9D2CB66-56E3-4665-B26C-D32948ABA25C'
				)
		BEGIN
			SELECT UserId
				,UserName
			FROM @HeirarchyUsers
			WHERE UserId IN (
					SELECT UserId
					FROM @BuUsers
					)
				AND UserId != @UserId
				AND (UserName LIKE '%' + ISNULL(@SearchText, '') + '%')
			ORDER BY UserName ASC
		END
				-- Shared Sales
		ELSE IF (
				@RoleType = 'D1E03B48-C313-4BB6-928C-E62C44E1DBDE'
				AND @UserProfile = 'B9D2CB66-56E3-4665-B26C-D32948ABA25C'
				)
		BEGIN
			Insert into @CommonUserTable (UserId, UserName)
				SELECT UserId, UserName
					FROM @BuUsers

			Insert into @CommonUserTable (UserId, UserName)
				SELECT UserId, UserName
					FROM @HeirarchyUsers

			Select distinct UserId, UserName from @CommonUserTable
				where UserId != @UserId
				AND (UserName LIKE '%' + ISNULL(@SearchText, '') + '%')
			ORDER BY UserName ASC
		END
				-- Shared Support
		ELSE IF (
				@RoleType = 'D1E03B48-C313-4BB6-928C-E62C44E1DBDE'
				AND @UserProfile = '43B21452-64F8-4301-83D2-BC0F429282E0'
				)
		BEGIN
			SELECT UserId
				,UserName
			FROM @BuUsers
			WHERE UserId != @UserId
				AND (UserName LIKE '%' + ISNULL(@SearchText, '') + '%')
			ORDER BY UserName ASC
		END
	END TRY

	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500)
			,@ErrorDetail VARCHAR(1000)
			,@ParameterList VARCHAR(2000)

		SET @StrProcedure_Name = ERROR_PROCEDURE()
		SET @ErrorDetail = ERROR_MESSAGE()

		EXEC Admin_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name
			,@ErrorDetail = @ErrorDetail
			,@ParameterList = @ParameterList
	END CATCH
END 
