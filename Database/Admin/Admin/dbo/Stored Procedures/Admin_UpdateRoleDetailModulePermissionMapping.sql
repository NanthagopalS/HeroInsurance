-------- =============================================      
-- Author  :  <Author, Parth>      
-- Create Date : <Create Date, 14-Feb-2023>      
-- Description : <Description,Admin_InsertRoleDetailModulePermissionMapping>      
-- =============================================      
CREATE PROCEDURE [dbo].[Admin_UpdateRoleDetailModulePermissionMapping]           
(        
 --@RoleModulePermissionId varchar(100),
 @ItemNo int = 0,
 @ModuleId varchar(100),  
 @RoleId varchar(100),
 @RoleTypeId varchar(100),  
 @AddPermission bit ,          
 @EditPermission bit,          
 @ViewPermission bit,          
 @DeletePermission bit,          
 @DownloadPermission bit,          
 @UpdatedBy VARCHAR(50) = NULL          
)          
AS          
BEGIN          
 BEGIN TRY          
  BEGIN TRANSACTION      
	
	IF(@ItemNo = 1)
	BEGIN
		DELETE FROM Admin_RoleModulePermissionMapping WHERE RoleId = @RoleId
	END

	INSERT INTO [dbo].[Admin_RoleModulePermissionMapping]          
        ([ModuleID], [RoleTypeId], [RoleId], [AddPermission], [EditPermission], [ViewPermission], [DeletePermission],[DownloadPermission], [CreatedBy])          
	VALUES (@ModuleId, @RoleTypeId, @RoleId, @AddPermission, @EditPermission, @ViewPermission, @DeletePermission, @DownloadPermission, @UpdatedBy)

   IF @@TRANCOUNT > 0          
            COMMIT          
 END TRY                          
 BEGIN CATCH                    
 IF @@TRANCOUNT > 0          
        ROLLBACK            
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                      
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                      
  SET @ErrorDetail=ERROR_MESSAGE()                                      
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                       
 END CATCH          
          
END 
