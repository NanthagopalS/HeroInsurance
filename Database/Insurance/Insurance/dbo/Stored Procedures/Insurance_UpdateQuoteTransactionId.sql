 --exec [dbo].[UpdateLeadPaymentLink] '','',''
CREATE   PROCEDURE [dbo].[Insurance_UpdateQuoteTransactionId]  
@LeadId VARCHAR(50) NULL,
@QuoteTransactionId VARCHAR(50) NULL
AS
BEGIN 
  BEGIN TRY
	
	UPDATE Insurance_LeadDetails SET QuoteTransactionID = @QuoteTransactionId
	WHERE LeadId = @LeadId 
	
 END TRY      
 BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
 END CATCH    

END