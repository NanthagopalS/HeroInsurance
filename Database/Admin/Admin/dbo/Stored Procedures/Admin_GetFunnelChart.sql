--select * from HeroInsurance.dbo.Insurance_StageMaster       
/*       
exec [dbo].[Admin_GetFunnelChart] '2023-09-01' , '2023-09-09', '45BBA540-07C5-4D4C-BEFF-771AE2FC32B0'      
*/    
CREATE     PROCEDURE [dbo].[Admin_GetFunnelChart] (    
 @StartDate VARCHAR(100)    
 ,@EndDate VARCHAR(100)    
 ,@UserId VARCHAR(100)    
 )    
AS    
BEGIN    
 DECLARE @StartDate1 VARCHAR(100) = NULL    
  ,@EndDate1 VARCHAR(100) = NULL    
  ,@UserId1 VARCHAR(100) = NULL    
    
 SET @StartDate1 = CAST(@StartDate AS DATE)    
 SET @EndDate1 = CAST(@EndDate AS DATE)    
 SET @UserId1 = @UserId    
    
    
 DECLARE @UsersByHierarchy TABLE (UserId VARCHAR(100))    
 DECLARE @IsSuperAdmin BIT = 0    
    
 INSERT INTO @UsersByHierarchy (UserId)    
 EXEC [HeroIdentity].[dbo].[Identity_GetUserIdForDataVisibility] @UserId    
    
 SET @IsSuperAdmin = (    
   SELECT CASE     
     WHEN UserId IS NULL    
      THEN 0    
     ELSE 1    
     END AS IsSuperAdmin    
   FROM @UsersByHierarchy    
   WHERE UserId = '0'    
   )    
    
 DECLARE @IsAdmin INT = (    
   SELECT CASE     
     WHEN RM.RoleName = 'POSP'    
      THEN 0    
     WHEN RM.RoleName != 'POSP'    
      THEN 1    
     END AS IsAdmin    
   FROM HeroIdentity.dbo.Identity_User AS IU WITH (NOLOCK)    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_RoleMaster] RM ON IU.RoleId = RM.RoleId    
   WHERE IU.UserId = @UserId    
   )    
 DECLARE @TempStatus TABLE (    
  Quotations INT    
  ,Proposal INT    
  ,PolicyIssued INT    
  ,PreQuote INT    
  ,Breakin INT    
  )    
    
 BEGIN TRY    
  INSERT INTO @TempStatus    
  VALUES (    
   0    
   ,0    
   ,0    
   ,0    
   ,0    
   )    
    
  BEGIN    
   --For POSP     
   if (@IsAdmin = 1)    
   BEGIN    
   UPDATE @TempStatus    
   SET Quotations = (    
     SELECT COUNT(LD.LeadId) AS LeadCount    
     FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD    
     LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM WITH (NOLOCK) ON SM.StageID = LD.StageId    
     WHERE SM.StageID = 'A69719FC-AB1B-4BA5-87D0-EDB9024A93E7'    
      AND (    
       (LD.CreatedBy IN (    
        SELECT UserId    
        FROM @UsersByHierarchy    
        )    
       OR @IsSuperAdmin = 1)    
       )    
      AND (    
       CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)    
        AND CAST(@EndDate1 AS DATE)    
       )    
          
     )    
    END    
   else     
   BEGIN    
   UPDATE @TempStatus    
   SET Quotations = (    
     SELECT COUNT(LD.LeadId) AS LeadCount    
     FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD    
     LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM WITH (NOLOCK) ON SM.StageID = LD.StageId    
     WHERE SM.StageID = 'A69719FC-AB1B-4BA5-87D0-EDB9024A93E7'    
      AND (    
       (LD.CreatedBy IN (    
        SELECT UserId    
        FROM @UsersByHierarchy    
        )    
       OR @IsSuperAdmin = 1)    
       )    
      AND (    
       CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)    
        AND CAST(@EndDate1 AS DATE)    
       )    
      AND (    
        (    
         LD.PhoneNumber IS NOT NULL    
         AND LD.PhoneNumber != ''    
         )    
        OR (    
         LD.LeadName IS NOT NULL    
         AND LD.LeadName != ''    
         )    
        OR (    
         LD.Email IS NOT NULL    
         AND LD.Email != ''    
         )    
        )    
       )    
    END    
    
        
   if (@IsAdmin = 1)    
   BEGIN    
   UPDATE @TempStatus    
   SET Proposal = (    
     SELECT COUNT(LD.LeadId) AS LeadCount    
     FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD WITH (NOLOCK)    
     LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM WITH (NOLOCK) ON SM.StageID = LD.StageId    
     WHERE SM.StageID = 'ADB9EB9C-CB73-4DE3-BAF7-151F90C2A6F2'    
      AND (    
       (LD.CreatedBy IN (    
        SELECT UserId    
        FROM @UsersByHierarchy    
        )    
       OR @IsSuperAdmin = 1)    
       )    
      AND (    
       CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)    
        AND CAST(@EndDate1 AS DATE)    
      )    
     )    
    END    
    
   else     
   BEGIN    
   UPDATE @TempStatus    
   SET Proposal = (    
     SELECT COUNT(LD.LeadId) AS LeadCount    
     FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD WITH (NOLOCK)    
     LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM WITH (NOLOCK) ON SM.StageID = LD.StageId    
     WHERE SM.StageID = 'ADB9EB9C-CB73-4DE3-BAF7-151F90C2A6F2'    
      AND (    
       (LD.CreatedBy IN (    
        SELECT UserId    
        FROM @UsersByHierarchy    
        )    
       OR @IsSuperAdmin = 1)    
       )    
      AND (    
       CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)    
        AND CAST(@EndDate1 AS DATE)    
       )    
      AND (    
        (    
         LD.PhoneNumber IS NOT NULL    
         AND LD.PhoneNumber != ''    
         )    
        OR (    
         LD.LeadName IS NOT NULL    
         AND LD.LeadName != ''    
         )    
        OR (    
         LD.Email IS NOT NULL    
         AND LD.Email != ''    
         )    
      )    
     )    
   END    
    
   if (@IsAdmin = 1)    
   BEGIN    
   UPDATE @TempStatus    
   SET PolicyIssued = (    
     SELECT COUNT(LD.LeadId) AS LeadCount    
     FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD WITH (NOLOCK)    
     LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM WITH (NOLOCK) ON SM.StageID = LD.StageId    
     INNER JOIN [HeroInsurance].[dbo].[Insurance_PaymentTransaction] PT ON PT.LeadId = LD.LeadId  AND LD.QuoteTransactionID = PT.QuoteTransactionId
  INNER JOIN [HeroInsurance].[dbo].[Insurance_Insurer] LI WITH (NOLOCK) ON LI.InsurerId = PT.InsurerId  
     INNER JOIN [HeroInsurance].[dbo].[Insurance_PaymentStatus] PS ON PS.PaymentId = PT.STATUS    
     WHERE PS.PaymentId = 'A25D747B-167E-4C1B-AE13-E6CC49A195F8'    
      AND (    
       (LD.CreatedBy IN (    
        SELECT UserId    
        FROM @UsersByHierarchy    
        )    
       OR @IsSuperAdmin = 1)    
       )    
      AND (    
       CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)    
        AND CAST(@EndDate1 AS DATE)    
       )    
   AND LI.IsActive = 1  
     )    
      
    END    
    
    else    
    BEGIN    
   UPDATE @TempStatus    
   SET PolicyIssued = (    
     SELECT COUNT(LD.LeadId) AS LeadCount    
     FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD WITH (NOLOCK)    
     LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM WITH (NOLOCK) ON SM.StageID = LD.StageId    
     INNER JOIN [HeroInsurance].[dbo].[Insurance_PaymentTransaction] PT ON PT.LeadId = LD.LeadId   
  INNER JOIN [HeroInsurance].[dbo].[Insurance_Insurer] LI WITH (NOLOCK) ON LI.InsurerId = PT.InsurerId  
     INNER JOIN [HeroInsurance].[dbo].[Insurance_PaymentStatus] PS ON PS.PaymentId = PT.STATUS    
     WHERE PS.PaymentId = 'A25D747B-167E-4C1B-AE13-E6CC49A195F8'    
      AND (    
       (LD.CreatedBy IN (    
        SELECT UserId    
        FROM @UsersByHierarchy    
        )    
       OR @IsSuperAdmin = 1)    
       )    
      AND (    
       CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)    
        AND CAST(@EndDate1 AS DATE)    
       )    
      AND (    
        (    
         LD.PhoneNumber IS NOT NULL    
         AND LD.PhoneNumber != ''    
         )    
        OR (    
         LD.LeadName IS NOT NULL    
         AND LD.LeadName != ''    
         )    
        OR (    
         LD.Email IS NOT NULL    
         AND LD.Email != ''    
         )    
       )  
    AND LI.IsActive = 1  
     )    
    END    
    
    
   if (@IsAdmin = 1)    
   BEGIN    
   UPDATE @TempStatus    
   SET PreQuote = (    
     SELECT COUNT(LD.LeadId) AS LeadCount    
     FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD    
     LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM ON SM.StageID = LD.StageId    
     WHERE SM.StageID = 'AB4FA6D2-2C04-431A-8E6F-692359BAC662'    
      AND (    
       (LD.CreatedBy IN (    
        SELECT UserId    
        FROM @UsersByHierarchy    
        )    
       OR @IsSuperAdmin = 1)    
       )    
      AND (    
       CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)    
        AND CAST(@EndDate1 AS DATE)    
       )    
     )    
    END    
    
   else    
   BEGIN    
   UPDATE @TempStatus    
   SET PreQuote = (    
     SELECT COUNT(LD.LeadId) AS LeadCount    
     FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD    
     LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM ON SM.StageID = LD.StageId    
     WHERE SM.StageID = 'AB4FA6D2-2C04-431A-8E6F-692359BAC662'    
      AND (    
       (LD.CreatedBy IN (    
        SELECT UserId    
        FROM @UsersByHierarchy    
        )    
       OR @IsSuperAdmin = 1)    
       )    
      AND (    
       CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)    
        AND CAST(@EndDate1 AS DATE)    
       )    
      AND (    
        (    
         LD.PhoneNumber IS NOT NULL    
         AND LD.PhoneNumber != ''    
         )    
        OR (    
         LD.LeadName IS NOT NULL    
         AND LD.LeadName != ''    
         )    
        OR (    
         LD.Email IS NOT NULL    
         AND LD.Email != ''    
         )    
       )    
     )    
   END    
    
   if (@IsAdmin = 1)    
   BEGIN    
   UPDATE @TempStatus    
   SET Breakin = (    
     SELECT COUNT(LD.LeadId) AS LeadCount    
     FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD WITH (NOLOCK)    
     LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM WITH (NOLOCK) ON SM.StageID = LD.StageId    
     WHERE SM.StageID = '405F4696-CDFB-4065-B364-9410B56BC78D'    
      AND (    
       (LD.CreatedBy IN (    
        SELECT UserId    
        FROM @UsersByHierarchy    
        )    
       OR @IsSuperAdmin = 1)    
       )    
      AND (    
       CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)    
        AND CAST(@EndDate1 AS DATE)    
       )    
     )    
     END    
    
   else    
   BEGIN    
   UPDATE @TempStatus    
   SET Breakin = (    
     SELECT COUNT(LD.LeadId) AS LeadCount    
     FROM [HeroInsurance].[dbo].[Insurance_LeadDetails] LD WITH (NOLOCK)    
     LEFT JOIN [HeroInsurance].[dbo].[Insurance_StageMaster] SM WITH (NOLOCK) ON SM.StageID = LD.StageId    
     WHERE SM.StageID = '405F4696-CDFB-4065-B364-9410B56BC78D'    
      AND (    
       (LD.CreatedBy IN (    
        SELECT UserId    
        FROM @UsersByHierarchy    
        )    
       OR @IsSuperAdmin = 1)    
       )    
      AND (    
       CAST(LD.CreatedOn AS DATE) BETWEEN CAST(@StartDate1 AS DATE)    
        AND CAST(@EndDate1 AS DATE)    
       )    
      AND (    
        (    
         LD.PhoneNumber IS NOT NULL    
         AND LD.PhoneNumber != ''    
         )    
        OR (    
         LD.LeadName IS NOT NULL    
         AND LD.LeadName != ''    
         )    
        OR (    
         LD.Email IS NOT NULL    
         AND LD.Email != ''    
         )    
        )    
     )    
  END    
 END    
    
  SELECT *    
  FROM @TempStatus    
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
