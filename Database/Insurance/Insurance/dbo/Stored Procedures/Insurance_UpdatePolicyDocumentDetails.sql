      
      
CREATE PROCEDURE [dbo].[Insurance_UpdatePolicyDocumentDetails]     
@QuoteTransactionID VARCHAR(50) = NULL,
@InsurerId VARCHAR(50) = NULL,     
@PolicyDocumentLink VARCHAR(500) = NULL,
@DocumentId VARCHAR(100) = NULL,
@CustomerId VARCHAR(50) = NULL,
@PolicyNumber VARCHAR(50) = NULL
AS      
BEGIN      
	BEGIN TRY      
		IF(@InsurerId='0A326B77-AFD5-44DA-9871-1742624CFF16')
		BEGIN
			UPDATE Insurance_PaymentTransaction SET DocumentId=@DocumentId, CustomerId=@CustomerId, PolicyNumber = @PolicyNumber, UpdatedOn = GETDATE()
			WHERE QuoteTransactionId=@QuoteTransactionID and InsurerId=@InsurerId
		END
		ELSE
		BEGIN
			UPDATE Insurance_PaymentTransaction SET PolicyDocumentLink=@PolicyDocumentLink, DocumentId=@DocumentId, UpdatedOn = GETDATE()
			WHERE QuoteTransactionId=@QuoteTransactionID and InsurerId=@InsurerId
		END
		

		SELECT DocumentId
		FROM Insurance_PaymentTransaction WITH(NOLOCK) WHERE QuoteTransactionId=@QuoteTransactionID and InsurerId=@InsurerId

	END TRY      
	BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
	END CATCH      
END 