-- exec [Insurance_GetCommercialVehicleOtherDetailsAskOptions] '0012A2CD-20ED-4557-86A6-B753EF66B9FC'          
CREATE    PROCEDURE [dbo].[Insurance_GetCommercialVehicleOtherDetailsAskOptions] (@VariantId VARCHAR(50) = NULL)      
AS      
BEGIN      
   BEGIN TRY      
      DECLARE @CVSubCategoryId VARCHAR(100)      
      
      SET @CVSubCategoryId = (      
            SELECT Variant.CVSubCategoryId      
            FROM Insurance_Variant Variant WITH (NOLOCK)      
            LEFT JOIN Insurance_CommercialVehicleSubCategory CVSubCategory WITH (NOLOCK) ON CAST(Variant.CVSubCategoryId AS VARCHAR) = CVSubCategory.CVBodyTypeOptions      
            WHERE Variant.VariantId = @VariantId      
            )      
      
      DECLARE @CVBodyType VARCHAR(MAX)      
      
      SET @CVBodyType = (      
            SELECT CVBodyTypeOptions      
            FROM Insurance_CommercialVehicleSubCategory      
            WHERE SubCategoryId = @CVSubCategoryId      
            )      
      
      DECLARE @AskForHazardusVehicleUse BIT = (      
            SELECT TOP 1 AskForHazardusVehicleUse      
            FROM Insurance_CommercialVehicleSubCategory      
            WHERE SubCategoryId = @CVSubCategoryId      
            )      
      DECLARE @AskForIfTrailer BIT = (      
            SELECT TOP 1 AskForIfTrailer      
            FROM Insurance_CommercialVehicleSubCategory      
            WHERE SubCategoryId = @CVSubCategoryId      
            )      
      DECLARE @AskCarrierType BIT = (      
            SELECT TOP 1 AskCarrierType      
            FROM Insurance_CommercialVehicleSubCategory      
            WHERE SubCategoryId = @CVSubCategoryId      
            )      
      
      SELECT AskForHazardusVehicleUse,      
         AskForIfTrailer,      
         AskCarrierType      
      FROM Insurance_CommercialVehicleSubCategory      
      WHERE SubCategoryId = @CVSubCategoryId      
      
      IF OBJECT_ID('#testCVBodyType') IS NOT NULL      
         DROP TABLE #testCVBodyType      
      
      CREATE TABLE #testCVBodyType (CVBodyType INT)      
      
      INSERT INTO #testCVBodyType (CVBodyType)      
      SELECT value      
      FROM string_split(@CVBodyType, ',')      
      
      SELECT VehicleBodyId AS Id,      
         VehicleBodyName AS Name      
      FROM Insurance_CommercialVehicleBodyType VehicleBody WITH (NOLOCK)      
      INNER JOIN #testCVBodyType CVBody WITH (NOLOCK) ON CVBody.CVBodyType = VehicleBody.VehicleBodyId      
         AND VehicleBody.IsActive = 1      
      
      DECLARE @CVUsageNature VARCHAR(MAX)      
      
      SET @CVUsageNature = (      
            SELECT CVUsageNatureOptions      
            FROM Insurance_CommercialVehicleSubCategory      
            WHERE SubCategoryId = @CVSubCategoryId      
            )      
      
      IF OBJECT_ID('#testCVUsageNature') IS NOT NULL      
         DROP TABLE #testCVUsageNature      
      
      CREATE TABLE #testCVUsageNature (CVUsageNature INT)      
      
      INSERT INTO #testCVUsageNature (CVUsageNature)      
      SELECT value      
      FROM string_split(@CVUsageNature, ',')      
      
      SELECT UsageNatureId AS Id,      
         UsageNatureName AS Name      
      FROM Insurance_CommercialVehicleUsageNature UsageNature WITH (NOLOCK)      
      INNER JOIN #testCVUsageNature CVUsageNature WITH (NOLOCK) ON CVUsageNature.CVUsageNature = UsageNature.UsageNatureId      
      WHERE UsageNature.IsActive = 1           
    
      
      IF OBJECT_ID('#AskForIfTrailerOptions') IS NOT NULL      
         DROP TABLE #AskForIfTrailerOptions      
      
      CREATE TABLE #AskForIfTrailerOptions (      
         Id VARCHAR(50),      
         [Name] VARCHAR(50)      
         )      
      
      INSERT INTO #AskForIfTrailerOptions      
      VALUES (      
         'Yes',      
         'Yes'      
         ),      
         (      
         'No',      
         'No'      
         )      
      
      IF OBJECT_ID('#AskForHazardusVehicleUseOptions') IS NOT NULL      
         DROP TABLE #AskForHazardusVehicleUseOptions      
      
      CREATE TABLE #AskForHazardusVehicleUseOptions (      
         Id VARCHAR(50),      
         [Name] VARCHAR(50)      
         )      
      
      INSERT INTO #AskForHazardusVehicleUseOptions      
      VALUES (      
         'Yes',      
         'Yes'      
         ),      
         (      
         'No',      
         'No'      
         )      
      
      SELECT *      
      FROM #AskForIfTrailerOptions      
      
      SELECT *      
      FROM #AskForHazardusVehicleUseOptions      
      
      SELECT CarrierTypeId AS Id,      
         CarrierTypeName AS [Name],      
         CarrierTypeId AS [Value]      
      FROM HeroInsurance.[dbo].Insurance_CommercialVehicleCarrierType      
   --- Usage Type Block Start  
    DECLARE @CVUsageType VARCHAR(MAX)      
      
      SET @CVUsageType = (      
            SELECT UsageTypeId      
            FROM Insurance_CommercialVehicleSubCategory      
            WHERE SubCategoryId = @CVSubCategoryId      
            )      
    
    IF OBJECT_ID('#testCVUsageType') IS NOT NULL      
         DROP TABLE #testCVUsageType      
      
      CREATE TABLE #testCVUsageType (CVUsageType INT)      
      INSERT INTO #testCVUsageType (CVUsageType)      
      SELECT value      
      FROM string_split(@CVUsageType, ',')      
    
     SELECT UsageTypeId AS Id, UsageTypeName AS Name      
  FROM [Insurance_CommercialVehicleUsageType] UsageType WITH (NOLOCK)      
  INNER JOIN #testCVUsageType CVUsageType WITH (NOLOCK) ON CVUsageType.CVUsageType = UsageType.UsageTypeId        
  --- Usage Type Block End  

  DECLARE @PCVVehicleCategory VARCHAR(MAX)      
      
      SET @PCVVehicleCategory = (      
            SELECT PCVVehicleCategoryId      
            FROM Insurance_CommercialVehicleSubCategory      
            WHERE SubCategoryId = @CVSubCategoryId      
            )      
      
      IF OBJECT_ID('#testPCVVehicleCategory') IS NOT NULL      
         DROP TABLE #testPCVVehicleCategory      
      
      CREATE TABLE #testPCVVehicleCategory (PCVVehicleCategory INT)      
      
      INSERT INTO #testPCVVehicleCategory (PCVVehicleCategory)      
      SELECT value      
      FROM string_split(@PCVVehicleCategory, ',')      
      
      SELECT PCVVehicleCategoryId AS Id,      
         CategoryName AS Name      
      FROM Insurance_PCVVehicleCategory PCVVehicleCategory WITH (NOLOCK)      
      INNER JOIN #testPCVVehicleCategory PCVCat WITH (NOLOCK) ON PCVCat.PCVVehicleCategory = PCVVehicleCategory.PCVVehicleCategoryId      
      WHERE PCVVehicleCategory.IsActive = 1 

   END TRY      
      
   BEGIN CATCH      
      DECLARE @StrProcedure_Name VARCHAR(500),      
         @ErrorDetail VARCHAR(1000),      
         @ParameterList VARCHAR(2000)      
      
      SET @StrProcedure_Name = ERROR_PROCEDURE()      
      SET @ErrorDetail = ERROR_MESSAGE()      
      
      EXEC Insurance_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name,      
         @ErrorDetail = @ErrorDetail,      
         @ParameterList = @ParameterList      
   END CATCH      
END