-- =============================================  
-- Author:  <Author,,AMBI GUPTA>  
-- Create date: <Create Date,25-Nov-2022>  
-- Description: <Description,GetVariant>  
--[Insurance_GetVariant] 'FCF80694-00A0-4D3F-A2C3-3BD8C85A0ECA','59D4C8B7-EDBB-446A-B19A-661F0A550BE2'  
-- =============================================  
CREATE   PROCEDURE [dbo].[Insurance_GetVariant] @ModelId VARCHAR(50) = NULL
	,@FuelId VARCHAR(50) = NULL
AS
BEGIN
	BEGIN TRY
		-- SET NOCOUNT ON added to prevent extra result sets from  
		-- interfering with SELECT statements.  
		SET NOCOUNT ON;

		SELECT CAST(VariantId AS VARCHAR(50)) AS VariantId
			,CASE 
				WHEN CVC.CategoryId = 1
					THEN CONCAT (
							RTRIM(VARIANT.VariantName)
							,' - '
							,VARIANT.GVW
							) -- GCV 
				WHEN CVC.CategoryId = 2
					THEN CONCAT (
							RTRIM(VARIANT.VariantName)
							,' - '
							,VARIANT.CubicCapacity
							,' - '
							,VARIANT.SeatingCapacity
							) -- PCV
				WHEN CVC.CategoryId = 3
					THEN CONCAT (
							RTRIM(VARIANT.VariantName)
							,' - '
							,VARIANT.GVW
							) -- MiscD
				ELSE CONCAT (
						RTRIM(VARIANT.VariantName)
						,' - '
						,VARIANT.CubicCapacity
						)
				END AS VariantName
		FROM Insurance_Variant VARIANT WITH (NOLOCK)
		LEFT JOIN Insurance_CommercialVehicleSubCategory CVSC WITH (NOLOCK) ON VARIANT.CVSubCategoryId = CVSC.SubCategoryId
		LEFT JOIN Insurance_CommercialVehicleCategory CVC WITH (NOLOCK) ON CVSC.CategoryId = CVC.CategoryId
		WHERE VARIANT.IsActive = 1
			AND VARIANT.ModelId = @ModelId
			AND VARIANT.FuelId = @FuelId
		ORDER BY VariantName
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


