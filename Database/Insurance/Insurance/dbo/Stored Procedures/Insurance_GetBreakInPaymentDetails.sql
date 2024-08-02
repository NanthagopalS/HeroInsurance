 --exec [dbo].[Insurance_GetBreakInPaymentDetails] 'DC874A12-6667-41AB-A7A1-3BB832B59CEB','51D9F30E-DC48-4CD9-9AE8-628A5EA7F046'      
CREATE PROCEDURE [dbo].[Insurance_GetBreakInPaymentDetails]        
@InsurerId VARCHAR(50) NULL,      
@QuoteTransactionId VARCHAR(50) NULL       
AS      
BEGIN       
  BEGIN TRY      
      
  DECLARE @QuoteId VARCHAR(100)      
    
 --SELECT @QuoteId = QuoteTransactionId FROM Insurance_QuoteTransaction WHERE RefQuoteTransactionId = @QuoteTransactionId      
    
      
      
 SELECT Quote.RequestBody ProposalRequest,      
  Quote.ResponseBody ProposalResponse,      
  Quote.RefQuoteTransactionId RefQuoteTransactionId,    
  Payment.ApplicationId ApplicationId,     
  Payment.CustomerId,  
  Payment.ProposalNumber,  
  Leads.GrossPremium GrossPremium,    
  Leads.PaymentLink PaymentLink,    
  Leads.UpdatedOn PaymentLinkCreatedDate,    
  Leads.VehicleTypeId VehicleTypeId,    
  Leads.IsBreakin IsBreakin,    
  Leads.IsBreakinApproved IsBreakinApproved,    
  Leads.LeadId,    
  Leads.PANNumber,    
  Leads.PhoneNumber,    
  Leads.Email,    
  Leads.LeadName AS LeadFirstName,    
  Leads.LastName AS LeadLastName,
  Leads.CarOwnedBy AS CustomerType,
  Leads.CompanyName AS CompanyName
 FROM Insurance_QuoteTransaction Quote WITH(NOLOCK)      
 LEFT JOIN Insurance_PaymentTransaction Payment WITH(NOLOCK)       
 ON Payment.QuoteTransactionId = Quote.QuoteTransactionId AND Payment.InsurerId = Quote.InsurerId      
 LEFT JOIN Insurance_LeadDetails Leads WITH(NOLOCK)       
 ON Leads.QuoteTransactionID = Quote.QuoteTransactionId AND Leads.InsurerId = Quote.InsurerId      
 WHERE Quote.InsurerId=@InsurerId AND Quote.QuoteTransactionId=@QuoteTransactionId    
 AND Quote.StageID='ADB9EB9C-CB73-4DE3-BAF7-151F90C2A6F2'      
 ORDER BY Quote.CreatedOn DESC      
       
 END TRY            
 BEGIN CATCH                       
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                    
 END CATCH          
      
END  
  
  