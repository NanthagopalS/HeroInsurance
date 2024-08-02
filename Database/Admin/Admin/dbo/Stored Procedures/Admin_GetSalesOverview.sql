
-- exec [Admin_GetSalesOverview] '45EDBC53-507B-4B28-90BD-F0972027CECA','07-22-2023','07-24-2023'  
CREATE PROCEDURE [dbo].[Admin_GetSalesOverview] (
	@UserId VARCHAR(100)
	,@StartDate VARCHAR(100)
	,@EndDate VARCHAR(100)
	)
AS
BEGIN
	BEGIN TRY
		DECLARE @CurrentPolicySold DECIMAL(18, 0) = 0
			,@CurrentPremiumSold DECIMAL(18, 0) = 0
			,@PreviousPolicySold DECIMAL(18, 0) = 0
			,@PreviousPremiumSold DECIMAL(18, 0) = 0
			,@PolicyPercentage DECIMAL(18, 0) = 0
			,@PremiumPercentage DECIMAL(18, 0) = 0
			,@PolicyArrowSign VARCHAR(5) = 'Up'
			,@PremiumArrowSign VARCHAR(5) = 'Up'
		DECLARE @TempTable TABLE (
			Id INT
			,TitleName VARCHAR(100)
			,Amount DECIMAL(18, 0)
			,PercentageValue DECIMAL(18, 0)
			,ArrowSign VARCHAR(5)
			)
		DECLARE @UserId1 VARCHAR(100) = @UserId
			,@StartDate1 VARCHAR(20) = CAST(@StartDate AS DATE)
			,@EndDate1 VARCHAR(20) = CAST(@EndDate AS DATE)
			,@RoleType VARCHAR(100)
			,@PrevStartDate VARCHAR(20) = NULL
			,@PrevEndDate VARCHAR(20) = NULL
			,@DateDifference INT = 0
		DECLARE @UsersByHierarchy TABLE (UserId VARCHAR(100))
		DECLARE @IsSuperAdmin BIT = 0

		INSERT INTO @UsersByHierarchy (UserId)
		EXEC [HeroIdentity].[dbo].[Identity_GetUserIdForDataVisibility] @UserId

		SET @IsSuperAdmin = (
				SELECT CASE 
						WHEN UserId IS NULL
							THEN 0
						ELSE 1
						END AS IsSuperAdmin
				FROM @UsersByHierarchy
				WHERE UserId = '0'
				)
		SET @DateDifference = (CAST(DATEDIFF(day, CAST(@EndDate1 AS DATE), CAST(@StartDate1 AS DATE)) AS INT) - 1)
		SET @PrevEndDate = CAST(DATEADD(day, - 1, CAST(@StartDate1 AS DATE)) AS DATE)
		SET @PrevStartDate = CAST(DATEADD(day, @DateDifference, CAST(@StartDate1 AS DATE)) AS DATE)

		SELECT PM.Amount AS PMAmount
			,LD.LeadId AS LDLeadId
			,LD.CreatedOn
			,LD.CreatedBy
		INTO #TEMP_SALES_OVERVIEW
		FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD WITH (NOLOCK)
		INNER JOIN [HeroInsurance].[dbo].[Insurance_PaymentTransaction] PM WITH (NOLOCK) ON PM.LeadId = LD.LeadId
		INNER JOIN [HeroInsurance].[dbo].[Insurance_Insurer] LI WITH (NOLOCK) ON LI.InsurerId = PM.InsurerId
		INNER JOIN [HeroInsurance].[dbo].[Insurance_PaymentStatus] PS WITH (NOLOCK) ON PS.PaymentId = PM.STATUS
		WHERE PS.PaymentId = 'A25D747B-167E-4C1B-AE13-E6CC49A195F8'
			AND (
				(
					LD.CreatedBy IN (
						SELECT UserId
						FROM @UsersByHierarchy
						)
					OR @IsSuperAdmin = 1
					)
				)
			AND (
				CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)
					AND CAST(@EndDate1 AS DATE)
				)
            AND LI.IsActive = 1
		SELECT PM.Amount AS PMAmount
			,LD.LeadId AS LDLeadId
			,LD.CreatedOn
			,LD.CreatedBy
		INTO #TEMP_PREV_SALES_OVERVIEW
		FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD WITH (NOLOCK)
		INNER JOIN [HeroInsurance].[dbo].[Insurance_PaymentTransaction] PM WITH (NOLOCK) ON PM.LeadId = LD.LeadId
		INNER JOIN [HeroInsurance].[dbo].[Insurance_Insurer] LI WITH (NOLOCK) ON LI.InsurerId = PM.InsurerId
		INNER JOIN [HeroInsurance].[dbo].[Insurance_PaymentStatus] PS WITH (NOLOCK) ON PS.PaymentId = PM.STATUS
		WHERE PS.PaymentId = 'A25D747B-167E-4C1B-AE13-E6CC49A195F8'
			AND (
				(
					LD.CreatedBy IN (
						SELECT UserId
						FROM @UsersByHierarchy
						)
					OR @IsSuperAdmin = 1
					)
				)
			AND (
				CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@PrevStartDate AS DATE)
					AND CAST(@PrevEndDate AS DATE)
				)
            AND LI.IsActive = 1

		SET @CurrentPolicySold = (
				SELECT COUNT(LDLeadId) AS PolicyCount
				FROM #TEMP_SALES_OVERVIEW
				)
		SET @CurrentPremiumSold = (
				SELECT SUM(PMAmount) AS TotalAmount
				FROM #TEMP_SALES_OVERVIEW
				)
		SET @PreviousPolicySold = (
				SELECT COUNT(LDLeadId) AS PolicyCount
				FROM #TEMP_PREV_SALES_OVERVIEW
				)
		SET @PreviousPremiumSold = (
				SELECT SUM(PMAmount) AS TotalAmount
				FROM #TEMP_PREV_SALES_OVERVIEW
				)

		IF (@PreviousPolicySold = @CurrentPolicySold)
		BEGIN
			SET @PolicyPercentage = 0
		END
		ELSE IF (@PreviousPolicySold = 0)
		BEGIN
			SET @PolicyPercentage = 100
		END
		ELSE IF (@CurrentPolicySold = 0)
		BEGIN
			SET @PolicyPercentage = 0
		END
		ELSE IF (@PreviousPolicySold > @CurrentPolicySold)
		BEGIN
			SET @PolicyPercentage = (@CurrentPolicySold / @PreviousPolicySold) * 100
		END
		ELSE IF (@PreviousPolicySold < @CurrentPolicySold)
		BEGIN
			SET @PolicyPercentage = (@PreviousPolicySold / @CurrentPolicySold) * 100
		END

		IF (@CurrentPolicySold < @PreviousPolicySold)
		BEGIN
			SET @PolicyArrowSign = 'Down'
		END

		IF (@CurrentPremiumSold = @PreviousPremiumSold)
		BEGIN
			SET @PremiumPercentage = 0
		END
		ELSE IF (@PreviousPremiumSold = 0)
		BEGIN
			SET @PremiumPercentage = 100
		END
		ELSE IF (@CurrentPremiumSold = 0)
		BEGIN
			SET @PremiumPercentage = 0
		END
		ELSE IF (@PreviousPremiumSold > @CurrentPremiumSold)
		BEGIN
			SET @PremiumPercentage = (@CurrentPremiumSold / @PreviousPremiumSold) * 100
		END

		IF (@PreviousPremiumSold < @CurrentPremiumSold)
		BEGIN
			SET @PremiumPercentage = (@PreviousPremiumSold / @CurrentPremiumSold) * 100
		END

		IF (@CurrentPremiumSold < @PreviousPremiumSold)
		BEGIN
			SET @PremiumArrowSign = 'Down'
		END

		IF (@CurrentPremiumSold IS NULL)
		BEGIN
			SET @PolicyArrowSign = 'Up';
			SET @PremiumArrowSign = 'Up';
		END;

		INSERT INTO @TempTable
		VALUES (
			1
			,'Total Policy Sold'
			,@CurrentPolicySold
			,@PolicyPercentage
			,@PolicyArrowSign
			)
			,(
			2
			,'Total Premium Sold'
			,@CurrentPremiumSold
			,@PremiumPercentage
			,@PremiumArrowSign
			)

		SELECT Id
			,TitleName
			,Amount
			,PercentageValue
			,ArrowSign
		FROM @TempTable
		ORDER BY Id
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