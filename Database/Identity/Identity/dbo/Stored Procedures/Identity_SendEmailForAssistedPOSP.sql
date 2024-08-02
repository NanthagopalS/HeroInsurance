CREATE
	

 PROCEDURE [dbo].[Identity_SendEmailForAssistedPOSP] (@UserId VARCHAR(100) = NULL)
AS
BEGIN
	BEGIN TRY
		UPDATE Identity_User
		SET UserProfileStage = 4, UpdatedOn = GETDATE()
		WHERE UserId = @UserId

		-- update StageId    
		EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserId
			,'E6F84D7A-A6F9-4141-B5BD-A20DAFA1D371'

		SELECT IU.UserName
			,IU.EmailId
			,IU.UserId
			,IU.POSPId
		FROM [dbo].[Identity_User] IU WITH (NOLOCK)
		LEFT JOIN [dbo].[Identity_UserDetail] UD WITH (NOLOCK) ON UD.UserId = IU.UserId
		LEFT JOIN [dbo].[Identity_User] createdUser WITH (NOLOCK) ON UD.CreatedBy = createdUser.UserId
		WHERE IU.UserId = @UserId -- and IU.UserProfileStage = 4  
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