-- =========================================================================================           
-- Author:  <Author, Suraj>        
-- Create date: <Create Date, 11-May-2023>        
-- Description: <Description, Admin_GetNotificationDetailById>  
-- Deactivate a POSP User by UserId  
-- =========================================================================================           
 CREATE    PROCEDURE [dbo].[Admin_DeactivatePOSPUserById]         
 (        
 @UserId VARCHAR(100),  
 @IsActive int = 0,  
 @DeactivatePOSPId VARCHAR(100) = null  
 )   
 As  
 Begin    
  
 BEGIN TRY  
  update [HeroIdentity].[dbo].[Identity_User] set IsActive=@IsActive  
  where UserId=@UserId  
  
  update [HeroIdentity].[dbo].[Identity_UserDetail] set IsActive=@IsActive  
  where UserId=@UserId  
  
  update [HeroIdentity].[dbo].[Identity_PanVerification] set IsActive= @IsActive  
  where UserId=@UserId  
  
  IF @DeactivatePOSPId IS NOT NULL  
  BEGIN  
  update [HeroAdmin].[dbo].[Admin_DeActivatePOSP] set IsNocGenerated=1, Status = 'DEACTIVATED' where Id=@DeactivatePOSPId;  
  EXEC HeroPOSP.dbo.POSP_InsertUpdatePOSPStage @UserId ,'B6E3FE9B-0202-486F-ADD7-C11639970690'
  END  
  
 END TRY  
  
   
 BEGIN CATCH          
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
 END CATCH        
     
END