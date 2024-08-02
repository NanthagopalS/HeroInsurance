CREATE  
    
  
 PROCEDURE [dbo].[POSP_InsertAgreementDocument] (  
 @UserId VARCHAR(100) = NULL  
 ,@AgreementId VARCHAR(100) = NULL  
 ,@ProcessType VARCHAR(50) = NULL  
 )  
AS  
BEGIN  
 DECLARE @StatusId VARCHAR(500) = NULL  
  ,@StampId VARCHAR(100) = NULL  
  
 BEGIN TRY  
  IF (@ProcessType = 'Agreement')  
  BEGIN  
   SET @StatusId = (  
     SELECT Id  
     FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster] WITH (NOLOCK)  
     WHERE StatusName = 'Agreement Sign'  
      AND PriorityIndex = 10  
     )  
  
   --UPDATE Breadcrum stage for PSOP User              
   UPDATE [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusDetail]  
   SET StatusId = @StatusId, UpdatedOn = GETDATE()  
   WHERE UserId = @UserId  
    AND StatusId IN (  
     SELECT Id  
     FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster] WITH (NOLOCK)  
     WHERE StatusName = 'Agreement Sign'  
      AND PriorityIndex IN (  
       8  
       ,9  
       )  
     )  
  
   IF EXISTS (  
     SELECT TOP (1) [Id]  
     FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH (NOLOCK)  
     WHERE UserId = @UserId  
      AND IsActive = 1  
     )  
   BEGIN  
    UPDATE [HeroPOSP].[dbo].[POSP_Agreement]  
    SET AgreementId = @AgreementId  
     ,UpdatedOn = GETDATE()  
    WHERE UserId = @UserId  
     AND IsActive = 1  
   END  
   ELSE  
   BEGIN  
    INSERT INTO [HeroPOSP].[dbo].[POSP_Agreement] (  
     UserId  
     ,AgreementId  
     ,UpdatedOn  
     )  
    VALUES (  
     @UserId  
     ,@AgreementId  
     ,GETDATE()  
     )  
   END  
  
   SET @StampId = (  
     SELECT TOP (1) StampId  
     FROM [HeroPOSP].[dbo].[POSP_Agreement]  
     WHERE UserId = @UserId  
      AND IsActive = 1  
     )  
  
   UPDATE [HeroAdmin].[dbo].[Admin_StampData]  
   SET StampStatus = 'Used', UpdatedOn = GETDATE()  
   WHERE Id = @StampId  
  
   --UPDATE PROFILE STAGE FOR USERS                  
   UPDATE [HeroIdentity].[dbo].[Identity_User]  
   SET UserProfileStage = 5, UpdatedOn = GETDATE()  
   WHERE UserId = @UserId  

   EXEC HeroPOSP.dbo.POSP_InsertUpdatePOSPStage @UserId, '769D2D45-C4A2-4205-9CC4-CE32D1C6415E'

  END  
  ELSE  
  BEGIN  
   IF EXISTS (  
     SELECT TOP (1) [Id]  
     FROM [HeroPOSP].[dbo].[POSP_Agreement]  
     WHERE UserId = @UserId  
      AND IsActive = 1  
     )  
   BEGIN  
    UPDATE [HeroPOSP].[dbo].[POSP_Agreement]  
    SET PreSignedAgreementId = @AgreementId, UpdatedOn = GETDATE()  
     ,IsActive = 1  
    WHERE UserId = @UserId  
   END  
   ELSE  
   BEGIN  
    SET @StampId = (  
      SELECT TOP (1) Id  
      FROM [HeroAdmin].[dbo].[Admin_StampData]  
      WHERE StampStatus = 'Available'  
      ORDER BY SrNo  
      )  
  
    UPDATE [HeroAdmin].[dbo].[Admin_StampData]  
    SET StampStatus = 'Blocked', UpdatedOn = GETDATE()  
    WHERE Id = @StampId  
  
    ---POSP Agreement Initiate         
    INSERT INTO [HeroPOSP].[dbo].[POSP_Agreement] (  
     UserId  
     ,PreSignedAgreementId  
     ,StampId  
     )  
    VALUES (  
     @UserId  
     ,@AgreementId  
     ,@StampId  
     )  
   END  
  END  
  
  SELECT *  
  FROM POSP_Agreement WITH (NOLOCK)  
  WHERE UserId = @UserId  
 END TRY  
  
 BEGIN CATCH  
  DECLARE @StrProcedure_Name VARCHAR(500)  
   ,@ErrorDetail VARCHAR(1000)  
   ,@ParameterList VARCHAR(2000)  
  
  SET @StrProcedure_Name = ERROR_PROCEDURE()  
  SET @ErrorDetail = ERROR_MESSAGE()  
  
  EXEC POSP_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name  
   ,@ErrorDetail = @ErrorDetail  
   ,@ParameterList = @ParameterList  
 END CATCH  
END 
