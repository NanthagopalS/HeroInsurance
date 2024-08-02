
  
  
-- =============================================  
-- Author:  <Author, AMBI GUPTA>  
-- Create date: <Create Date,29-NOV-2022>  
-- Description: <Description,,INSERT USER DETAIL>  
--Identity_InsertUser 'ambi.gupta','a.a@a.com','9987848972'  
-- =============================================  
CREATE       PROCEDURE [dbo].[Identity_InsertUser1]   
(  
 @UserName VARCHAR(100) = NULL,  
 @EmailId VARCHAR(100) = NULL,  
 @MobileNo VARCHAR(10) = NULL,  
 @BackOfficeUserId VARCHAR(100) = NULL,
 @ReferralUserId VARCHAR(100) = NULL
)  
AS  
BEGIN  
 BEGIN TRY  
    
  DECLARE @UserId VARCHAR(50) = NULL, @RoleId VARCHAR(50) = NULL, @StatusId varchar(50) = NULL  
  DECLARE @CodePatternPOSPLead VARCHAR(50) = NULL, @POSPLeadId INT = 0, @CodePatternPOSPId VARCHAR(50) = NULL, @POSPId INT = 0    
  DECLARE @FinalCodePOSPLead VARCHAR(50) = NULL, @FinalCodePOSPId VARCHAR(50) = NULL  
  
  DECLARE @CreatedByMode VARCHAR(20) = 'Self'  
  
  If(@BackOfficeUserId IS NOT NULL OR @BackOfficeUserId <> '')  
  BEGIN  
   SET @CreatedByMode = 'Assisted'  
  END  
  
  --For POSP Lead Id   
  SET @CodePatternPOSPLead = (SELECT CodePattern FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WITH(NOLOCK) WHERE [Code] = 'POSPL' AND IsActive = 1)  
  
  SET @POSPLeadId = (SELECT NextValue FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WITH(NOLOCK) WHERE [Code] = 'POSPL' AND IsActive = 1)  
  
  SET @FinalCodePOSPLead = CONCAT(@CodePatternPOSPLead, CAST(@POSPLeadId AS VARCHAR))  
  
  --For POSP Id   
  --SET @CodePatternPOSPId = (SELECT CodePattern FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WHERE [Code] = 'POSP' AND IsActive = 1)  
  
  --SET @POSPId = (SELECT NextValue FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WHERE [Code] = 'POSP' AND IsActive = 1)  
  
  --SET @FinalCodePOSPId = CONCAT(@CodePatternPOSPId, CAST(@POSPId AS VARCHAR))  
  
  IF EXISTS(SELECT TOP 1 MobileNo FROM Identity_User WITH(NOLOCK) WHERE MobileNo = @MobileNo)  
  BEGIN  
   SELECT USERID, 1 IsUserExists FROM Identity_User WITH(NOLOCK) WHERE MobileNo = @MobileNo  
  END  
  ELSE  
  BEGIN  
   SELECT @RoleId = ROLEID FROM Identity_RoleMaster WITH(NOLOCK) WHERE RoleName = 'POSP'  
  
   INSERT INTO Identity_User (UserName, EmailId, MobileNo, RoleId, POSPLeadId, POSPId, CreatedByMode,ReferralId)  
   VALUES(@UserName, @EmailId, @MobileNo, @RoleId, @FinalCodePOSPLead, @FinalCodePOSPId, @CreatedByMode, @ReferralUserId)  
  
   SET @POSPLeadId = @POSPLeadId + 1  
  
   --SET @POSPId = @POSPId + 1  
  
   UPDATE [HeroIdentity].[dbo].[Identity_AutoGenerateId] SET NextValue = @POSPLeadId WHERE [Code] = 'POSPL' AND IsActive = 1  
  
   --UPDATE [HeroIdentity].[dbo].[Identity_AutoGenerateId] SET NextValue = @POSPId WHERE [Code] = 'POSP' AND IsActive = 1  
     
   SET @UserId = (SELECT USERId FROM Identity_User WITH(NOLOCK) WHERE MobileNo = @MobileNo)  
  
   ---Manage Breadcrumb START      
  
   INSERT INTO Identity_UserBreadcrumStatusDetail (StatusId, UserId, CreatedBy)  
   SELECT Id as StatusId, @UserId, @UserId FROM Identity_UserBreadcrumStatusMaster WITH(NOLOCK)  WHERE PriorityIndex in (1, 2, 5, 8) order by PriorityIndex  
  
   ---Manage Breadcrumb END  
  
   --Insert the master refferal details of the User Start  
  
   INSERT into [HeroPOSP].[dbo].[POSP_ReferralDetails](RefererralTypeId, ReferralModeId, IsActive, CreatedOn, POSPUserId)  
   SELECT RT.RefererralTypeId, RM.ReferralModeId, 1, GETDATE(), @UserId  
   FROM [HeroPOSP].[dbo].[POSP_ReferralType] RT WITH(NOLOCK) 
   CROSS JOIN [HeroPOSP].[dbo].[POSP_ReferralMode] RM WITH(NOLOCK) 
  
   --Insert the master refferal details of the User End  
  
   SELECT USERID, 0 IsUserExists FROM Identity_User WITH(NOLOCK) WHERE MobileNo = @MobileNo  
  END  
  
 END TRY                  
 BEGIN CATCH            
         
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  END