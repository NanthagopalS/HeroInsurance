-- =============================================
-- Author:		<Author,,AMBI GUPTA>
-- Create date: <Create Date,25-Nov-2022>
-- Description:	<Description,GetMakeModelFuel>
--[Insurance_GetFuelTypeByModelId] 'FCF80694-00A0-4D3F-A2C3-3BD8C85A0ECA'
-- =============================================
CREATE       PROCEDURE [dbo].[Insurance_GetFuelTypeByModelId]
(
	@ModelId VARCHAR(50)=NULL
)
AS
BEGIN
	BEGIN TRY
		-- SET NOCOUNT ON added to prevent extra result sets from
		-- interfering with SELECT statements.
		SET NOCOUNT ON;

		SELECT DISTINCT CAST(FUEL.FuelId AS VARCHAR(50))FuelId,FuelName 
		FROM Insurance_Variant VARIANT WITH(NOLOCK)
		JOIN Insurance_Fuel FUEL WITH(NOLOCK) ON VARIANT.FuelId=FUEL.FuelId
		WHERE MODELID = @ModelId
		ORDER BY FUELNAME
  
	END TRY                
	BEGIN CATCH          
		     
		DECLARE @StrProcedure_Name varchar(500), @ErrorDetail varchar(1000), @ParameterList varchar(2000)                            
		SET @StrProcedure_Name=ERROR_PROCEDURE()                            
		SET @ErrorDetail=ERROR_MESSAGE()                            
		EXEC Insurance_InsertErrorDetail @StrProcedure_Name=@StrProcedure_Name,@ErrorDetail=@ErrorDetail,@ParameterList=@ParameterList                             
	END CATCH
END

