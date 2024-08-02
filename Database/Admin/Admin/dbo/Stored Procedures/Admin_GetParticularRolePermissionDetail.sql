-- =============================================    
-- Author		:  <Author, Parth>    
-- Create Date	: <Create Date,14-Feb-2023>    
-- Description	: <Description,Admin_GetParticularRoleDetail>    
-- =============================================    
CREATE PROCEDURE [dbo].[Admin_GetParticularRolePermissionDetail]     
(    
  @RoleId VARCHAR(100)
)    
AS    
BEGIN    
 BEGIN TRY    
     
	SELECT RM.RoleModulePermissionId, RM.RoleTypeId, RM.ModuleId, AM.ModuleName, AM.ModuleGroupName, RM.AddPermission, RM.EditPermission, RM.ViewPermission, RM.DeletePermission, RM.DownloadPermission 
	FROM Admin_RoleModulePermissionMapping RM WITH(NOLOCK)
	INNER JOIN Admin_Module AM WITH(NOLOCK) ON AM.ModuleId = RM.ModuleId
	WHERE RM.RoleId = @RoleId AND RM.IsActive = 1
	ORDER BY AM.ModuleGroupName, AM.PriorityIndex

END TRY                    
BEGIN CATCH      
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
END   
