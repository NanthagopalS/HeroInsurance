-- =============================================                                
-- Author:  <Author, Harsh Patel >                                
-- Create date: <Create Date,3-12-2022>                                
-- Description: <Description,,InsertUserDocumentDetail>                                
-- =============================================                                
CREATE   PROCEDURE [dbo].[Identity_InsertUserDocumentDetail] (
	@UserId VARCHAR(100) = NULL
	,@DocumentTypeId VARCHAR(100) = NULL
	,@DocumentId VARCHAR(100) = NULL
	,@DocumentFileName VARCHAR(200) = NULL
	,@FileSize VARCHAR(200) = NULL
	,@IsAdminUpdating BIT = 0
	)
AS
BEGIN
	DECLARE @DocumentTypeCount INT = 0
		,@UploadedCount INT = 0

	BEGIN TRY
		DECLARE @isAlias BIT
			,@check VARCHAR(100)

		SET @check = (
				SELECT AliasName
				FROM HeroIdentity.dbo.Identity_UserDetail WITH (NOLOCK)
				WHERE UserId = @UserId
				)

		IF (
				@check != ''
				OR @check != NULL
				)
		BEGIN
			SET @isAlias = 1
		END
		ELSE
		BEGIN
			SET @isAlias = 0
		END

		IF OBJECT_ID('#UserDocument') IS NOT NULL
			DROP TABLE #UserDocument;

		SELECT *
		INTO #UserDocument
		FROM [HeroIdentity].[dbo].[Identity_DocumentTypeMaster] WITH (NOLOCK);

		UPDATE #UserDocument
		SET IsMandatory = CASE 
				WHEN (
						DocumentType = 'POSP Declaration / Affidavit'
						AND @isAlias = 1
						)
					THEN 1
				WHEN (
						DocumentType = 'POSP Declaration / Affidavit'
						AND @isAlias = 0
						)
					THEN 0
				ELSE IsMandatory
				END

		UPDATE Identity_DocumentDetail
		SET IsActive = 0, UpdatedOn = GETDATE()
		WHERE UserId = @UserId
			AND DocumentTypeId = @DocumentTypeId

		INSERT INTO Identity_DocumentDetail (
			UserId
			,DocumentTypeId
			,DocumentFileName
			,VerifyByUserId
			,DocumentId
			,FileSize
			)
		VALUES (
			@UserId
			,@DocumentTypeId
			,@DocumentFileName
			,'true'
			,@DocumentId
			,@FileSize
			)

		SET @DocumentTypeCount = (
				SELECT COUNT(Id)
				FROM #UserDocument UD WITH (NOLOCK)
				WHERE ud.IsActive = 1
					AND ud.IsMandatory = 1
				)

		DROP TABLE #UserDocument;

		SET @UploadedCount = (
				SELECT COUNT(DISTINCT DocumentTypeId)
				FROM [HeroIdentity].[dbo].[Identity_DocumentDetail] WITH (NOLOCK)
				WHERE DocumentTypeId IN (
						SELECT DISTINCT Id
						FROM [HeroIdentity].[dbo].[Identity_DocumentTypeMaster] WITH (NOLOCK)
						WHERE IsActive = 1
							AND IsMandatory = 1
						)
					AND IsActive = 1
					AND UserId = @UserId
				)
	END TRY

	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500)
			,@ErrorDetail VARCHAR(1000)
			,@ParameterList VARCHAR(2000)

		SET @StrProcedure_Name = ERROR_PROCEDURE()
		SET @ErrorDetail = ERROR_MESSAGE()

		EXEC Identity_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name
			,@ErrorDetail = @ErrorDetail
			,@ParameterList = @ParameterList
	END CATCH
END
