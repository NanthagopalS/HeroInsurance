-------- =============================================        
-- Author  :  <Author, Parth>        
-- Create Date : <Create Date, 14-Feb-2023>        
-- Description : <Description,Admin_InsertRoleDetail>        
-- =============================================        
CREATE PROCEDURE [dbo].[Admin_InsertRoleDetail]         
(        
	 @RoleTypeId varchar(100),      
	 @RoleName varchar(100),      
	 @BUId varchar(100),    
	 @RoleLevelId varchar(100),    
	 @IsActive bit,        
	 @CreatedBy varchar(50)           
)        
AS        
BEGIN        
 BEGIN TRY        
  BEGIN TRANSACTION        
  INSERT INTO [dbo].[Admin_RoleMaster]        
      ([RoleTypeID]        
     ,[RoleName]        
     ,[RoleLevelID]    
     ,[BUId]    
     ,[IsActive]    
     ,[CreatedBy]
	 ,[CreatedOn])        
     VALUES(        
    @RoleTypeId        
   ,@RoleName         
   ,@RoleLevelId    
   ,@BUId    
   ,@IsActive    
   ,@CreatedBy  
   ,GETDATE()
  )     
  SELECT top 1 [RoleId] AS IdentityRoleId From [dbo].[Admin_RoleMaster] WITH(NOLOCK)
   WHERE RoleTypeId = @RoleTypeId OR RoleLevelId = @RoleLevelId OR  
   [BUId] = @BUId ORDER BY [CreatedOn] DESC
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
