-- =============================================  
-- Author:  <Author,,FIROZ S>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
--exec  [dbo].[Insurance_GetPolicyDocumentDetails]  '0A326B77-AFD5-44DA-9871-1742624CFF16','854987DE-B717-4F4C-AED7-5F1BC9151'  
-- =============================================  
  
CREATE PROCEDURE [dbo].[Insurance_GetPolicyDocumentDetails]    
@InsurerId VARCHAR(50) = NULL,  
@QuotetransactionId VARCHAR(50)= NULL  
  
AS  
BEGIN   
 BEGIN TRY  
  DECLARE @TransactionId VARCHAR(50), @LeadId VARCHAR(50), @VehicleTypeId VARCHAR(50), @PolicyTypeId VARCHAR(50), @RequestBody NVARCHAR(MAX),  @EngineNumber VARCHAR(50), @VehicleNumber VARCHAR(50),  @CategoryId INT
    
  SELECT @LeadId=LeadId, @TransactionId = TransactionId   
  FROM Insurance_QuoteTransaction WITH(NOLOCK)   
  WHERE QuoteTransactionId = @QuotetransactionId  
  
  SELECT @VehicleTypeId = VehicleTypeId,@PolicyTypeId = PolicyTypeId, @EngineNumber = EngineNumber, @VehicleNumber = VehicleNumber    
  FROM Insurance_LeadDetails WITH(NOLOCK) WHERE LeadId = @LeadId  
  
  SELECT @CategoryId = VehicleCategoryId FROM Insurance_CommercialLeadDetail WITH(NOLOCK) WHERE LeadId = @LeadId 

  IF(@InsurerId = 'FD3677E5-7938-46C8-9CD2-FAE188A1782C')  
  BEGIN  
   SELECT @RequestBody = RequestBody FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE InsurerId = @InsurerId 
   AND TransactionId = (SELECT ApplicationId FROM Insurance_PaymentTransaction WITH(NOLOCK) 
   WHERE InsurerId=@InsurerId AND QuoteTransactionId=@QuotetransactionId) AND StageID = 'ADB9EB9C-CB73-4DE3-BAF7-151F90C2A6F2'    
  END  
  
  SELECT  PaymentTransactionId,  
  QuoteTransactionId,  
  ApplicationId,  
  LeadId,  
  ProposalNumber,  
  PolicyNumber,  
  PaymentTransactionNumber,  
  Amount,  
  Status PaymentStatus,  
  CKYCStatus,  
  CKYCLink,  
  CKYCFailReason CKYCFailedReason,  
  PolicyDocumentLink,  
  DocumentId,  
  IsTP,  
  BankName,  
  PaymentDate,  
  CustomerId,  
  @RequestBody RequestBody,  
  CreatedBy UserId
  FROM Insurance_PaymentTransaction Payment WITH(NOLOCK)   
  WHERE InsurerId=@InsurerId AND QuoteTransactionId=@QuotetransactionId  
  
  SELECT @TransactionId AS TransactionId,  
  @VehicleTypeId AS VehicleTypeId,  
  @PolicyTypeId AS PolicyTypeId,  
  @EngineNumber AS EngineNumber,  
  @VehicleNumber AS VehicleNumber,
  @CategoryId AS CVCategoryId
 END TRY     
   
BEGIN CATCH                    
        
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH     
END