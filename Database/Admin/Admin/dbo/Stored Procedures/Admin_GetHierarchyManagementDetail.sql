-- =============================================        
-- Author  :  <Author, Parth>        
-- Create Date : <Create Date,14-Feb-2023>        
-- Description : <Description,Admin_GetParticularRoleDetail>        
-- =============================================        
CREATE   PROCEDURE [dbo].[Admin_GetHierarchyManagementDetail]         
(        
  @RoleId VARCHAR(100),  
  @RoleTypeId VARCHAR(100),   
  @ParentUserId VARCHAR(100) = NULL,
  @ParentUserRoleId VARCHAR(100) = NULL
)        
AS        
BEGIN        
	 BEGIN TRY
 
		 DECLARE @SearchRoleId VARCHAR(100) = NULL


		 IF(@RoleTypeId = 'B9D2CB66-56E3-4665-B26C-D32948ABA25C')
		 BEGIN

			 IF((ISNULL(@ParentUserId,'') <> '' AND ISNULL(@ParentUserRoleId,'') <> '') OR (@ParentUserId != null AND @ParentUserRoleId != null))
			 BEGIN		
					  SELECT U.UserId, U.UserName, RM.RoleId, Rm.RoleName, UD.ProfilePictureID AS ProfilePictureStream
					  FROM Admin_UserRoleMapping UM WITH(NOLOCK)
						  INNER JOIN [HeroIdentity].[dbo].[Identity_User] U WITH(NOLOCK) ON U.UserId = UM.UserId
						  LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] UD WITH(NOLOCK) on UD.UserId = U.UserId AND UD.UserId = UM.UserId
						  INNER JOIN Admin_RoleMaster RM WITH(NOLOCK) on RM.RoleTypeID = UM.RoleTypeId --AND RM.RoleTypeId = @RoleTypeId
					  WHERE UM.ReportingUserId = @ParentUserId AND UM.RoleId = RM.RoleId				
							AND UM.RoleId IN (SELECT RoleId FROM Admin_UserRoleMapping WITH(NOLOCK) where ReportingIdentityRoleId = @ParentUserRoleId AND ReportingUserId = @ParentUserId AND IsActive = 1) 
							AND RM.RoleId IN (SELECT RoleId FROM Admin_UserRoleMapping WITH(NOLOCK) where ReportingIdentityRoleId = @ParentUserRoleId AND ReportingUserId = @ParentUserId AND IsActive = 1) 
							AND UM.UserId != @ParentUserId AND UM.IsActive = 1 AND U.IsActive = 1 AND RM.IsActive = 1
					  ORDER BY UM.CreatedOn DESC
				  
					  SELECT @ParentUserId  AS ParentUserId, @ParentUserRoleId  AS ParentUserRoleId

					  --UM.RoleTypeId = @RoleTypeId AND
			 END
			 ELSE
			 BEGIN
					  SELECT U.UserId, U.UserName, RM.RoleId, RM.RoleName, UD.ProfilePictureID AS ProfilePictureStream
					  FROM Admin_UserRoleMapping UM WITH(NOLOCK)
						  INNER JOIN [HeroIdentity].[dbo].[Identity_User] U WITH(NOLOCK) ON U.UserId = UM.UserId
						  LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] UD WITH(NOLOCK) on UD.UserId = U.UserId
						  INNER JOIN Admin_RoleMaster RM WITH(NOLOCK) on RM.RoleTypeID = UM.RoleTypeId AND RM.RoleId = UM.RoleId --AND RM.RoleId = @RoleId AND RM.RoleTypeId = @RoleTypeId
					  where UM.UserId = @RoleId 
					  AND UM.IsActive = 1 AND U.IsActive = 1 AND RM.IsActive = 1
					  ORDER BY UM.CreatedOn DESC

					  SELECT NULL AS ParentUserId, NULL AS ParentUserRoleId
			 END

		 END
		 ELSE IF((ISNULL(@ParentUserId,'') <> '' AND ISNULL(@ParentUserRoleId,'') <> '') OR (@ParentUserId != null AND @ParentUserRoleId != null))
		 BEGIN		
				  SELECT U.UserId, U.UserName, RM.RoleId, Rm.RoleName, UD.ProfilePictureID AS ProfilePictureStream
				  FROM Admin_UserRoleMapping UM WITH(NOLOCK)
					  INNER JOIN [HeroIdentity].[dbo].[Identity_User] U WITH(NOLOCK) ON U.UserId = UM.UserId
					  LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] UD WITH(NOLOCK) on UD.UserId = U.UserId AND UD.UserId = UM.UserId
					  INNER JOIN Admin_RoleMaster RM WITH(NOLOCK) on RM.RoleTypeID = UM.RoleTypeId --AND RM.RoleTypeId = @RoleTypeId
				  WHERE UM.ReportingUserId = @ParentUserId AND UM.RoleId = RM.RoleId				
						AND UM.RoleId IN (SELECT RoleId FROM Admin_UserRoleMapping WITH(NOLOCK) where ReportingIdentityRoleId = @ParentUserRoleId AND ReportingUserId = @ParentUserId AND IsActive = 1) 
						AND RM.RoleId IN (SELECT RoleId FROM Admin_UserRoleMapping WITH(NOLOCK) where ReportingIdentityRoleId = @ParentUserRoleId AND ReportingUserId = @ParentUserId AND IsActive = 1) 
						AND UM.UserId != @ParentUserId AND UM.IsActive = 1 AND U.IsActive = 1 AND RM.IsActive = 1
				  ORDER BY UM.CreatedOn DESC
				  
				  SELECT @ParentUserId  AS ParentUserId, @ParentUserRoleId  AS ParentUserRoleId

				  --UM.RoleTypeId = @RoleTypeId AND
		 END
		 ELSE
		 BEGIN
				  SELECT U.UserId, U.UserName, RM.RoleId, RM.RoleName, UD.ProfilePictureID AS ProfilePictureStream
				  FROM Admin_UserRoleMapping UM WITH(NOLOCK)
					  INNER JOIN [HeroIdentity].[dbo].[Identity_User] U WITH(NOLOCK) ON U.UserId = UM.UserId
					  LEFT JOIN [HeroIdentity].[dbo].[Identity_UserDetail] UD WITH(NOLOCK) on UD.UserId = U.UserId
					  INNER JOIN Admin_RoleMaster RM WITH(NOLOCK) on RM.RoleTypeID = UM.RoleTypeId AND RM.RoleId = @RoleId AND RM.RoleTypeId = @RoleTypeId
				  where UM.RoleId = @RoleId AND UM.RoleTypeId = @RoleTypeId
				  AND UM.IsActive = 1 AND U.IsActive = 1 AND RM.IsActive = 1
				  ORDER BY UM.CreatedOn DESC
				  SELECT NULL AS ParentUserId, NULL AS ParentUserRoleId
		 END
	END TRY                        
	BEGIN CATCH          
	  
	  DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000) 
	  SET @StrProcedure_Name=ERROR_PROCEDURE() 
	  SET @ErrorDetail=ERROR_MESSAGE() 
	  EXEC Admin_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name, @ErrorDetail=@ErrorDetail, @ParameterList=@ParameterList                            
	END CATCH 
END 
