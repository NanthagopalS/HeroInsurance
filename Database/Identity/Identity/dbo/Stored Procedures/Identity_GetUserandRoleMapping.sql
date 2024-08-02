﻿-- =============================================
-- Author:		<Author, Girish>
-- Create date: <Create Date,05-01-2023>
-- Description:	<Description,Identity_GetUserandRoleMapping>
/*	
	 Identity_GetUserandRoleMapping
*/
-- =============================================
 CREATE PROCEDURE [dbo].[Identity_GetUserandRoleMapping] 
 	 
AS

BEGIN
	BEGIN TRY	
	   BEGIN      
			SELECT  U.UserName, U.EmailId, U.MobileNo, U.EmpID, 
			 U.DOB, U.Gender, RT.RoleTypeName, UM.RoleTypeID, UM.UserID, 
             UM.IdentityRoleId, IR.RoleTitleName, UM.CategoryID,
			 C.UserCategoryName, UM.IsActive, UM.UserRoleID,UM.CreatedOn
       FROM  Identity_UserRoleMapping UM WITH(NOLOCK) INNER JOIN
             Identity_User U WITH(NOLOCK) ON UM.UserID = U.UserId INNER JOIN
             Identity_RoleType RT WITH(NOLOCK) ON UM.RoleTypeID = RT.RoleTypeID INNER JOIN
             Identity_UserRole IR WITH(NOLOCK) ON UM.IdentityRoleId = IR.IdentityRoleId INNER JOIN
             Identity_UserCategory C WITH(NOLOCK) ON UM.CategoryID = C.CategoryID
		END

	END TRY                
	BEGIN CATCH  
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END

