-- EXEC [dbo].[Insurance_GetQuoteTransactionId] '0BC1CDC2-D008-4305-BCC0-E034A9CF29C7'
CREATE PROCEDURE [dbo].[Insurance_GetQuoteTransactionId]

@QuoteTransactionId VARCHAR(100) NULL
AS
BEGIN
	BEGIN TRY
		SET NOCOUNT ON;

		SELECT RefQuoteTransactionId
		FROM Insurance_QuoteTransaction WITH(NOLOCK) 
		WHERE QuoteTransactionID = @QuoteTransactionId

	END TRY                
	BEGIN CATCH          
			 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END