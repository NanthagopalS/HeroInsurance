
-- select UserId from dbo.[Func_GetUsersOfBU]('A122E628-7B63-4D96-B6BC-3ED426021FCB','')  
CREATE FUNCTION [dbo].[Func_GetUsersOfBU] (
	@BuId VARCHAR(100)
	,@UserId VARCHAR(100)
	)
RETURNS @UsersByHierarchy TABLE (UserId VARCHAR(100))
AS
BEGIN
	WITH ReportingUsersOfBU
	AS (
		SELECT ReportingUser.UserId
		FROM HeroAdmin.dbo.Admin_UserRoleMapping ReportingUser WITH (NOLOCK)
		JOIN Identity_User ReportingUserName WITH (NOLOCK) ON ReportingUser.ReportingUserId = ReportingUserName.UserId
		WHERE ReportingUser.BUId = @BuId
		)
	INSERT INTO @UsersByHierarchy (UserId)
	SELECT UserId
	FROM ReportingUsersOfBU

	IF ISNULL(@UserId, '') = ''
		AND @UserId != ''
	BEGIN
		INSERT INTO @UsersByHierarchy (UserId)
		VALUES (@UserId)
	END

	INSERT INTO @UsersByHierarchy (UserId)
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