-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
  
-- =============================================  
-- Author:  <Author, Girish>  
-- Create date: <Create Date,30-Dec-2022>  
-- Description: <Description,Admin_GetUserRoleMapping Admin>  
/*   
  Admin_GetUserRoleMapping  
*/  
-- =============================================  
 CREATE   PROCEDURE [dbo].[Admin_GetUserRoleMapping]   
     
AS  
  
BEGIN  
 BEGIN TRY   
   BEGIN        
      SELECT   UM.UserID, UM.RoleID,   
      UM.ReportingUserID, UM.CategoryID,  
      UM.BUID, UM.RoleTypeID,   
      UM.IsActive,UM.UserRoleMappingId,   
      UM.CreatedBy,UM.CreatedOn,  
      U.UserName, U.EmailId, U.MobileNo,   
      U.EmpID, U.DOB, C.UserCategoryName,   
      B.BUName, RT.RoleTypeName, RM.RoleName           
     FROM   Admin_UserRoleMapping UM WITH(NOLOCK) INNER JOIN  
      [HeroIdentity].[dbo].[Identity_User] U WITH(NOLOCK) ON UM.UserID = U.UserId INNER JOIN  
      Admin_UserCategory C WITH(NOLOCK) ON UM.CategoryID = C.UserCategoryId INNER JOIN  
      Admin_BU B WITH(NOLOCK) ON UM.BUID = B.BUID INNER JOIN  
      Admin_RoleType RT WITH(NOLOCK) ON UM.RoleTypeID = RT.RoleTypeID INNER JOIN  
      [HeroIdentity].[dbo].[Identity_RoleMaster] RM WITH(NOLOCK) ON UM.RoleID = RM.RoleId 
	  ORDER By UM.CreatedBy DESC
  END  
  
 END TRY                  
 BEGIN CATCH    
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  
END
