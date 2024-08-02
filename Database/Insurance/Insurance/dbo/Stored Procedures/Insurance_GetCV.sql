
-- exec [Insurance_GetCV] 'AB5FED07-6B34-40E2-8F7C-00079200D35A'
CREATE    PROCEDURE [dbo].[Insurance_GetCV] (    
 @VariantId VARCHAR(50) = NULL       
 )    
AS    
BEGIN    
 BEGIN TRY  

	Declare @CVSubCategoryId VARCHAR(100) 
	Set @CVSubCategoryId = (Select Variant.CVSubCategoryId from Insurance_Variant Variant WITH (NOLOCK) 
	LEFT JOIN Insurance_CommercialVehicleSubCategory CVSubCategory WITH (NOLOCK) On CAST(Variant.CVSubCategoryId as varchar) = CVSubCategory.CVBodyTypeOptions
	where Variant.VariantId = @VariantId)

	Declare @CVBodyType VARCHAR(MAX)
	Set @CVBodyType = (Select CVBodyTypeOptions from Insurance_CommercialVehicleSubCategory where SubCategoryId = @CVSubCategoryId)
    

	IF OBJECT_ID('#testCVBodyType') IS NOT NULL
    DROP TABLE #testCVBodyType
	Create table #testCVBodyType(
		CVBodyType INT
	)
	Insert into #testCVBodyType (CVBodyType) Select value from string_split(@CVBodyType,',')
    

	Select VehicleBodyId, VehicleBodyName from Insurance_CommercialVehicleBodyType VehicleBody WITH (NOLOCK) 
	Inner JOIN #testCVBodyType CVBody WITH(NOLOCK) ON CVBody.CVBodyType = VehicleBody.VehicleBodyId
	and VehicleBody.IsActive= 1


	Declare @CVUsageNature VARCHAR(MAX)
	Set @CVUsageNature = (Select CVUsageNatureOptions from Insurance_CommercialVehicleSubCategory where SubCategoryId = @CVSubCategoryId)
    

	IF OBJECT_ID('#testCVUsageNature') IS NOT NULL
    DROP TABLE #testCVUsageNature
	Create table #testCVUsageNature(
		CVUsageNature INT
	)
	Insert into #testCVUsageNature (CVUsageNature) Select value from string_split(@CVUsageNature,',')

	Select UsageNatureId, UsageNatureName from Insurance_CommercialVehicleUsageNature UsageNature WITH (NOLOCK) 
	Inner JOIN #testCVUsageNature CVUsageNature WITH(NOLOCK) ON CVUsageNature.CVUsageNature = UsageNature.UsageNatureId
	where UsageNature.IsActive = 1 

	--Select * from Insurance_CommercialVehicleUsageNature where UsageNatureId In (Select @CVUsageNature)

	
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