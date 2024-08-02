  
CREATE PROCEDURE [dbo].[Insurance_Job_GetCholaCKYCStatus]    
@InsurerId VARCHAR(50) = NULL
AS  
BEGIN   
 BEGIN TRY  
	SELECT TOP 10 KYCId AS TransactionId , TransactionId as appRefNo ,ckyctrans.QuoteTransactionId AS QuoteTransactionId
	FROM Insurance_CKYCTransaction ckyctrans WITH (NOLOCK)
	INNER JOIN dbo.Insurance_QuoteTransaction quotetrans WITH (NOLOCK) ON quotetrans.QuoteTransactionId = ckyctrans.QuoteTransactionId
	WHERE ckyctrans.InsurerId = @InsurerId 
	AND (CKYCStatus NOT IN ('Rejected','Success') OR CKYCStatus IS NULL) 
	AND KYCId IS NOT NULL
	ORDER BY ckyctrans.CreatedOn DESC
      
 END TRY     
   
BEGIN CATCH                    
                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH     
END