/*  
  [dbo].[POSP_InsertPOSPTrainingDetail] 'BF69B507-176F-435A-BC6C-3EE73BDE179B','Finish','EA8946BA-F743-4C9C-A938-40440EF1DADF'  
  */
CREATE
	

 PROCEDURE [dbo].[POSP_InsertPOSPTrainingDetail] (
	@UserId VARCHAR(500)
	,@TrainingStatus VARCHAR(500)
	,--General/Life/Finish    
	@TrainingId VARCHAR(500) = NULL
	)
AS
BEGIN
	DECLARE @IdealEndDateTime DATETIME
	DECLARE @StatusId VARCHAR(50) = NULL
	DECLARE @IIBCHECK VARCHAR(100) = NULL

	SET @IdealEndDateTime = (
			SELECT DATEADD(MINUTE, 2, GETDATE()) AS DateAddedValue
			)

	BEGIN TRY
		IF (@TrainingStatus = 'General Insurance')
		BEGIN
			SET @StatusId = (
					SELECT Id
					FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster] WITH (NOLOCK)
					WHERE StatusName = 'Training & Exam'
						AND PriorityIndex = 6
					)

			--UPDATE Breadcrum stage for PSOP User            
			UPDATE [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusDetail]
			SET StatusId = @StatusId, UpdatedOn = GETDATE()
			WHERE UserId = @UserId
				AND StatusId IN (
					SELECT Id
					FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster] WITH (NOLOCK)
					WHERE StatusName = 'Training & Exam'
						AND PriorityIndex IN (
							5
							,6
							)
					)

			-- update StageId to Training     
			UPDATE [HeroPOSP].[dbo].[POSP_UserStageStatusDetail]
			SET StageId = 'E6F84D7A-A6F9-4141-B5BD-A20DAFA1D371', UpdatedOn = GETDATE()
			WHERE UserId = @UserId

			IF NOT EXISTS (
					SELECT TOP 1 Id
					FROM POSP_Training WITH (NOLOCK)
					WHERE UserId = @UserId
						AND IsActive = 1
					)
			BEGIN
				--Set all old entry to false    
				UPDATE [dbo].[POSP_Training]
				SET IsActive = 0
				WHERE UserId = @UserId

				--Insert New Training Entry...    
				INSERT INTO [dbo].[POSP_Training] (
					UserId
					,GeneralInsuranceIdealEndDateTime
					)
				VALUES (
					@UserId
					,@IdealEndDateTime
					)
			END
		END
		ELSE IF (@TrainingStatus = 'Life Insurance')
		BEGIN
			UPDATE POSP_Training
			SET GeneralInsuranceEndDateTime = GETDATE()
				,LifeInsuranceStartDateTime = GETDATE()
				,IsGeneralInsuranceCompleted = 1
				,LifeInsuranceIdealEndDateTime = @IdealEndDateTime, UpdatedOn = GETDATE()
			WHERE UserId = @UserId
				AND IsActive = 1
				AND Id = @TrainingId
		END
		ELSE IF (@TrainingStatus = 'Finish')
		BEGIN
			UPDATE POSP_Training
			SET LifeInsuranceEndDateTime = GETDATE()
				,IsLifeInsuranceCompleted = 1
				,IsTrainingCompleted = 1, UpdatedOn = GETDATE()
			WHERE UserId = @UserId
				AND IsActive = 1
				AND Id = @TrainingId

			-- update StageId to Exam    
			UPDATE [HeroPOSP].[dbo].[POSP_UserStageStatusDetail]
			SET StageId = 'D56CA403-6B9E-48F9-B608-F008472EFACC', UpdatedOn = GETDATE()
			WHERE UserId = @UserId

			-- Check IIB Status and KYC    
			SELECT @IIBCHECK = CASE 
					WHEN EXISTS (
							SELECT 1
							FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusDetail]
							WHERE UserId = @UserId
								AND (
									StatusId = 'D07C1631-7FF7-4B4F-A28C-677A7961ED23'
									OR StatusId = '69F5F8AF-863B-46A5-AF6C-29332A9400B7'
									)
							)
						THEN 1
					ELSE 0
					END;

			IF @IIBCHECK = 1
				--Update StageId to Agreement    
			BEGIN
				UPDATE [HeroPOSP].[dbo].[POSP_UserStageStatusDetail]
				SET StageId = '6D26CFBA-B5FA-46C8-A048-3E96982D90B7', UpdatedOn = GETDATE()
				WHERE UserId = @UserId;
			END
			
			EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserId, 'D56CA403-6B9E-48F9-B608-F008472EFACC'

		END

		SELECT TOP (1) Id AS TrainingId
			,GeneralInsuranceStartDateTime
			,GeneralInsuranceIdealEndDateTime
			,GeneralInsuranceEndDateTime
			,LifeInsuranceStartDateTime
			,LifeInsuranceIdealEndDateTime
			,LifeInsuranceEndDateTime
		FROM [dbo].[POSP_Training] WITH (NOLOCK)
		WHERE IsActive = 1
			AND UserId = @UserId
		ORDER BY GeneralInsuranceStartDateTime DESC
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
