/* 
exec  [dbo].[POSP_GetPOSPButtonDetail] 'BF69B507-176F-435A-BC6C-3EE73BDE179B'  
*/
--select  * from [POSP_Agreement] where userid='59EB4248-B09B-42A3-B29C-BDE7373CA89F'  
CREATE
	

 PROCEDURE [dbo].[POSP_GetPOSPButtonDetail] (@UserId VARCHAR(100) = NULL)
AS
BEGIN
	DECLARE @Result TABLE (
		ButtonId INT
		,ButtonType VARCHAR(50)
		,ButtonValue VARCHAR(50)
		,ButtonStatus VARCHAR(50)
		)
	DECLARE @IIBStatus VARCHAR(200) = NULL
		,@IIBUploadStatus VARCHAR(200) = NULL
	DECLARE @ProfileButton VARCHAR(50) = 'Profile Verification Success'
		,@TrainingAndExamButton VARCHAR(50) = 'Start Your Training'
		,@AgreementButton VARCHAR(50) = 'Sign Your Agreement'
		,@ButtonStatus VARCHAR(50) = NULL

	BEGIN TRY
		SET @IIBStatus = (
				SELECT IIBStatus
				FROM [HeroIdentity].[dbo].[Identity_User] WITH (NOLOCK)
				WHERE UserId = @UserId
				)
		SET @IIBUploadStatus = (
				SELECT IIBUploadStatus
				FROM [HeroIdentity].[dbo].[Identity_User] WITH (NOLOCK)
				WHERE UserId = @UserId
				)

		IF EXISTS (
				SELECT [Id]
				FROM [HeroIdentity].[dbo].[Identity_DocumentDetail] WITH (NOLOCK)
				WHERE UserId = @UserId
					AND IsActive = 1
					AND IsVerify = 1
					AND IsApprove = 0
				)
		BEGIN
			PRINT ('re-upload')

			SET @ProfileButton = 'Re-upload Document'
			SET @ButtonStatus = 'Enable'
		END
		ELSE IF EXISTS (
				SELECT [Id]
				FROM [HeroIdentity].[dbo].[Identity_DocumentDetail] WITH (NOLOCK)
				WHERE UserId = @UserId
					AND IsActive = 1
					AND IsVerify = 0
					AND (
						IsApprove IS NULL
						OR IsApprove = ''
						OR IsApprove = NULL
						)
				)
		BEGIN
			SET @ProfileButton = 'KYC Pending'
			SET @ButtonStatus = 'Enable'
		END
		ELSE
		BEGIN
				SET @ProfileButton = 'Profile Verification Success'
				SET @ButtonStatus = 'Complete'
		END

		INSERT INTO @Result
		VALUES (
			1
			,'ProfileButton'
			,@ProfileButton
			,@ButtonStatus
			)

		SET @ButtonStatus = 'Enable'

		IF EXISTS (
				SELECT Id
				FROM [HeroPOSP].[dbo].[POSP_Training] WITH (NOLOCK)
				WHERE UserId = @UserId
					AND IsActive = 1
				)
		BEGIN
			IF EXISTS (
					SELECT Id
					FROM [HeroPOSP].[dbo].[POSP_Training] WITH (NOLOCK)
					WHERE UserId = @UserId
						AND IsActive = 1
						AND IsTrainingCompleted = 0
					)
			BEGIN
				SET @TrainingAndExamButton = 'Continue Training'
				SET @ButtonStatus = 'Enable'
			END
			ELSE
			BEGIN
				IF EXISTS (
						SELECT [Id]
						FROM [HeroPOSP].[dbo].[POSP_Exam] WITH (NOLOCK)
						WHERE UserId = @UserId
							AND IsActive = 1
							AND IsCleared = 1
							AND ExamEndDateTime IS NOT NULL
						)
				BEGIN
					SET @TrainingAndExamButton = 'Download Certificate'
					SET @ButtonStatus = 'Complete'
				END
				ELSE
				BEGIN
					SET @TrainingAndExamButton = 'Start Your Exam'
					SET @ButtonStatus = 'Enable'
				END
			END
		END
		ELSE
		BEGIN
			SET @TrainingAndExamButton = 'Start Your Training'
			SET @ButtonStatus = 'Enable'
		END

		INSERT INTO @Result
		VALUES (
			2
			,'TrainingAndExamButton'
			,@TrainingAndExamButton
			,@ButtonStatus
			)

		--Sign Your Agreement && Download Agreement        
		IF (
				@ButtonStatus = 'Complete'
				AND @TrainingAndExamButton = 'Download Certificate'
				)
		BEGIN
			IF EXISTS (
					SELECT Id
					FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH (NOLOCK)
					WHERE UserId = @UserId
						AND IsActive = 1
						AND AgreementId IS NOT NULL
					)
			BEGIN
				INSERT INTO @Result
				VALUES (
					3
					,'AgreementButton'
					,'Download Agreement'
					,'Complete'
					)
			END
			ELSE
			BEGIN
				IF EXISTS (
						SELECT Id
						FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH (NOLOCK)
						WHERE UserId = @UserId
							AND IsActive = 0
							AND AgreementId IS NULL
							AND IsRevoked = 1
						)
				BEGIN
					INSERT INTO @Result
					VALUES (
						3
						,'AgreementButton'
						,@AgreementButton
						,'Disable'
						)
				END
				ELSE IF EXISTS (
						SELECT Id
						FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH (NOLOCK)
						WHERE UserId = @UserId
							AND IsActive = 1
							AND AgreementId IS NULL
							AND DATEDIFF(DAY, CreatedOn, GETDATE()) > 3
						)
				BEGIN
					INSERT INTO @Result
					VALUES (
						3
						,'AgreementButton'
						,@AgreementButton
						,'Disable'
						)
				END
				ELSE
				BEGIN
					IF EXISTS (
							SELECT TOP (1) Id
							FROM [HeroIdentity].[dbo].[Identity_EmailVerification] WITH (NOLOCK)
							WHERE UserId = @UserId
								AND IsVerify = 1
							)
					BEGIN
						IF (
								@ProfileButton = 'Profile Verification Success'
								AND @IIBStatus = 'NOT EXISTING'
								AND @IIBUploadStatus = 'SUCCESS'
								)
						BEGIN
							INSERT INTO @Result
							VALUES (
								3
								,'AgreementButton'
								,@AgreementButton
								,'Enable'
								)
						END
						ELSE
						BEGIN
							INSERT INTO @Result
							VALUES (
								3
								,'AgreementButton'
								,@AgreementButton
								,'Disable'
								)
						END
					END
					ELSE
					BEGIN
						INSERT INTO @Result
						VALUES (
							3
							,'AgreementButton'
							,@AgreementButton
							,'Disable'
							)
					END
				END
			END
		END
		ELSE
		BEGIN
			INSERT INTO @Result
			VALUES (
				3
				,'AgreementButton'
				,@AgreementButton
				,'Disable'
				)
		END

		DECLARE @cnt INT
			,@usercount INT;

		SELECT @cnt = count(ButtonType)
		FROM @Result
		WHERE ButtonType = 'AgreementButton'
			AND ButtonStatus = 'Enable'

		SELECT @usercount = count(userid)
		FROM [HeroPOSP].[dbo].[POSP_Agreement]
		WHERE UserId = @UserId
			AND IsActive = 1

		IF (@cnt > 0)
		BEGIN
			IF (@usercount = 0)
			BEGIN
				INSERT INTO [HeroPOSP].[dbo].[POSP_Agreement] (UserId)
				SELECT @UserId
			END
		END

		SELECT ButtonType
			,ButtonValue
			,ButtonStatus
		FROM @Result
		ORDER BY ButtonId
	END TRY

	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500)
			,@ErrorDetail VARCHAR(1000)
			,@ParameterList VARCHAR(2000)

		SET @StrProcedure_Name = ERROR_PROCEDURE()
		SET @ErrorDetail = ERROR_MESSAGE()

		EXEC POSP_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name
			,@ErrorDetail = @ErrorDetail
			,@ParameterList = @ParameterList
	END CATCH
END 
