 -- EXEC [dbo].[Insurance_UpdateIFFCOProposalId] 'HERO71650'
CREATE     procedure [dbo].[Insurance_UpdateIFFCOProposalId]  
@QuoteTransactionId varchar(50),
@ProposalNumber varchar(50)
AS
BEGIN 
  BEGIN TRY
	UPDATE Insurance_PaymentTransaction SET QuoteTransactionId = @QuoteTransactionId, UpdatedOn = GETDATE() WHERE ProposalNumber = @ProposalNumber
 END TRY      
 BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
 END CATCH    

END