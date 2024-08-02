-- =============================================      
-- Author:  <Author, Parth>      
-- Create date: <Create Date,14-Feb-2023>      
-- Description: <Description,Admin_UpdateRoleDetail>      
-- =============================================      
CREATE PROCEDURE [dbo].[Admin_UpdateRoleDetail]       
(      
 @RoleId varchar(100),    
 @RoleName VARCHAR(100),      
 @RoleTypeId  VARCHAR(100),      
 @RoleLevelId  VARCHAR(100),      
 @BUId VARCHAR(100),        
 @UpdatedBy VARCHAR(100),      
 @IsActive bit  
)      
AS      
BEGIN      
 BEGIN TRY      
 IF EXISTS(SELECT DISTINCT RoleId FROM [Admin_RoleMaster] WITH(NOLOCK)  
 WHERE RoleId = @RoleId)        
  BEGIN      
  UPDATE [Admin_RoleMaster] SET      
		 [RoleName]  = @RoleName  
		,[RoleTypeID] = @RoleTypeID  
        ,[RoleLevelID]= @RoleLevelId  
        ,[BUId] = @BUId  
        ,[IsActive] = @IsActive      
        ,[UpdatedBy]= @UpdatedBy      
        ,[UpdatedOn]=getDate()        
      WHERE RoleId=@RoleId      
 END           
 END TRY                      
 BEGIN CATCH              
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                  
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                  
  SET @ErrorDetail=ERROR_MESSAGE()                                  
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                   
 END CATCH      
      
END 
