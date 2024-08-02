/*
	EXEC [Admin_UpdateParticularPOSPDetailForIIBDashboard] '1B399CE0-CD90-4EC1-967E-EFB9601BEDA8','NOT EXISTING','SUCCESS'
*/
CREATE
	

 PROCEDURE [dbo].[Admin_UpdateParticularPOSPDetailForIIBDashboard] (
	@UserId VARCHAR(200)
	,@IIBStatus VARCHAR(200)
	,@IIBUploadStatus VARCHAR(200)
	)
AS
BEGIN
	DECLARE @StampId VARCHAR(100) = NULL
		,@DocumentCount INT = 0
		,@ApprovedCount INT = 0
	DECLARE @UserId1 VARCHAR(200) = @UserId
		,@IIBStatus1 VARCHAR(200) = @IIBStatus
		,@IIBUploadStatus1 VARCHAR(200) = @IIBUploadStatus
		,@StampIdAGG VARCHAR(100) = NULL

	BEGIN TRY
		UPDATE [HeroIdentity].[dbo].[Identity_User]
		SET IIBStatus = @IIBStatus
			,IIBUploadStatus = @IIBUploadStatus
			,UpdatedOn = GetDate()
			,iib_upload_date = GETDATE()
		WHERE UserId = @UserId

		IF NOT EXISTS (
				SELECT TOP (1) [Id]
				FROM [HeroPOSP].[dbo].[POSP_Agreement]
				WHERE UserId = @UserId
					AND IsActive = 1
				)
		BEGIN
			SET @StampIdAGG = (
					SELECT TOP (1) Id
					FROM [HeroAdmin].[dbo].[Admin_StampData]
					WHERE StampStatus = 'Available'
					ORDER BY SrNo
					)

			UPDATE [HeroAdmin].[dbo].[Admin_StampData]
			SET StampStatus = 'Blocked'
				,UpdatedOn = GETDATE()
			WHERE Id = @StampIdAGG

			---POSP Agreement Initiate without presigned Agrement ID          
			INSERT INTO [HeroPOSP].[dbo].[POSP_Agreement] (
				UserId
				,PreSignedAgreementId
				,StampId
				)
			VALUES (
				@UserId
				,NULL
				,@StampIdAGG
				)

			EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserId
				,'6D26CFBA-B5FA-46C8-A048-3E96982D90B7'
		END

		IF (
				@IIBStatus1 = 'NOT EXISTING'
				AND @IIBUploadStatus1 = 'SUCCESS'
				)
		BEGIN
			IF NOT EXISTS (
					SELECT TOP (1) UserId
					FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH (NOLOCK)
					WHERE UserId = @UserId
						AND IsActive = 1
					)
			BEGIN
				--check upload doc...kyc check      
				SET @DocumentCount = (
						SELECT count(Id)
						FROM [HeroIdentity].[dbo].[Identity_DocumentTypeMaster] WITH (NOLOCK)
						WHERE IsMandatory = 1
							AND IsActive = 1
						)
				SET @ApprovedCount = (
						SELECT count(Id)
						FROM [HeroIdentity].[dbo].[Identity_DocumentDetail] WITH (NOLOCK)
						WHERE UserId = @UserId
							AND IsActive = 1
							AND IsVerify = 1
							AND IsApprove = 1
							AND DocumentTypeId IN (
								SELECT Id
								FROM [HeroIdentity].[dbo].[Identity_DocumentTypeMaster] WITH (NOLOCK)
								WHERE IsMandatory = 1
									AND IsActive = 1
								)
						)

				IF (@DocumentCount = @ApprovedCount)
				BEGIN
					--Email check....      
					IF EXISTS (
							SELECT TOP (1) Id
							FROM [HeroIdentity].[dbo].Identity_EmailVerification WITH (NOLOCK)
							WHERE UserId = @UserId
								AND IsActive = 1
								AND IsVerify = 1
							)
					BEGIN
						IF NOT EXISTS (
								SELECT TOP 1 UserId
									,*
								FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH (NOLOCK)
								WHERE UserId = @UserId
								)
						BEGIN
							SET @StampId = (
									SELECT TOP (1) Id
									FROM [HeroAdmin].[dbo].[Admin_StampData] WITH (NOLOCK)
									WHERE StampStatus = 'Available'
									ORDER BY SrNo
									)

							UPDATE [HeroAdmin].[dbo].[Admin_StampData]
							SET StampStatus = 'Blocked'
								,UpdatedOn = GETDATE()
							WHERE Id = @StampId

							---POSP Agreement Initiate      
							INSERT INTO [HeroPOSP].[dbo].[POSP_Agreement] (
								UserId
								,StampId
								)
							VALUES (
								@UserId
								,@StampId
								)

							EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserId
								,'6D26CFBA-B5FA-46C8-A048-3E96982D90B7'
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

		EXEC Admin_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name
			,@ErrorDetail = @ErrorDetail
			,@ParameterList = @ParameterList
	END CATCH
END 
