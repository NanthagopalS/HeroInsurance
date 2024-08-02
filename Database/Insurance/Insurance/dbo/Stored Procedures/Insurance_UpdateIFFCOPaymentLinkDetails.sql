CREATE   PROCEDURE [dbo].[Insurance_UpdateIFFCOPaymentLinkDetails]  
@InsurerId VARCHAR(50) NULL,
@QuoteTransactionId VARCHAR(50) NULL,
@PaymentLink VARCHAR(MAX) NULL,
@ProposalRequest NVARCHAR(MAX) NULL,
@ProposalNumber VARCHAR(50) NULL,
@UpdatedBy VARCHAR(50) NULL,
@OldProposalNumber VARCHAR(50) NULL
AS
BEGIN 
  BEGIN TRY

	DECLARE @LeadId VARCHAR(50) 

	SELECT @LeadId = LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE InsurerId=@InsurerId AND QuoteTransactionId=@QuoteTransactionId

	UPDATE Insurance_QuoteTransaction SET RequestBody = @ProposalRequest, TransactionId = @ProposalNumber, UpdatedOn = GETDATE()  WHERE QuoteTransactionId = @QuoteTransactionId

	UPDATE Insurance_PaymentTransaction 
	SET ApplicationId = @ProposalNumber, ProposalNumber = @ProposalNumber, UpdatedOn = GETDATE()
	WHERE ApplicationId = @OldProposalNumber

	UPDATE Insurance_LeadDetails SET PaymentLink = @PaymentLink, UpdatedBy = @UpdatedBy, UpdatedOn = GETDATE() WHERE LeadId = @LeadId

	SELECT @PaymentLink AS PaymentLink
	
 END TRY      
 BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
 END CATCH    

END