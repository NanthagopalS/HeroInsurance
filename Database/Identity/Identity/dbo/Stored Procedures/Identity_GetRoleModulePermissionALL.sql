
-- =============================================
-- Author:		<Author, Girish>
-- Create date: <Create Date,22-Dec-2022>
-- Description:	<Description,Identity_GetRoleModulePermission Admin>
-- =============================================
 CREATE PROCEDURE [dbo].[Identity_GetRoleModulePermissionALL] 

AS

BEGIN
	BEGIN TRY			
	    			SELECT  UR.RoleTitleName, RT.RoleTypeName, RM.CreatedOn, 
							RM.isActive, RM.RoleModulePermissionID, RM.AddPermission, 
							RM.EditPermission, RM.ViewPermission, RM.DeletePermission, 
							RM.DownloadPermission, RM.IdentityRoleId, 
						    RM.CreatedBy, RT.RoleTypeID, UR.BUID
					FROM   Identity_RoleModulePermissionMapping RM WITH(NOLOCK) INNER JOIN
						    Identity_UserRole UR WITH(NOLOCK) ON RM.IdentityRoleId = UR.IdentityRoleId INNER JOIN
							Identity_RoleType RT WITH(NOLOCK) ON UR.RoleTypeID = RT.RoleTypeID
	END TRY                
	BEGIN CATCH  
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END

