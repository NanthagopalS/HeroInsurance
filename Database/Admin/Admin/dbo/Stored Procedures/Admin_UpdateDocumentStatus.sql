CREATE  
    
  
 PROCEDURE [dbo].[Admin_UpdateDocumentStatus] (  
 @BackOfficeRemark VARCHAR(100)  
 ,@DocumentId VARCHAR(100)  
 ,@IsApprove BIT  
 )  
AS  
BEGIN  
 DECLARE @UserId VARCHAR(100) = NULL  
  ,@IIBStatus VARCHAR(100) = NULL  
  ,@IIBUploadStatus VARCHAR(100) = NULL  
 DECLARE @StampId VARCHAR(100) = NULL  
  ,@DocumentCount INT = 0  
  ,@ApprovedCount INT = 0  
  ,@StatusId VARCHAR(500) = NULL  
  
 BEGIN TRY  
  SELECT *  
  FROM [HeroIdentity].[dbo].[Identity_DocumentDetail]  
  
  UPDATE [HeroIdentity].[dbo].[Identity_DocumentDetail]  
  SET IsVerify = 1  
   ,IsApprove = @IsApprove  
   ,BackOfficeRemark = @BackofficeRemark  
   ,VerifyOn = GETDATE()  
   ,CreatedOn = GETDATE()
  WHERE DocumentId = @DocumentId  
  
  SET @UserId = (  
    SELECT TOP (1) UserId  
    FROM [HeroIdentity].[dbo].[Identity_DocumentDetail] WITH (NOLOCK)  
    WHERE DocumentId = @DocumentId  
    )  
  SET @IIBStatus = (  
    SELECT TOP (1) IIBStatus  
    FROM [HeroIdentity].[dbo].[Identity_User] WITH (NOLOCK)  
    WHERE UserId = @UserId  
    )  
  SET @IIBUploadStatus = (  
    SELECT TOP (1) IIBUploadStatus  
    FROM [HeroIdentity].[dbo].[Identity_User] WITH (NOLOCK)  
    WHERE UserId = @UserId  
    )  
  SET @StatusId = (  
    SELECT Id  
    FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster] WITH (NOLOCK)  
    WHERE StatusName = 'KYC Pending'  
     AND PriorityIndex = 3  
    )  
  
  -- logic if alias is present then make POSP Declaration / Affidavit is mandatory otherwise not      
  DECLARE @isAlias BIT  
   ,@check VARCHAR(100)  
  
  SET @check = (  
    SELECT AliasName  
    FROM HeroIdentity.dbo.Identity_UserDetail WITH (NOLOCK)  
    WHERE UserId = @UserId  
    )  
  
  IF (  
    @check != ''  
    OR @check != NULL  
    )  
  BEGIN  
   SET @isAlias = 1  
  END  
  ELSE  
  BEGIN  
   SET @isAlias = 0  
  END  
  
  IF OBJECT_ID('#UserDocument') IS NOT NULL  
   DROP TABLE #UserDocument;  
  
  SELECT *  
  INTO #UserDocument  
  FROM [HeroIdentity].[dbo].[Identity_DocumentTypeMaster] WITH (NOLOCK);  
  
  UPDATE #UserDocument  
  SET IsMandatory = CASE   
    WHEN (  
      DocumentType = 'POSP Declaration / Affidavit'  
      AND @isAlias = 1  
      )  
     THEN 1  
    WHEN (  
      DocumentType = 'POSP Declaration / Affidavit'  
      AND @isAlias = 0  
      )  
     THEN 0  
    ELSE IsMandatory  
    END  
  
  --UPDATE Breadcrum stage for PSOP User                
  UPDATE [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusDetail]  
  SET StatusId = @StatusId,
  CreatedOn= GETDATE()
  WHERE UserId = @UserId  
   AND StatusId IN (  
    SELECT Id  
    FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster]  
    WHERE StatusName = 'KYC Not Started'  
     AND PriorityIndex = 2  
    )  
  
  --check upload doc...        
  SET @DocumentCount = (  
    SELECT COUNT(Id)  
    FROM #UserDocument UD  
    WHERE ud.IsActive = 1  
     AND ud.IsMandatory = 1  
    )  
  SET @ApprovedCount = (  
    SELECT count(Id)  
    FROM [HeroIdentity].[dbo].[Identity_DocumentDetail] WITH (NOLOCK)  
    WHERE UserId = @UserId  
     AND IsActive = 1  
     AND IsVerify = 1  
     AND IsApprove = 1  
     AND DocumentTypeId IN (  
      SELECT Id  
      FROM [HeroIdentity].[dbo].[Identity_DocumentTypeMaster] WITH (NOLOCK)  
      WHERE IsMandatory = 1  
       AND IsActive = 1  
      )  
    )  
  
  IF (@DocumentCount <= @ApprovedCount)  
  BEGIN  
   SET @StatusId = (  
     SELECT Id  
     FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster] WITH (NOLOCK)  
     WHERE StatusName = 'KYC Completed'  
      AND PriorityIndex = 4  
     )  
  
   --UPDATE Breadcrum stage for PSOP User                
   UPDATE [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusDetail]  
   SET StatusId = @StatusId,
   CreatedOn= GETDATE()
   WHERE UserId = @UserId  
    AND StatusId IN (  
     SELECT Id  
     FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster]  
     WHERE StatusName = 'KYC Pending'  
      AND PriorityIndex = 3  
     )  
	      UPDATE HeroIdentity.dbo.Identity_User SET IsDocQCVerified = 1 WHERE UserId = @UserId  

  END  
  
  IF (  
    @IIBStatus = 'NOT EXISTING'  
    AND @IIBUploadStatus = 'SUCCESS'  
    )  
  BEGIN  
   IF NOT EXISTS (  
     SELECT TOP (1) UserId  
     FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH (NOLOCK)  
     WHERE UserId = @UserId  
      AND IsActive = 1  
     )  
   BEGIN  
    SET @StampId = (  
      SELECT TOP (1) Id  
      FROM [HeroAdmin].[dbo].[Admin_StampData] WITH (NOLOCK)  
      WHERE StampStatus = 'Available'  
      ORDER BY SrNo  
      )  
    --check upload doc...        
    SET @DocumentCount = (  
      SELECT COUNT(Id)  
      FROM #UserDocument UD  
      WHERE ud.IsActive = 1  
       AND ud.IsMandatory = 1  
      )  
    SET @ApprovedCount = (  
      SELECT count(Id)  
      FROM [HeroIdentity].[dbo].[Identity_DocumentDetail] WITH (NOLOCK)  
      WHERE UserId = @UserId  
       AND IsActive = 1  
       AND IsVerify = 1  
       AND IsApprove = 1  
       AND DocumentTypeId IN (  
        SELECT Id  
        FROM [HeroIdentity].[dbo].[Identity_DocumentTypeMaster] WITH (NOLOCK)  
        WHERE IsMandatory = 1  
         AND IsActive = 1  
        )  
      )  
  
    IF (@DocumentCount <= @ApprovedCount)  
    BEGIN  
     SET @StatusId = (  
       SELECT Id  
       FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster] WITH (NOLOCK)  
       WHERE StatusName = 'KYC Completed'  
        AND PriorityIndex = 4  
       )  
  
     --UPDATE Breadcrum stage for PSOP User                
     UPDATE [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusDetail]  
     SET StatusId = @StatusId, CreatedOn= GETDATE()
     WHERE UserId = @UserId  
      AND StatusId IN (  
       SELECT Id  
       FROM [HeroIdentity].[dbo].[Identity_UserBreadcrumStatusMaster]  
       WHERE StatusName = 'KYC Pending'  
        AND PriorityIndex = 3  
       )  
       
     UPDATE HeroIdentity.dbo.Identity_User SET IsDocQCVerified = 1, UpdatedOn= GETDATE() WHERE UserId = @UserId  
  
     IF NOT EXISTS (  
       SELECT TOP 1 UserId  
       FROM [HeroPOSP].[dbo].[POSP_Agreement] WITH (NOLOCK)  
       WHERE UserId = @UserId  
       )  
     BEGIN  
      UPDATE [HeroAdmin].[dbo].[Admin_StampData]  
      SET StampStatus = 'Blocked', CreatedOn= GETDATE()
      WHERE Id = @StampId  
  
      ---POSP Agreement Initiate        
      INSERT INTO [HeroPOSP].[dbo].[POSP_Agreement] (  
       UserId  
       ,StampId  
       )  
      VALUES (  
       @UserId  
       ,@StampId  
       )  
     END  
    END  
   END  
  END  
  
  DROP TABLE #UserDocument;  
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
