-- =============================================
-- Author:		<Author,,AMBI GUPATA>
-- Create date: <Create Date,,25-NOV-2022>
-- Description:	<Description,,INSERT USER DETAIL>
-- =============================================
CREATE   PROCEDURE [dbo].[Insurance_InsertUserDetails] 
(
	@UserName VARCHAR(80) = NULL,
	@EmailId VARCHAR(80) = NULL,
	@Mobile VARCHAR(10) = NULL,
	@RoleId VARCHAR(100)=NULL
)
AS
BEGIN
	BEGIN TRY
		
		INSERT INTO IDENTITY_USERDETAILS (UserName,EmailId,Mobile,RoleId)
		VALUES(@UserName,@EmailId,@Mobile,@RoleId)

	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH

END
