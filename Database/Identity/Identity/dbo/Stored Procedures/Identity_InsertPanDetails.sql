-- =============================================              
-- Author:  <Author,,FIROZ S>              
-- Create date: <Create Date,05-DEC-2022>              
-- Description: <Description,[Identity_InsertPanDetails]>              
--[Identity_InsertPanDetails] '1F18D0DF-35C9-4689-8152-2428648E69DD','BFBPG3950A','','','','','','CHECKPANDETAILS'              
-- =============================================              
CREATE    PROCEDURE [dbo].[Identity_InsertPanDetails] @UserId VARCHAR(50) = NULL  
 ,@PanNumber VARCHAR(20) = NULL  
 ,@Name VARCHAR(100) = NULL  
 ,@FatherName VARCHAR(100) = NULL  
 ,@DOB VARCHAR(20) = NULL  
 ,@Instanceid VARCHAR(100) = NULL  
 ,@InstancecallbackUrl VARCHAR(100) = NULL  
 ,@Condition VARCHAR(50) = NULL  
AS  
BEGIN  
 BEGIN TRY  
  IF (@Condition = 'CHECKPANDETAILS')  
  BEGIN  
   SELECT IUP.[PanVerificationId]  
    ,IUP.[Name] AS [Name]  
    ,IUP.[DOB]  
    ,IUP.[PanNumber]  
    ,IU.EmailId  
    ,IUP.FatherName  
    ,1 AS IsUserExists  
   FROM Identity_User IU WITH (NOLOCK)  
   INNER JOIN Identity_PanVerification IUP WITH (NOLOCK) ON IU.UserId = IUP.UserId  
   WHERE IUP.PanNumber = @PanNumber  
    AND IU.IsActive = 1  
    AND IUP.IsActive = 1  
    --AND DATEDIFF(MONTH, CAST(IUP.PanAddUpdateOn AS DATE), CAST(GETDATE() AS DATE)) <= 6              
    /*              
     SELECT IUP.[PanVerificationId], IUP.[Name] as [Name], IUP.[DOB], IUP.[PanNumber], IU.EmailId              
     FROM Identity_User IU              
     LEFT JOIN Identity_PanVerification IUP WITH(NOLOCK) ON IU.UserId = IUP.UserId              
     WHERE IU.UserId = @UserId AND IUP.PanNumber = @PanNumber              
     AND DATEDIFF(MONTH, CAST(IUP.PanAddUpdateOn AS DATE), CAST(GETDATE() AS DATE)) <= 6              
    */  
  END  
  ELSE  
  BEGIN  
   IF EXISTS (  
     SELECT TOP 1 PanNumber  
     FROM dbo.Identity_PanVerification WITH (NOLOCK)  
     WHERE PanNumber = @PanNumber  
      AND IsActive = 1  
      AND UserId = @UserId  
      AND UserId IN (  
       SELECT UserId  
       FROM Identity_User  
       WHERE UserId = @UserId  
        AND IsActive = 1  
       )  
     )  
   BEGIN  
    UPDATE dbo.Identity_PanVerification  
    SET PanNumber = @PanNumber  
     ,Name = @Name  
     ,FatherName = @FatherName  
     ,DOB = @DOB  
     ,Instanceid = @Instanceid  
     ,InstancecallbackUrl = @InstancecallbackUrl  
     ,UpdatedBy = '1'  
     ,UpdatedOn = GETDATE()  
     ,PanAddUpdateOn = GETDATE()  
     ,IsActive = 1  
    WHERE PanNumber = @PanNumber  
  
    UPDATE Identity_User  
    SET UserName = @Name , UpdatedOn = GETDATE() 
    WHERE UserId = @UserId  
  
    UPDATE Identity_UserDetail  
    SET DateofBirth = @DOB  
     ,PAN = @PanNumber  , UpdatedOn = GETDATE()
    WHERE UserId = @UserId  
   END  
   ELSE  
   BEGIN  
    INSERT INTO dbo.Identity_PanVerification (  
     PanNumber  
     ,Name  
     ,FatherName  
     ,DOB  
     ,InstanceId  
     ,InstanceCallbackUrl  
     ,CreatedBy  
     ,CreatedOn  
     ,PanAddUpdateOn  
     ,UserId  
     ,IsActive  
     )  
    VALUES (  
     @PanNumber  
     ,@Name  
     ,@FatherName  
     ,@DOB  
     ,@Instanceid  
     ,@InstancecallbackUrl  
     ,'1'  
     ,GETDATE()  
     ,GETDATE()  
     ,@UserId  
     ,1  
     )  
  
    IF NOT EXISTS (  
      SELECT TOP 1 USerId  
      FROM Identity_UserDetail WITH (NOLOCK)  
      WHERE UserId = @UserId  
      )  
    BEGIN  
     INSERT INTO dbo.Identity_UserDetail (  
      UserId  
      ,DateofBirth  
      ,PAN  
      ,IsActive  
      ,CreatedBy  
      )  
     VALUES (  
      @UserId  
      ,@DOB  
      ,@PanNumber  
      ,1  
      ,GETDATE()  
      )  
    END  
    ELSE  
    BEGIN  
     UPDATE Identity_UserDetail  
     SET DateofBirth = @DOB  
      ,PAN = @PanNumber , UpdatedOn = GETDATE() 
     WHERE UserId = @UserId  
    END  
  
    UPDATE Identity_User  
    SET UserName = @Name  , UpdatedOn = GETDATE()
    WHERE UserId = @UserId  
   END  
  
   --UPDATE PROFILE STAGE FOR USERS                  
   UPDATE Identity_User  
   SET UserProfileStage = 1 , UpdatedOn = GETDATE() 
   WHERE UserId = @UserId  
       
   SELECT IUP.[PanVerificationId]  
    ,IUP.[Name] AS Name  
    ,IUP.[DOB]  
    ,IUP.[PanNumber]  
    ,IU.EmailId  
    ,IUP.FatherName  
    ,0 AS IsUserExists  
   FROM Identity_User IU WITH (NOLOCK)  
   LEFT JOIN Identity_PanVerification IUP WITH (NOLOCK) ON IU.UserId = IUP.UserId  
   WHERE IU.UserId = @UserId  
    AND IUP.PanNumber = @PanNumber  
    AND IUp.IsActive = 1  
    AND DATEDIFF(MONTH, CAST(IUP.PanAddUpdateOn AS DATE), CAST(GETDATE() AS DATE)) <= 6  
  END  
  
     EXEC HeroPOSP.dbo.POSP_InsertUpdatePOSPStage @UserId, '8EB4B84F-F12D-44E2-9A62-2C3A4848ABB4'  

 END TRY  
  
 BEGIN CATCH  
  DECLARE @StrProcedure_Name VARCHAR(500)  
   ,@ErrorDetail VARCHAR(1000)  
   ,@ParameterList VARCHAR(2000)  
  
  SET @StrProcedure_Name = ERROR_PROCEDURE()  
  SET @ErrorDetail = ERROR_MESSAGE()  
  
  EXEC dbo.Identity_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name  
   ,@ErrorDetail = @ErrorDetail  
   ,@ParameterList = @ParameterList  
 END CATCH  
END 
