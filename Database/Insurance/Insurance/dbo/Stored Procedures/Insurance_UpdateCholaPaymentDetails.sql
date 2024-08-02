CREATE   PROCEDURE [dbo].[Insurance_UpdateCholaPaymentDetails]       
@TransactionId VARCHAR(50) = NULL,  
@InsurerId VARCHAR(50) = NULL,
@Amount VARCHAR(50) = NULL,
@PaymentTransactionNumber VARCHAR(50) = NULL
AS        
BEGIN        
 BEGIN TRY        
	DECLARE @LeadId VARCHAR(50)

	UPDATE Insurance_PaymentTransaction
	SET Amount = @Amount,
	PaymentTransactionNumber = @PaymentTransactionNumber,
	Status = 'A25D747B-167E-4C1B-AE13-E6CC49A195F8'
	WHERE ApplicationId = @TransactionId AND InsurerId = @InsurerId
  
	SELECT @LeadId = LeadId FROM Insurance_PaymentTransaction WITH(NOLOCK) WHERE ApplicationId = @TransactionId and InsurerId = @InsurerId
	UPDATE Insurance_LeadDetails SET StageId='D07216AF-ACAD-4EEA-8CFF-2910BA77E5EE' WHERE LeadId=@LeadId--Updating Stage to Payment

 END TRY        
 BEGIN CATCH                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                
 END CATCH        
END