      
      
CREATE PROCEDURE [dbo].[Insurance_InsertQuoteTransaction]      
@InsurerId VARCHAR(50) = NULL,      
@ResponseBody VARCHAR(max) = NULL,       
@RequestBody VARCHAR(max) = NULL,       
@CommonResponse VARCHAR(max) = NULL,
@Stage varchar(20) = null,
@LeadId NVARCHAR(50) = null,
@MaxIDV decimal(18,2) = null,
@MinIDV decimal(18,2) = null,
@RecommendedIDV decimal(18,2) = null,
@TransactionId VARCHAR(100) = NULL
AS      
BEGIN      
 BEGIN TRY      
  
 DECLARE @QuoteTranID varchar(50) = null, @StageID varchar(50) = (select stageid from Insurance_StageMaster WITH(NOLOCK) where stage= @Stage) ;  
 SET @QuoteTranID = NEWID()  
 INSERT INTO dbo.Insurance_QuoteTransaction      
 (QuoteTransactionId, InsurerId,ResponseBody,RequestBody,CommonResponse,CreatedBy,StageID,LeadId,MaxIDV,MinIDV,RecommendedIDV,TransactionId)       
 VALUES (@QuoteTranID,@InsurerId,@ResponseBody,@RequestBody,@CommonResponse,'1',@StageID, @LeadId,@MaxIDV,@MinIDV,@RecommendedIDV, @TransactionId) 
 
 update Insurance_LeadDetails set StageId = @StageID, UpdatedOn = GETDATE() where LeadId = @LeadId
    
 select Logo,SelfVideoClaims,SelfDescription,IsRecommended,RecommendedDescription from Insurance_Insurer WITH(NOLOCK)  where InsurerId = @InsurerId      
 SELECT GarageId,WorkshopName,FullAddress,City,State,Pincode,Latitude,Longitude,ProductType,EmailId,MobileNumber,ContactPerson      
 FROM  Insurance_CashlessGarage WITH(NOLOCK)       
 WHERE insurerId=@InsurerId       
      
 SELECT Id AS PremiumBasicDetailsId,Title       
 FROM Insurance_PremiumBasicDetail WITH(NOLOCK) WHERE insurerId=@InsurerId       
      
 SELECT PremiumBasicDetailId,Id as SubtitleId,Subtitle, Description, Icon      
 FROM Insurance_PremiumBasicSubtitleDetail WITH(NOLOCK) WHERE insurerId=@InsurerId       
      
 SELECT @QuoteTranID QuoteTransactionId  
  
 END TRY      
 BEGIN CATCH                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                              
 END CATCH      
END 