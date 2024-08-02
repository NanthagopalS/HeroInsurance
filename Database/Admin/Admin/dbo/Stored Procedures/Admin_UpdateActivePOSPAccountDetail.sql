  
CREATE PROCEDURE [dbo].[Admin_UpdateActivePOSPAccountDetail]         
(        
 @POSPUserId VARCHAR(200),  
 @PreSaleUserId VARCHAR(200),  
 @PostSaleUserId VARCHAR(200),  
 @MarketingUserId VARCHAR(200),  
 @ClaimUserId VARCHAR(200),
 @SourcedBy VARCHAR(200),  
 @CreatedBy VARCHAR(200),  
 @ServicedBy VARCHAR(200)  
)        
AS        
BEGIN  
  
 BEGIN TRY        
   
 Update [HeroIdentity].[dbo].[Identity_UserDetail]   
 Set  PreSale = @PreSaleUserId, PostSale = @PostSaleUserId, Marketing = @MarketingUserId,  
 Claim = @ClaimUserId, POSPSourceTypeId = @SourcedBy, CreatedBy = @CreatedBy, ServicedByUserId = @ServicedBy, CreatedOn= GETDATE()  
 where UserId = @POSPUserId  
  
 END TRY                        
  
  
 BEGIN CATCH                
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
        
END 
