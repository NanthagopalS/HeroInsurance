
-- =============================================
-- Author:		<Author, Girish>
-- Create date: <Create Date,30-Dec-2022>
-- Description:	<Description,Identity_GetUserRoleMapping Admin>
/*	
	 Identity_GetUserRoleMapping
*/
-- =============================================
 CREATE PROCEDURE [dbo].[Identity_GetUserRoleMapping] 
 	 
AS

BEGIN
	BEGIN TRY	
		 BEGIN      
			   SELECT   UM.UserID, UM.RoleID, 
						UM.ReportingUserID, UM.CategoryID,
						UM.BUID, UM.RoleTypeID, 
						UM.IsActive,UM.UserRoleID, 
						UM.CreatedBy,UM.CreatedOn,
						U.UserName, U.EmailId, U.MobileNo, 
						U.EmpID, U.DOB, C.UserCategoryName, 
						B.BUName, RT.RoleTypeName, RM.RoleName         
			  FROM   Identity_UserRoleMapping UM WITH(NOLOCK) INNER JOIN
					 Identity_User U WITH(NOLOCK) ON UM.UserID = U.UserId INNER JOIN
					 Identity_UserCategory C WITH(NOLOCK) ON UM.CategoryID = C.CategoryID INNER JOIN
					 Identity_BU B WITH(NOLOCK) ON UM.BUID = B.BUID INNER JOIN
					 Identity_RoleType RT WITH(NOLOCK) ON UM.RoleTypeID = RT.RoleTypeID INNER JOIN
					 Identity_RoleMaster RM WITH(NOLOCK) ON UM.RoleID = RM.RoleId
		END

	END TRY                
	BEGIN CATCH  
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END

