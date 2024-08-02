  
-- =============================================    
-- Author:  <Firoz S>    
-- Create date: <12-05-2023>    
-- Description: <Insurance_GetHDFCPaymentFields>    
--exec [dbo].[Insurance_GetHDFCPaymentFields] '106323102642699'  
-- =============================================    
CREATE PROCEDURE [dbo].[Insurance_GetHDFCPaymentFields]      
@ApplicationId VARCHAR(50) NULL  
AS      
BEGIN      
 BEGIN TRY      
      
   DECLARE @QuoteTransactionId VARCHAR(50), @ProposalNumber VARCHAR(50), @TransactionId VARCHAR(50), @LeadId VARCHAR(50),  
   @VehicleTypeId VARCHAR(50), @PolicyTypeId VARCHAR(50), @PolicyNumber VARCHAR(50), @GrossPremium VARCHAR(50),  
   @BankName VARCHAR(50), @PaymentDate VARCHAR(50), @CategoryId INT
  
   SELECT @QuoteTransactionId = QuoteTransactionId,@ProposalNumber=ProposalNumber, @PolicyNumber = @PolicyNumber,  
   @GrossPremium =Amount, @BankName = BankName, @PaymentDate = PaymentDate  
   FROM Insurance_PaymentTransaction WITH(NOLOCK) WHERE ApplicationId = @ApplicationId  
  
   SELECT @LeadId=LeadId, @TransactionId = TransactionId   
   FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionId = @QuoteTransactionId  
  
   SELECT @VehicleTypeId = VehicleTypeId,@PolicyTypeId = PolicyTypeId   
   FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadId  

   SELECT @CategoryId = VehicleCategoryId FROM Insurance_CommercialLeadDetail WITH(NOLOCK) WHERE LeadId = @LeadId
     
   SELECT @QuoteTransactionId QuoteTransactionId,  
    @ProposalNumber ProposalNumber,  
    @TransactionId TransactionId,  
    @VehicleTypeId VehicleTypeId,  
    @PolicyTypeId PolicyTypeId,  
    @PolicyNumber PolicyNumber,  
    @GrossPremium GrossPremium,  
    @BankName BankName,  
    @PaymentDate PaymentDate,  
    @ApplicationId ApplicationId,
	@LeadId LeadId,
	@CategoryId CategoryId
  
   
 END TRY                      
 BEGIN CATCH                          
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
END