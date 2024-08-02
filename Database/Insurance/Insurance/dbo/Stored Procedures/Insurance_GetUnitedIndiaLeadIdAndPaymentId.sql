
  
--[dbo].[Insurance_GetUnitedIndiaLeadIdAndPaymentId] '85F8472D-8255-4E80-B34A-61DB86781309','85F8472D-8255-4E80-B34A-61DB8678135C'  
CREATE PROCEDURE [dbo].[Insurance_GetUnitedIndiaLeadIdAndPaymentId]  
@QuoteTransactionId VARCHAR(50) = NULL,  
@InsurerId VARCHAR(50) = NULL  
AS  
BEGIN   
 BEGIN TRY  
  SELECT PAY.ApplicationId PaymentId, PAY.LeadId, LEAD.VehicleTypeId VehicleTypeId
  FROM Insurance_PaymentTransaction PAY WITH(NOLOCK) 
  INNER JOIN Insurance_LeadDetails LEAD WITH(NOLOCK) ON LEAD.LeadId = PAY.LeadId
  WHERE PAY.InsurerId = @InsurerId AND PAY.QuoteTransactionId = @QuoteTransactionId  
 END TRY     
   
BEGIN CATCH                    
                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH     
END