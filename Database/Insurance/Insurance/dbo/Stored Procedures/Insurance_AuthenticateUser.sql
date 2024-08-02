
-- EXEC [Insurance_AuthenticateUser] 'C9CC590F-3ED0-42B5-9047-AAFFD71568BF','1ABF6E56-FF59-4813-83B1-84F26473B99B','HERO/ENQ/107107'

CREATE     PROCEDURE [dbo].[Insurance_AuthenticateUser]
(
	@TransactionId VARCHAR(50) = NULL,
	@UserId VARCHAR(50) = NULL,
	@LeadId VARCHAR(50) = NULL,
	@StageId VARCHAR(50) = NULL
)
AS
BEGIN
BEGIN TRY
	DECLARE @CreationTime DATETIME, @Result BIT, @Day VARCHAR(50),  @CurrentDay VARCHAR(50), @CurrentStageId VARCHAR(50)

	SELECT @CurrentStageId = StageId from Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadId

	SELECT @CreationTime = CreatedOn FROM Insurance_SharePlanDetails WITH(NOLOCK) 
	WHERE Id = @TransactionId AND UserId = @UserId AND LeadId = @LeadId

	SET	@Day = CAST(@CreationTime as date)

	SET @CurrentDay = CAST(GETDATE() as date)

	PRINT @Day
	PRINT @CurrentDay

	IF @Day = @CurrentDay AND @CurrentStageId = @StageId
		BEGIN
			SEt @Result = 1
		END
	ELSE
		BEGIN
			SET @Result = 0
		END		
	SELECT @Result Result
END TRY 
BEGIN CATCH
 DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
END CATCH
END