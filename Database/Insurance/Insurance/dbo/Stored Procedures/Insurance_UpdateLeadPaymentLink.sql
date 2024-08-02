 --exec [dbo].[UpdateLeadPaymentLink] '','',''
CREATE PROCEDURE [dbo].[Insurance_UpdateLeadPaymentLink]  
@InsurerId VARCHAR(50) NULL,
@QuoteTransactionId VARCHAR(50) NULL,
@PaymentLink VARCHAR(MAX) NULL,
@UpdatedBy VARCHAR(50) NULL,
@PaymentCorrelationId VARCHAR(50) NULL
AS
BEGIN 
  BEGIN TRY
	DECLARE @LeadId VARCHAR(50) 
	SELECT @LeadId = LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE InsurerId=@InsurerId AND QuoteTransactionId=@QuoteTransactionId
	
	UPDATE Insurance_LeadDetails SET PaymentLink=@PaymentLink,
		UpdatedBy = @UpdatedBy,UpdatedOn = GETDATE()
	WHERE LeadId = @LeadId AND QuoteTransactionID = @QuoteTransactionId

	UPDATE Insurance_PaymentTransaction 
	SET PaymentCorrelationId = @PaymentCorrelationId, UpdatedOn = GETDATE() --Updating payment id only for chola during breakin case 
	WHERE LeadId = @LeadId

	IF(@InsurerId = '77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA') --Updating payment id only for chola during breakin case
	BEGIN
		UPDATE Insurance_PaymentTransaction 
		SET ApplicationId = @PaymentCorrelationId, QuoteTransactionId = @QuoteTransactionId, UpdatedOn = GETDATE()
		WHERE LeadId = @LeadId
	END

	SELECT @PaymentLink AS PaymentLink
	
 END TRY      
 BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
 END CATCH    

END