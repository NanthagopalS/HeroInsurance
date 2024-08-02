
-- =============================================  
-- Author:  <Author, Parth>  
-- Create date: <Create Date,22-FEB-2023>  
-- Description: <Description,Update Admin User>  
-- Identity_ResetPassword ''  
-- =============================================  
CREATE PROCEDURE [dbo].[Identity_ResetPassword] (@EmailId VARCHAR(100))
AS
BEGIN
	BEGIN TRY
		IF EXISTS (
				SELECT TOP 1 UserId
				FROM Identity_User WITH (NOLOCK)
				WHERE EmailId = @EmailId
					AND RoleId != '2D6B0CE9-15C7-4839-93D1-8387944BC42F'
				)
		BEGIN
			SELECT UserId
				,[Password]
				,1 AS IsValidate
			FROM Identity_User WITH (NOLOCK)
			WHERE EmailId = @EmailId
		END
		ELSE
		BEGIN
			SELECT NULL AS UserId
				,0 AS IsValidate
				,'User Not Exists' AS [Message]
		END
	END TRY

	BEGIN CATCH
		IF @@TRANCOUNT > 0
			ROLLBACK

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
