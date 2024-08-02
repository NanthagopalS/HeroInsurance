-- =========================================================================================                         
-- Author:  <Author, Ankit>                      
-- Create date: <Create Date,2-Feb-2023>                      
-- Description: <Description, Admin_GetPOSPCountOnboardingDetail>                
-- =========================================================================================                         
CREATE        
         
          
    PROCEDURE [dbo].[Admin_GetPOSPCountOnboardingDetail]        
AS        
DECLARE @CountResult TABLE (        
 Id INT        
 ,ColumnName VARCHAR(100)        
 ,ColumnValue INT        
 ,GroupNumber INT        
 )        
        
BEGIN        
 BEGIN TRY        
  DECLARE @TotalPOSPCount INT = 1        
        
  SET @TotalPOSPCount = (        
    SELECT COUNT(DISTINCT UserId)        
    FROM [HeroIdentity].[dbo].[Identity_User] WITH (NOLOCK)        
    WHERE IsActive = 1        
     AND POSPId IS NOT NULL        
    )        
        
  IF (@TotalPOSPCount < 1)        
  BEGIN        
   SET @TotalPOSPCount = 1        
  END        
        
  --GroupNumber-1                
  INSERT @CountResult        
  SELECT 1        
   ,'Total POSP Onboarded'        
   ,COUNT(DISTINCT IU.UserId)        
   ,1 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)        
   JOIN [HeroPOSP].[dbo].[POSP_UserStageStatusDetail] USD WITH (NOLOCK) ON USD.UserId= IU.UserId        
  WHERE (USD.StageId='769D2D45-C4A2-4205-9CC4-CE32D1C6415E' OR USD.StageId='B6E3FE9B-0202-486F-ADD7-C11639970690')
  AND IU.RoleId ='2D6B0CE9-15C7-4839-93D1-8387944BC42F'
  
          
        
  --GroupNumber-2                
  INSERT @CountResult        
  SELECT 2        
   ,'Total POSP'        
   ,COUNT(DISTINCT UserStage.UserId)        
   ,2 AS GroupNumber        
  FROM [HeroPOSP].[dbo].[POSP_UserStageStatusDetail] UserStage WITH (NOLOCK)  
  JOIN HeroIdentity.dbo.Identity_User IU WITH(NOLOCK) ON UserStage.UserId = IU.UserId  
  WHERE UserStage.UserId  IS NOT NULL      
  AND IU.POSPId IS NOT NULL  
  AND IU.RoleId ='2D6B0CE9-15C7-4839-93D1-8387944BC42F'
          
        
  INSERT @CountResult        
  SELECT 3        
   ,'Total POSP New Request'        
   ,COUNT(DISTINCT UserId)        
   ,2 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_User] WITH (NOLOCK)        
  WHERE CAST(CreatedOn AS DATE) = CAST(GETDATE() AS DATE)        
   AND POSPId IS NOT NULL     
   AND RoleId ='2D6B0CE9-15C7-4839-93D1-8387944BC42F'
        
  --GroupNumber-3                
  INSERT @CountResult        
  SELECT 4        
   ,'Activated'        
   ,CAST((CAST(COUNT(DISTINCT IU.UserId) AS NUMERIC(18, 2)) / CAST(@TotalPOSPCount AS NUMERIC(18, 2)) * 100) AS NUMERIC(18, 0))        
   ,3 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)        
  INNER JOIN [HeroIdentity].[dbo].[Identity_OTP] IOTP WITH (NOLOCK) ON IU.UserId = IOTP.UserId        
  WHERE IU.IsActive = 1        
   AND IU.POSPId IS NOT NULL        
   AND IOTP.IsActive = 1        
   AND DATEDIFF(DAY, IOTP.OTPSendDateTime, GETDATE()) <= 5        
        
  INSERT @CountResult        
  SELECT 5        
   ,'InActive'        
   ,CAST((CAST(COUNT(DISTINCT IU.UserId) AS NUMERIC(18, 2)) / CAST(@TotalPOSPCount AS NUMERIC(18, 2)) * 100) AS NUMERIC(18, 0))        
   ,3 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)        
  INNER JOIN [HeroIdentity].[dbo].[Identity_OTP] IOTP WITH (NOLOCK) ON IU.UserId = IOTP.UserId        
  WHERE IU.IsActive = 1        
   AND IU.POSPId IS NOT NULL        
   AND IOTP.IsActive = 1        
   AND DATEDIFF(DAY, IOTP.OTPSendDateTime, GETDATE()) > 5        
        
  INSERT @CountResult        
  SELECT 6        
   ,'DeActivated'        
   ,CAST((CAST(COUNT(DISTINCT UserId) AS NUMERIC(18, 2)) / CAST(@TotalPOSPCount AS NUMERIC(18, 2)) * 100) AS NUMERIC(18, 0))        
   ,3 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_User] WITH (NOLOCK)        
  WHERE IsActive = 0        
   AND POSPId IS NOT NULL        
        
  --GroupNumber-4                 
  INSERT @CountResult        
  SELECT 7        
   ,'PAN verification'        
   ,COUNT(DISTINCT IU.UserId)        
   ,5 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)        
  WHERE (        
    IU.UserProfileStage = 0        
    OR IU.UserProfileStage IS NULL        
    )        
   AND IU.IsActive = 1        
   AND IU.IsRegistrationVerified = 1        
        
  INSERT @CountResult        
  SELECT 8        
   ,'Profile Details'        
   ,COUNT(DISTINCT IU.UserId)        
   ,4 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)        
  WHERE IU.UserProfileStage = 1        
   AND IU.IsActive = 1        
   AND IU.POSPId IS NOT NULL        
        
  INSERT @CountResult        
  SELECT 9        
   ,'Payout Details'        
   ,COUNT(DISTINCT IU.UserId)        
   ,4 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)        
  WHERE IU.IsActive = 1        
   AND IU.POSPId IS NOT NULL        
   AND IU.UserProfileStage = 2        
        
  INSERT @CountResult        
  SELECT 10        
   ,'Document Submission'        
   ,COUNT(DISTINCT IU.UserId)        
   ,4 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)        
  WHERE IU.IsActive = 1        
   AND IU.POSPId IS NOT NULL        
   AND IU.UserProfileStage = 3        
        
  INSERT @CountResult        
  SELECT 11        
   ,'Training'        
   ,COUNT(DISTINCT IU.UserId)        
   ,4 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)        
  LEFT JOIN [HeroPOSP].[dbo].[POSP_Training] PT WITH (NOLOCK) ON PT.UserId = IU.UserId        
  WHERE (        
    (        
     Iu.UserId NOT IN (        
      SELECT Pt.UserId        
      FROM [HeroPOSP].[dbo].[POSP_Training] PT        
      )        
     OR Pt.IsTrainingCompleted != 1        
     )        
    )        
   AND Iu.IsActive = 1        
        
  INSERT @CountResult        
  SELECT 12        
   ,'Exam'        
   ,COUNT(DISTINCT PE.UserId)        
   ,4 AS GroupNumber        
  FROM [HeroPOSP].[dbo].[POSP_Exam] PE WITH (NOLOCK)        
  JOIN [HeroPOSP].[dbo].[POSP_Training] PT WITH (NOLOCK) ON PT.UserId = PE.UserId        
  JOIN [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK) ON PE.UserId = IU.UserId        
  WHERE IU.IsActive = 1        
   AND PE.IsCleared != 1        
   AND PT.IsTrainingCompleted = 1        
   AND IU.POSPId IS NOT NULL        
        
  INSERT @CountResult        
  SELECT 13        
   ,'Agreement'        
   ,COUNT(DISTINCT UserId)        
   ,4 AS GroupNumber        
  FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH (NOLOCK)        
  WHERE IsActive = 1        
   AND AgreementId IS NOT NULL        
   AND UserId IN (        
    SELECT UserId        
    FROM [HeroIdentity].[dbo].[Identity_User] WITH (NOLOCK)        
    WHERE IsActive = 1        
     AND POSPId IS NOT NULL        
    )        
        
  --GroupNumber-5                    
  INSERT @CountResult        
  SELECT 14        
   ,'Document Quality Check'        
   ,COUNT(DISTINCT DD.UserId)        
   ,5 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_DocumentDetail] DD WITH (NOLOCK)        
  JOIN [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK) ON DD.UserId = IU.UserId        
  WHERE IU.IsActive = 1 and DD.IsActive = 0        
        
  INSERT @CountResult        
  SELECT 15        
   ,'IIB Upload'        
   ,COUNT(DISTINCT IU.UserId)        
   ,4 AS GroupNumber        
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)        
  LEFT JOIN [HeroPOSP].[dbo].[POSP_Exam] PE WITH (NOLOCK)  ON PE.UserId = IU.UserId        
  WHERE IU.IsActive = 1 and PE.IsCleared = 1        
   AND (        
    IIBUploadStatus = 'Pending'        
    OR IIBUploadStatus = 'Failed'        
    )        
   AND POSPId IS NOT NULL        
        
  ---------------------------                
  SELECT Id        
   ,GroupNumber        
   ,ColumnName        
   ,ColumnValue        
  FROM @CountResult        
  ORDER BY GroupNumber        
   ,Id        
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
