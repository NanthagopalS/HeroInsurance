CREATE      PROCEDURE [dbo].[Insurance_InsertQuoteRequest] 
(
	@RequestBody NVARCHAR(MAX) = NULL,
	@LeadId VARCHAR(100) = NULL,
	@Stage VARCHAR(100) = NULL,
	@UserId VARCHAR(100) = NULL,
	@QuoteTransactionId VARCHAR(100) = NULL
)
AS
BEGIN
BEGIN TRY
	DECLARE @SatgeId VARCHAR(100), @GetLeadId VARCHAR(50)

	SELECT @SatgeId = StageID FROM Insurance_StageMaster WITH(NOLOCK) WHERE Stage = @Stage

	IF EXISTS(SELECT TOP 1 LeadId FROM Tbl_QuotationRequest WITH(NOLOCK) WHERE LeadId = @LeadId)
	BEGIN
		DELETE FROM Tbl_QuotationRequest WHERE LeadId = @LeadId AND StageID = @SatgeId
	END

	IF @SatgeId = '1A0E2EC7-6B19-4739-BD99-4599B77C5CA8'
	BEGIN
		SELECT @LeadId = LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionId = @QuoteTransactionId
	END

	INSERT INTO Tbl_QuotationRequest (LeadId,RequestBody,StageID,CreatedBy) 
	VALUES (@LeadId, @RequestBody, @SatgeId, @UserId)

END TRY 
BEGIN CATCH
 DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
END CATCH
END