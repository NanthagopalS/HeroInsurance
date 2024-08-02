-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- =============================================  
-- Author:  <Author, Girish>  
-- Create date: <Create Date,05-Jan-2023>  
-- Description: <Description,Admin_GetUserAndRoleMappingSearch Admin>  
-- =============================================  
 CREATE   PROCEDURE [dbo].[Admin_GetUserAndRoleMappingSearch]   
 (  
  @EMPID VARCHAR(50),  
  @RoleTypeName VARCHAR(50),  
  @isActive bit,  
  @CreatedFrom VARCHAR(50),  
     @CreatedTo VARCHAR(50)   
 )  
AS  
  
BEGIN  
 BEGIN TRY  
   SELECT  U.UserName, U.EmailId, U.MobileNo, U.EmpID,   
    U.DOB, U.Gender, RT.RoleTypeName, UM.RoleTypeID, UM.UserID,   
             UM.IdentityRoleId, IR.RoleTitleName, UM.CategoryID,  
    C.UserCategoryName, UM.IsActive, UM.UserRoleMappingId,UM.CreatedOn  
       FROM  Admin_UserRoleMapping UM WITH(NOLOCK) INNER JOIN  
             [HeroIdentity].[dbo].[Identity_User] U WITH(NOLOCK) ON UM.UserID = U.UserId INNER JOIN  
             Admin_RoleType RT WITH(NOLOCK) ON UM.RoleTypeID = RT.RoleTypeID INNER JOIN  
             [HeroIdentity].[dbo].[Identity_UserRole] IR WITH(NOLOCK) ON UM.IdentityRoleId = IR.IdentityRoleId INNER JOIN  
             Admin_UserCategory C WITH(NOLOCK) ON UM.CategoryID = C.UserCategoryId  
    Where      
    U.EMPID= @EMPID   
    OR  
    RT.RoleTypeName = @RoleTypeName    
    OR  
    UM.IsActive =@IsActive        
    OR      
    cast(UM.CreatedOn as date) BETWEEN CAST(@CreatedFrom as date) and CAST(@CreatedTo as date)  
	ORDER BY UM.CreatedOn DESC
 END TRY                  
 BEGIN CATCH    
  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                              
  SET @StrProcedure_Name=ERROR_PROCEDURE()                              
  SET @ErrorDetail=ERROR_MESSAGE()                              
  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                               
 END CATCH  
  
END 
