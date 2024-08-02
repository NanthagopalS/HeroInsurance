-- =============================================  
-- Author:  <Author, Girish>  
-- Create date: <Create Date,29-DEC-2022>  
-- Description: <Description,,Admin_InsertUserRoleMapping>   
-- =============================================  
CREATE   PROCEDURE [dbo].[Admin_InsertUserRoleMapping]   
(   
    @UserID varchar(100),  
    @RoleID varchar(100),
    @ReportingUserID varchar(100),
    @CategoryID varchar(100),
    @BUID varchar(100), 
    @RoleTypeID varchar(100),
    @IsActive bit  
   
)  
AS  
BEGIN  
 BEGIN TRY  
  BEGIN TRANSACTION  
  INSERT INTO [dbo].[Admin_UserRoleMapping]  
           (  
      [UserID]  
           ,[RoleID]  
           ,[ReportingUserID]  
           ,[CategoryID]  
           ,[BUID]  
           ,[RoleTypeID]  
           ,[IsActive])  
     VALUES(  
           @UserID,  
           @RoleID,   
           @ReportingUserID,   
           @CategoryID,  
           @BUID,   
           @RoleTypeID,  
           @IsActive)  
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
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
