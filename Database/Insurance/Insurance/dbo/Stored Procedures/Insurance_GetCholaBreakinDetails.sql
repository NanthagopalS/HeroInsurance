CREATE   PROCEDURE [dbo].[Insurance_GetCholaBreakinDetails]
@QuoteTransactionId VARCHAR(50) NULL  
AS
BEGIN
	BEGIN TRY

		SELECT IsBreakin,IsBreakinApproved, BreakinId, PaymentLink
		FROM Insurance_LeadDetails WITH(NOLOCK)
		WHERE QuoteTransactionID = @QuoteTransactionId AND InsurerId = '77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA' 
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END