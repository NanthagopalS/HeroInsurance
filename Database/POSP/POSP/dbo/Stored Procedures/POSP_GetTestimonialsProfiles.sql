-- =============================================    
-- Author: <Author, YASH SINGH>    
-- Create date: <Create Date, 11-8-2023>    
-- Description: <Description,[POSP_Training]>    
--[POSP_GetTraining]    
-- =============================================    
 CREATE     PROCEDURE [dbo].[POSP_GetTestimonialsProfiles]
AS
BEGIN
	BEGIN TRY
		-- SET NOCOUNT ON added to prevent extra result sets from    
		-- interfering with SELECT statements.    
		SET NOCOUNT ON;

		SELECT Id
			,POSP_Id
			,Name
			,IMAGE
			,feedback
			,starCount
		FROM [dbo].[POSP_TestimonialsDetails] WITH (NOLOCK)
		ORDER BY IMAGE DESC
	END TRY

	BEGIN CATCH
		DECLARE @StrProcedure_Name VARCHAR(500)
			,@ErrorDetail VARCHAR(1000)
			,@ParameterList VARCHAR(2000)

		SET @StrProcedure_Name = ERROR_PROCEDURE()
		SET @ErrorDetail = ERROR_MESSAGE()

		EXEC POSP_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name
			,@ErrorDetail = @ErrorDetail
			,@ParameterList = @ParameterList
	END CATCH
END