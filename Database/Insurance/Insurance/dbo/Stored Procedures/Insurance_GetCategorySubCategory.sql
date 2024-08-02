
-- =============================================
-- Author:		<Author,,Yash Singh>
-- Create date: <Create Date,11-Oct-2023
-- Description:	<Description,GetCategorySubCategory>
-- =============================================
CREATE   PROCEDURE [dbo].[Insurance_GetCategorySubCategory] 
AS
BEGIN
	BEGIN TRY
		

		SELECT 
			VSC.SubCategoryId
			,VSC.SubCategoryName
			,VSC.CategoryId AS MasterCategoryId
		FROM [Insurance_CommercialVehicleSubCategory] VSC WITH (NOLOCK)
		
		SELECT VC.CategoryId
			,VC.CategoryName
			,VC.IsOtherDetails
			FROM  Insurance_CommercialVehicleCategory VC WITH (NOLOCK) 

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