

CREATE      PROCEDURE [dbo].[POSP_InsertUpdatePOSPStage]    
(    
@UserId varchar(100),
@StageId varchar(100)
) 
AS
BEGIN
	BEGIN TRY
		IF EXISTS (SELECT * FROM POSP_UserStageStatusDetail WHERE UserId = @UserID)
			BEGIN
				UPDATE POSP_UserStageStatusDetail SET StageId = @StageId, UpdatedOn = GETDATE() WHERE UserId = @UserId
			END
		ELSE
			BEGIN
				INSERT INTO POSP_UserStageStatusDetail (UserId,StageId) VALUES (@UserId,@StageId)
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