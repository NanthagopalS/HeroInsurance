          
-- =========================================================================================                           
-- Author:  <Author, Ankit>                        
-- Create date: <Create Date,2-Feb-2023>                        
-- Description: <Description, Admin_GetPOSPCountOnboardingDetail>                  
-- =========================================================================================                           
CREATE       PROCEDURE [dbo].[Admin_AllPOSPBackOfficePendingReport]          
AS          
BEGIN          
 BEGIN TRY          
  DECLARE @TempDataTable TABLE (          
   StageName VARCHAR(100)          
   ,StageCount INT          
   )          
          
  INSERT @TempDataTable          
  SELECT 'IIB Upload' AS StageName          
   ,COUNT(IU.UserId) AS StageCount          
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)          
   JOIN HeroPosp.dbo.POSP_UserStageStatusDetail USSD WITH(NOLOCK) ON IU.UserId = USSD.UserId      
  WHERE       
    USSD.StageId='EE1FF4F7-3BF9-402E-9461-47FE0F8A06C1'      
 AND IU.RoleId = '2D6B0CE9-15C7-4839-93D1-8387944BC42F'         
   AND IU.POSPId IS NOT NULL        
   AND IU.IsActive = 1      
          
  INSERT @TempDataTable          
  SELECT 'Document Quality Check' AS StageName          
   ,COUNT(DISTINCT UserId) AS StageCount          
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)          
  WHERE IU.IsActive = 1        
  AND IU.IsDocQCVerified = 0        
   AND IU.RoleId = '2D6B0CE9-15C7-4839-93D1-8387944BC42F'      
   AND IU.POSPId IS NOT NULL    
   AND IU.UserProfileStage = 4  
          
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