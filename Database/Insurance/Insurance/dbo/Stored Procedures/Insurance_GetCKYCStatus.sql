-- =============================================        
-- Author:  <Author,,Parth Gandhi>        
-- Create date: <Create Date,5-May-2023>        
-- Description: <Description,Getting CKYC Status Data>        
-- [Insurance_GetCKYCStatus] '0A326B77-AFD5-44DA-9871-1742624CFF16','B0E679C4-90AA-4E6A-99ED-14A584F0B57D'      
-- =============================================        
CREATE   PROCEDURE [dbo].[Insurance_GetCKYCStatus]          
@InsurerID varchar(50),         
@QuoteTransactionId varchar(50)      
AS        
BEGIN         
BEGIN TRY          
BEGIN        
 SELECT kycTran.InsurerId,    
  CASE  
    WHEN insurer.InsurerName = 'HDFC ERGO General Insurance Co. Ltd.' THEN 'HDFC ERGO'  
    ELSE insurer.InsurerName  
  END AS InsurerName,  
  insurer.Logo AS Logo,  
  leadDetails.LeadName as [Name],      
  leadDetails.DOB,      
  leadDetails.Gender,      
  kycTran.CKYCStatus as KYCStatus,      
  kycTran.KYCId,      
  kycTran.CKYCNumber as KYCNumber,      
  ISNULL(leadAddress.Address1,'') + ISNULL(leadAddress.Address2,'') + ISNULL(leadAddress.Address3,'') as [Address]       
 from [Insurance_CKYCTransaction] kycTran with (NOLOCK)      
 LEFT JOIN [Insurance_LeadDetails] leadDetails with (NOLOCK) on kycTran.LeadId = leadDetails.LeadId      
 LEFT JOIN [Insurance_Insurer] insurer with (NOLOCK) on kycTran.InsurerId = insurer.InsurerId      
 LEFT JOIN [Insurance_LeadAddressDetails] leadAddress with (NOLOCK) on leadAddress.LeadID = leadDetails.LeadId      
 WHERE kycTran.InsurerId = @InsurerID AND kycTran.QuoteTransactionId = @QuoteTransactionId      
 ORDER BY kycTran.CreatedOn desc    
END          
END TRY                  
 BEGIN CATCH                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
END