-- EXEC [dbo].[Insurance_GetICICIQuoteConfirmDetails] '8ABC1A51-7213-4AF8-B686-4DCBC37541E2'
CREATE PROCEDURE [dbo].[Insurance_GetICICIQuoteConfirmDetails]
@QuoteTransactionId VARCHAR(100) NULL
AS
BEGIN
	BEGIN TRY
		--DECLARE @RefQuoteId VARCHAR(50)
		SET NOCOUNT ON;

		--SELECT @RefQuoteId = RefQuoteTransactionId
		--FROM Insurance_QuoteTransaction QT WITH(NOLOCK) 
		--INNER JOIN Insurance_LeadDetails LD WITH(NOLOCK) 
		--ON QT.QuoteTransactionId = LD.QuoteTransactionID
		--WHERE LD.QuoteTransactionID = @QuoteTransactionId

		SELECT RequestBody, ResponseBody, QuoteTransactionId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionId = @QuoteTransactionId
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END