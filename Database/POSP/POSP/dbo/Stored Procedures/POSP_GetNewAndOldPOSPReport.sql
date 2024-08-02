/*        
exec [POSP_GetNewAndOldPOSPReport] '','','2023-08-01','2023-08-25',1,500,'','0'        
exec [POSP_GetNewAndOldPOSPReport] '','Profile ONGOING','2023-08-24','2023-08-25',1,10,'45BBA540-07C5-4D4C-BEFF-771AE2FC32B0',''      
*/
CREATE
	

 PROCEDURE [dbo].[POSP_GetNewAndOldPOSPReport] (
	@Searchtext VARCHAR(100) = NULL
	,-- POSPId, UserName, Mobile Number                    
	@StageId VARCHAR(100) = NULL
	,-- comes stage name from POSPStage table                    
	@StartDate VARCHAR(100) = NULL
	,@EndDate VARCHAR(100) = NULL
	,@CurrentPageIndex INT = 1
	,@CurrentPageSize INT = 10
	,@status int = NULL -- 0- inactive, 1- active, 2 - new POSP
	)
AS
DECLARE @TempDataTable TABLE (
	DeactivateId VARCHAR(100)
	,UserID VARCHAR(100)
	,UserName VARCHAR(100)
	,RoleName VARCHAR(100)
	,EmailId VARCHAR(100)
	,MobileNo VARCHAR(100)
	,CreatedDate VARCHAR(100)
	,Created_By VARCHAR(100)
	,Sourced_By VARCHAR(100)
	,Serviced_By VARCHAR(100)
	,PosCode VARCHAR(100)
	,Stage VARCHAR(100)
	,POS_Lead_ID VARCHAR(100)
	,Profile_Creation_Status VARCHAR(100)
	,TrainingStatus VARCHAR(100)
	,Exam_Status VARCHAR(100)
	,Document_QC_Status VARCHAR(100)
	,TrainingStart VARCHAR(100)
	,TrainingEnd VARCHAR(100)
	,ExamStart VARCHAR(100)
	,ExamEnd VARCHAR(100)
	,IIBUploadStatus VARCHAR(100)
	,IIBUploadDate VARCHAR(100)
	,AgreementStatus VARCHAR(100)
	,AgreementStart VARCHAR(100)
	,AgreementEnd VARCHAR(100)
	,Pincode VARCHAR(100)
	,City VARCHAR(100)
	,[State] VARCHAR(100)
	,AddressLine1 VARCHAR(200)
	,AddressLine2 VARCHAR(200)
	,AddressLine3 VARCHAR(200)
	,Aadhaarnumber VARCHAR(15)
	,Pannumber VARCHAR(10)
	,BankName VARCHAR(100)
	,BankAccountNo VARCHAR(100)
	,IFSC_Code VARCHAR(50)
	,Adhaar_Front VARCHAR(12)
	,Adhaar_Back VARCHAR(12)
	,PAN_Doc VARCHAR(10)
	,QualificationCertificate_Doc VARCHAR(100)
	,CancelCheque_Doc VARCHAR(100)
	,POS_Training_Certificate_Doc VARCHAR(100)
	,GST_CERTIFICATE_Doc VARCHAR(100)
	,TearnAndCondition_Doc VARCHAR(100)
	,GSTNumber VARCHAR(100)
	,PosSource VARCHAR(100)
	,DateofBirth VARCHAR(100)
	,POSPActivationDate VARCHAR(100)
	,-- (Agreement Sign Date)             
	BeneficiaryName VARCHAR(100)
	,ActiveForBusiness VARCHAR(100)
	,[STATUS] int
	,CreatedOn VARCHAR(100)
	)

BEGIN
	BEGIN TRY
		INSERT @TempDataTable
		SELECT DEAC.Id AS DeactivateId
			,IU.POSPId AS UserID
			,IU.UserName
			,RM.RoleName
			,IU.EmailId
			,IU.MobileNo
			,convert(DATE, IU.CreatedOn) AS CreatedDate
			,Created.UserName AS Created_By
			,SourceUser.UserName AS Sourced_By
			,SerUser.UserName AS Serviced_By
			,IU.POSPId AS PosCode
			,CASE 
				WHEN PA.Id IS NOT NULL
					AND IU.UserProfileStage = 5
					AND PA.AgreementId IS NOT NULL
					THEN 'Profile Completion'
				WHEN PA.Id IS NOT NULL
					AND PA.AgreementId IS NOT NULL
					THEN 'Agreement COMPLETED'
				WHEN PA.Id IS NOT NULL
					AND PA.AgreementId IS NULL
					THEN 'Agreement ONGOING'
				WHEN IU.UserProfileStage = 4 AND IU.IIBStatus = 'Pending'
					THEN 'IIB Upload'
				WHEN PE.Id IS NOT NULL
					AND PE.ExamEndDateTime IS NOT NULL
					THEN 'Exam COMPLETED'
				WHEN PEFailed.Id IS NOT NULL
					THEN 'Exam FAILED'
				WHEN PT.Id IS NOT NULL
					AND PT.IsTrainingCompleted = 1
					THEN 'Training COMPLETED'
				WHEN PT.Id IS NOT NULL
					AND PT.IsTrainingCompleted = 0
					THEN 'Training ONGOING'
				WHEN IU.UserProfileStage = 4
					THEN 'Document Submission'
				WHEN IU.UserProfileStage = 2
					THEN 'Payout Pending'
				ELSE 'Profile ONGOING'
				END AS Stage
			,IU.POSPLeadId AS POS_Lead_ID
			,'' AS Profile_Creation_Status
			,CASE 
				WHEN PT.IsTrainingCompleted = 1
					THEN 'Yes'
				ELSE 'No'
				END AS TrainingStatus
			,CASE 
				WHEN PE.IsCleared = 1
					THEN 'Yes'
				ELSE 'No'
				END AS Exam_Status
			,CASE 
				WHEN UBDSD.Id IS NOT NULL
					THEN 'YES'
				ELSE 'NO'
				END AS Document_QC_Status
			,PT.GeneralInsuranceStartDateTime AS TrainingStart
			,PT.LifeInsuranceEndDateTime AS TrainingEnd
			,PE.ExamStartDateTime AS ExamStart
			,PE.ExamEndDateTime AS ExamEnd
			,IU.IIBUploadStatus
			,IU.iib_upload_date AS IIBUploadDate
			,CASE 
				WHEN PA.AgreementId IS NULL
					THEN 'NO'
				WHEN PA.AgreementId IS NOT NULL
					THEN 'YES'
				END AS AgreementStatus
			,PA.CreatedOn AS AgreementStart
			,CASE 
				WHEN PA.AgreementId IS NOT NULL
					THEN PA.UpdatedOn
				END AS AgreementEnd
			,UAD.Pincode
			,IC.CityName AS City
			,US.StateName AS [State]
			,UAD.AddressLine1
			,UAD.AddressLine2
			,'' AS AddressLine3
			,UD.Aadhaarnumber
			,UD.PAN AS Pannumber
			,BM.BankName
			,UBD.AccountNumber AS BankAccountNo
			,UBD.IFSC AS IFSC_Code
			,CASE 
				WHEN ADHARFront.DocumentId IS NULL
					THEN 'NO'
				WHEN ADHARFront.DocumentId IS NOT NULL
					THEN 'YES'
				END AS Adhaar_Front
			,CASE 
				WHEN ADHARBack.DocumentId IS NULL
					THEN 'NO'
				WHEN ADHARBack.DocumentId IS NOT NULL
					THEN 'YES'
				END AS Adhaar_Back
			,CASE 
				WHEN PANDoc.DocumentId IS NULL
					THEN 'NO'
				WHEN PANDoc.DocumentId IS NOT NULL
					THEN 'YES'
				END AS PAN_Doc
			,CASE 
				WHEN EDUCATION.DocumentId IS NULL
					THEN 'NO'
				WHEN EDUCATION.DocumentId IS NOT NULL
					THEN 'YES'
				END AS QualificationCertificate_Doc
			,CASE 
				WHEN CANCheck.DocumentId IS NULL
					THEN 'NO'
				WHEN CANCheck.DocumentId IS NOT NULL
					THEN 'YES'
				END AS CancelCheque_Doc
			,CASE 
				WHEN PE.IsCleared = '1'
					AND PT.IsTrainingcompleted = '1'
					THEN 'YES'
				ELSE 'NO'
				END AS POS_Training_Certificate_Doc
			,'' AS GST_CERTIFICATE_Doc
			,'' AS TearnAndCondition_Doc
			,'N/A' AS GSTNumber
			,IU.CreatedByMode AS PosSource
			,UD.DateofBirth
			,PA.UpdatedOn AS POSPActivationDate
			,-- (Agreement Sign Date)             
			UBD.AccountHolderName AS BeneficiaryName
			,'' AS ActiveForBusiness
			,CASE WHEN DEAC.Id IS NOT NULL AND DEAC.[Status] = 'DEACTIVATED' THEN 0 
				WHEN DEAC.ID IS NULL AND PA.AgreementId IS NOT NULL THEN 1
				ELSE 2 END AS [STATUS]
			,IU.CreatedOn
		FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)
		LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] UD WITH (NOLOCK) ON UD.UserId = IU.UserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_UserAddressDetail] UAD WITH (NOLOCK) ON UAD.UserId = IU.UserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_UserBankDetail] UBD WITH (NOLOCK) ON UBD.UserId = IU.UserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_State] US WITH (NOLOCK) ON US.StateId = UAD.StateId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_City] IC WITH (NOLOCK) ON IC.CityId = UAD.CityId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_DocumentDetail] ADHARFront WITH (NOLOCK) ON ADHARFront.UserId = IU.UserId
			AND ADHARFront.DocumentTypeId = '870F55F9-1697-428E-8EB5-7D2DF020E112'
			AND ADHARFront.IsActive = 1
		LEFT JOIN [HeroIdentity].[dbo].[Identity_DocumentDetail] ADHARBack WITH (NOLOCK) ON ADHARBack.UserId = IU.UserId
			AND ADHARBack.DocumentTypeId = 'D8ADB259-3EAD-4CC5-BE41-7978B23B9E58'
			AND ADHARBack.IsActive = 1
		LEFT JOIN [HeroIdentity].[dbo].[Identity_DocumentDetail] PANDoc WITH (NOLOCK) ON PANDoc.UserId = IU.UserId
			AND PANDoc.DocumentTypeId = '440848B0-6E5F-49F5-887F-E65AE905E347'
			AND PANDoc.IsActive = 1
		LEFT JOIN [HeroIdentity].[dbo].[Identity_DocumentDetail] EDUCATION WITH (NOLOCK) ON EDUCATION.UserId = IU.UserId
			AND EDUCATION.DocumentTypeId = 'F4BD34B2-6C66-4A2D-A3D3-B9F467B37072'
			AND EDUCATION.IsActive = 1
		LEFT JOIN [HeroIdentity].[dbo].[Identity_DocumentDetail] CANCheck WITH (NOLOCK) ON CANCheck.UserId = IU.UserId
			AND CANCheck.DocumentTypeId = '23DC5851-02EA-439F-ABAC-4FA35D561240'
			AND CANCheck.IsActive = 1
		LEFT JOIN [HeroIdentity].[dbo].[Identity_RoleMaster] RM WITH (NOLOCK) ON RM.RoleId = IU.RoleId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_BankNameMaster] BM WITH (NOLOCK) ON BM.Id = UBD.BankId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusDetail] UBDSD WITH (NOLOCK) ON UBDSD.UserId = IU.UserId
			AND UBDSD.StatusId = 'D07C1631-7FF7-4B4F-A28C-677A7961ED23'
		LEFT JOIN [HeroAdmin].[dbo].[Admin_DeActivatePOSP] DEAC WITH (NOLOCK) ON IU.pospid = DEAC.DeActivatePospId AND DEAC.IsActive=1
		LEFT JOIN [HeroIdentity].[dbo].[Identity_User] SerUser WITH (NOLOCK) ON SerUser.UserId = UD.ServicedByUserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_User] SourceUser WITH (NOLOCK) ON SourceUser.UserId = UD.SourcedByUserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_User] Created WITH (NOLOCK) ON Created.UserId = IU.CreatedBy
		LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] PA WITH (NOLOCK) ON PA.UserId = IU.UserId
			AND PA.IsActive = 1
		LEFT JOIN [HeroPOSP].[dbo].[POSP_Exam] PE WITH (NOLOCK) ON PE.UserId = IU.UserId
			AND PE.IsActive = 1
			AND PE.IsCleared = 1
		LEFT JOIN [HeroPOSP].[dbo].[POSP_Exam] PEFailed WITH (NOLOCK) ON PEFailed.UserId = IU.UserId
			AND PEFailed.IsActive = 0
			AND PEFailed.IsCleared = 0
		LEFT JOIN [HeroPOSP].[dbo].[POSP_Training] PT WITH (NOLOCK) ON PT.UserId = IU.UserId
			AND PT.IsActive = 1
		WHERE (
				CAST(IU.CreatedOn AS DATE) >= CAST(@StartDate AS DATE)
				OR ISNULL(@StartDate, '') = ''
				)
			AND (
				CAST(IU.CreatedOn AS DATE) <= CAST(@EndDate AS DATE)
				OR ISNULL(@EndDate, '') = ''
				)

		SELECT *
		INTO #NEW_AND_OLD_POSP
		FROM @TempDataTable
		WHERE (
				UserID LIKE '%' + ISNULL(@SearchText, '') + '%'
				OR Pannumber LIKE '%' + ISNULL(@SearchText, '') + '%'
				OR UserName LIKE '%' + ISNULL(@SearchText, '') + '%'
				OR MobileNo LIKE '%' + ISNULL(@SearchText, '') + '%'
				)
			AND (
				Stage = @StageId
				OR ISNULL(@StageId, '') = ''
				)
			
			AND (
				(@status = [Status])
				OR ISNULL(@status, '') = ''
				)

		DECLARE @TotalRecords INT = (
				SELECT COUNT(*)
				FROM #NEW_AND_OLD_POSP
				)

		IF (@CurrentPageSize = - 1)
		BEGIN
			SELECT *
				,@TotalRecords AS TotalRecords
			FROM #NEW_AND_OLD_POSP
			ORDER BY CreatedOn DESC

			DELETE #NEW_AND_OLD_POSP
		END
		ELSE
		BEGIN
			SELECT *
				,@TotalRecords AS TotalRecords
			FROM #NEW_AND_OLD_POSP
			ORDER BY CreatedOn DESC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS

			FETCH NEXT @CurrentPageSize ROWS ONLY
		END
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