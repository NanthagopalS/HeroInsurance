
-- =============================================
-- Author:		<Author, Girish>
-- Create date: <Create Date,22-DEC-2022>
-- Description:	<Description,,Update ROLEMODULEPERMISSIONMAPPING  DETAIL>
-- =============================================
CREATE PROCEDURE [dbo].[Identity_UpdateRoleModulePermissionMapping] 
(
	@RoleID VARCHAR(50),
	@ModuleID int ,
	@Roletypeid int,
	@AddPermission bit ,
	@EditPermission bit,
	@ViewPermission bit,
	@DeletePermission bit,
	@DownloadPermission bit,
	@UpdatedBy VARCHAR(50)=null,
	@UpdatedOn Datetime=null,	
	@isActive bit =0
)
AS
BEGIN
	BEGIN TRY
		BEGIN TRANSACTION
	UPDATE [dbo].[Identity_RoleModulePermissionMapping]
    SET 
		   [ModuleID] = @ModuleID
		  ,[AddPermission] = @AddPermission
		  ,[EditPermission] = @EditPermission
		  ,[ViewPermission] = @ViewPermission
		  ,[DeletePermission] = @DeletePermission
		  ,[DownloadPermission] = @DownloadPermission  
		  ,[UpdatedBy] = @UpdatedBy
	      ,[UpdatedOn]= @UpdatedOn
		  ,[isActive] = @isActive 
	WHERE 
	[RoleID] = @RoleID and [RoleTypeID] = @RoleTypeID
	 	IF @@TRANCOUNT > 0
            COMMIT
	END TRY                
	BEGIN CATCH          
	IF @@TRANCOUNT > 0
        ROLLBACK  
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
