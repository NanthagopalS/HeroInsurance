--exec[Admin_GetSourcedByUserList] '45BBA540-07C5-4D4C-BEFF-771AE2FC32B0'
CREATE   PROCEDURE [dbo].[Admin_GetSourcedByUserList]                 
 (                
   @BUId VARCHAR(100) = NULL         
 )                
AS    
BEGIN  
BEGIN TRY  
declare @issales bit    
set @issales= (select top 1 IsSales from Admin_BU where BUId=@BUId and IsActive = 1)    
    
IF(@issales = 1)    
BEGIN    
 SELECT  U.UserId ,U.UserName           
 FROM [HeroAdmin].[dbo].[Admin_UserRoleMapping] (NOLOCK) UM           
 INNER JOIN [HeroIdentity].[dbo].[Identity_User] (NOLOCK) U ON UM.UserId = U.UserId           
 INNER JOIN [HeroAdmin].[dbo].[Admin_RoleType] (NOLOCK) RT ON UM.RoleTypeID = RT.RoleTypeID           
 INNER JOIN [HeroAdmin].[dbo].[Admin_RoleMaster] (NOLOCK) RM on UM.RoleId = RM.RoleId      
 INNER JOIN [HeroAdmin].[dbo].[admin_usercategory] (NOLOCK) UC on UC.UserCategoryId = UM.CategoryId    
 where      
 uc.UserCategoryId='B9D2CB66-56E3-4665-B26C-D32948ABA25C' and UM.IsActive = 1 and U.IsActive = 1 and rt.IsActive=1 and rm.IsActive=1 and uc.IsActive=1    
 and RT.RoleTypeID= 'D1E03B48-C313-4BB6-928C-E62C44E1DBDE'    
END    
ELSE    
BEGIN    
 SELECT  U.UserId ,U.UserName           
 FROM [HeroAdmin].[dbo].[Admin_UserRoleMapping] (NOLOCK) UM           
 INNER JOIN [HeroIdentity].[dbo].[Identity_User] (NOLOCK) U ON UM.UserId = U.UserId           
 INNER JOIN [HeroAdmin].[dbo].[Admin_RoleType] (NOLOCK) RT ON UM.RoleTypeID = RT.RoleTypeID           
 INNER JOIN [HeroAdmin].[dbo].[Admin_RoleMaster] (NOLOCK) RM on UM.RoleId = RM.RoleId      
 INNER JOIN [HeroAdmin].[dbo].[admin_usercategory] (NOLOCK) UC on UC.UserCategoryId = UM.CategoryId    
 where       
 uc.UserCategoryId='B9D2CB66-56E3-4665-B26C-D32948ABA25C'    
 and RT.RoleTypeName= 'C924DA2C-B0DA-40A5-B1F7-99DD6A859E16' and UM.IsActive = 1 and U.IsActive = 1 and rt.IsActive=1 and rm.IsActive=1 and uc.IsActive=1    
END  
  
END TRY  
  
BEGIN CATCH        
  DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                                    
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                    
  SET @ErrorDetail=ERROR_MESSAGE()                                    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                     
END CATCH     
 END