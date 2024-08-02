
--- exec POSP_GetCardDetails '64D44231-0957-452C-A818-F00396B4368D'  
CREATE PROCEDURE [dbo].[POSP_GetCardDetails] (@UserId VARCHAR(100))
AS
BEGIN
	BEGIN TRY
		DECLARE @PremiumCollection INT
			,@Card VARCHAR(50)
			,@POSPId VARCHAR(100)
			,@TodayDate DATETIME = GETDATE()

		SET @PremiumCollection = (
				SELECT sum(convert(INT, TotalPremium))
				FROM [HeroInsurance].[dbo].[Insurance_LeadDetails]
				WHERE CreatedBy = @UserId
				GROUP BY CreatedBy
				)

		IF @PremiumCollection <= 500000
			AND @PremiumCollection = NULL
		BEGIN
			SET @Card = 'Bronze'
		END
		ELSE IF (
				500000 < @PremiumCollection
				AND @PremiumCollection <= 999999
				)
		BEGIN
			SET @Card = 'Silver'
		END
		ELSE IF (
				1000000 < @PremiumCollection
				AND @PremiumCollection <= 4999999
				)
		BEGIN
			SET @Card = 'Gold'
		END
		ELSE IF (5000000 < @PremiumCollection)
		BEGIN
			SET @Card = 'Platinum'
		END
		ELSE
		BEGIN
			SET @Card = 'Bronze'
		END

		-- Check if data exists for the given UserId and Category
		IF EXISTS (
				SELECT 1
				FROM POSP_CardStage
				WHERE UserId = @UserId
					AND StageValue = @Card
				)
		BEGIN
			DECLARE @LastUpdateDate DATETIME
				,@Result BIT = 0

			-- Get the last update date for the data
			SELECT @LastUpdateDate = UpdatedOn
			FROM POSP_CardStage
			WHERE UserId = @UserId
				AND StageValue = @Card

			-- Calculate the difference between the last update date and today's date
			DECLARE @DateDifference INT = DATEDIFF(DAY, @LastUpdateDate, @TodayDate)

			-- Check if the update is less than 2 days from today's date
			IF @DateDifference < 2
			BEGIN
				-- Return true
				SET @Result = 1
			END
			ELSE
			BEGIN
				-- Return false
				SET @Result = 0
			END
		END
		ELSE
		BEGIN
			-- Insert the data into the table
			INSERT INTO POSP_CardStage (
				UserId
				,StageValue
				,UpdatedOn
				)
			VALUES (
				@UserId
				,@Card
				,@TodayDate
				)

			-- Deactivate data with the same UserId but different Category
			UPDATE POSP_CardStage
			SET IsActive = 0
			WHERE UserId = @UserId
				AND StageValue <> @Card
		END

		SET @POSPId = (
				SELECT POSPId
				FROM [HeroIdentity].[dbo].[Identity_User]
				WHERE UserId = @UserId
				)

		SELECT CASE WHEN @PremiumCollection IS NOT NULL THEN @PremiumCollection ELSE 0 END AS gwp
			,@Card AS category
			,@POSPId AS POSP_Id
			,@Result AS Result
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