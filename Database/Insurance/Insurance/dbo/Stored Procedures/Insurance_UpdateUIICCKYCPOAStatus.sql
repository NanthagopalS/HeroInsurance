CREATE     PROCEDURE [dbo].[Insurance_UpdateUIICCKYCPOAStatus]     
@QuotetransactionId VARCHAR(50) = NULL,
@CKYCStatus VARCHAR(50) = NULL,
@OEMUniqId VARCHAR(50) = NULL
AS      
BEGIN      
	BEGIN TRY      
		UPDATE Insurance_PaymentTransaction 
		SET CKYCStatus = @CKYCStatus 
		WHERE QuoteTransactionId = @QuotetransactionId AND PaymentCorrelationId = @OEMUniqId

		SELECT * FROM Insurance_LeadDetails WITH(NOLOCK) 
		WHERE QuoteTransactionID  = @QuotetransactionId
	END TRY      
	BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
	END CATCH      
END