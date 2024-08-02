              
              
              
-- =============================================              
-- Author:  <Author,,VISHAL KANJARIYA>              
-- Create date: <Create Date,,02-DEC-2022>              
-- Description: <Description,,UPDATE USER BANK DETAIL>              
-- =============================================              
CREATE        PROCEDURE [dbo].[Identity_UpdateUserBankDetail]               
(              
 @UserId VARCHAR(100) = NULL,              
 @BankId VARCHAR(100) = NULL,              
 @IFSC VARCHAR(20) = NULL,              
 @AccountHolderName VARCHAR(100) = NULL,              
 @AccountNumber VARCHAR(20) = NULL,            
 @IsDraft bit = 0  ,          
 @IsAdminUpdating bit = 0          
)              
AS              
BEGIN              
 BEGIN TRY              
                
  IF EXISTS(SELECT DISTINCT Id FROM [dbo].[Identity_UserBankDetail] WITH(NOLOCK) WHERE UserId = @UserId)              
  BEGIN              
   UPDATE [dbo].[Identity_UserBankDetail] SET               
    BankId = @BankId,              
    IFSC = @IFSC,              
    AccountHolderName = @AccountHolderName,              
    AccountNumber = @AccountNumber , UpdatedOn = GETDATE()             
   WHERE              
    UserId = @UserId              
  END              
  ELSE              
  BEGIN              
   INSERT INTO [dbo].[Identity_UserBankDetail] (UserId, BankId, IFSC, AccountHolderName, AccountNumber)               
   VALUES               
   (@UserId, @BankId, @IFSC, @AccountHolderName, @AccountNumber)              
  END                
              
              
    --UPDATE PROFILE STAGE FOR USERS            
 IF(@IsDraft = 1 AND @IsAdminUpdating = 0)               
 BEGIN                
  UPDATE Identity_User SET UserProfileStage =2, UpdatedOn = GETDATE() WHERE UserId=@UserId        
   EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserId, '8A2010DF-0137-4ED0-BDA6-6A8DF9128827'    
 END           
        
       
  IF(@IsDraft!=1 and @IsAdminUpdating = 0)        
   BEGIN        
    UPDATE Identity_User SET UserProfileStage = 3, UpdatedOn = GETDATE() WHERE UserId = @UserId     
 EXEC HeroPosp.dbo.POSP_InsertUpdatePOSPStage @UserId, '2955D6F3-7B6C-4EB0-8678-A80F5B8A0047'  
   END        
                 
              
  SELECT USR.MOBILENO               
  FROM [Identity_User] USR WITH(NOLOCK)              
  JOIN [Identity_UserBankDetail] BANK WITH(NOLOCK) ON USR.UserId=BANK.UserId              
  WHERE AccountNumber = @AccountNumber              
                
 END TRY                              
 BEGIN CATCH                        
                     
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                          
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                          
  SET @ErrorDetail=ERROR_MESSAGE()                                          
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList              
                
 END CATCH              
              
END
