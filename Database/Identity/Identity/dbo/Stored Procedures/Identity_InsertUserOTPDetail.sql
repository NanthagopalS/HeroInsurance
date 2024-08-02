-- =============================================  
-- Author:  <Author, VISHAL KANJARIYA>  
-- Create date: <Create Date,05-DEC-2022>  
-- Description: <Description,,INSERT USER OTP DETAIL>  
--Identity_InsertUserOTPDetail '9987848971','','1064','1F18D0DF-35C9-4689-832-2428648E69DD','VERIFYOTP'  
-- =============================================  
CREATE   PROCEDURE [dbo].[Identity_InsertUserOTPDetail] (
	@MobileNo VARCHAR(10) = NULL
	,@OTPId VARCHAR(100) = NULL
	,@OTPNumber VARCHAR(10) = NULL
	,@UserId VARCHAR(50) = NULL
	,@Condition VARCHAR(50) = NULL
	)
AS
BEGIN
	BEGIN TRY
		DECLARE @UserProfileStage INT = NULL
			,@WrongOTPCount INT = 0
			,@CodePatternPOSPId VARCHAR(50) = NULL
			,@POSPId INT = 0
		DECLARE @MobileNumber VARCHAR(20) = NULL
			,@FinalCodePOSPId VARCHAR(50) = NULL

		IF (@Condition = 'VERIFYOTP')
		BEGIN
			SELECT @UserProfileStage = UserProfileStage
			FROM Identity_User WITH (NOLOCK)
			WHERE UserId = @UserId
				AND IsActive = 1

			SELECT @MobileNumber = MobileNo
			FROM Identity_User WITH (NOLOCK)
			WHERE UserId = @UserId
				AND IsActive = 1

			IF EXISTS (
					SELECT TOP 1 OTPNumber
					FROM [Identity_OTP] WITH (NOLOCK)
					WHERE IsActive = 1
						AND OTPNumber = @OTPNumber
						AND UserId = @UserId
						AND DATEDIFF(MINUTE, OTPSENDDATETIME, GETDATE()) <= 3
					ORDER BY OTPSendDateTime DESC
					)
			BEGIN
				--LogIn Entry  
				UPDATE Identity_UserLog
				SET IsActive = 0
				WHERE UserId = @UserId

				INSERT INTO Identity_UserLog (UserId)
				VALUES (@UserId)

				UPDATE [Identity_OTP]
				SET IsVerify = 1
					,UpdatedOn = GETDATE()
				WHERE OTPNumber = @OTPNumber
					AND UserId = @UserId
					AND IsActive = 1

				SELECT CAST(1 AS BIT) IsValidOTP
					,@UserProfileStage AS UserProfileStage
					,@MobileNumber AS MobileNumber
					,'0' AS WrongOTPCount

				IF EXISTS (
						SELECT POSPId
						FROM [HeroIdentity].[dbo].[Identity_User] WITH (NOLOCK)
						WHERE UserId = @UserId
							AND POSPId IS NULL
						)
					--POSP ID after authentication  
				BEGIN
					SET @CodePatternPOSPId = (
							SELECT CodePattern
							FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WITH (NOLOCK)
							WHERE [Code] = 'POSP'
								AND IsActive = 1
							)
					SET @POSPId = (
							SELECT NextValue
							FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WITH (NOLOCK)
							WHERE [Code] = 'POSP'
								AND IsActive = 1
							)
					SET @FinalCodePOSPId = CONCAT (
							@CodePatternPOSPId
							,CAST(@POSPId AS VARCHAR)
							)
					SET @POSPId = @POSPId + 1

					UPDATE [HeroIdentity].[dbo].[Identity_AutoGenerateId]
					SET NextValue = @POSPId
						,UpdatedOn = GETDATE()
					WHERE [Code] = 'POSP'
						AND IsActive = 1

					UPDATE [HeroIdentity].[dbo].[Identity_User]
					SET POSPId = @FinalCodePOSPId
						,IsRegistrationVerified = 1
						,UpdatedOn = GETDATE()
					WHERE UserId = @UserId
				END
			END
			ELSE
			BEGIN
				UPDATE [Identity_OTP]
				SET IsVerify = 0
					,UpdatedOn = GETDATE()
				WHERE UserId = @UserId
					AND DATEDIFF(MINUTE, OTPSENDDATETIME, GETDATE()) <= 3

				IF EXISTS (
						SELECT WrongOTPCount
						FROM Identity_OTP WITH (NOLOCK)
						WHERE UserId = @UserId
							AND DATEDIFF(MINUTE, OTPSENDDATETIME, GETDATE()) <= 3
							AND IsActive = 1
						)
				BEGIN
					SET @WrongOTPCount = (
							SELECT WrongOTPCount
							FROM Identity_OTP WITH (NOLOCK)
							WHERE UserId = @UserId
								AND DATEDIFF(MINUTE, OTPSENDDATETIME, GETDATE()) <= 3
								AND IsActive = 1
							)
				END
				ELSE
				BEGIN
					SET @WrongOTPCount = 0
				END

				SET @WrongOTPCount = @WrongOTPCount + 1

				UPDATE [Identity_OTP]
				SET WrongOTPCount = @WrongOTPCount
					,UpdatedOn = GETDATE()
				WHERE UserId = @UserId
					AND DATEDIFF(MINUTE, OTPSENDDATETIME, GETDATE()) <= 3
					AND IsActive = 1

				SELECT CAST(0 AS BIT) IsValidOTP
					,@UserProfileStage AS UserProfileStage
					,@MobileNumber AS MobileNumber
					,CAST(@WrongOTPCount AS VARCHAR(10)) AS WrongOTPCount
			END
		END
		ELSE
		BEGIN
			IF (ISNULL(@UserId, '') = '')
			BEGIN
				SELECT @UserId = UserId
				FROM Identity_User WITH (NOLOCK)
				WHERE MobileNo = @MobileNo
					AND IsActive = 1
			END

			UPDATE [dbo].[Identity_OTP]
			SET IsActive = 0
				,UpdatedOn = GETDATE()
			WHERE UserId = @UserId
				AND @MobileNo = @MobileNo

			INSERT INTO [dbo].[Identity_OTP] (
				OTPId
				,OTPNumber
				,UserId
				,MobileNo
				,CreatedBy
				)
			VALUES (
				@OTPId
				,@OTPNumber
				,@UserId
				,@MobileNo
				,@UserId
				)
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
