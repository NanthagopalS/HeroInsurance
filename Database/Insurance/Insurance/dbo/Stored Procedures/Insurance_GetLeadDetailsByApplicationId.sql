CREATE   PROCEDURE [dbo].[Insurance_GetLeadDetailsByApplicationId]            
@ApplicationId  VARCHAR(100) = NULL,    
@QuoteTransactionId VARCHAR(100) = NULL,    
@InsurerId  VARCHAR(100) = NULL    
AS          
BEGIN           
 BEGIN TRY    
 IF(@ApplicationId != '' OR @ApplicationId IS NOT NULL)    
 BEGIN    
  SELECT LeadId as LeadID FROM Insurance_PaymentTransaction WITH (NOLOCK) WHERE ApplicationId = @ApplicationId AND InsurerId = @InsurerId    
 END    
 ELSE IF(@QuoteTransactionId != '' OR @QuoteTransactionId IS NOT NULL)    
 BEGIN    
  SELECT LeadId as LeadID, TransactionId, VehicleTypeId FROM Insurance_QuoteTransaction WITH (NOLOCK) WHERE QuoteTransactionId = @QuoteTransactionId AND InsurerId = @InsurerId    
 END    
 ELSE    
 BEGIN    
  SELECT NULL as LeadID    
 END    
 END TRY                  
BEGIN CATCH                            
                         
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                              
  SET @ErrorDetail=ERROR_MESSAGE()                                              
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                               
 END CATCH             
END