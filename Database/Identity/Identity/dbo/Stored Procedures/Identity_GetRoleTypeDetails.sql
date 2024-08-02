

-- =============================================
-- Author:		<Author,,Girish>
-- Create date: <Create Date,,19-Dec-2022>
-- Description:	<Description,[Identity_GetRoleTypeDetails]>
--[Identity_RoleType]
-- =============================================
CREATE PROCEDURE [dbo].[Identity_GetRoleTypeDetails]

AS
BEGIN
	BEGIN TRY
	    SET NOCOUNT ON;
		SELECT * FROM Identity_RoleType WITH(NOLOCK)
	
	END TRY                
	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END
