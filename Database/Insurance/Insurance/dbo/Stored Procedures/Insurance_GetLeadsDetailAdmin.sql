
/*    
exec [Insurance_GetLeadsDetailAdmin] 'Customers','','','2023-04-16','2023-06-16',1,10,'8E5CFA10-4CAA-4100-92AD-9A0E9B3ACC4F'    
exec [Insurance_GetLeadsDetailAdmin] 'Customers','','2d566966-5525-4ed7-bd90-bb39e8418f39,6e7cb14f-d5a8-4c8b-8ab8-99c6e0030056','2023-03-23','2023-05-23',1,500,'E16ABF13-5BA1-477C-A224-740B5F849E64'    
exec [Insurance_GetLeadsDetailAdmin] 'Customers','','','2023-01-01','2023-12-31',1,10000,'E16ABF13-5BA1-477C-A224-740B5F849E64'    
exec [Insurance_GetLeadsDetailAdmin] 'Renewals','','','2023-03-29','2023-05-29',1,500,'B3769E78-5331-4284-AA0E-3010E6DCD67A'    
exec [Insurance_GetLeadsDetailAdmin] 'Policies','','','2023-01-01','2023-12-31',1,500,'0340F56C-02C3-4B02-9A21-E2AAA771D7AA'    
*/
CREATE PROCEDURE [dbo].[Insurance_GetLeadsDetailAdmin] (
	@CustomerType VARCHAR(100) = NULL
	,@SearchText VARCHAR(100) = NULL
	,@PolicyType VARCHAR(100) = NULL
	,@StartDate VARCHAR(100) = NULL
	,@EndDate VARCHAR(100) = NULL
	,@CurrentPageIndex INT = NULL
	,@CurrentPageSize INT = NULL
	,@CreatedBy VARCHAR(50) = NULL
	)
AS
BEGIN
	BEGIN TRY
		DECLARE @TotalRecord INT = 0
		DECLARE @IsAdmin BIT = 0
		DECLARE @UsersByHierarchy TABLE (UserId VARCHAR(100))
		DECLARE @IsSuperAdmin BIT = 0

		SET @IsAdmin = (
				SELECT CASE 
						WHEN RM.RoleName = 'POSP'
							THEN 0
						WHEN RM.RoleName != 'POSP'
							THEN 1
						END AS IsAdmin
				FROM HeroIdentity.dbo.Identity_User AS IU WITH (NOLOCK)
				LEFT JOIN [HeroIdentity].[dbo].[Identity_RoleMaster] RM ON IU.RoleId = RM.RoleId
				WHERE IU.CreatedBy = @CreatedBy
				)

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
			,VARIANT.VariantName
			,FULE.FuelId
			,FULE.FuelName
			,MAKE.MakeName AS Maker
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
			,INSURER.InsurerName
			,LEADDETAILS.Email AS EmailId
			,ISNULL(PT.PolicyNumber, PT.ProposalNumber) AS PolicyNo
			,INSTYPE.InsuranceName AS ProductType
			,CAST(LEADDETAILS.PolicyStartDate AS DATE) AS PolicyIssueDate
			,LEADDETAILS.GrossPremium AS Premium
			,CAST(LEADDETAILS.CreatedOn AS DATE) AS GeneratedOn
			,LEADDETAILS.CreatedOn
			,CAST(LEADDETAILS.PolicyEndDate AS DATE) AS ExpiringOn
			,POLICYTYPE.PreviousPolicyType AS PolicyType
			,LEADDETAILS.PolicyNumber
			,LEADDETAILS.VehicleTypeId
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
		WHERE (
				CAST(LEADDETAILS.CreatedOn AS DATE) >= CAST(@StartDate AS DATE)
				OR ISNULL(@StartDate, '') = ''
				)
			AND (
				CAST(LEADDETAILS.CreatedOn AS DATE) <= CAST(@EndDate AS DATE)
				OR ISNULL(@EndDate, '') = ''
				)
			AND (
				LEADDETAILS.CreatedBy = @CreatedBy
				OR ISNULL(@CreatedBy, '') = ''
				)
			AND LEADDETAILS.IsActive = 1
			AND (
				(
					(
						LEADDETAILS.LeadName LIKE '%' + ISNULL(@SearchText, '') + '%'
						OR ISNULL(@SearchText, '') = ''
						)
					OR (
						INSTYPE.InsuranceName LIKE '%' + ISNULL(@SearchText, '') + '%'
						OR ISNULL(@SearchText, '') = ''
						)
					OR (
						LEADDETAILS.PhoneNumber LIKE '%' + ISNULL(@SearchText, '') + '%'
						OR ISNULL(@SearchText, '') = ''
						)
					OR (
						LEADDETAILS.Email LIKE '%' + ISNULL(@SearchText, '') + '%'
						OR ISNULL(@SearchText, '') = ''
						)
					)
				OR ISNULL(@SearchText, '') = ''
				)
			AND (
				LEADDETAILS.VehicleTypeId IN (
					SELECT value
					FROM STRING_SPLIT(@PolicyType, ',')
					)
				OR ISNULL(@PolicyType, '') = ''
				)
			AND PT.[Status] = 'A25D747B-167E-4C1B-AE13-E6CC49A195F8'
			AND (
				LEADDETAILS.CreatedBy = @CreatedBy
				OR @CreatedBy IS NULL
				)
			AND (
				LEADDETAILS.CreatedBy IN (
					SELECT UserId
					FROM @UsersByHierarchy
					)
				OR @IsSuperAdmin = 1
				)

		IF (@CustomerType = 'Customers')
		BEGIN
			SELECT @TotalRecord = COUNT(1)
			FROM #TEMP WITH (NOLOCK)

			SELECT *
				,@TotalRecord AS TotalRecord
			FROM #TEMP WITH (NOLOCK)
			WHERE PolicyNo IS NOT NULL
			ORDER BY CreatedOn DESC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS

			FETCH NEXT @CurrentPageSize ROWS ONLY;
		END
		ELSE IF (@CustomerType = 'Renewals')
		BEGIN
			SELECT @TotalRecord = COUNT(1)
			FROM #TEMP WITH (NOLOCK)
			WHERE GETDATE() >= DATEADD(day, - 45, ExpiringOn)

			SELECT DATEADD(day, - 45, ExpiringOn)
				,*
				,@TotalRecord AS TotalRecord
			FROM #TEMP WITH (NOLOCK)
			WHERE GETDATE() >= DATEADD(day, - 45, ExpiringOn)
			ORDER BY DATEADD(day, - 45, ExpiringOn) ASC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS

			FETCH NEXT @CurrentPageSize ROWS ONLY;
		END
		ELSE IF (@CustomerType = 'Policies')
		BEGIN
			SELECT @TotalRecord = COUNT(1)
			FROM #TEMP WITH (NOLOCK)

			SELECT *
				,@TotalRecord AS TotalRecord
			FROM #TEMP WITH (NOLOCK)
			ORDER BY CreatedOn DESC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS

			FETCH NEXT @CurrentPageSize ROWS ONLY;
		END

		DELETE #TEMP
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