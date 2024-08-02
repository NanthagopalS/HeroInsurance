

-- exec  [dbo].[Insurance_GetCholaQuoteResponse]   '77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA', '82CC6C17-539A-4A1F-992C-996605B4604B'
CREATE procedure [dbo].[Insurance_GetCholaQuoteResponse]  
@InsurerId VARCHAR(50),
@QuotetransactionId VARCHAR(50)

AS
BEGIN 
BEGIN TRY

SELECT ResponseBody, LeadId
FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionId = @QuotetransactionId AND InsurerId = @InsurerId

--SELECT CarOwnedBy FROM Insurance_LeadDetails WITH(NOLOCK) WHERE QuoteTransactionID = @QuotetransactionId AND InsurerId = @InsurerId

END TRY                        
 BEGIN CATCH                  
               
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH   
END