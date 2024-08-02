

-- =============================================
-- Author:		<Author,,Girish>
-- Create date: <Create Date,,19-Dec-2022>
-- Description:	<Description,[GetModuleDetails]>
--[Identity_Module]
-- =============================================
CREATE PROCEDURE [dbo].[Identity_GetModuleDetails]

AS
BEGIN
	BEGIN TRY
	    SET NOCOUNT ON;
		SELECT * FROM [dbo].[Identity_Module] WITH(NOLOCK) 
	
	END TRY                
	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500), @ErrorDetail VARCHAR(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Identity_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END
