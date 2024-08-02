-- =============================================      
-- Author:  <Author, Girish>      
-- Create date: <Create Date,22-DEC-2022>      
-- Description: <Description,,INSERT ROLEMODULEPERMISSIONMAPPING  DETAIL>      
-- =============================================      
CREATE   PROCEDURE [dbo].[Admin_InsertRoleModulePermissionMapping]       
(         
 @ModuleID varchar(100), 
 @RoleTypeId varchar(100),
 @IdentityRoleId varchar(100),  
 @AddPermission bit ,      
 @EditPermission bit,      
 @ViewPermission bit,      
 @DeletePermission bit,      
 @DownloadPermission bit,      
 @CreatedBy VARCHAR(50) = NULL      
)      
AS      
BEGIN      
 BEGIN TRY      
  BEGIN TRANSACTION      
  INSERT INTO [dbo].[Admin_RoleModulePermissionMapping]      
           (      
            [ModuleID]   
		   ,[RoleTypeId]
           ,[IdentityRoleId]     
           ,[AddPermission]      
           ,[EditPermission]      
           ,[ViewPermission]      
           ,[DeletePermission]      
           ,[DownloadPermission]      
     ,[CreatedBy])      
      
     VALUES      
           (          
    @ModuleID ,         
	@RoleTypeId,
    @IdentityRoleId,    
    @AddPermission ,      
    @EditPermission,      
    @ViewPermission,      
    @DeletePermission,      
    @DownloadPermission,      
    @CreatedBy      
     )      
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
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------  
