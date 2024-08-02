-- =============================================          
-- Author: <Author, ANKIT GHOSH>          
-- Create date: <Create Date, 31-March-2023>          
-- Description: <Description,[POSP_InsertReferralNewUserDetails]>          
--[POSP_InsertReferralNewUserDetails]          
-- =============================================          
CREATE     PROCEDURE [dbo].[POSP_InsertReferralNewUserDetails]          
(                
 @ReferralMode VARCHAR(100) = Null,          
 @UserName VARCHAR(100) = Null,          
 @EmailId VARCHAR(100) = Null,          
 @PhoneNumber VARCHAR(100) = Null,    
 @ReferralUserId VARCHAR(100) = Null    
 )           
AS          
BEGIN          
 BEGIN TRY          
       
   IF EXISTS(SELECT top 1 ReferralNewUserId FROM [POSP_ReferralNewUserDetails] WITH(NOLOCK) WHERE EmailId = @EmailId)    
   BEGIN    
  SELECT 1 as IsEmailExist,'Referral is already on this mail' as ErrorMessage    
   END    
   ELSE IF EXISTS(SELECT top 1 ReferralNewUserId FROM [POSP_ReferralNewUserDetails] WITH(NOLOCK) WHERE PhoneNumber = @PhoneNumber)    
   BEGIN    
  SELECT 1 as IsMobileExist,'Referral is already on this mobile number' as ErrorMessage    
   END   
  -- ELSE IF EXISTS(SELECT top 1 UserId FROM [HeroIdentity].[dbo].[Identity_User] WHERE EmailId = @EmailId)    
  -- BEGIN    
  --SELECT 1 as IsEmailExist,'Not refer to own email' as ErrorMessage    
  -- END  
  -- ELSE IF EXISTS(SELECT top 1 UserId FROM [HeroIdentity].[dbo].[Identity_User] WHERE MobileNo = @PhoneNumber)    
  -- BEGIN    
  --SELECT 1 as IsEmailExist,'Not refer to own mobile number' as ErrorMessage    
  -- END  
   ELSE    
   BEGIN    
   Insert into [dbo].[POSP_ReferralNewUserDetails] (UserName, EmailId, PhoneNumber,RefererralMode,ReferralUserId)         
   values (@UserName, @EmailId, @PhoneNumber,@ReferralMode,@ReferralUserId)          
       
   SELECT TOP 1 RD.ReferralNewUserId as ReferralId,IU.UserName as POSPName, IU.POSPId as POSPId     
   FROM [dbo].[POSP_ReferralNewUserDetails] as RD WITH(NOLOCK)    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK) on IU.UserId = RD.ReferralUserId    
   ORDER BY RD.CreatedOn desc          
  END        
  
 END TRY                          
 BEGIN CATCH                    
                 
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList          
 END CATCH          
END 