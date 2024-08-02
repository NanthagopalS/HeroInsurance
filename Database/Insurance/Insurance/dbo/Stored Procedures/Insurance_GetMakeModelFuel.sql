-- =============================================    
-- Author:  <Author,,AMBI GUPTA>    
-- Create date: <Create Date,25-Nov-2022>    
-- Description: <Description,GetMakeModelFuel>    
-- [Insurance_GetMakeModelFuel] '88a807b3-90e4-484b-b5d2-65059a8e1a91','833E4DC9-A7EE-4C7D-ABBB-C6B993BF428C'    
-- added vechicle category ID condition for CV -- Suraj 13-10-2023  
-- =============================================    
CREATE  
   
   
 PROCEDURE [dbo].[Insurance_GetMakeModelFuel] (  
 @VehicleType VARCHAR(50) = NULL  
 ,@CVCategoryId VARCHAR(50) = NULL  
 )  
AS  
BEGIN  
 BEGIN TRY  
  -- SET NOCOUNT ON added to prevent extra result sets from    
  -- interfering with SELECT statements.    
  SET NOCOUNT ON;  
  
  SELECT MAKE.MakeName  
   ,MAKE.MakeId  
   ,MAKE.IsPopular  
   ,MODEL.ModelId  
   ,MODEL.ModelNamE  
   ,MODEL.IsPopularModel  
   ,MAKE.Logo ImageURL  
   ,MODEL.IsCommercial  
  INTO #TMPMAKEMODEL  
  FROM Insurance_Variant VARIANT WITH (NOLOCK)  
  LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId  
  LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId  
  LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId  
  LEFT JOIN Insurance_VehicleType VT WITH (NOLOCK) ON VARIANT.VehicleTypeId = VT.VehicleTypeId  
  LEFT JOIN Insurance_InsuranceType INSTYPE WITH (NOLOCK) ON VT.InsuranceTypeId = INSTYPE.InsuranceTypeId  
  WHERE INSTYPE.InsuranceTypeId = @VehicleType  
   AND MAKE.MakeId IS NOT NULL  
   AND MODEL.ModelId IS NOT NULL  
   AND (  
    ISNULL(@CVCategoryId, '') = ''  
    OR VARIANT.CVSubCategoryId  = @CVCategoryId  
    )  
  
  SELECT DISTINCT MAKEID  
   ,MakeName  
   ,IsPopular  
   ,ImageURL  
  FROM #TMPMAKEMODEL WITH (NOLOCK)  
  ORDER BY IsPopular DESC  
   ,MakeName  
  
  SELECT DISTINCT MODEL.ModelId  
   ,MODEL.ModelNamE  
   ,MODEL.IsPopularModel  
   ,MakeId  
  FROM #TMPMAKEMODEL MODEL WITH (NOLOCK)  
  ORDER BY IsPopularModel DESC  
   ,ModelName  
  
  SELECT CAST(FuelId AS VARCHAR(50)) FuelId  
   ,FuelName  
  FROM Insurance_Fuel WITH (NOLOCK)  
  ORDER BY FUELNAME  
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

