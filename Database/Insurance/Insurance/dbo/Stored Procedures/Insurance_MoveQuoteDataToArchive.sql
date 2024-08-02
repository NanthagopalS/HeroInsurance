CREATE  PROCEDURE [dbo].[Insurance_MoveQuoteDataToArchive]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TwoMonthAgo DATE
    SET @TwoMonthAgo = DATEADD(MONTH, -1, GETDATE())

    INSERT INTO [HeroLogs].[dbo].[Insurance_QuoteTransaction_Logs] 
	([QuoteTransactionId],[InsurerId],[RequestBody],[ResponseBody],[CommonResponse],[CreatedBy]
	,[CreatedOn],[UpdatedBy],[UpdatedOn],[StageID],[LeadId],[MinIDV],[MaxIDV],[RecommendedIDV]
	,[TransactionId],[VehicleTypeId],[PolicyTypeId],[IsBrandNew],[RefQuoteTransactionId],[SelectedIDV]
	,[CustomIDV],[ProposalId],[PolicyId],[VehicleNumber],[RTOId])
    SELECT 
     [QuoteTransactionId],[InsurerId],[RequestBody],[ResponseBody],[CommonResponse],[CreatedBy]
	,[CreatedOn],[UpdatedBy],[UpdatedOn],[StageID],[LeadId],[MinIDV],[MaxIDV],[RecommendedIDV]
	,[TransactionId],[VehicleTypeId],[PolicyTypeId],[IsBrandNew],[RefQuoteTransactionId],[SelectedIDV]
	,[CustomIDV],[ProposalId],[PolicyId],[VehicleNumber],[RTOId] 
	FROM [dbo].[Insurance_QuoteTransaction]
    WHERE [dbo].[Insurance_QuoteTransaction].CreatedOn < @TwoMonthAgo

    DELETE FROM  [dbo].[Insurance_QuoteTransaction]
    WHERE [dbo].[Insurance_QuoteTransaction].CreatedOn < @TwoMonthAgo
END