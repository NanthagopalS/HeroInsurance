/*        
exec [POSP_GetNewAndOldPOSPReportUI] '','','2023-08-01','2023-08-25',1,500,'','0'        
*/
CREATE
 PROCEDURE [dbo].[POSP_GetNewAndOldPOSPReportUI] (
	@Searchtext VARCHAR(100) = NULL
	,-- POSPId, UserName, Mobile Number                    
	@StageId VARCHAR(100) = NULL
	,-- comes stage name from POSPStage table                    
	@StartDate VARCHAR(100) = NULL
	,@EndDate VARCHAR(100) = NULL
	,@CurrentPageIndex INT = 1
	,@CurrentPageSize INT = 10
	,@UserId VARCHAR(100) = NULL
	,@status int = NULL -- 0- inactive, 1- active, 2 - new POSP
	)
AS
DECLARE @TempDataTable TABLE (
	UserName VARCHAR(100)
	,RoleName VARCHAR(100)
	,EmailId VARCHAR(100)
	,MobileNo VARCHAR(100)
	,Pannumber VARCHAR(10)
	,Stage VARCHAR(100)
	,CreatedDate VARCHAR(100)
	,Created_By VARCHAR(100)
	,Sourced_By VARCHAR(100)
	,Serviced_By VARCHAR(100)
	,PosCode VARCHAR(100)
	,PosSource VARCHAR(100)
	,IsActive int
	)

BEGIN
	BEGIN TRY
		INSERT @TempDataTable
		SELECT     IU.UserName   
			,RM.RoleName   
			,IU.EmailId   
			,IU.MobileNo   
			,PAN AS Pannumber
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
			,convert(DATE, IU.CreatedOn) AS CreatedDate   
			,Created.UserName AS Created_By   
			,SourceUser.UserName AS Sourced_By   
			,SerUser.UserName AS Serviced_By   
			,IU.POSPId AS PosCode   
			,IU.CreatedByMode AS PosSource
			,CASE WHEN DEAC.Id IS NOT NULL AND DEAC.Status='DEACTIVATED' THEN 0 
				WHEN DEAC.ID IS NULL AND PA.AgreementId IS NOT NULL THEN 1
				ELSE 2 END AS IsActive
		FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)
		LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] UD WITH (NOLOCK) ON UD.UserId = IU.UserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_UserAddressDetail] UAD WITH (NOLOCK) ON UAD.UserId = IU.UserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_UserBankDetail] UBD WITH (NOLOCK) ON UBD.UserId = IU.UserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_RoleMaster] RM WITH (NOLOCK) ON RM.RoleId = IU.RoleId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_User] SerUser WITH (NOLOCK) ON SerUser.UserId = UD.ServicedByUserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_User] SourceUser WITH (NOLOCK) ON SourceUser.UserId = UD.SourcedByUserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_User] Created WITH (NOLOCK) ON Created.UserId = IU.CreatedBy
		LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] PA WITH (NOLOCK) ON PA.UserId = IU.UserId AND PA.IsActive = 1
		LEFT JOIN [HeroPOSP].[dbo].[POSP_Exam] PE WITH (NOLOCK) ON PE.UserId = IU.UserId AND PE.IsActive = 1
		LEFT JOIN [HeroAdmin].[dbo].[Admin_DeActivatePOSP] DEAC WITH (NOLOCK) ON IU.pospid = DEAC.DeActivatePospId
		LEFT JOIN [HeroPOSP].[dbo].[POSP_Exam] PEFailed WITH (NOLOCK) ON PEFailed.UserId = IU.UserId
			AND PEFailed.IsActive = 0
			AND PEFailed.IsCleared = 0
		LEFT JOIN [HeroPOSP].[dbo].[POSP_Training] PT WITH (NOLOCK) ON PT.UserId = IU.UserId
			AND PT.IsActive = 1
			AND PA.IsActive = 1
		WHERE CAST(IU.CreatedOn AS DATE) >= CAST(@StartDate AS DATE)AND CAST(IU.CreatedOn AS DATE) <= CAST(@EndDate AS DATE)

		SELECT *
		INTO #NEW_AND_OLD_POSP
		FROM @TempDataTable
		WHERE (ISNULL(@status, '')='' OR IsActive = @status)
				AND
				(
				Stage = @StageId
				OR ISNULL(@StageId, '') = ''
				)
				AND
				(
				Pannumber LIKE '%' + ISNULL(@SearchText, '') + '%'
				OR UserName LIKE '%' + ISNULL(@SearchText, '') + '%'
				OR MobileNo LIKE '%' + ISNULL(@SearchText, '') + '%'
				)


		DECLARE @TotalRecords INT = (
				SELECT COUNT(*)
				FROM #NEW_AND_OLD_POSP
				)

		SELECT *
			,@TotalRecords AS TotalRecords
		FROM #NEW_AND_OLD_POSP
		ORDER BY CreatedDate DESC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS

		FETCH NEXT @CurrentPageSize ROWS ONLY
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