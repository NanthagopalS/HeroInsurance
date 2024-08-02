
CREATE procedure [dbo].[Insurance_GetCholaQuotationId] 
@KycId VARCHAR(50)

AS
BEGIN 
Declare
@QuoteTransactionId VARCHAR(50)
BEGIN TRY


SELECT @QuoteTransactionId=QuoteTransactionId FROM Insurance_CKYCTransaction WITH(NOLOCK) WHERE KYCId = @KycId

SELECT @QuoteTransactionId
END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END