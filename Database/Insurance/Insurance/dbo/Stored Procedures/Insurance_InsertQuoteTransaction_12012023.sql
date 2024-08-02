  
  
CREATE   PROCEDURE [dbo].[Insurance_InsertQuoteTransaction_12012023]  
@InsurerId VARCHAR(50) = NULL,  
@ResponseBody VARCHAR(max) = NULL,   
@RequestBody VARCHAR(max) = NULL,   
@CommonResponse VARCHAR(max) = NULL  
AS  
BEGIN  
 BEGIN TRY  
  INSERT INTO dbo.Insurance_QuoteTransaction  
  (InsurerId,ResponseBody,RequestBody,CommonResponse,CreatedBy)   
  VALUES (@InsurerId,@ResponseBody,@RequestBody,@CommonResponse,'1')  
  
  select Logo,SelfVideoClaims,SelfDescription from Insurance_Insurer WITH(NOLOCK) where InsurerId = @InsurerId  
  
  SELECT GarageId,WorkshopName,FullAddress,City,State,Pincode,Latitude,Longitude,ProductType,EmailId,MobileNumber,ContactPerson  
  FROM  Insurance_CashlessGarage WITH(NOLOCK)   
  WHERE insurerId=@InsurerId   
  
  SELECT Id AS PremiumBasicDetailsId,Title   
  FROM Insurance_PremiumBasicDetail WITH(NOLOCK) WHERE insurerId=@InsurerId   
  
  SELECT PremiumBasicDetailId,Id as SubtitleId,Subtitle, Description, Icon  
  FROM Insurance_PremiumBasicSubtitleDetail WITH(NOLOCK) WHERE insurerId=@InsurerId   
  
 END TRY  
 BEGIN CATCH             
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC dbo.Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                          
 END CATCH  
END  