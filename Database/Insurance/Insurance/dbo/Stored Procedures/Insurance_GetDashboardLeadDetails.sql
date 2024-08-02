
/*        
EXEC [Insurance_GetDashboardLeadDetails] 'LEAD','12049EB5-0C81-42F8-B071-95C0017C958C','','','','','','','2023-04-22','2023-06-22',1,5,1,0
EXEC [Insurance_GetDashboardLeadDetails] 'LEAD','','','','','','','','','',1,5000,0   
EXEC [Insurance_GetDashboardLeadDetails] 'LEAD','0655B30D-0CD7-4C9D-9FCF-754FFCBCB7B9','','','','','','','','',1,5,1,0  
EXEC [Insurance_GetDashboardLeadDetails] 'LEAD','45BBA540-07C5-4D4C-BEFF-771AE2FC32B0','','','','','','','','',1,100,0,1  
EXEC [Insurance_GetDashboardLeadDetails] 'LEAD','F24C9610-CF2D-4D95-A11D-F1B736475BAA','','','','','','','2023-05-24','2023-07-24',1,10,0,0 

EXEC [Insurance_GetDashboardLeadDetails] 'LEAD','45BBA540-07C5-4D4C-BEFF-771AE2FC32B0','','','','','','A25D747B-167E-4C1B-AE13-E6CC49A195F8','2023-07-21','2023-09-19',1,1000,0,1 


*/
CREATE PROCEDURE [dbo].[Insurance_GetDashboardLeadDetails] (
	@ViewLeadsType VARCHAR(100) = 'LEAD'
	,@UserId VARCHAR(100) = NULL
	,@SearchText VARCHAR(100) = NULL
	,@LeadType VARCHAR(100) = NULL
	,-- no idea               
	@POSPId VARCHAR(100) = NULL
	,@PolicyType VARCHAR(100) = NULL
	,@PreQuote VARCHAR(100) = NULL
	,@AllStatus VARCHAR(100) = NULL
	,@StartDate VARCHAR(100) = NULL
	,@EndDate VARCHAR(100) = NULL
	,@CurrentPageIndex INT = 1
	,@CurrentPageSize INT = 500
	,@IsFromDashboard BIT = 0
	,@IsFromPaymentGatewayScreen BIT = 0
	)
AS
BEGIN
	BEGIN TRY
		DECLARE @TotalRecord INT = 5
		DECLARE @LEAD_PROSPECT_BORDER INT = 5000000
		DECLARE @IsAdmin BIT = 0
		DECLARE @UsersByHierarchy TABLE (UserId VARCHAR(100))
		DECLARE @IsSuperAdmin BIT = 0
		DECLARE @LEADVIEWTABLETEMP AS TABLE (
			Education VARCHAR(100)
			,Profession VARCHAR(100)
			,RegNo VARCHAR(100)
			,VehicleManufacturerName VARCHAR(100)
			,Model VARCHAR(100)
			,VechicleType VARCHAR(100)
			,RegDate VARCHAR(20)
			,AddressType VARCHAR(10)
			,Address1 VARCHAR(200)
			,Address2 VARCHAR(200)
			,Address3 VARCHAR(200)
			,Pincode VARCHAR(10)
			,City VARCHAR(100)
			,AState VARCHAR(100)
			,Country VARCHAR(100)
			,DOB DATE
			,Gender VARCHAR(50)
			,UserId VARCHAR(50)
			,POSPId VARCHAR(50)
			,LeadId VARCHAR(50)
			,CustomerName VARCHAR(100)
			,StageValue VARCHAR(50)
			,MobileNo VARCHAR(50)
			,EmailId VARCHAR(50)
			,PolicyType VARCHAR(50)
			,PolicyTypeId VARCHAR(50)
			,GeneratedOn DATETIME
			,ExpiringOn VARCHAR(50)
			,Product VARCHAR(50)
			,Amount FLOAT
			,PolicyStatus VARCHAR(50)
			,PaymentStatus VARCHAR(50)
			,CreatedBy VARCHAR(50)
			,StageId VARCHAR(50)
			,VehicleTypeId VARCHAR(50)
			,IsActive BIT
			,VehicleType VARCHAR(50)
			,InsurerId VARCHAR(50)
			,QuoteTransactionId VARCHAR(50)
			)

		SET @IsAdmin = (
				SELECT CASE 
						WHEN RM.RoleName = 'POSP'
							THEN 0
						WHEN RM.RoleName != 'POSP'
							THEN 1
						END AS IsAdmin
				FROM HeroIdentity.dbo.Identity_User AS IU WITH (NOLOCK)
				LEFT JOIN [HeroIdentity].[dbo].[Identity_RoleMaster] RM ON IU.RoleId = RM.RoleId
				WHERE IU.UserId = @UserId
				)

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

		SELECT ' ' AS Education
			,IL.Profession
			,IL.VehicleNumber AS RegNo
			,MAKE.MakeName AS VehicleManufacturerName
			,MODEL.ModelName AS Model
			,VEHTYPE.VehicleType AS VechicleType
			,IL.RegistrationDate AS RegDate
			,LEADADDRESSDET.AddressType
			,LEADADDRESSDET.Address1
			,LEADADDRESSDET.Address2
			,LEADADDRESSDET.Address3
			,LEADADDRESSDET.Pincode
			,LEADADDRESSDET.City AS City
			,LEADADDRESSDET.STATE AS AState
			,LEADADDRESSDET.Country AS Country
			,IL.DOB
			,CASE 
				WHEN IL.Gender IS NULL
					OR IL.Gender = ''
					THEN ''
				ELSE CONCAT (
						UPPER(LEFT(IL.Gender, 1))
						,LOWER(RIGHT(IL.Gender, LEN(IL.Gender) - 1))
						)
				END AS Gender
			,IU.UserId
			,IU.POSPId
			,IL.LeadId
			,IL.LeadName AS CustomerName
			,SM.Stage AS StageValue
			,IL.PhoneNumber AS MobileNo
			,IL.Email AS EmailId
			,INSURETYPE.InsuranceName AS PolicyType
			,IL.VehicleTypeId AS PolicyTypeId
			,IL.CreatedOn AS GeneratedOn
			,IL.PolicyEndDate AS ExpiringOn
			,INSURETYPE.InsuranceType AS Product
			,CONVERT(float,IL.GrossPremium) AS Amount
			,CASE 
				WHEN PM.[Status] = 'A25D747B-167E-4C1B-AE13-E6CC49A195F8' THEN 'successful'
				WHEN PM.[Status] = '0151C6E3-8DC5-4BBD-860A-F1501A7647B2' THEN 'Pending'
				WHEN PM.[Status] = 'EBA950EF-6739-4236-8DF0-EA8E69E65C66' THEN 'Cancelled'
				END AS PolicyStatus
			,PAYSTATUS.PaymentStatus AS PaymentStatus
			,CASE 
				WHEN IU.POSPId IS NOT NULL
					THEN IU.POSPId + '-' + IU.UserName
				WHEN IU.EmpID IS NOT NULL
					THEN IU.EmpID + '-' + IU.UserName
				END AS CreatedBy
			,IL.StageId
			,IL.VehicleTypeId
			,IL.IsActive
			,VEHTYPE.VehicleType
			,IL.InsurerId AS InsurerId
			,PM.QuoteTransactionId AS QuoteTransactionId
		INTO #TEMP_LEADS
		FROM [Insurance_LeadDetails] IL WITH (NOLOCK)
		LEFT JOIN [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK) ON IU.UserId = IL.CreatedBy
		LEFT JOIN [Insurance_StageMaster] SM WITH (NOLOCK) ON SM.StageId = IL.StageId
		LEFT JOIN [Insurance_PaymentTransaction] PM WITH (NOLOCK) ON PM.LeadId = IL.LeadId
			AND IL.QuoteTransactionID = PM.QuoteTransactionId
		LEFT JOIN [Insurance_InsuranceType] INSURETYPE WITH (NOLOCK) ON INSURETYPE.InsuranceTypeId = IL.VehicleTypeId
		LEFT JOIN [Insurance_LeadAddressDetails] LEADADDRESSDET WITH (NOLOCK) ON IL.LeadId = LEADADDRESSDET.LeadID
		LEFT JOIN [Insurance_PaymentStatus] PAYSTATUS WITH (NOLOCK) ON PAYSTATUS.PaymentId = PM.[Status]
		LEFT JOIN [Insurance_Variant] VARIANT WITH (NOLOCK) ON VARIANT.VariantId = IL.VariantId
		LEFT JOIN [Insurance_VehicleType] VEHTYPE WITH (NOLOCK) ON VEHTYPE.VehicleTypeId = VARIANT.VehicleTypeId
		LEFT JOIN [Insurance_Model] MODEL WITH (NOLOCK) ON MODEL.ModelId = VARIANT.ModelId
		LEFT JOIN [Insurance_Make] MAKE WITH (NOLOCK) ON MAKE.MakeId = MODEL.MakeId
		WHERE IL.IsManualPolicy = 0 AND (
				CAST(IL.CreatedOn AS DATE) >= CAST(@StartDate AS DATE)
				OR ISNULL(@StartDate, '') = ''
				)
			AND (
				CAST(IL.CreatedOn AS DATE) <= CAST(@EndDate AS DATE)
				OR ISNULL(@EndDate, '') = ''
				)
			AND (
				LOWER(PM.[Status]) = LOWER(@AllStatus)
				OR ISNULL(@AllStatus, '') = ''
				)
			AND (
				IL.CreatedBy IN (
					SELECT UserId
					FROM @UsersByHierarchy
					)
				OR @IsSuperAdmin = 1
				)
			AND (
				IL.StageId = @PreQuote
				OR ISNULL(@PreQuote, '') = ''
				)
			AND IL.IsActive = 1
			AND (
				(@IsFromPaymentGatewayScreen = 0 
					AND PM.[Status] IS NULL
				)
				OR (
					@IsFromPaymentGatewayScreen = 1
					AND PM.PaymentTransactionId IS NOT NULL
					AND PM.[Status] IS NOT NULL
					)
				)
			AND (
				(IL.LeadName LIKE '%' + ISNULL(@SearchText, '') + '%')
				OR (INSURETYPE.InsuranceName LIKE '%' + ISNULL(@SearchText, '') + '%')
				OR (IL.PhoneNumber LIKE '%' + ISNULL(@SearchText, '') + '%')
				OR (IL.Email LIKE '%' + ISNULL(@SearchText, '') + '%')
				OR (IU.POSPId LIKE '%' + ISNULL(@SearchText, '') + '%')
				OR (IL.LeadId LIKE '%' + ISNULL(@SearchText, '') + '%')
				OR ISNULL(@SearchText, '') = ''
				)
		ORDER BY IL.CreatedOn DESC

		IF (@IsFromDashboard = 1)
		BEGIN
			IF (@ViewLeadsType = 'PROSPECT')
			BEGIN
				IF (@IsAdmin = 0)
				BEGIN
					INSERT @LEADVIEWTABLETEMP
					SELECT *
					FROM #TEMP_LEADS TL WITH (NOLOCK)
					WHERE (Amount > @LEAD_PROSPECT_BORDER)
						AND (
							(
								TL.MobileNo IS NOT NULL
								AND TL.MobileNo != ''
								)
							OR (
								TL.CustomerName IS NOT NULL
								AND TL.CustomerName != ''
								)
							OR (
								TL.EmailId IS NOT NULL
								AND TL.EmailId != ''
								)
							)
				END
				ELSE
				BEGIN
					INSERT @LEADVIEWTABLETEMP
					SELECT *
					FROM #TEMP_LEADS TL WITH (NOLOCK)
					WHERE (Amount > @LEAD_PROSPECT_BORDER)
				END
			END
			ELSE
			BEGIN
				IF (@IsAdmin = 0)
				BEGIN
					INSERT @LEADVIEWTABLETEMP
					SELECT *
					FROM #TEMP_LEADS TL WITH (NOLOCK)
					WHERE (
							Amount <= @LEAD_PROSPECT_BORDER
							OR ISNULL(Amount, '') = ''
							)
						AND (
							(
								TL.MobileNo IS NOT NULL
								AND TL.MobileNo != ''
								)
							OR (
								TL.CustomerName IS NOT NULL
								AND TL.CustomerName != ''
								)
							OR (
								TL.EmailId IS NOT NULL
								AND TL.EmailId != ''
								)
							)
				END
				ELSE
				BEGIN
					INSERT @LEADVIEWTABLETEMP
					SELECT *
					FROM #TEMP_LEADS WITH (NOLOCK)
					WHERE (
							Amount <= @LEAD_PROSPECT_BORDER
							OR ISNULL(Amount, '') = ''
							)
				END
			END

			BEGIN
				SELECT TOP (5) *
				FROM @LEADVIEWTABLETEMP
				ORDER BY GeneratedOn DESC
			END
		END
		ELSE
		BEGIN
			IF (@ViewLeadsType = 'PROSPECT')
			BEGIN
				IF (@IsAdmin = 0)
				BEGIN
					INSERT @LEADVIEWTABLETEMP
					SELECT *
					FROM #TEMP_LEADS TL WITH (NOLOCK)
					WHERE (Amount > @LEAD_PROSPECT_BORDER)
						AND (
							(
								TL.MobileNo IS NOT NULL
								AND TL.MobileNo != ''
								)
							OR (
								TL.CustomerName IS NOT NULL
								AND TL.CustomerName != ''
								)
							OR (
								TL.EmailId IS NOT NULL
								AND TL.EmailId != ''
								)
							)
				END
				ELSE
				BEGIN
					INSERT @LEADVIEWTABLETEMP
					SELECT *
					FROM #TEMP_LEADS TL WITH (NOLOCK)
					WHERE (Amount > @LEAD_PROSPECT_BORDER)
				END
			END
			ELSE
			BEGIN
				IF (@IsAdmin = 0)
				BEGIN
					INSERT @LEADVIEWTABLETEMP
					SELECT *
					FROM #TEMP_LEADS TL WITH (NOLOCK)
					WHERE (
							Amount <= @LEAD_PROSPECT_BORDER
							OR ISNULL(Amount, '') = ''
							)
						AND (
							PolicyTypeId = @PolicyType
							OR ISNULL(@PolicyType, '') = ''
							)
						AND (
							(
								TL.MobileNo IS NOT NULL
								AND TL.MobileNo != ''
								)
							OR (
								TL.CustomerName IS NOT NULL
								AND TL.CustomerName != ''
								)
							OR (
								TL.EmailId IS NOT NULL
								AND TL.EmailId != ''
								)
							)
				END
				ELSE
				BEGIN
					INSERT @LEADVIEWTABLETEMP
					SELECT *
					FROM #TEMP_LEADS WITH (NOLOCK)
					WHERE (
							Amount <= @LEAD_PROSPECT_BORDER
							OR ISNULL(Amount, '') = ''
							)
						AND (
							PolicyTypeId = @PolicyType
							OR ISNULL(@PolicyType, '') = ''
							)
				END
			END

			BEGIN
				SELECT @TotalRecord = COUNT(1)
				FROM @LEADVIEWTABLETEMP

				SELECT *
					,@TotalRecord AS TotalRecord
				FROM @LEADVIEWTABLETEMP
				ORDER BY GeneratedOn DESC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS

				FETCH NEXT @CurrentPageSize ROWS ONLY
			END
		END

		DROP TABLE #TEMP_LEADS
	END TRY

	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500)
			,@ErrorDetail VARCHAR(1000)
			,@ParameterList VARCHAR(2000)

		SET @StrProcedure_Name = ERROR_PROCEDURE()
		SET @ErrorDetail = ERROR_MESSAGE()

		EXEC Insurance_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name
			,@ErrorDetail = @ErrorDetail
			,@ParameterList = @ParameterList
	END CATCH
END 