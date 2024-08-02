-- =============================================  
-- Author:  <Author, Girish>  
-- Create date: <Create Date,09-01-2023>  
-- Description: <Description,,INSERT Admin_UserRole  DETAIL>  
-- =============================================  
CREATE   PROCEDURE [dbo].[Admin_InsertUserRole]   
(   
  
 @RoleTypeID varchar(100),  
 @RoleTitleName varchar(100),
 @BUID varchar(100),
 @RoleLevelID varchar(100),
 @CreatedBy varchar(100)  
)  
AS  
BEGIN  
 BEGIN TRY  
  BEGIN TRANSACTION    
  INSERT INTO [HeroIdentity].[dbo].[Identity_UserRole]   
           (  
      [RoleTypeID]  
     ,[RoleTitleName]  
     ,[BUID]  
     ,[RoleLevelID]  
     ,[CreatedBy]  
           )        
     VALUES  
            (  
    @RoleTypeID ,  
    @RoleTitleName,  
    @BUID ,  
    @RoleLevelID ,  
    @CreatedBy  
   )  
   SELECT @@IDENTITY AS [IdentityRoleId];  
     
   --SELECT IDENT_CURRENT('Admin_UserRole') as IdentityRoleId    
   --SELECT MAX(IdentityRoleId)  as IdentityRoleId FROM Admin_UserRole  
   --SELECT SCOPE_IDENTITY() AS [IdentityRoleId];  
    
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
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
