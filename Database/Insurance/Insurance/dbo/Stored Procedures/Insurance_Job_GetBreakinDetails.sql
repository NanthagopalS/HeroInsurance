CREATE PROCEDURE [dbo].[Insurance_Job_GetBreakinDetails]     
AS      
BEGIN      
	BEGIN TRY      
		
		SELECT TOP 10 PolicyNumber, QuoteTransactionId, LeadId
		FROM Insurance_LeadDetails WITH(NOLOCK) 
		WHERE ISNULL(PolicyNumber,'') != '' AND ISNULL(IsBreakin,0) = 1 AND IsBreakinApproved IS NULL
		AND InsurerId='78190CB2-B325-4764-9BD9-5B9806E99621'
		ORDER BY CreatedOn

	END TRY      
	BEGIN CATCH                 
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
		SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
		SET @ErrorDetail=ERROR_MESSAGE()                                  
		EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
	END CATCH      
END 