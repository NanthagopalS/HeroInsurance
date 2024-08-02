-- =============================================              
-- Author:  <Author,Ankit Ghosh>            
-- Create date: <Create Date,25-01-2023>           
-- Description: <Description,,InsertAgreementDetail>              
-- =============================================              
CREATE
	

 PROCEDURE [dbo].[POSP_InsertAgreementDetail] (
	@UserId VARCHAR(50) = NULL
	,@SignatureId VARCHAR(100) = NULL
	,@ProcessType VARCHAR(50) = NULL
	)
AS
BEGIN
	DECLARE @StatusId VARCHAR(500) = NULL

	BEGIN TRY
		IF (@ProcessType = 'Signature')
		BEGIN
			UPDATE [HeroIdentity].[dbo].[Identity_User]
			SET SignatureDocumentId = @SignatureId
			WHERE UserId = @UserId
				AND IsActive = 1

			SELECT PreSignedAgreementId
			FROM [dbo].[POSP_Agreement] WITH (NOLOCK)
			WHERE UserId = @UserId
		END
		ELSE IF (@ProcessType = 'Agreement')
		BEGIN
			SET @StatusId = (
					SELECT Id
					FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster] WITH (NOLOCK)
					WHERE StatusName = 'Agreement Sign'
						AND PriorityIndex = 10
					)

			--UPDATE Breadcrum stage for PSOP User          
			UPDATE [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusDetail]
			SET StatusId = @StatusId, UpdatedOn = GETDATE()
			WHERE UserId = @UserId
				AND StatusId IN (
					SELECT Id
					FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster]
					WHERE StatusName = 'Agreement Sign'
						AND PriorityIndex IN (
							8
							,9
							)
					)

			UPDATE [HeroPOSP].[dbo].[POSP_Agreement]
			SET PreSignedAgreementId = 'Deleted'
				,AgreementId = @SignatureId
				,UpdatedOn = GETDATE()
			WHERE UserId = @UserId

			--UPDATE PROFILE STAGE FOR USERS              
			UPDATE [HeroIdentity].[dbo].[Identity_User]
			SET UserProfileStage = 5, UpdatedOn = GETDATE()
			WHERE UserId = @UserId
		END
				--If PreSignedAgreementId not found... Error PreSigned Agreement is not available.  
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
