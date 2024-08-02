-- =============================================  
-- Author:  <Author, Girish>  
-- Create date: <Create Date,22-Dec-2022>  
-- Description: <Description,Admin_GetRoleModulePermission Admin>  
-- =============================================  
 CREATE   PROCEDURE [dbo].[Admin_GetRoleModulePermission]   
 (  
   
  @RoleTitleName VARCHAR(50)=Null,  
  @RoleTypeName VARCHAR(50)=Null,  
  @CreatedFrom VARCHAR(50)=Null,  
  @CreatedTo VARCHAR(50)=Null
   
 )  
AS  
  
BEGIN  
 BEGIN TRY  
   SELECT  UR.RoleTitleName, RT.RoleTypeName, RM.CreatedOn,   
       RM.isActive, RM.RoleModulePermissionID, RM.AddPermission,   
       RM.EditPermission, RM.ViewPermission, RM.DeletePermission,   
       RM.DownloadPermission,RM.IdentityRoleId,   
          RM.CreatedBy, RT.RoleTypeID, UR.BUID  
     FROM   Admin_RoleModulePermissionMapping RM WITH(NOLOCK) 
	 INNER JOIN  [HeroIdentity].[dbo].[Identity_UserRole] UR WITH(NOLOCK) ON RM.IdentityRoleId = UR.IdentityRoleId 
	 INNER JOIN  Admin_RoleType RT WITH(NOLOCK) ON UR.RoleTypeID = RT.RoleTypeID  
     where          
     UR.RoleTitleName=  @RoleTitleName   
     OR   
     RT.RoleTypeName =  @RoleTypeName        
     OR      
     (cast(RM.CreatedOn as date) BETWEEN @CreatedFrom and @CreatedTo )  
 END TRY                  
 BEGIN CATCH    
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  
END 
