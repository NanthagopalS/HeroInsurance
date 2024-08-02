  
-------- =============================================            
-- Author:  <Author, Parth>            
-- Create date: <Create Date, 10-Apr-2023>            
-- Description: <Description,,Admin_InsertNotification >            
-- =============================================            
CREATE   PROCEDURE [dbo].[Admin_InsertRaiseRequest] (  
 @ConcernTypeId VARCHAR(100)  
 ,@SubConcernTypeId VARCHAR(100)  
 ,@SubjectText VARCHAR(500)  
 ,@DetailText VARCHAR(max)  
 ,@DocumentId VARCHAR(max)  
 ,@UserId VARCHAR(100) = NULL  
 )  
AS  
BEGIN  
 BEGIN TRY  
  BEGIN  
   DECLARE @CodePatternPOSPId VARCHAR(50) = NULL  
    ,@FinalCodePOSPId VARCHAR(50) = NULL  
    ,@DeactivateId VARCHAR(50) = NULL  
    ,@IndexPOSPHelpId INT = 0  
  
   BEGIN  
    SET @CodePatternPOSPId = (  
      SELECT CodePattern  
      FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WITH (NOLOCK)  
      WHERE [Code] = 'POSPHelp'  
       AND IsActive = 1  
      )  
    SET @IndexPOSPHelpId = (  
      SELECT NextValue  
      FROM [HeroIdentity].[dbo].[Identity_AutoGenerateId] WITH (NOLOCK)  
      WHERE [Code] = 'POSPHelp'  
       AND IsActive = 1  
      )  
    SET @FinalCodePOSPId = CONCAT (  
      @CodePatternPOSPId  
      ,CAST(@IndexPOSPHelpId AS VARCHAR)  
      )  
    SET @IndexPOSPHelpId = @IndexPOSPHelpId + 1  
  
    UPDATE [HeroIdentity].[dbo].[Identity_AutoGenerateId]  
    SET NextValue = @IndexPOSPHelpId, UpdatedOn = GETDATE()  
    WHERE [Code] = 'POSPHelp'  
     AND IsActive = 1  
   END  
  
   INSERT INTO [Admin_HelpAndSupport] (  
    ID  
    ,ConcernTypeId  
    ,SubConcernTypeId  
    ,SubjectText  
    ,DetailText  
    ,DocumentId  
    ,STATUS  
    ,UserId  
    )  
   VALUES (  
    @FinalCodePOSPId  
    ,@ConcernTypeId  
    ,@SubConcernTypeId  
    ,@SubjectText  
    ,@DetailText  
    ,@DocumentId  
    ,'REQUEST RAISED'  
    ,@UserId  
    )  
  END  
 END TRY  
  
 BEGIN CATCH  
  IF @@TRANCOUNT > 0  
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