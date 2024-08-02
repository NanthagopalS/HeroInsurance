      
      
CREATE   PROCEDURE [dbo].[Insurance_InsertICICIPaymentTransaction]      
@InsurerId VARCHAR(50) = NULL,      
@ResponseBody VARCHAR(max) = NULL,       
@RequestBody VARCHAR(max) = NULL,       
@Stage varchar(20) = null,
@TransactionId VARCHAR(100) = NULL
AS      
BEGIN      
 BEGIN TRY      
  
 DECLARE @QuoteTranID VARCHAR(50) = null, @StageID VARCHAR(50) = (SELECT stageid FROm Insurance_StageMaster WITH(NOLOCK) WHERE stage= @Stage);  
 SET @QuoteTranID = NEWID()  
 INSERT INTO dbo.Insurance_QuoteTransaction
 (QuoteTransactionId, InsurerId,ResponseBody,RequestBody,CreatedBy,StageID,TransactionId)       
 VALUES (@QuoteTranID,@InsurerId,@ResponseBody,@RequestBody,'1',@StageID, @TransactionId) 

  
 END TRY      
 BEGIN CATCH                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
 END CATCH      
END