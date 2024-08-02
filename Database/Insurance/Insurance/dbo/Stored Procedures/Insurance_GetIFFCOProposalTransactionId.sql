 -- EXEC [dbo].[Insurance_GetIFFCOProposalTransactionId] 'HERO71650'
CREATE   procedure [dbo].[Insurance_GetIFFCOProposalTransactionId]  
@ProposalNumber varchar(50)  
AS
BEGIN 
  BEGIN TRY
	SELECT QuoteTransactionId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE TransactionId = @ProposalNumber
 END TRY      
 BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
 END CATCH    

END