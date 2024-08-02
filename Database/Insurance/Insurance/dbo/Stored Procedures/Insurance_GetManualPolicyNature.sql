-- =============================================  
-- Author:  <Author,,YASH SINGH>  
-- Create date: <Create Date,,>  
-- Description: <Description,,>  
-- =============================================  
CREATE   PROCEDURE [dbo].[Insurance_GetManualPolicyNature]
AS
BEGIN
	BEGIN TRY
		SELECT PolicyNatureTypeId
			,PolicyNatureTypeName
		FROM Insurance_ManualPolicyNatureTypeMaster WITH (NOLOCK)
		WHERE IsActive = 1
	END TRY

	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500)
			,@ErrorDetail VARCHAR(1000)
			,@ParameterList VARCHAR(2000)

		SET @StrProcedure_Name = ERROR_PROCEDURE()
		SET @ErrorDetail = ERROR_MESSAGE()

		EXEC Insurance_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name
			,@ErrorDetail = @ErrorDetail
			,@ParameterList = @ParameterList
	END CATCH
END