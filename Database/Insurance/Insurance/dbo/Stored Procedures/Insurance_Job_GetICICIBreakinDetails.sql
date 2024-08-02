CREATE PROCEDURE [dbo].[Insurance_Job_GetICICIBreakinDetails]     
AS      
BEGIN      
	BEGIN TRY      
		
		SELECT PolicyNumber, QuoteTransactionId ,BreakinId
		FROM Insurance_LeadDetails WITH(NOLOCK) 
		WHERE ISNULL(PolicyNumber,'') != '' AND ISNULL(IsBreakin,0) = 1 AND ISNULL(IsBreakinApproved,0) = 0
		AND InsurerId='FD3677E5-7938-46C8-9CD2-FAE188A1782C' 
		AND ISNULL(BreakinId,'') != ''  

	END TRY      
	BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
	END CATCH      
END 