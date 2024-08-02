/*                
  [dbo].[Admin_GetPOSPManagementDetailList] '0','','Payout Details','','',-1,100                                      
  */                
CREATE                  
                           
 PROCEDURE [dbo].[Admin_GetPOSPManagementDetailList] (                
 @POSPStatus VARCHAR(1) = '0'                
 ,-- 0 - New POSP / 1 - Active POSP   / 2- Inactive or Deactivae POSP                        
 @Searchtext VARCHAR(100) = NULL                
 ,-- POSPId, UserName, Mobile Number                            
 @Stage VARCHAR(100) = NULL                
 ,-- comes stage name from POSPStage table                            
 @CreatedBy VARCHAR(100) = NULL                
 ,-- All, Assisted, Self                           
 @RelationManagerId VARCHAR(100) = NULL                
 ,@CurrentPageIndex INT = 1                
 ,@CurrentPageSize INT = 10                
 )                
AS                
DECLARE @TempDataTable TABLE (                
 UserId VARCHAR(100)                
 ,POSPId VARCHAR(100)                
 ,POSPName VARCHAR(100)                
 ,MobileNumber VARCHAR(100)                
 ,EmailId VARCHAR(100)                
 ,CreatedBy VARCHAR(100)                
 ,Stage VARCHAR(100)                
 ,RelationManagerId VARCHAR(100)                
 ,TaggedPolicy VARCHAR(100)                
 ,CreatedOn DATETIME                
 ,IsActive BIT                
 ,PospStatus VARCHAR(1)                
 ,IsDocQCVerified BIT          
 ,UserProfileStage INT      
 )                
                
BEGIN                
 BEGIN TRY                
  INSERT @TempDataTable                
  SELECT IU.UserId                
   ,IU.POSPId                
   ,IU.UserName AS POSPName                
   ,IU.MobileNo AS MobileNumber                
   ,IU.EmailId AS EmailId                
   ,IU.CreatedByMode AS CreatedBy                
   ,CASE             
  WHEN @Stage='Document Quality Check'             
   THEN 'Document Pending' ELSE CASE                    
  WHEN UserStage.StageId = '6D26CFBA-B5FA-46C8-A048-3E96982D90B7'               
   THEN 'Agreement ONGOING'                
  WHEN UserStage.StageId = 'EE1FF4F7-3BF9-402E-9461-47FE0F8A06C1'              
   THEN 'IIB Upload'                
  WHEN UserStage.StageId = '220FF0B3-BA2E-4B40-9828-4FC30065DE14'                
   THEN 'Exam FAILED'                
  WHEN UserStage.StageId = 'D56CA403-6B9E-48F9-B608-F008472EFACC'                
   THEN 'Exam ONGOING'                
  WHEN UserStage.StageId = 'E6F84D7A-A6F9-4141-B5BD-A20DAFA1D371'              
   THEN 'Training ONGOING'                
  WHEN UserStage.StageId = '2955D6F3-7B6C-4EB0-8678-A80F5B8A0047'                 
   THEN 'Document Submission'                
  WHEN UserStage.StageId = '8A2010DF-0137-4ED0-BDA6-6A8DF9128827'              
   THEN 'Payout Pending'                
  WHEN UserStage.StageId = '8EB4B84F-F12D-44E2-9A62-2C3A4848ABB4'                
   THEN 'Profile ONGOING'                
  WHEN UserStage.StageId = 'B6181A24-83A8-4144-B58D-2118947711A5'                 
   THEN 'PAN ONGOING'                
  END              
    END AS Stage                
   ,serUsr.UserName AS RelationManagerId                
   ,'Motor' AS TaggedPolicy                
   ,CAST(IU.CreatedOn AS DATETIME) AS CreatedOn                
   ,IU.IsActive                
   ,CASE      
   WHEN UserStage.StageId ='B6E3FE9B-0202-486F-ADD7-C11639970690' THEN '2'
   WHEN UserStage.StageId = '769D2D45-C4A2-4205-9CC4-CE32D1C6415E' THEN '1'                
   ELSE  '0'                
   END AS PospStatus               
 ,IU.IsDocQCVerified              
 ,IU.UserProfileStage      
  FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)                
  LEFT JOIN [HeroPOSP].[dbo].[POSP_Agreement] PA WITH (NOLOCK) ON PA.UserId = IU.UserId                
   AND PA.IsActive = 1                
  LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] UD WITH (NOLOCK) ON UD.UserId = IU.UserId                
  LEFT JOIN [HeroIdentity].[dbo].[Identity_User] serUsr WITH (NOLOCK) ON UD.ServicedByUserId = serUsr.UserId              
  INNER JOIN [HeroPOSP].[dbo].[POSP_UserStageStatusDetail] UserStage WITH (NOLOCK) ON UserStage.UserId = IU.UserId            
  LEFT JOIN [HeroAdmin].[dbo].[Admin_DeActivatePOSP] DEAC WITH (NOLOCK) ON IU.POSPId = DEAC.DeActivatePospId                
   AND [Status] = 'DEACTIVATED'                
   AND DEAC.IsActive = 1                
  WHERE IU.RoleId = '2D6B0CE9-15C7-4839-93D1-8387944BC42F'                
   AND (                
    ISNULL(@RelationManagerId, '') = ''                
    OR (UD.ServicedByUserId = @RelationManagerId)                
    )                
   AND IU.POSPId IS NOT NULL                
   AND (                
    ISNULL(@CreatedBy, '') = ''                
    OR (             
     (                
      @CreatedBy = 'Assisted'                
      AND IU.CreatedByMode = @CreatedBy                
      )                
     OR (                
      @CreatedBy = 'Self'                
      AND IU.CreatedByMode = @CreatedBy                
      )                
     )                
    )                 
                
  SELECT *                
  INTO #POSP_MGMT                
  FROM @TempDataTable                
  WHERE PospStatus = @POSPStatus                
   AND (                
    ISNULL(@Searchtext, '') = ''                
    OR (                
     (POSPName LIKE '%' + @Searchtext + '%')                
     OR (MobileNumber LIKE '%' + @Searchtext + '%')                
     OR (POSPId LIKE '%' + @Searchtext + '%')                
     OR (EmailId LIKE '%' + @Searchtext + '%')                
     )                
    )                
   AND (                
    ISNULL(@Stage, '') = ''                
    OR (                
     (                
      @Stage = 'Document Quality Check'                
      AND IsDocQCVerified = 0              
   AND UserProfileStage = 4      
      )                
     OR (                
      @Stage = 'Payout Details'                
      AND Stage = 'Payout Pending'                
      )                
     OR (                
      @Stage = 'Document Submission'                
      AND Stage = 'Document Submission'                
      )                
     OR (                
      @Stage = 'Training'                
      AND (Stage = 'Training ONGOING')                
      )                
     OR (                
      @Stage = 'Exam'                
      AND (                
       Stage = 'Exam FAILED'                
       OR Stage = 'Exam ONGOING'                
       )                
      )                
     OR (                
      @Stage = 'Agreement'                
      AND (                
       Stage = 'Agreement COMPLETED'                
       OR Stage = 'Agreement ONGOING'                
       )                
      )                
     OR (                
      @Stage = 'IIB Upload'                
      AND Stage = 'IIB Upload'                
      )                
     OR (                
      @Stage = 'PAN verification'                
      AND Stage = 'PAN ONGOING'                
      )                
     OR (                
      @Stage = 'Profile Details'                
      AND Stage = 'Profile ONGOING'                
      )                
     )                
    )                 
                
  DECLARE @TotalRecords INT = (                
    SELECT COUNT(1) AS TotalRecords                
    FROM #POSP_MGMT                
    )                
                
  IF @CurrentPageIndex = - 1                
  BEGIN                
   SELECT *                
    ,SUBSTRING(Stage, 1, CHARINDEX(' ', Stage) - 1) AS StageValue                
    ,SUBSTRING(Stage, CHARINDEX(' ', Stage) + 1, LEN(Stage) - CHARINDEX(' ', Stage)) AS StatusValue                
    ,RelationManagerId AS RelationManager                
    ,@TotalRecords AS TotalRecords                
   FROM #POSP_MGMT                
   ORDER BY MobileNumber desc                
                
   SELECT 0 AS CurrentPageIndex                
    ,0 AS PreviousPageIndex                
    ,0 AS NextPageIndex                
    ,0 AS CurrentPageSize                
    ,0 AS TotalRecord                
  END                
  ELSE                
  BEGIN                
   SELECT *                
    ,SUBSTRING(Stage, 1, CHARINDEX(' ', Stage) - 1) AS StageValue                
    ,SUBSTRING(Stage, CHARINDEX(' ', Stage) + 1, LEN(Stage) - CHARINDEX(' ', Stage)) AS StatusValue                
    ,RelationManagerId AS RelationManager                
    ,@TotalRecords AS TotalRecords                
   FROM #POSP_MGMT                
   ORDER BY CreatedOn DESC OFFSET((@CurrentPageIndex - 1) * @CurrentPageSize) ROWS                
              
   FETCH NEXT @CurrentPageSize ROWS ONLY;                
                
   SELECT @CurrentPageIndex AS CurrentPageIndex                
    ,@CurrentPageIndex - 1 AS PreviousPageIndex                
    ,@CurrentPageIndex + 1 AS NextPageIndex                
    ,@CurrentPageSize AS CurrentPageSize                
    ,@TotalRecords AS TotalRecord                
  END                
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