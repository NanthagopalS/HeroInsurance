
-- EXEC  [dbo].[Identity_GetICICIPOSPDetails] '017776C0-8769-4A4B-BA36-90761B2B8F69'

-- =============================================
-- Author:		<Author,,Nanthagopal >
-- Create date: <Create Date,,17/04/2023>
-- Description:	<Description,GetPOSPDetails>
-- =============================================
CREATE PROCEDURE [dbo].[Identity_GetICICIPOSPDetails]
@UserId VARCHAR(100) = NULL
AS
BEGIN
   BEGIN TRY  
		SELECT 
		USERS.POSPId,
		USERDETAIL.PAN,
		USERDETAIL.AadhaarNumber,
		ROLES.RoleName
		FROM Identity_User USERS WITH(NOLOCK)
		LEFT JOIN Identity_UserDetail  USERDETAIL WITH(NOLOCK) ON USERDETAIL.UserId = USERS.UserId
		LEFT JOIN Identity_RoleMaster ROLES WITH(NOLOCK) ON USERS.RoleId = ROLES.RoleId
		WHERE USERS.UserId = @UserId
   END TRY  
   BEGIN CATCH  
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END