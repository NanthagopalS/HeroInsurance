CREATE     PROCEDURE [dbo].[Insurance_GetIFFCOBreakinDetails]
@QuoteTransactionId VARCHAR(50) NULL  
AS
BEGIN
	BEGIN TRY

		SELECT IsBreakin,IsBreakinApproved, BreakinId, PaymentLink, CarOwnedBy
		FROM Insurance_LeadDetails WITH(NOLOCK)
		WHERE QuoteTransactionID = @QuoteTransactionId AND InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3' 
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END