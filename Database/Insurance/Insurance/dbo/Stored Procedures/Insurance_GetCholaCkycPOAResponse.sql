
CREATE procedure [dbo].[Insurance_GetCholaCkycPOAResponse]  
@InsurerId VARCHAR(50),
@QuotetransactionId VARCHAR(50)
AS
BEGIN 
BEGIN TRY

SELECT quotetrans.TransactionId AS AppRefNo, KYCId AS TransactionId, quotetrans.LeadId AS LeadId
FROM Insurance_QuoteTransaction quotetrans WITH(NOLOCK) 
INNER JOIN Insurance_CKYCTransaction kyctrans WITH(NOLOCK) ON quotetrans.QuoteTransactionId = kyctrans.QuoteTransactionId 
WHERE kyctrans.QuoteTransactionId = @QuotetransactionId AND kyctrans.InsurerId = @InsurerId

END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END