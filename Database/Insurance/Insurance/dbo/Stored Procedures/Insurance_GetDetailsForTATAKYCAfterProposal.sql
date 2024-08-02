-- ==========================================================================================   
-- Author:  <Author,Firoz S>    
-- Create date: <Create Date,13-10-2023,>    
-- Description: <Description,Need to get the details from db which stored during the proposal to perform kyc,>    
-- ==========================================================================================   
CREATE PROCEDURE [dbo].[Insurance_GetDetailsForTATAKYCAfterProposal]    
@QuoteTransactionId VARCHAR(50) = NULL,    
@InsurerId VARCHAR(50) = NULL    
AS    
BEGIN     
 BEGIN TRY    
  SELECT LEAD.LeadId AS LeadID,  
  LEAD.PolicyNumber,  
  LEAD.LeadName + ' '+LEAD.LastName AS LeadName,  
  LEAD.DOB,  
  LEAD.Gender,  
  LEAD.CompanyName,
  CKYC.PhotoId AS KYC_RequestId  
  FROM Insurance_LeadDetails LEAD WITH(NOLOCK)   
  INNER JOIN Insurance_CKYCTransaction CKYC WITH(NOLOCK) ON CKYC.InsurerId = LEAD.InsurerId   
  AND CKYC.QuoteTransactionId = LEAD.QuoteTransactionID  
  WHERE LEAD.InsurerId = @InsurerId AND LEAD.QuoteTransactionId = @QuoteTransactionId    
  ORDER BY CKYC.CreatedOn DESC  
 END TRY          
 BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH       
END