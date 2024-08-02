-- =============================================
-- Author: <Author, ANKIT GHOSH>
-- Create date: <Create Date, 2-1-2023>
-- Description:	<Description,[POSP_Training]>
--[POSP_GetTraining]
-- =============================================
CREATE     PROCEDURE [dbo].[POSP_GetTraining]

AS
BEGIN
	BEGIN TRY
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		SELECT Id, UserId, GeneralInsuranceStartDateTime, GeneralInsuranceEndDateTime, LifeInsuranceStartDateTime, LifeInsuranceEndDateTime,  IsGeneralInsuranceCompleted, IsLifeInsuranceCompleted, IsTrainingCompleted, IsActive, CreatedBy, CreatedOn, UpdatedBy, UpdatedOn
		FROM [dbo].[POSP_Training] WITH(NOLOCK)
	
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC POSP_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList
	END CATCH
END
