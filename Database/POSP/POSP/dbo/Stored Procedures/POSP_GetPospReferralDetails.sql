    
CREATE     PROCEDURE [dbo].[POSP_GetPospReferralDetails]    
(    
 @UserId VARCHAR(500)    
)    
AS    
BEGIN    
 BEGIN TRY     
 Select RefererralTypeId, RefererralType, ImageURL, ReferralBaseURL +  @UserId as ReferralBaseURL, PriorityIndex   
 from [HeroPOSP].[dbo].[POSP_ReferralType] WITH(NOLOCK)  
 
   
 Select ReferralModeId, ReferralModeType, ImageUrl, PriorityIndex from [HeroPOSP].[dbo].[POSP_ReferralMode] WITH(NOLOCK)   
    
 Select ReferralId,POSPId from [HeroIdentity].[dbo].[Identity_User] WITH(NOLOCK) Where UserId = @UserId    
    
    
 --INSERT into POSP_ReferralDetails(RefererralTypeId, ReferralModeId, IsActive, CreatedOn, POSPUserId)    
 --  SELECT RT.RefererralTypeId, RM.ReferralModeId, 1, GETDATE(), USERID    
 --  FROM POSP_ReferralType RT    
 --  CROSS JOIN POSP_ReferralMode RM    
     
    
  END TRY      
      
 BEGIN CATCH                      
                   
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                        
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                        
  SET @ErrorDetail=ERROR_MESSAGE()                                        
  EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                         
 END CATCH       
END    