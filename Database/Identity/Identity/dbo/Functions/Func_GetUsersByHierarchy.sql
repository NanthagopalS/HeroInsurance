
-- select UserId from dbo.[Func_GetUsersByHierarchy]('45BBA540-07C5-4D4C-BEFF-771AE2FC32B0',null)  
CREATE FUNCTION [dbo].[Func_GetUsersByHierarchy] (
	@UserId VARCHAR(100)
	,@BuId VARCHAR(100)
	)
RETURNS @UsersByHierarchy TABLE (Userid VARCHAR(100))
AS
BEGIN
	WITH ReportingHierarchy
	AS (
		SELECT ReportingUser.UserId
		FROM HeroAdmin.dbo.Admin_UserRoleMapping ReportingUser WITH (NOLOCK)
		JOIN Identity_User ReportingUserName WITH (NOLOCK) ON ReportingUser.ReportingUserId = ReportingUserName.UserId
		JOIN Identity_User UserName WITH (NOLOCK) ON ReportingUser.UserId = UserName.UserId
		WHERE ReportingUser.ReportingUserId = @UserId
			AND (
				ReportingUser.BUId = @BuId
				OR ISNULL(@BuId, '') = ''
				)
			AND ReportingUserName.IsActive = 1
		--ReportingUser.ReportingUserId is null --and       
		
		UNION ALL
		
		SELECT ReportingUser.UserId
		FROM HeroAdmin.dbo.Admin_UserRoleMapping ReportingUser WITH (NOLOCK)
		JOIN Identity_User ReportingUserName WITH (NOLOCK) ON ReportingUser.ReportingUserId = ReportingUserName.UserId
		JOIN Identity_User UserName WITH (NOLOCK) ON ReportingUser.UserId = UserName.UserId
		JOIN ReportingHierarchy AS rh ON ReportingUser.ReportingUserId = rh.UserId
		WHERE (
				ReportingUser.BUId = @BuId
				OR ISNULL(@BuId, '') = ''
				)
			AND ReportingUserName.IsActive = 1
		)
	INSERT INTO @UsersByHierarchy (Userid)
	SELECT *
	FROM ReportingHierarchy WITH (NOLOCK)

	INSERT INTO @UsersByHierarchy (Userid)
	VALUES (@UserId)

	INSERT INTO @UsersByHierarchy (Userid)
	SELECT UserId
	FROM HeroIdentity.dbo.Identity_UserDetail WITH (NOLOCK)
	WHERE IsActive = 1
		AND (
			SourcedByUserId IN (
				SELECT Userid
				FROM @UsersByHierarchy
				)
			OR CreatedBy IN (
				SELECT Userid
				FROM @UsersByHierarchy
				)
			OR ServicedByUserId IN (
				SELECT Userid
				FROM @UsersByHierarchy
				)
			)

	RETURN
END