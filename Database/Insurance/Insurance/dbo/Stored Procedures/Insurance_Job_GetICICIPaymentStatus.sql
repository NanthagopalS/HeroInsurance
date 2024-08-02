CREATE PROCEDURE [dbo].[Insurance_Job_GetICICIPaymentStatus]  
AS
BEGIN 
	BEGIN TRY
		SELECT ApplicationId, QuoteTransactionId FROM Insurance_PaymentTransaction WITH(NOLOCK) 
		WHERE InsurerId = 'FD3677E5-7938-46C8-9CD2-FAE188A1782C' AND Status IS NULL 
		AND ISNULL(ApplicationId,'') != ''  

	END TRY   
	
BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END