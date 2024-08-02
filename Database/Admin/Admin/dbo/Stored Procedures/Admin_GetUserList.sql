-------- =============================================  
-- Author:  <Author, Ankit>  
-- Create date: <Create Date, 24-Feb-2023>  
-- Description: <Description,,Admin_GetUserList>  
-- =============================================  
CREATE     PROCEDURE [dbo].[Admin_GetUserList]   
 (
	@RoleId VARCHAR(100) = NULL -- RoleId / POSP
 )
AS  
BEGIN  
 BEGIN TRY   

	IF(@RoleId = 'BU')
	BEGIN

		SELECT IU.UserId, IU.UserName 
		FROM [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK)
		WHERE IsActive = 1 
		AND UserId IN 
		(
			  SELECT UserId
			  FROM [HeroAdmin].[dbo].[Admin_UserRoleMapping] WITH(NOLOCK)
			  WHERE IsActive = 1 AND RoleId IN 
			  (
				SELECT RoleId FROM [HeroAdmin].[dbo].[Admin_RoleMaster] WITH(NOLOCK) WHERE IsActive = 1 AND RoleTypeID IN 
					(
						SELECT RoleTypeID FROM [HeroAdmin].[dbo].[Admin_RoleType] WITH(NOLOCK) WHERE RoleTypeName = 'Central'
					)
			  )
		) 
		ORDER BY IU.CreatedOn DESC
	END
	ELSE IF(@RoleId IS NOT NULL AND @RoleId <> '' AND @RoleId != 'POSP')
	 BEGIN

		SELECT IU.UserId, IU.UserName 
		FROM [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK)
		WHERE IsActive = 1 AND UserId IN (SELECT UserId FROM [HeroAdmin].[dbo].[Admin_UserRoleMapping] WITH(NOLOCK) WHERE RoleId = @RoleId) 
		ORDER BY IU.CreatedOn DESC

	 END
	 ELSE IF(@RoleId = 'POSP')
	 BEGIN

		SELECT IU.UserId, IU.UserName 
		FROM [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK)
		WHERE IsActive = 1 AND UserId IN (SELECT RoleId FROM [HeroIdentity].[dbo].[Identity_RoleMaster] WITH(NOLOCK) WHERE RoleName = 'POSP') 
		ORDER BY IU.CreatedOn DESC
	  
	  END
	  ELSE
	  BEGIN
		
		SELECT IU.UserId, IU.UserName 
		FROM [HeroIdentity].[dbo].[Identity_User] as IU WITH(NOLOCK)
		WHERE IsActive = 1 AND UserId NOT IN (SELECT RoleId FROM [HeroIdentity].[dbo].[Identity_RoleMaster] WITH(NOLOCK) WHERE RoleName = 'POSP') ORDER BY IU.UserName
	  
	  END

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
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
