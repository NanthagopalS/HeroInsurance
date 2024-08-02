-- =============================================    
-- Author		:  <Author, Parth>    
-- Create Date	: <Create Date,14-Feb-2023>    
-- Description	: <Description,Admin_GetParticularRoleDetail>    
-- =============================================    
CREATE PROCEDURE [dbo].[Admin_GetParticularRoleDetail]     
(    
  @RoleId VARCHAR(100)
)    
AS    
BEGIN    
 BEGIN TRY    
   SELECT RM.RoleTypeID, RM.RoleName, Rm.RoleLevelID, Rm.BUId, Rm.IsActive
     FROM Admin_RoleMaster RM WITH(NOLOCK)
     where RM.RoleId = @RoleId    	 

END TRY                    
BEGIN CATCH      
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                                
  SET @StrProcedure_Name=ERROR_PROCEDURE()                                
  SET @ErrorDetail=ERROR_MESSAGE()                                
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                                 
 END CATCH    
END   
