
-- exec Admin_GetTotalSales '1ABF6E56-FF59-4813-83B1-84F26473B99B','2023-09-01','2023-09-14'  
-- exec Admin_GetTotalSales '45BBA540-07C5-4D4C-BEFF-771AE2FC32B0','2023-09-01','2023-09-14'  
CREATE PROCEDURE [dbo].[Admin_GetTotalSales] (
	@UserId VARCHAR(100) = NULL
	,@StartDate VARCHAR(100) = NULL
	,@EndDate VARCHAR(100) = NULL
	,@ImageLogoUrl VARCHAR(100) = NULL
	)
AS
BEGIN
	BEGIN TRY
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

		BEGIN
			SELECT CAST(LI.InsurerId AS VARCHAR(100)) AS ICId
				,LI.InsurerName AS ICName
				,@ImageLogoUrl + LI.Logo AS ICLogoImageString
				,IType.InsuranceType AS ProductTypeName
				,SUM(PM.Amount) AS TotalAmount
				,MAX(PM.UpdatedOn) AS LastSalesDate
			FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD WITH (NOLOCK)
			INNER JOIN [HeroInsurance].[dbo].[Insurance_PaymentTransaction] PM WITH (NOLOCK) ON PM.LeadId = LD.LeadId
			INNER JOIN [HeroInsurance].[dbo].[Insurance_Insurer] LI WITH (NOLOCK) ON LI.InsurerId = PM.InsurerId
			INNER JOIN [HeroInsurance].[dbo].[Insurance_InsuranceType] IType WITH (NOLOCK) ON IType.InsuranceTypeId = LD.VehicleTypeId
			INNER JOIN [HeroInsurance].[dbo].[Insurance_PaymentStatus] PS WITH (NOLOCK) ON PS.PaymentId = PM.STATUS
			WHERE PS.PaymentId = 'A25D747B-167E-4C1B-AE13-E6CC49A195F8'
				AND LI.IsActive = 1
				AND (
					LI.Logo IS NOT NULL
					OR LI.Logo <> ''
					)
				AND (
				CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate AS DATE)
					AND CAST(@EndDate AS DATE)
				)
				AND (
					(
						LD.CreatedBy IN (
							SELECT UserId
							FROM @UsersByHierarchy
							)
						OR @IsSuperAdmin = 1
						)
					)
			GROUP BY LI.InsurerId
				,LI.InsurerName
				,@ImageLogoUrl + LI.Logo
				,IType.InsuranceType
			ORDER BY LastSalesDate DESC;
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
