CREATE
	

 PROCEDURE [dbo].[Admin_GetAgreementManagementDetail] (
	@SearchText VARCHAR(100)
	,@StatusId VARCHAR(50)
	,@StartDate VARCHAR(500)
	,@EndDate VARCHAR(500)
	,@CurrentPageIndex INT = 1
	,@CurrentPageSize INT = 10
	)
AS
DECLARE @SearchText1 VARCHAR(100) = @SearchText
	,@StatusId1 VARCHAR(100) = @StatusId
	,@StartDate1 VARCHAR(100) = @StartDate
	,@EndDate1 VARCHAR(100) = @EndDate
	,@CurrentPageIndex1 INT = @CurrentPageIndex
	,@CurrentPageSize1 INT = @CurrentPageSize
DECLARE @PreviousPageIndex1 INT = @CurrentPageIndex1 - 1
	,@NextPageIndex1 INT = @CurrentPageIndex1 + 1
DECLARE @TempDataTable TABLE (
	UserId VARCHAR(100)
	,StampId VARCHAR(100)
	,StampData VARCHAR(100)
	,POSPId VARCHAR(100)
	,UserName VARCHAR(100)
	,MobileNo VARCHAR(100)
	,AgreementSignDate VARCHAR(100)
	,AgreementStatus VARCHAR(100)
	)
DECLARE @TempDataTable2 TABLE (
	UserId VARCHAR(100)
	,StampId VARCHAR(100)
	,StampData VARCHAR(100)
	,POSPId VARCHAR(100)
	,UserName VARCHAR(100)
	,MobileNo VARCHAR(100)
	,AgreementSignDate VARCHAR(100)
	,AgreementStatus VARCHAR(100)
	)

BEGIN
	BEGIN TRY
		INSERT @TempDataTable
		SELECT UserId
			,StampId
			,StampData
			,POSPId
			,UserName
			,MobileNo
			,AgreementSignDate
			,AgreementStatus
		FROM (
			SELECT IU.UserId
				,ST.Id AS StampId
				,ST.StampData
				,IU.POSPId
				,IU.UserName
				,IU.MobileNo
				,CASE 
					WHEN PA.UpdatedOn IS NULL
						THEN PA.CreatedOn
					ELSE PA.UpdatedOn
					END AS AgreementSignDate
				,CASE 
					WHEN PA.Id IS NOT NULL
						AND PA.AgreementId IS NOT NULL
						THEN 'SIGNED'
					WHEN PA.Id IS NOT NULL
						AND PA.AgreementId IS NULL
						AND DATEDIFF(DAY, PA.CreatedOn, GETDATE()) > 3
						THEN 'EXPIRED'
					WHEN PA.Id IS NOT NULL
						AND PA.AgreementId IS NULL
						AND DATEDIFF(DAY, PA.CreatedOn, GETDATE()) < 4
						THEN 'PENDING'
					END AS AgreementStatus
			FROM [HeroIdentity].[dbo].[Identity_User](NOLOCK) IU
			LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement](NOLOCK) PA ON PA.UserId = IU.UserId
				AND PA.IsActive = 1
			LEFT JOIN [Admin_StampData](NOLOCK) ST ON ST.Id = PA.StampId
				AND ST.IsActive = 1
			LEFT JOIN [HeroIdentity].[dbo].[Identity_EmailVerification](NOLOCK) IE ON IU.UserId = IE.UserId
				AND IE.IsVerify = 1
			WHERE IU.IsActive = 1
				AND PA.IsActive = 1
				AND ST.IsActive = 1
				AND IE.IsVerify = 1
			) t

		INSERT @TempDataTable
		SELECT UserId
			,StampId
			,StampData
			,POSPId
			,UserName
			,MobileNo
			,AgreementSignDate
			,AgreementStatus
		FROM (
			SELECT IU.UserId
				,ST.Id AS StampId
				,ST.StampData
				,IU.POSPId
				,IU.UserName
				,IU.MobileNo
				,CASE 
					WHEN PA.UpdatedOn IS NULL
						THEN PA.CreatedOn
					ELSE PA.UpdatedOn
					END AS AgreementSignDate
				,CASE 
					WHEN PA.Id IS NOT NULL
						AND PA.AgreementId IS NULL
						THEN 'REVOKED'
					END AS AgreementStatus
			FROM [HeroIdentity].[dbo].[Identity_User](NOLOCK) IU
			LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement](NOLOCK) PA ON PA.UserId = IU.UserId
			LEFT JOIN [HeroAdmin].[dbo].[Admin_StampData](NOLOCK) ST ON ST.Id = PA.StampId
				AND ST.IsActive = 1
			LEFT JOIN [HeroIdentity].[dbo].[Identity_EmailVerification](NOLOCK) IE ON IU.UserId = IE.UserId
				AND IE.IsVerify = 1
			WHERE IU.IsActive = 1
				AND ST.IsActive = 1
				AND PA.IsActive = 0
				AND PA.IsRevoked = 1
				AND IE.IsVerify = 1
			) t

		INSERT @TempDataTable2
		SELECT UserId
			,StampId
			,StampData
			,POSPId
			,UserName
			,MobileNo
			,AgreementSignDate
			,AgreementStatus
		FROM @TempDataTable
		WHERE (
				(
					(
						@Searchtext1 IS NULL
						OR @Searchtext1 = ''
						)
					OR (UserName LIKE '%' + @Searchtext1 + '%')
					)
				OR (
					(
						@Searchtext1 IS NULL
						OR @Searchtext1 = ''
						)
					OR (MobileNo LIKE '%' + @Searchtext1 + '%')
					)
				OR (
					(
						@Searchtext1 IS NULL
						OR @Searchtext1 = ''
						)
					OR (POSPId LIKE '%' + @Searchtext1 + '%')
					)
				)
			AND (
				(
					@StatusId1 IS NULL
					OR @StatusId1 = ''
					OR @StatusId1 = 'All'
					)
				OR AgreementStatus = @StatusId1
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
				OR CAST(AgreementSignDate AS DATE) BETWEEN CAST(@StartDate1 AS DATE)
					AND CAST(@EndDate1 AS DATE)
				)

		SELECT UserId
			,StampId
			,StampData
			,POSPId
			,UserName
			,MobileNo
			,AgreementSignDate
			,AgreementStatus
		FROM @TempDataTable
		WHERE (
				(
					(
						@Searchtext1 IS NULL
						OR @Searchtext1 = ''
						)
					OR (UserName LIKE '%' + @Searchtext1 + '%')
					)
				OR (
					(
						@Searchtext1 IS NULL
						OR @Searchtext1 = ''
						)
					OR (MobileNo LIKE '%' + @Searchtext1 + '%')
					)
				OR (
					(
						@Searchtext1 IS NULL
						OR @Searchtext1 = ''
						)
					OR (POSPId LIKE '%' + @Searchtext1 + '%')
					)
				)
			AND (
				(
					@StatusId1 IS NULL
					OR @StatusId1 = ''
					OR @StatusId1 = 'All'
					)
				OR AgreementStatus = @StatusId1
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
				OR CAST(AgreementSignDate AS DATE) BETWEEN CAST(@StartDate1 AS DATE)
					AND CAST(@EndDate1 AS DATE)
				)
		ORDER BY CAST(AgreementSignDate AS DATETIME) DESC OFFSET(@CurrentPageIndex1 - 1) * @CurrentPageSize1 ROWS

		FETCH NEXT @CurrentPageSize1 ROWS ONLY

		SELECT @CurrentPageIndex1 AS CurrentPageIndex
			,@PreviousPageIndex1 AS PreviousPageIndex
			,@NextPageIndex1 AS NextPageIndex
			,@CurrentPageSize1 AS CurrentPageSize
			,(
				SELECT COUNT(UserId)
				FROM @TempDataTable2
				) AS TotalRecord
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
  
