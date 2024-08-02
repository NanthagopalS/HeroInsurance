/*
exec   [Admin_GetDeactivatedUser] '','','','',1,10  
*/
CREATE
	

 PROCEDURE [dbo].[Admin_GetDeactivatedUser] (
	@SearchText VARCHAR(100) = NULL
	,@RelationManagerId VARCHAR(100) = NULL
	,@StartDate VARCHAR(100) = NULL
	,@EndDate VARCHAR(100) = NULL
	,@CurrentPageIndex INT = 1
	,@CurrentPageSize INT = 500
	)
AS
BEGIN
	BEGIN TRY
		DECLARE @TotalRecord INT = 0

		BEGIN
			SELECT IU.UserId
				,IU.POSPId
				,IU.UserName AS POSPName
				,IU.MobileNo AS MobileNumber
				,IU.EmailId AS EmailId
				,IU.CreatedByMode AS CreatedBy
				,'Deactivated POSP' AS Stage
				,UD.ServicedByUserId AS RelationManagerId
				,'Motor' AS TaggedPolicy
				,DeActivePOSP.CreatedOn
				,IU.IsActive
				,serUsr.UserName AS RelationManager
			INTO #Temp
			FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)
			LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] UD WITH (NOLOCK) ON UD.UserId = IU.UserId
			LEFT JOIN [HeroIdentity].[dbo].[Identity_User] serUsr WITH (NOLOCK) ON UD.ServicedByUserId = serUsr.UserId
			LEFT JOIN [HeroAdmin].[dbo].[Admin_DeActivatePOSP] DeActivePOSP WITH (NOLOCK) ON DeActivePOSP.DeactivatePospId = IU.POSPId
			INNER JOIN [HeroPOSP].[dbo].[POSP_UserStageStatusDetail] UserStage WITH (NOLOCK) ON UserStage.UserId = IU.UserId
				AND UserStage.StageId = 'B6E3FE9B-0202-486F-ADD7-C11639970690'
			WHERE (
					CAST(IU.CreatedOn AS DATE) >= CAST(@StartDate AS DATE)
					OR ISNULL(@StartDate, '') = ''
					)
				AND (
					CAST(IU.CreatedOn AS DATE) <= CAST(@EndDate AS DATE)
					OR ISNULL(@EndDate, '') = ''
					)
				AND (
					ISNULL(@SearchText, '') = ''
					OR (IU.UserName LIKE '%' + @Searchtext + '%')
					OR (IU.MobileNo LIKE '%' + @Searchtext + '%')
					OR (IU.POSPId LIKE '%' + @Searchtext + '%')
					)
				AND (
					ISNULL(@RelationManagerId, '') = ''
					OR UD.ServicedByUserId = @RelationManagerId
					)
				AND IU.IsActive = 0
			ORDER BY DeActivePOSP.CreatedOn DESC
		END

		BEGIN
			SELECT @TotalRecord = COUNT(1)
			FROM #TEMP WITH (NOLOCK)

			SELECT *
				,@TotalRecord AS TotalRecord
			FROM #TEMP
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

		EXEC Admin_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name
			,@ErrorDetail = @ErrorDetail
			,@ParameterList = @ParameterList
	END CATCH
END