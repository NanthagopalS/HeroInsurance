--[dbo].[Insurance_Job_GetTATABreakinAndPaymentStatus] '85F8472D-8255-4E80-B34A-61DB8678135C','1'    
CREATE PROCEDURE [dbo].[Insurance_Job_GetTATABreakinAndPaymentStatus]    
@InsurerId VARCHAR(50) = NULL,    
@IsPayment BIT = NULL    
AS    
BEGIN     
 BEGIN TRY    
      
  IF(@IsPayment = 1)    
  BEGIN    
   SELECT TOP 5 PAY.ApplicationId PaymentId, PAY.LeadId LeadId, LEAD.VehicleTypeId VehicleTypeId, 
   LEAD.PolicyNumber as ProposalNo
   FROM Insurance_PaymentTransaction PAY WITH(NOLOCK)    
   INNER JOIN Insurance_LeadDetails LEAD WITH(NOLOCK) ON LEAD.LeadId = PAY.LeadId  
   WHERE PAY.InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C' AND PAY.Status = '0151C6E3-8DC5-4BBD-860A-F1501A7647B2'--Pending    
   ORDER BY PAY.CreatedOn DESC    
  END    
  ELSE    
  BEGIN    
   SELECT TOP 5 LeadId,    
   PolicyNumber as ProposalNo,    
   BreakinId as TicketId,  
   VehicleTypeId,
   QuoteTransactionID QuoteTransactionId
   FROM Insurance_LeadDetails WITH(NOLOCK)     
   WHERE InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C' AND IsBreakin = 1    
   AND IsBreakinApproved IS NULL AND BreakinId IS NOT NULL     
   ORDER BY CreatedOn DESC    
  END    
 END TRY       
     
BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH       
END