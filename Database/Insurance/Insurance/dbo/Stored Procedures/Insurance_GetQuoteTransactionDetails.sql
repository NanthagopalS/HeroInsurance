 --[dbo].[Insurance_GetQuoteTransactionDetails]  'D1ED85DA-7243-403F-AE22-E4C840E416F2'      
CREATE    PROCEDURE [dbo].[Insurance_GetQuoteTransactionDetails]        
@TransactionID VARCHAR(50)        
AS        
BEGIN        
     
 DECLARE @leadid VARCHAR(50), @vehicleNumber VARCHAR(20), @SATPInsurer VARCHAR(100), @InsurerId VARCHAR(50), @SAODInsurerName VARCHAR(100), @SAODInsurerCode VARCHAR(50),@SATPInsurerName VARCHAR(100), @SATPInsurerCode VARCHAR(50), @PreviousSATPInsurer VARCHAR(50)  , @FinancierName VARCHAR(100) = NULL, 
 @FinancierBranchCode VARCHAR(100) = NULL,  @FinancierBranch VARCHAR(100) = NULL,  @FinancierAddress VARCHAR(100) = NULL    
    
 SELECT @leadid=LeadId, @InsurerId = InsurerId FROM [dbo].[Insurance_QuoteTransaction] WITH(NOLOCK)     
 WHERE QuoteTransactionId=@TransactionID        
     
 SELECT @PreviousSATPInsurer = PreviousSATPInsurer FROM [dbo].[Insurance_LeadDetails] WITH(NOLOCK) WHERE LeadId=@leadid    
 IF(ISNULL( @PreviousSATPInsurer, '' ) = '')    
 BEGIN    
  SELECT @PreviousSATPInsurer = NULL -- AS INSURERID IS UNIQUE IDENTIFIER CANNOT CHECK FOR EMPTY CHARACTER    
 END    
    
 SELECT @SATPInsurer = InsurerName FROM Insurance_Insurer WITH(NOLOCK)     
 WHERE InsurerId = @PreviousSATPInsurer    
      
 IF(@InsurerId = 'DC874A12-6667-41AB-A7A1-3BB832B59CEB') --For United india financier name and code required  
 BEGIN    
 SELECT @FinancierName = FIN.[Financier Name] , @FinancierBranchCode = FIN.[Financier Code]  
 FROM MOTOR.UIIC_FinancierMaster FIN  
 INNER JOIN Insurance_LeadDetails LEADS   
 ON LEADS.LoadProvidedCompany = FIN.[Financier Code]  
 WHERE LEADS.LeadId = @leadid

 SELECT @FinancierBranch = BranchName ,@FinancierAddress = BranchAddress
 FROM [MOTOR].[UIIC_FinancierBranchMaster] BRANCH
 INNER JOIN Insurance_LeadDetails LEADS   
 ON LEADS.LoadProvidedCity = BRANCH.FinancierBranchCode
 WHERE LEADS.LeadId = @leadid

 END   
  
 SELECT InsurerId,RequestBody,ResponseBody,CommonResponse,ProposalId,TransactionId,PolicyId, @SATPInsurer PreviousSATPInsurerName, @FinancierName FinancierName, @FinancierAddress FinancierAddress, @FinancierBranch FinancierBranch    
 FROM [dbo].[Insurance_QuoteTransaction] WITH(NOLOCK)     
 WHERE QuoteTransactionId=@TransactionID        
      
     
 SELECT * FROM [dbo].[Insurance_LeadDetails] WITH(NOLOCK) WHERE LeadId=@leadid       
    
 SELECT @vehicleNumber = VehicleNumber FROM [dbo].[Insurance_LeadDetails] WITH(NOLOCK) WHERE LeadId=@leadid        
       
 SELECT  ProposalRequestBody FROM Insurance_ProposalDynamicDetails WITH(NOLOCK) WHERE QuoteTransactionId=@TransactionID        
      
 SELECT CKYCRequestBody FROM Insurance_CKYCTransaction WITH(NOLOCK) WHERE QuoteTransactionId= @TransactionID      
      
 SELECT vehicleCubicCapacity AS VehicleCubicCapacity FROM Insurance_VehicleRegistration WITH(NOLOCK) WHERE vehicleNumber=@vehicleNumber    
     
 SELECT TOP 1 KYCId FROM Insurance_CKYCTransaction WITH(NOLOCK) WHERE QuoteTransactionId= @TransactionID ORDER BY CreatedOn DESC    
    
 IF(@InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C') --FOR TATA NEED PREVIOUS INSUERE DETAILS FROM TATA MASTER FOR PROPOSAL    
 BEGIN    
  SELECT @SAODInsurerCode = Code ,@SAODInsurerName = Name FROM MOTOR.TATA_PrevInsuranceMaster    
  WHERE INSURERID = (SELECT PreviousSAODInsurer FROM Insurance_LeadDetails WHERE LeadId = @leadid AND InsurerId = @InsurerId)    
    
  SELECT @SATPInsurerCode = Code ,@SATPInsurerName = Name FROM MOTOR.TATA_PrevInsuranceMaster    
  WHERE INSURERID = (SELECT PreviousSATPInsurer FROM Insurance_LeadDetails WHERE LeadId = @leadid AND InsurerId = @InsurerId)    
 END    
 SELECT @SAODInsurerCode SAODInsurerCode,@SAODInsurerName PreviousSAODInsurerName,@SATPInsurerCode SATPInsurerCode,@SATPInsurerName PreviousSATPInsurerName    
END  
  
  