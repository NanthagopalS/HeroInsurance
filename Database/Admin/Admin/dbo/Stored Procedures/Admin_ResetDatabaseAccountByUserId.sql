/*              
exec [Admin_ResetDatabaseAccountByUserId] '0568AF78-2D2A-4ADE-B5BD-B70860F177F7','1,2','45BBA540-07C5-4D4C-BEFF-771AE2FC32B0'             
*/    
CREATE     PROCEDURE [dbo].[Admin_ResetDatabaseAccountByUserId] (    
 @UserId VARCHAR(100)    
 ,@RejectionReasonId VARCHAR(100)    
 ,@RejectedBy VARCHAR(100)    
 )    
AS    
BEGIN    
 BEGIN TRY    
  BEGIN TRANSACTION    
    
  BEGIN    
   CREATE TABLE #POSPDocumentDetail (    
    DocumentId VARCHAR(100)    
    ,DocumentType VARCHAR(100)    
    )    
    
   INSERT INTO #POSPDocumentDetail    
   SELECT DocumentId    
    ,'POSP_DOCUMENT' AS DocumentType    
   FROM [HeroIdentity].[dbo].[Identity_DocumentDetail]    
   WHERE UserId = @UserId    
    
   INSERT INTO #POSPDocumentDetail    
   SELECT DocumentId    
    ,'POSP_EXAMDOCUMENT' AS DocumentType    
   FROM [HeroPOSP].[dbo].[POSP_Exam]    
   WHERE UserId = @UserId    
    
   CREATE TABLE #POSPDATA (    
    UserId VARCHAR(100)    
    ,UserName VARCHAR(100)    
    ,UserEmail VARCHAR(100)    
    ,MobileNo VARCHAR(15)    
    ,PAN VARCHAR(15)    
    )    
    
   INSERT INTO #POSPDATA    
   SELECT IU.UserID    
    ,IU.UserName    
    ,IU.EmailId    
    ,IU.MobileNo    
    ,UD.PAN    
   FROM [HeroIdentity].[dbo].[Identity_User] IU WITH (NOLOCK)    
   LEFT JOIN [HeroIdentity].[dbo].[Identity_Userdetail] UD WITH (NOLOCK) ON UD.UserId = IU.UserId    
   WHERE IU.UserId = @UserId    
  END    
  
  IF EXISTS (    
    SELECT TOP (1) UserId    
    FROM [HeroIdentity].[dbo].[Identity_User] WITH (NOLOCK)    
    WHERE UserId = @UserId    
     AND UserProfileStage != 5    
    )    
  
  BEGIN    
   EXEC [HeroPOSP].[dbo].[POSP_InsertUpdatePOSPStage] @UserId ,'B6181A24-83A8-4144-B58D-2118947711A5'    
  
   INSERT INTO [HeroPOSP].[dbo].[POSP_PanRejectionReasonsLogs] (    
    UserId    
    ,RejectionReasonId    
    ,RejectedBy    
    ,RejectedPanNumber    
    )    
   VALUES (    
    @UserId    
    ,@RejectionReasonId    
    ,@RejectedBy    
    ,(    
     SELECT PAN    
     FROM HeroIdentity.dbo.Identity_UserDetail WITH (NOLOCK)    
     WHERE UserId = @UserId    
     )    
    )    
   
   UPDATE [HeroIdentity].[dbo].Identity_User    
   SET UserProfileStage = NULL,IIBStatus='PENDING',IIBUploadStatus='PENDING',iib_upload_date = NULL    
   WHERE UserId = @UserId    
  
   DELETE    
   FROM [HeroIdentity].[dbo].Identity_DocumentDetail    
   WHERE UserId = @UserId    
    
   DELETE    
   FROM [HeroIdentity].[dbo].Identity_EmailVerification    
   WHERE UserId = @UserId    
    
   DELETE    
   FROM [HeroIdentity].[dbo].Identity_OTP    
   WHERE UserId = @UserId    
  
   DELETE    
   FROM [HeroIdentity].[dbo].Identity_PanVerification    
   WHERE UserId = @UserId    
    
   DELETE    
   FROM [HeroIdentity].[dbo].Identity_ResetPasswordVerification    
   WHERE UserId = @UserId    
    
   DELETE    
   FROM [HeroIdentity].[dbo].Identity_UserAddressDetail    
   WHERE UserId = @UserId    
    
   DELETE    
   FROM [HeroIdentity].[dbo].Identity_UserBankDetail    
   WHERE UserId = @UserId    
    
   DELETE    
   FROM [HeroIdentity].[dbo].Identity_UserBreadcrumStatusDetail    
   WHERE UserId = @UserId    
    
   DELETE    
   FROM [HeroIdentity].[dbo].Identity_UserDetail    
   WHERE UserId = @UserId    
  
   --POSP                
   DELETE    
   FROM [HeroPOSP].[dbo].POSP_Agreement    
   WHERE UserId = @UserId    
    
   DELETE    
   FROM [HeroPOSP].[dbo].POSP_ExamPaperDetail    
   WHERE ExamId IN (    
     SELECT Id    
     FROM [HeroPOSP].[dbo].POSP_Exam    
     WHERE UserId = @UserId    
     )    
    
   DELETE    
   FROM [HeroPOSP].[dbo].POSP_Exam    
   WHERE UserId = @UserId    
    
   DELETE    
   FROM [HeroPOSP].[dbo].POSP_Rating    
   WHERE UserId = @UserId    
    
   DELETE    
   FROM [HeroPOSP].[dbo].POSP_TrainingProgressDetail    
   WHERE TrainingId IN (    
     SELECT Id    
     FROM [HeroPOSP].[dbo].POSP_Training    
     WHERE UserId = @UserId    
     )    
    
   DELETE    
   FROM [HeroPOSP].[dbo].POSP_Training    
   WHERE UserId = @UserId    
  END    
    
  COMMIT    
    
  SELECT *    
  FROM #POSPDocumentDetail    
    
  SELECT UserId    
   ,UserName    
   ,UserEmail AS EmailId    
   ,MobileNo    
   ,PAN    
  FROM #POSPDATA    
 END TRY    
    
 BEGIN CATCH    
  ROLLBACK    
    
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