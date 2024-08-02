
/*      
exec [Insurance_GetCustomerDetail] 'Customers','','','2023-04-16','2023-06-16',1,40,'8E5CFA10-4CAA-4100-92AD-9A0E9B3ACC4F'      
exec [Insurance_GetCustomerDetail] 'Customers','','2d566966-5525-4ed7-bd90-bb39e8418f39,6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056','2023-03-23','2023-05-23',1,500,'E16ABF13-5BA1-477C-A224-740B5F849E64'      
exec [Insurance_GetCustomerDetail] 'Customers','','','2023-01-01','2023-12-31',1,10000,'E16ABF13-5BA1-477C-A224-740B5F849E64'      
exec [Insurance_GetCustomerDetail] 'Renewals','','','2023-03-29','2023-05-29',1,10,'B3769E78-5331-4284-AA0E-3010E6DCD67A'      
exec [Insurance_GetCustomerDetail] 'Policies','','','2023-07-16','2023-09-14',1,500,'45BBA540-07C5-4D4C-BEFF-771AE2FC32B0','',''      
*/

CREATE     PROCEDURE [dbo].[Insurance_GetCustomerDetail] (
	@CustomerType VARCHAR(100) = NULL -- Customers,Renewals,Policies
	,@SearchText VARCHAR(100) = NULL
	,@PolicyType VARCHAR(max) = NULL -- 4W,2W
	,@StartDate VARCHAR(100) = NULL
	,@EndDate VARCHAR(100) = NULL
	,@CurrentPageIndex INT = 1
	,@CurrentPageSize INT = 500
	,@CreatedBy VARCHAR(50) = NULL
	,@PolicyStatus VARCHAR(10) = NULL -- new,renewal,expired
	,@PolicyNature VARCHAR(10) = NULL -- ONLINE,OFFLINE
	)
AS
BEGIN
	BEGIN TRY
		DECLARE @TEMPCUSTOMERDETAILS TABLE (
			RTOCode VARCHAR(100)
			,IsBrandNew bit
			,VariantId VARCHAR(100)
			,RTOId VARCHAR(100)
			,PolicyTypeId VARCHAR(100)
			,RegNo VARCHAR(100)
			,RegDate VARCHAR(100)
			,Model VARCHAR(100)
			,VehicleType VARCHAR(100)
			,RegAuthority VARCHAR(100)
			,VariantName VARCHAR(100)
			,FuelId VARCHAR(100)
			,FuelName VARCHAR(100)
			,Maker VARCHAR(100)
			,PreviousSAODInsurer VARCHAR(100)
			,PrevPolicyTypeId VARCHAR(100)
			,IsPrevPolicy VARCHAR(100)
			,PreviousSATPInsurer VARCHAR(100)
			,PreviousPolicyNumberSAOD VARCHAR(100)
			,PreviousPolicyExpirySAOD VARCHAR(100)
			,PrevPolicyNCB VARCHAR(100)
			,PrevPolicyClaims VARCHAR(100)
			,IsPolicyExpired VARCHAR(100)
			,PrevPolicyExpiryDate VARCHAR(100)
			,ODTPExp VARCHAR(100)
			,Stage VARCHAR(100)
			,StageId VARCHAR(100)
			,QuoteTransactionID VARCHAR(100)
			,InsurerId VARCHAR(100)
			,UserId VARCHAR(100)
			,CustomerName VARCHAR(100)
			,MobileNo VARCHAR(100)
			,LeadId VARCHAR(100)
			,PolicyStartDate VARCHAR(100)
			,PolicyEndDate VARCHAR(100)
			,TotalPremium VARCHAR(100)
			,InsurerName VARCHAR(100)
			,EmailId VARCHAR(100)
			,PolicyNo VARCHAR(100)
			,ProductType VARCHAR(100)
			,PolicyIssueDate VARCHAR(100)
			,Premium VARCHAR(100)
			,GeneratedOn VARCHAR(100)
			,CreatedOn VARCHAR(100)
			,ExpiringOn VARCHAR(100)
			,PolicyType VARCHAR(100)
			,PolicyNumber VARCHAR(100)
			,VehicleTypeId VARCHAR(100)
			,PolicyStatus VARCHAR(100)
			,PolicyNature VARCHAR(100)
			,ActualExpiry VARCHAR(100)
			)
		DECLARE @TotalRecord INT = 0
		DECLARE @IsSuperAdmin BIT = 0
		DECLARE @UsersByHierarchy TABLE (UserId VARCHAR(100))

		INSERT INTO @UsersByHierarchy (UserId)
		EXEC [HeroIdentity].[dbo].[Identity_GetUserIdForDataVisibility] @CreatedBy

		SET @IsSuperAdmin = (
				SELECT CASE 
						WHEN UserId IS NULL
							THEN 0
						ELSE 1
						END AS IsSuperAdmin
				FROM @UsersByHierarchy
				WHERE UserId = '0'
				)

		SELECT RTO.RTOCode
			,LEADDETAILS.IsBrandNew
			,LEADDETAILS.VariantId
			,LEADDETAILS.RTOId
			,LEADDETAILS.PolicyTypeId
			,LEADDETAILS.VehicleNumber AS RegNo
			,LEADDETAILS.RegistrationDate AS RegDate
			,MODEL.ModelName AS Model 
			,VEHTYPE.VehicleType
			,VECHI_REG.regAuthority AS RegAuthority
			,CASE WHEN LEADDETAILS.IsManualPolicy = 1 THEN MLD.Variant ELSE VARIANT.VariantName END AS VariantName
			,FULE.FuelId
			,CASE WHEN LEADDETAILS.IsManualPolicy = 1 THEN MLD.Fuel ELSE FULE.FuelName END AS FuelName
			,CASE 
				WHEN LEADDETAILS.IsManualPolicy = 1
					THEN MLD.Make
				ELSE MODEL.ModelName
				END AS Maker
			,LEADDETAILS.PreviousSAODInsurer
			,LEADDETAILS.PrevPolicyTypeId
			,LEADDETAILS.IsPrevPolicy
			,LEADDETAILS.PreviousSATPInsurer
			,LEADDETAILS.PreviousPolicyNumberSAOD
			,LEADDETAILS.PreviousPolicyExpirySAOD
			,LEADDETAILS.PrevPolicyNCB
			,LEADDETAILS.PrevPolicyClaims
			,LEADDETAILS.isPolicyExpired AS IsPolicyExpired
			,LEADDETAILS.PrevPolicyExpiryDate
			,CASE 
				WHEN LEADDETAILS.IsBrandNew = 1
					AND (
						PREPTYPE.PreviousPolicyTypeId = '20541BE3-D76E-4E73-9AB1-240CCB33DA5D'
						OR PREPTYPE.PreviousPolicyTypeId = '517D8F9C-F532-4D45-8034-ABECE46693E3'
						)
					AND ISDATE(LEADDETAILS.PolicyStartDate) = 1
					THEN CAST(DATEADD(DAY, - 1, DATEADD(YEAR, 1, LEADDETAILS.PolicyStartDate)) AS DATE)
				ELSE NULL
				END AS ODTPExp
			,STAGEMASTER.Stage
			,LEADDETAILS.StageId
			,LEADDETAILS.QuoteTransactionID
			,LEADDETAILS.InsurerId
			,LEADDETAILS.CreatedBy AS UserId
			,LEADDETAILS.LeadName AS CustomerName
			,LEADDETAILS.PhoneNumber AS MobileNo
			,LEADDETAILS.LeadId
			,LEADDETAILS.PolicyStartDate
			,LEADDETAILS.PolicyEndDate
			,LEADDETAILS.TotalPremium
			,INSURER.InsurerName
			,LEADDETAILS.Email AS EmailId
			,ISNULL(PT.PolicyNumber, PT.ProposalNumber) AS PolicyNo
			,INSTYPE.InsuranceName AS ProductType
			,CASE 
				WHEN ISDATE(LEADDETAILS.PolicyStartDate) = 1
					THEN CAST(LEADDETAILS.PolicyStartDate AS DATE)
				ELSE NULL
				END AS PolicyIssueDate
			,LEADDETAILS.GrossPremium AS Premium
			,CASE 
				WHEN ISDATE(LEADDETAILS.CreatedOn) = 1
					THEN CAST(LEADDETAILS.CreatedOn AS DATE)
				ELSE NULL
				END AS GeneratedOn
			,LEADDETAILS.CreatedOn
			,CASE 
				WHEN ISDATE(LEADDETAILS.PolicyEndDate) = 1
					THEN CAST(LEADDETAILS.PolicyEndDate AS DATE)
				ELSE NULL
				END AS ExpiringOn
			,POLICYTYPE.PreviousPolicyType AS PolicyType
			,LEADDETAILS.PolicyNumber
			,LEADDETAILS.VehicleTypeId
			,'' AS PolicyStatus
			,CASE 
				WHEN LEADDETAILS.IsManualPolicy = 1
					THEN 'Offline'
				ELSE 'Online'
				END AS PolicyNature
		INTO #TEMP
		FROM [Insurance_LeadDetails] LEADDETAILS WITH (NOLOCK)
		LEFT JOIN [Insurance_InsuranceType] INSTYPE WITH (NOLOCK) ON LEADDETAILS.VehicleTypeId = INSTYPE.InsuranceTypeId
		LEFT JOIN [Insurance_PaymentTransaction] PT WITH (NOLOCK) ON LEADDETAILS.QuoteTransactionID = PT.QuoteTransactionId
		LEFT JOIN [Insurance_Insurer] INSURER WITH (NOLOCK) ON LEADDETAILS.InsurerId = INSURER.InsurerId
		LEFT JOIN Insurance_PreviousPolicyType POLICYTYPE WITH (NOLOCK) ON LEADDETAILS.PolicyTypeId = POLICYTYPE.PreviousPolicyTypeId
		LEFT JOIN [Insurance_StageMaster] STAGEMASTER WITH (NOLOCK) ON STAGEMASTER.StageID = LEADDETAILS.StageId
		LEFT JOIN [Insurance_PreviousPolicyType] PREPTYPE WITH (NOLOCK) ON PREPTYPE.PreviousPolicyTypeId = LEADDETAILS.PolicyTypeId
		LEFT JOIN [Insurance_VehicleRegistration] VECHI_REG WITH (NOLOCK) ON LEADDETAILS.VehicleNumber = VECHI_REG.regNo
		LEFT JOIN [Insurance_Variant] VARIANT WITH (NOLOCK) ON LEADDETAILS.VariantId = VARIANT.VariantId
		LEFT JOIN [Insurance_VehicleType] VEHTYPE WITH (NOLOCK) ON VEHTYPE.VehicleTypeId = VARIANT.VehicleTypeId
		LEFT JOIN [Insurance_Model] MODEL WITH (NOLOCK) ON MODEL.ModelId = VARIANT.ModelId
		LEFT JOIN [Insurance_Make] MAKE WITH (NOLOCK) ON MODEL.MakeId = MAKE.MakeId
		LEFT JOIN [Insurance_Fuel] FULE WITH (NOLOCK) ON FULE.FuelId = VARIANT.FuelId
		LEFT JOIN [Insurance_rto] RTO WITH (NOLOCK) ON RTO.RTOId = LEADDETAILS.RTOId
		LEFT JOIN [Insurance_ManualLeadDetails] MLD WITH(NOLOCK) ON MLD.LeadId = LEADDETAILS.LeadId
		WHERE  (
				CAST(LEADDETAILS.CreatedOn AS DATE) >= CAST(@StartDate AS DATE)
				OR ISNULL(@StartDate, '') = ''
				)
			AND (
				CAST(LEADDETAILS.CreatedOn AS DATE) <= CAST(@EndDate AS DATE)
				OR ISNULL(@EndDate, '') = ''
				)
			AND LEADDETAILS.IsActive = 1
			AND (
				LEADDETAILS.VehicleTypeId IN (
					SELECT value
					FROM STRING_SPLIT(@PolicyType, ',')
					)
				OR ISNULL(@PolicyType, '') = ''
				)
			AND PT.[Status] = 'A25D747B-167E-4C1B-AE13-E6CC49A195F8'
			AND (
				LEADDETAILS.CreatedBy IN (
					SELECT UserId
					FROM @UsersByHierarchy
					)
				OR @IsSuperAdmin = 1
				)
			AND (
				LEADDETAILS.IsManualPolicy = CASE 
					WHEN LOWER(@PolicyNature) = 'online'
						THEN 0
					ELSE 1
					END
				OR ISNULL(@PolicyNature, '') = ''
				)

		SELECT 
			RTOCode
			,IsBrandNew
			,VariantId
			,RTOId
			,PolicyTypeId
			,RegNo
			,RegDate
			,Model 
			,VehicleType
			,RegAuthority
			,VariantName
			,FuelId
			,FuelName
			,Maker
			,PreviousSAODInsurer
			,PrevPolicyTypeId
			,IsPrevPolicy
			,PreviousSATPInsurer
			,PreviousPolicyNumberSAOD
			,PreviousPolicyExpirySAOD
			,PrevPolicyNCB
			,PrevPolicyClaims
			,IsPolicyExpired
			,PrevPolicyExpiryDate
			,ODTPExp
			,Stage
			,StageId
			,QuoteTransactionID
			,InsurerId
			,UserId
			,CustomerName
			,MobileNo
			,LeadId
			,CAST(PolicyStartDate AS DATE) AS PolicyStartDate
			,CAST(PolicyEndDate AS DATE) AS PolicyEndDate
			,TotalPremium
			,InsurerName
			,EmailId
			,PolicyNo
			,ProductType -- insurance name
			,PolicyIssueDate
			,Premium -- gross premium
			,GeneratedOn
			,CreatedOn
			,PolicyEndDate AS ExpiringOn
			,PolicyType
			,PolicyNumber
			,VehicleTypeId
			,PolicyStatus
			,PolicyNature
			,CASE WHEN (ODTPExp IS NOT NULL 
			AND CAST(ODTPExp AS DATE) > GETDATE()) THEN CAST(DATEADD(DAY, - 1, DATEADD(YEAR, 1, PolicyStartDate)) AS DATE) ELSE PolicyEndDate END AS ActualExpiry
			INTO #CUSTOMER_DETAILS_API
		FROM #TEMP
		WHERE (
				(
					CustomerName LIKE '%' + ISNULL(@SearchText, '') + '%'
					OR ProductType LIKE '%' + ISNULL(@SearchText, '') + '%'
					OR MobileNo LIKE '%' + ISNULL(@SearchText, '') + '%'
					OR EmailId LIKE '%' + ISNULL(@SearchText, '') + '%'
					OR PolicyNo LIKE '%' + ISNULL(@SearchText, '') + '%'
					OR RegNo LIKE '%' + ISNULL(@SearchText, '') + '%'
					OR InsurerName LIKE '%' + ISNULL(@SearchText, '') + '%'
				)
			OR ISNULL(@SearchText, '') = '')


		INSERT @TEMPCUSTOMERDETAILS
		SELECT 			
			RTOCode
			,IsBrandNew
			,VariantId
			,RTOId
			,PolicyTypeId
			,RegNo
			,RegDate
			,Model 
			,VehicleType
			,RegAuthority
			,VariantName
			,FuelId
			,FuelName
			,Maker
			,PreviousSAODInsurer
			,PrevPolicyTypeId
			,IsPrevPolicy
			,PreviousSATPInsurer
			,PreviousPolicyNumberSAOD
			,PreviousPolicyExpirySAOD
			,PrevPolicyNCB
			,PrevPolicyClaims
			,IsPolicyExpired
			,PrevPolicyExpiryDate
			,ODTPExp
			,Stage
			,StageId
			,QuoteTransactionID
			,InsurerId
			,UserId
			,CustomerName
			,MobileNo
			,LeadId
			,PolicyStartDate
			,PolicyEndDate
			,TotalPremium
			,InsurerName
			,EmailId
			,PolicyNo
			,ProductType -- insurance name
			,CAST(PolicyIssueDate AS DATE) AS PolicyIssueDate
			,Premium -- gross premium
			,GeneratedOn
			,CreatedOn
			,ExpiringOn
			,PolicyType
			,PolicyNumber
			,VehicleTypeId
			,CASE 
								WHEN CAST(ActualExpiry AS DATE) < GETDATE()
									THEN 'Expired'
								WHEN IsPrevPolicy = 1
									THEN 'Renewal'
								WHEN IsBrandNew = 1
									THEN 'New'
				END AS PolicyStatus
			,PolicyNature
			,ActualExpiry
			FROM
		#CUSTOMER_DETAILS_API
		WHERE 
				ISNULL(@PolicyStatus, '') = ''
				OR (
					(
						CAST(ActualExpiry AS DATE) < GETDATE()
						AND LOWER(@PolicyStatus) = 'expired'
						)
					OR (
						IsPrevPolicy = 1
						AND LOWER(@PolicyStatus) = 'renewal'
						)
					OR (
						IsBrandNew = 1
						AND LOWER(@PolicyStatus) = 'new'
						)
					)
				




		IF (@CustomerType = 'Customers')
		BEGIN
			SELECT @TotalRecord = COUNT(1)
			FROM @TEMPCUSTOMERDETAILS

			SELECT *
				,@TotalRecord AS TotalRecord
			FROM @TEMPCUSTOMERDETAILS
			WHERE PolicyNo IS NOT NULL
			ORDER BY CreatedOn DESC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS

			FETCH NEXT @CurrentPageSize ROWS ONLY;
		END
		ELSE IF (@CustomerType = 'Renewals')
		BEGIN
			SELECT @TotalRecord = COUNT(1)
			FROM @TEMPCUSTOMERDETAILS
			WHERE ActualExpiry BETWEEN GETDATE() AND DATEADD(day,45,GETDATE())
			 -- GETDATE() >= DATEADD(day, - 45, ExpiringOn)

			SELECT DATEADD(day, - 45, ActualExpiry) AS ExpiringMF5,ActualExpiry AS EXPDate,GETDATE() AS TODAY
				,*
				,@TotalRecord AS TotalRecord
			FROM @TEMPCUSTOMERDETAILS
			WHERE ActualExpiry BETWEEN GETDATE() AND DATEADD(day,45,GETDATE())
			-- GETDATE() >= DATEADD(day, - 45, ExpiringOn)
			ORDER BY
			CreatedOn ASC
			--DATEADD(day, - 45, ExpiringOn) ASC 
			OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS

			FETCH NEXT @CurrentPageSize ROWS ONLY;
		END
		ELSE IF (@CustomerType = 'Policies')
		BEGIN
			SELECT @TotalRecord = COUNT(1)
			FROM @TEMPCUSTOMERDETAILS

			SELECT *
				,@TotalRecord AS TotalRecord
			FROM @TEMPCUSTOMERDETAILS
			ORDER BY CreatedOn DESC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS

			FETCH NEXT @CurrentPageSize ROWS ONLY;
		END
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