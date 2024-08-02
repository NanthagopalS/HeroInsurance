CREATE   PROCEDURE [dbo].[POSP_GetPospAgreementDocument] (@UserId VARCHAR(50))
AS
BEGIN
	BEGIN TRY
		IF EXISTS (
				SELECT TOP (1) [Id]
				FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH (NOLOCK)
				WHERE UserId = @UserId
					AND IsActive = 1
					AND StampId IS NULL
				)
		BEGIN
			DECLARE @StampId VARCHAR(50) = (
					SELECT TOP 1 Id AS StampId
					FROM HeroAdmin.dbo.Admin_StampData WITH (NOLOCK)
					WHERE StampStatus = 'Available'
					ORDER BY CreatedOn ASC
					);

			UPDATE HeroAdmin.dbo.Admin_StampData
			SET StampStatus = 'Used'
			WHERE Id = @StampId

			UPDATE [HeroPOSP].[dbo].[POSP_Agreement]
			SET StampId = @StampId
			WHERE UserId = @UserId
		END

		SELECT u.UserId
			,ud.DateofBirth
			,ud.AadhaarNumber
			,pan.PanNumber AS PAN
			,ad.AddressLine1
			,ad.AddressLine2
			,ad.Pincode AS PinCode
			,ad.CityId
			,ad.StateId
			,u.SignatureDocumentId
			,u.UserName AS POSP_Name
			,pa.AgreementId
			,pa.PreSignedAgreementId
			,ud.Gender
			,u.POSPId
			,pan.FatherName
			,ICity.CityName
			,IState.StateName
			,SD.StampData AS StampNumber
			,pa.IsRevoked
		FROM [HeroIdentity].[dbo].[Identity_User] AS u WITH (NOLOCK)
		LEFT JOIN [HeroIdentity].[dbo].[Identity_UserAddressDetail] AS ad WITH (NOLOCK) ON ad.UserId = u.UserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] AS ud WITH (NOLOCK) ON ud.UserId = u.UserId
		LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] AS pa WITH (NOLOCK) ON pa.UserId = u.UserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_PanVerification] AS pan WITH (NOLOCK) ON pan.UserId = u.UserId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_City] AS ICity WITH (NOLOCK) ON ICity.CityId = ad.CityId
		LEFT JOIN [HeroIdentity].[dbo].[Identity_State] AS IState WITH (NOLOCK) ON IState.StateId = ad.StateId
		LEFT JOIN [HeroAdmin].[dbo].[Admin_StampData] AS SD WITH (NOLOCK) ON SD.Id = pa.StampId
		WHERE u.UserId = @UserId
			AND pan.IsActive = 1
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
