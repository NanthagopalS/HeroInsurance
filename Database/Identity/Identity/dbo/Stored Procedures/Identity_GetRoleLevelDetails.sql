

-- =============================================
-- Author:		<Author,,Girish>
-- Create date: <Create Date,,10-Jan-2023>
-- Description:	<Description,[Identity_GetRoleLevelDetails]>
--[Identity_RoleLevel]
-- =============================================
CREATE PROCEDURE [dbo].[Identity_GetRoleLevelDetails]

AS
BEGIN
	BEGIN TRY
	    SET NOCOUNT ON;
		SELECT RoleLevelID,RoleLevelName FROM Identity_RoleLevel WITH(NOLOCK)	
	END TRY                
	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END
