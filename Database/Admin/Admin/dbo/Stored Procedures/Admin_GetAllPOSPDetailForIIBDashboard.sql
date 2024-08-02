-- exec [dbo].[Admin_GetAllPOSPDetailForIIBDashboard] 'MANISHBHAI VITHTHALBHAI SAVANI','','','2023-07-10','2023-09-08',1,10
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- =========================================================================================           
-- Author:  <Author, Ankit>        
-- Create date: <Create Date,16-Feb-2023>        
-- Description: <Description, Admin_GetAllPOSPDetailForIIBDashboard>  
-- =========================================================================================           
CREATE PROCEDURE [dbo].[Admin_GetAllPOSPDetailForIIBDashboard] (
	@Searchtext VARCHAR(MAX) = NULL
	,@CreatedBy VARCHAR(100) = 'All'
	,@StatusType VARCHAR(MAX) = NULL
	,@StartDate VARCHAR(100) = NULL
	,@EndDate VARCHAR(100) = NULL
	,@CurrentPageIndex INT = 1
	,@CurrentPageSize INT = 10
	)
AS
DECLARE @Searchtext1 VARCHAR(max) = @Searchtext
	,@CreatedBy1 VARCHAR(100) = @CreatedBy
	,@StatusType1 VARCHAR(100) = @StatusType
	,@StartDate1 VARCHAR(100) = @StartDate
	,@EndDate1 VARCHAR(100) = @EndDate
	,@CurrentPageIndex1 INT = @CurrentPageIndex
	,@CurrentPageSize1 INT = @CurrentPageSize
DECLARE @PreviousPageIndex1 INT = @CurrentPageIndex1 - 1
	,@NextPageIndex1 INT = @CurrentPageIndex1 + 1
DECLARE @TempDataTable TABLE (
	UserId VARCHAR(100)
	,UserName VARCHAR(100)
	,MobileNumber VARCHAR(100)
	,CreatedByMode VARCHAR(100)
	,POSPId VARCHAR(100)
	,IIBStatus VARCHAR(100)
	,IIBUploadStatus VARCHAR(100)
	,PanNumber VARCHAR(100)
	)
DECLARE @TempStatus TABLE (StatusValue VARCHAR(100))

BEGIN
	BEGIN TRY
		INSERT @TempStatus
		SELECT *
		FROM STRING_SPLIT(@StatusType1, ',')

		IF (@CurrentPageIndex1 = - 1)
		BEGIN
			INSERT @TempDataTable
			SELECT IU.UserId
				,IU.UserName
				,IU.MobileNo AS MobileNumber
				,IU.CreatedByMode
				,IU.POSPId
				,IU.IIBStatus
				,IU.IIBUploadStatus
				,IPan.PanNumber
			FROM [HeroIdentity].[dbo].[Identity_User](NOLOCK) IU
			INNER JOIN [HeroIdentity].[dbo].[Identity_PanVerification](NOLOCK) IPan ON IPan.UserId = IU.UserId
			INNER JOIN [HeroPOSP].[dbo].[POSP_Exam](NOLOCK) PE ON PE.UserId = IU.UserId
				AND PE.IsActive = 1
				AND PE.IsCleared = 1
				AND PE.ExamEndDateTime IS NOT NULL
			LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement](NOLOCK) PA ON PA.UserId = IU.UserId
				AND PA.IsActive = 1
				AND PA.AgreementId IS NOT NULL

			WHERE (
					(
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IU.UserName LIKE '%' + @Searchtext1 + '%')
						)
					OR (
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IU.MobileNo LIKE @Searchtext1 + '%')
						)
					OR (
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IU.POSPId LIKE @Searchtext1 + '%')
						)
					OR (
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IPan.PanNumber LIKE @Searchtext1 + '%')
						)
					)
				AND (
					(
						@CreatedBy1 IS NULL
						OR @CreatedBy1 = ''
						OR @CreatedBy1 = 'All'
						)
					OR (IU.CreatedByMode = @CreatedBy1)
					)
				AND (
					(
						@StatusType1 IS NULL
						OR @StatusType1 = ''
						)
					OR (
						IU.IIBUploadStatus IN (
							SELECT *
							FROM @TempStatus
							)
						)
					)
				AND (
					(
						(
							@StartDate1 IS NULL
							OR @StartDate1 = ''
							)
						AND (
							@EndDate1 IS NULL
							OR @EndDate1 = ''
							)
						)
					OR (
						CAST(IU.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)
							AND CAST(@EndDate1 AS DATE)
						)
					)
				AND IU.RoleId IN (
					SELECT RoleId
					FROM [HeroIdentity].[dbo].[Identity_RoleMaster]
					WHERE RoleName = 'POSP'
					)
				AND PE.IsActive = 1
				AND PE.IsCleared = 1
				AND PE.ExamEndDateTime IS NOT NULL
				AND IPan.IsActive = 1
			ORDER BY IU.CreatedOn DESC

			SELECT UserId
				,UserName
				,MobileNumber
				,CreatedByMode
				,POSPId
				,IIBStatus
				,IIBUploadStatus
				,PanNumber
			FROM @TempDataTable
			ORDER BY UserName

			SELECT 0 AS CurrentPageIndex
				,0 AS PreviousPageIndex
				,0 AS NextPageIndex
				,0 AS CurrentPageSize
				,0 AS TotalRecord
		END
		ELSE
		BEGIN
			INSERT @TempDataTable
			SELECT IU.UserId
				,IU.UserName
				,IU.MobileNo AS MobileNumber
				,IU.CreatedByMode
				,IU.POSPId
				,IU.IIBStatus
				,IU.IIBUploadStatus
				,IPan.PanNumber
			FROM [HeroIdentity].[dbo].[Identity_User] IU
			INNER JOIN [HeroIdentity].[dbo].[Identity_PanVerification] IPan ON IPan.UserId = IU.UserId
			INNER JOIN [HeroPOSP].[dbo].[POSP_Exam] PE ON PE.UserId = IU.UserId
				AND PE.IsActive = 1
				AND PE.IsCleared = 1
			LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] PA ON PA.UserId = IU.UserId
				AND PA.IsActive = 1
				AND PA.AgreementId IS NOT NULL
			WHERE (
					(
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IU.UserName LIKE '%' + @Searchtext1 + '%')
						)
					OR (
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IU.MobileNo LIKE @Searchtext1 + '%')
						)
					OR (
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IU.POSPId LIKE @Searchtext1 + '%')
						)
					OR (
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IPan.PanNumber LIKE @Searchtext1 + '%')
						)
					)
				AND (
					(
						@CreatedBy1 IS NULL
						OR @CreatedBy1 = ''
						OR @CreatedBy1 = 'All'
						)
					OR (IU.CreatedByMode = @CreatedBy1)
					)
				AND (
					(
						@StatusType1 IS NULL
						OR @StatusType1 = ''
						)
					OR (
						IU.IIBUploadStatus IN (
							SELECT *
							FROM @TempStatus
							)
						)
					)
				AND (
					(
						(
							@StartDate1 IS NULL
							OR @StartDate1 = ''
							)
						AND (
							@EndDate1 IS NULL
							OR @EndDate1 = ''
							)
						)
					OR (
						CAST(IU.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)
							AND CAST(@EndDate1 AS DATE)
						)
					)
				AND IU.RoleId IN (
					SELECT RoleId
					FROM [HeroIdentity].[dbo].[Identity_RoleMaster]
					WHERE RoleName = 'POSP'
					)
				AND PE.IsActive = 1
				AND PE.IsCleared = 1
				AND PE.ExamEndDateTime IS NOT NULL
				AND IPan.IsActive = 1
			ORDER BY IU.CreatedOn DESC

			SELECT IU.UserId
				,IU.UserName
				,IU.MobileNo AS MobileNumber
				,IU.CreatedByMode
				,IU.POSPId
				,IU.IIBStatus
				,IU.IIBUploadStatus
				,IPan.PanNumber
			FROM [HeroIdentity].[dbo].[Identity_User] IU
			INNER JOIN [HeroIdentity].[dbo].[Identity_PanVerification] IPan ON IPan.UserId = IU.UserId
			INNER JOIN [HeroPOSP].[dbo].[POSP_Exam] PE ON PE.UserId = IU.UserId
				AND PE.IsActive = 1
				AND PE.IsCleared = 1
			LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] PA ON PA.UserId = IU.UserId
				AND PA.IsActive = 1
				AND PA.AgreementId IS NOT NULL
			WHERE (
					(
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IU.UserName LIKE '%' + @Searchtext1 + '%')
						)
					OR (
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IU.MobileNo LIKE @Searchtext1 + '%')
						)
					OR (
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IU.POSPId LIKE @Searchtext1 + '%')
						)
					OR (
						(
							@Searchtext1 IS NULL
							OR @Searchtext1 = ''
							)
						OR (IPan.PanNumber LIKE @Searchtext1 + '%')
						)
					)
				AND (
					(
						@CreatedBy1 IS NULL
						OR @CreatedBy1 = ''
						OR @CreatedBy1 = 'All'
						)
					OR (IU.CreatedByMode = @CreatedBy1)
					)
				AND (
					(
						@StatusType1 IS NULL
						OR @StatusType1 = ''
						)
					OR (
						IU.IIBUploadStatus IN (
							SELECT *
							FROM @TempStatus
							)
						)
					)
				AND (
					(
						(
							@StartDate1 IS NULL
							OR @StartDate1 = ''
							)
						AND (
							@EndDate1 IS NULL
							OR @EndDate1 = ''
							)
						)
					OR (
						CAST(IU.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)
							AND CAST(@EndDate1 AS DATE)
						)
					)
				AND IU.RoleId IN (
					SELECT RoleId
					FROM [HeroIdentity].[dbo].[Identity_RoleMaster]
					WHERE RoleName = 'POSP'
					)
				AND PE.IsActive = 1
				AND PE.IsCleared = 1
				AND PE.ExamEndDateTime IS NOT NULL
				AND IPan.IsActive = 1
			ORDER BY IU.CreatedOn DESC OFFSET(@CurrentPageIndex1 - 1) * @CurrentPageSize1 ROWS

			FETCH NEXT @CurrentPageSize1 ROWS ONLY

			SELECT @CurrentPageIndex1 AS CurrentPageIndex
				,@PreviousPageIndex1 AS PreviousPageIndex
				,@NextPageIndex1 AS NextPageIndex
				,@CurrentPageSize1 AS CurrentPageSize
				,(
					SELECT COUNT(UserId)
					FROM @TempDataTable
					) AS TotalRecord
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
