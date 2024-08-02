CREATE PROCEDURE [dbo].[Insurance_UpdateBreakinDetails]   
@LeadId VARCHAR(50) = NULL,  
@QuoteTransactionID VARCHAR(50) = NULL,  
@PolicyNumber VARCHAR(50) = NULL,  
@PaymentLink NVARCHAR(MAX) = NULL,  
@Stage VARCHAR(50) = NULL,  
@BreakInStatus Bit = NULL,
@InsurerId  VARCHAR(50) = NULL
AS        
BEGIN        
 BEGIN TRY        

  IF(ISNULL(@LeadId,'') ='')  
  BEGIN  
   SELECT @LeadId = LeadId FROM Insurance_QuoteTransaction WITH(NOLOCK) WHERE QuoteTransactionID = @QuoteTransactionID  
  END 
  
  UPDATE Insurance_LeadDetails SET PolicyNumber = @PolicyNumber,  
  PaymentLink = @PaymentLink,  
  StageId = (SELECT StageID FROM Insurance_StageMaster WITH(NOLOCK) WHERE Stage = @Stage),  
  IsBreakinApproved = @BreakInStatus, UpdatedOn = GETDATE()  
  WHERE LeadId = @LeadId  

  IF(@InsurerId IN('85F8472D-8255-4E80-B34A-61DB8678135C','78190CB2-B325-4764-9BD9-5B9806E99621','16413879-6316-4C1E-93A4-FF8318B14D37') AND @Stage = 'Proposal')
  BEGIN	
	UPDATE Insurance_QuoteTransaction SET StageID ='ADB9EB9C-CB73-4DE3-BAF7-151F90C2A6F2'
	WHERE InsurerId = @InsurerId AND QuoteTransactionId = @QuoteTransactionID
  END
  
  SELECT LeadId FROM Insurance_LeadDetails WITH(NOLOCK) WHERE QuoteTransactionId=@QuoteTransactionID  
  
 END TRY        
 BEGIN CATCH                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                
 END CATCH        
END