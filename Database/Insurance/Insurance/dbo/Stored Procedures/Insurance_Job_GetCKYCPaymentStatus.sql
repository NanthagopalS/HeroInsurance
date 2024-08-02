
CREATE PROCEDURE [dbo].[Insurance_Job_GetCKYCPaymentStatus]  
AS
BEGIN 
	BEGIN TRY
		SELECT ApplicationId, QuoteTransactionId,LeadId,CKYCStatus,Status,CreatedOn
		FROM(	
			SELECT ApplicationId, QuoteTransactionId,LeadId,CKYCStatus,Status,CreatedOn
			FROM Insurance_PaymentTransaction WITH(NOLOCK) 
			WHERE InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621' 
			AND (ISNULL(Status,'') ='' OR ISNULL(Status,'') ='0151C6E3-8DC5-4BBD-860A-F1501A7647B2' ) -- PENDING
			AND (ISNULL(CKYCStatus,'') ='' OR ISNULL(CKYCStatus,'') ='FAILED' )
			AND ISNULL(ApplicationId,'') != ''
			UNION ALL
			SELECT ApplicationId, QuoteTransactionId,LeadId,CKYCStatus,Status,CreatedOn
			FROM Insurance_PaymentTransaction WITH(NOLOCK) 
			WHERE InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621' 
			AND ISNULL(Status,'') ='A25D747B-167E-4C1B-AE13-E6CC49A195F8' --SUCCESS
			AND ISNULL(CKYCStatus,'') ='FAILED' 
		)A
		WHERE ISNULL(LEADID,'')!=''
		AND CAST(CREATEDON AS DATE) >= DATEADD(day, -15, GETDATE())

	END TRY   
	
BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END