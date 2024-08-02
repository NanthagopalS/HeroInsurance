          
          
CREATE    PROCEDURE [dbo].[Insurance_InsertPaymentTransaction]         
@QuoteTransactionID VARCHAR(50) = NULL,    
@InsurerId VARCHAR(50) = NULL,          
@ApplicationId VARCHAR(200) = NULL,          
@ProposalNumber VARCHAR(50) = NULL,        
@Amount VARCHAR(50) = NULL,        
@Status VARCHAR(50) = NULL,    
@PaymentTransactionNumber VARCHAR(MAX) = NULL,    
@CKYCStatus VARCHAR(50) = NULL,    
@Type VARCHAR(50) = NULL,    
@CKYCLink VARCHAR(200) = NULL,    
@CKYCFailReason VARCHAR(50) = NULL,    
@PolicyDocumentLink VARCHAR(500) = NULL,    
@DocumentId VARCHAR(100) = NULL,    
@PolicyNumber VARCHAR(100) = NULL,    
@CustomerId VARCHAR(100) = NULL,    
@IsTP BIT = NULL,    
@BreakinId VARCHAR(50) = NULL,    
@PaymentLink NVARCHAR(MAX),    
@BankName VARCHAR(100) NULL,    
@BankPaymentRefNum VARCHAR(100) NULL,    
@PaymentDate VARCHAR(50) NULL,    
@UserId VARCHAR(100) NULL,    
@PaymentCorrelationId VARCHAR(50) = NULL,    
@BreakinInspectionURL NVARCHAR(MAX) = NULL    
AS          
BEGIN          
 BEGIN TRY          
  IF(@Status='Payment Completed')    
   BEGIN    
    SET @Status ='A25D747B-167E-4C1B-AE13-E6CC49A195F8'    
   END    
  ELSE IF(@Status='Payment Failed' OR @Status='Payment Incomplete')    
   BEGIN    
    SET @Status ='EBA950EF-6739-4236-8DF0-EA8E69E65C66'    
   END    
  ELSE     
   BEGIN    
    SET @Status ='0151C6E3-8DC5-4BBD-860A-F1501A7647B2'    
   END    
    
  DECLARE @PaymentTransactionId VARCHAR(50), @LeadId VARCHAR(50), @InsurerLogo VARCHAR(100)    
    
    
  IF(@CKYCFailReason='S')    
  BEGIN    
   SET @CKYCFailReason=null    
  END    
  ELSE IF(@CKYCFailReason='F')    
  BEGIN    
   SET @CKYCFailReason='Fail'    
  END    
  ELSE IF(@CKYCFailReason='P')    
  BEGIN    
   SET @CKYCFailReason='Name Mismatch'    
  END    
  ELSE IF(@CKYCFailReason='A')    
  BEGIN    
   SET @CKYCFailReason='Address Mismatch'    
  END    
  ELSE IF(@CKYCFailReason='B')    
  BEGIN    
   SET @CKYCFailReason='Name and Address Mismatch'    
  END    
    
  IF(@Type='INSERT')    
  BEGIN    
   SET @PaymentTransactionId=NEWID()    
    
   SELECT @LeadId = LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionID = @QuoteTransactionID    
    
   INSERT INTO Insurance_PaymentTransaction(QuoteTransactionId,InsurerId,ApplicationId,ProposalNumber,CreatedBy,LeadId,Amount,PolicyNumber,CustomerId,PaymentTransactionNumber,PaymentCorrelationId, CKYCLink)    
   VALUES(@QuoteTransactionID,@InsurerId,@ApplicationId,@ProposalNumber,@UserId,@LeadId,@Amount,@PolicyNumber,@CustomerId,@PaymentTransactionNumber,@PaymentCorrelationId, @CKYCLink)    
       
   --IF(@BreakinId IS NOT NULL AND @InsurerId != '85F8472D-8255-4E80-B34A-61DB8678135C')    
   --BEGIN    
   -- UPDATE Insurance_LeadDetails SET StageId='405F4696-CDFB-4065-B364-9410B56BC78D', BreakinId = @BreakinId,     
   -- PaymentLink = @PaymentLink, UpdatedBy=@UserId, UpdatedOn = GETDATE(), BreakinInspectionURL = @BreakinInspectionURL    
   -- WHERE LeadId=@LeadId    
   --END    
   --ELSE    
   --BEGIN    
   -- UPDATE Insurance_LeadDetails SET BreakinId = @BreakinId,  --need to remove these code only should update paymentlink   
   -- PaymentLink = @PaymentLink, UpdatedBy=@UserId, UpdatedOn = GETDATE()     
   -- WHERE LeadId=@LeadId    
   --END    
  END    
  ELSE IF(@Type='UPDATE')    
  BEGIN   
  
  IF(@InsurerId = 'DC874A12-6667-41AB-A7A1-3BB832B59CEB') -- FOR UIIC Customer id Shuld not be uodated during update payment status
  BEGIN
	   UPDATE Insurance_PaymentTransaction SET Status = @Status, PaymentTransactionNumber =@PaymentTransactionNumber,    
     CKYCStatus = @CKYCStatus, CKYCLink = @CKYCLink, CKYCFailReason=@CKYCFailReason, PolicyDocumentLink=@PolicyDocumentLink, PolicyNumber = @PolicyNumber,    
     DocumentId=@DocumentId, UpdatedBy=@UserId,UpdatedOn=getdate(), IsTP = @IsTP, BankName = @BankName, BankPaymentRefNum = @BankPaymentRefNum, PaymentDate = @PaymentDate    
     WHERE ApplicationId = @ApplicationId 
  END
  ELSE
  BEGIN
	   UPDATE Insurance_PaymentTransaction SET Status = @Status, PaymentTransactionNumber =@PaymentTransactionNumber,    
     CKYCStatus = @CKYCStatus, CKYCLink = @CKYCLink, CKYCFailReason=@CKYCFailReason, PolicyDocumentLink=@PolicyDocumentLink, PolicyNumber = @PolicyNumber, CustomerId = @CustomerId,    
     DocumentId=@DocumentId, UpdatedBy=@UserId,UpdatedOn=getdate(), IsTP = @IsTP, BankName = @BankName, BankPaymentRefNum = @BankPaymentRefNum, PaymentDate = @PaymentDate    
     WHERE ApplicationId = @ApplicationId 
  END   
    
   SELECT @LeadId = LeadId FROM Insurance_PaymentTransaction WITH(NOLOCK) WHERE ApplicationId = @ApplicationId    
   UPDATE Insurance_LeadDetails SET StageId='D07216AF-ACAD-4EEA-8CFF-2910BA77E5EE', PolicyNumber = @PolicyNumber, UpdatedOn = GETDATE() WHERE LeadId=@LeadId    
  END    
  
    
  SELECT PaymentTransactionId,    
     PAY.QuoteTransactionId,    
    ApplicationId,    
    PAY.LeadId,    
    LeadName, --LEAD.LeadName+' '+LEAD.MiddleName+' '+LEAD.LastName AS LeadName,    
    ProposalNumber,    
    PaymentTransactionNumber,    
    Amount,    
    Status PaymentStatus,    
    CKYCStatus,    
    CKYCLink,    
    CKYCFailReason CKYCFailedReason,    
    PolicyDocumentLink,    
    DocumentId,    
    PAY.PolicyNumber,    
    CustomerId,    
    PAY.InsurerId,    
    INS.Logo    
  FROM Insurance_PaymentTransaction PAY WITH(NOLOCK)     
  LEFT JOIN Insurance_Insurer INS WITH(NOLOCK) ON PAY.InsurerId = INS.InsurerId     
  LEFT JOIN Insurance_LeadDetails LEAD WITH(NOLOCK) ON PAY.LeadId=LEAD.LeadId    
  WHERE ApplicationId=@ApplicationId    
    
 END TRY          
 BEGIN CATCH                     
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                  
 END CATCH          
END 