CREATE    
      
     
 PROCEDURE [dbo].[Admin_GetAllPOSPStageCount]    
AS    
BEGIN    
 BEGIN TRY    
  DECLARE @TempDataTable TABLE (    
   StageName VARCHAR(100)    
   ,StageCount INT    
   )    
    
  --POSP Onboarding Count      
  INSERT @TempDataTable    
  SELECT 'PAN verification' AS StageName    
   ,COUNT(*) AS StageCount    
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)    
  JOIN HeroPosp.dbo.POSP_UserStageStatusDetail USSD WITH(NOLOCK) ON IU.UserId = USSD.UserId  
  WHERE IU.IsActive = 1    
  AND USSD.StageId='B6181A24-83A8-4144-B58D-2118947711A5'  
   AND IU.RoleId = '2D6B0CE9-15C7-4839-93D1-8387944BC42F'   
   AND IU.POSPId IS NOT NULL   
    
  INSERT @TempDataTable    
  SELECT 'Profile Details' AS StageName    
   ,COUNT(*) AS StageCount    
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)    
    JOIN HeroPosp.dbo.POSP_UserStageStatusDetail USSD WITH(NOLOCK) ON IU.UserId = USSD.UserId  
  WHERE    
   IU.IsActive = 1    
     AND USSD.StageId='8EB4B84F-F12D-44E2-9A62-2C3A4848ABB4'  
   AND IU.RoleId = '2D6B0CE9-15C7-4839-93D1-8387944BC42F'   
   AND IU.POSPId IS NOT NULL   
  
    
  INSERT @TempDataTable    
  SELECT 'Payout Details' AS StageName    
   ,COUNT(*) AS StageCount    
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)    
    JOIN HeroPosp.dbo.POSP_UserStageStatusDetail USSD WITH(NOLOCK) ON IU.UserId = USSD.UserId  
  WHERE IU.IsActive = 1    
       AND USSD.StageId='8A2010DF-0137-4ED0-BDA6-6A8DF9128827'  
   AND IU.RoleId = '2D6B0CE9-15C7-4839-93D1-8387944BC42F'    
    
  INSERT @TempDataTable    
  SELECT 'Document Submission' AS StageName    
   ,COUNT(*) AS StageCount    
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)    
      JOIN HeroPosp.dbo.POSP_UserStageStatusDetail USSD WITH(NOLOCK) ON IU.UserId = USSD.UserId  
  WHERE IU.IsActive = 1    
   AND IU.RoleId = '2D6B0CE9-15C7-4839-93D1-8387944BC42F'    
AND USSD.StageId='2955D6F3-7B6C-4EB0-8678-A80F5B8A0047'  
  
    
  INSERT @TempDataTable    
  SELECT 'Training' AS StageName    
   ,COUNT(IU.UserId) AS StageCount                 
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)                 
  JOIN HeroPosp.dbo.POSP_UserStageStatusDetail USSD WITH(NOLOCK) ON IU.UserId = USSD.UserId  
    WHERE  USSD.StageId='E6F84D7A-A6F9-4141-B5BD-A20DAFA1D371'  
   AND IU.RoleId = '2D6B0CE9-15C7-4839-93D1-8387944BC42F'    
   AND IU.IsActive = 1  
    
  INSERT @TempDataTable    
  SELECT 'Exam' AS StageName    
   ,COUNT(*) AS StageCount    
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)    
   JOIN HeroPosp.dbo.POSP_UserStageStatusDetail USSD WITH(NOLOCK) ON IU.UserId = USSD.UserId  
   WHERE  
   (USSD.StageId='D56CA403-6B9E-48F9-B608-F008472EFACC' OR USSD.StageId='220FF0B3-BA2E-4B40-9828-4FC30065DE14')  
   AND IU.RoleId = '2D6B0CE9-15C7-4839-93D1-8387944BC42F'    
   AND IU.IsActive = 1    
    
  INSERT @TempDataTable    
  SELECT 'Agreement' AS StageName    
   ,COUNT(IU.UserId) AS StageCount    
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)    
  JOIN HeroPosp.dbo.POSP_UserStageStatusDetail USSD WITH(NOLOCK) ON IU.UserId = USSD.UserId  
  WHERE IU.RoleId = '2D6B0CE9-15C7-4839-93D1-8387944BC42F'    
   AND IU.IsActive = 1    
   AND USSD.StageId='6D26CFBA-B5FA-46C8-A048-3E96982D90B7'   
    
  SELECT StageName    
   ,StageCount    
  FROM @TempDataTable    
 END TRY    
    
 BEGIN CATCH    
  DECLARE @StrProcedure_Name VARCHAR(500)    
   ,@ErrorDetail VARCHAR(1000)    
   ,@ParameterList VARCHAR(2000)    
    
  SET @StrProcedure_Name = ERROR_PROCEDURE()    
  SET @ErrorDetail = ERROR_MESSAGE()    
    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name    
   ,@ErrorDetail = @ErrorDetail    
   ,@ParameterList = @ParameterList    
 END CATCH    
END