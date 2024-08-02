

-- =============================================
-- Author:		<Author, Girish S F>
-- Create date: <Create Date,21-DEC-2022>
-- Description:	<Description,,Check USER EMAIL EXIST>
-- =============================================
CREATE PROCEDURE [dbo].[Identity_VerfyUserEmail] 
(
	@EmailId VARCHAR(100) = NULL,	
	@UserId VARCHAR(50) = NULL	
)
AS
BEGIN
	BEGIN TRY		
		BEGIN

		    DECLARE @RoleId VARCHAR(50) =NULL
		    SELECT @RoleId= ROLEID FROM Identity_RoleMaster WITH(NOLOCK) WHERE RoleName='ADMIN'
			IF EXISTS(SELECT TOP 1 UserId , EmailId  FROM [Identity_User] WITH(NOLOCK) WHERE  UserId = @UserId AND EmailId = @EmailId and RoleId=@RoleId )
			BEGIN	
				SELECT 1 IsValidEmail
			END
			ELSE
			BEGIN
				SELECT 0 IsValidEmail
			END
		END
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
