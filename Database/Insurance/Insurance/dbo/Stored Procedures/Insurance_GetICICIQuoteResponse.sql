    
    
-- exec  [dbo].[Insurance_GetCholaQuoteResponse]   '16413879-6316-4C1E-93A4-FF8318B14D37', '3136CE5C-E055-4E6D-89ED-8B7E61118486'    
CREATE      PROCEDURE [dbo].[Insurance_GetICICIQuoteResponse]      
@InsurerId VARCHAR(50),    
@QuotetransactionId VARCHAR(50)    
    
AS    
BEGIN     
BEGIN TRY    
    
SELECT ResponseBody,LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionId = @QuotetransactionId AND InsurerId = @InsurerId    
    
END TRY                            
 BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH       
END