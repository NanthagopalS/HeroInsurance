-- EXEC [dbo].[Admin_GetIcVariantMappingStatus] '77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA', '16796'    
CREATE     
     
    
 PROCEDURE [dbo].[Admin_GetIcVariantMappingStatus] (    
 @InsurerId VARCHAR(100),    
 @IcVehicleCode VARCHAR(100)    
 )    
AS    
BEGIN    
 BEGIN TRY    
  IF (@InsurerId = '85F8472D-8255-4E80-B34A-61DB8678135C') -- TATA      
  BEGIN    
   SELECT 'TATA' AS IcName,    
    IC.VarientId HeroVarientID,    
    IC.SR_NO ICVehicleCode,    
    MAKE.MakeName AS HEROMake,    
    IM.ModelName AS HeroModel,    
    VARIANT.VariantId AS HeroVarient,    
    VARIANT.SeatingCapacity AS HeroSC,    
    VARIANT.CubicCapacity AS HCC,    
    FUEL.FuelName AS HeroFuel,    
    IC.TXT_MANUFACTURERNAME ICMake,    
    IC.TXT_MODEL ICModel,    
    IC.TXT_MODEL_VARIANT ICVarient,    
    IC.NUM_SEATING_CAPACITY ICSC,    
    IC.NUM_CUBIC_CAPACITY CCC,    
    IC.TXT_FUEL_TYPE ICFuel,    
    VARIANT.GVW AS HeroGVW,    
    0 ICGVW,    
    IC.IsManuallyMapped    
   FROM HeroInsurance.MOTOR.TATA_VehicleMaster IC WITH (NOLOCK)    
   LEFT JOIN HeroInsurance.dbo.INSURANCE_VARIANT VARIANT WITH(NOLOCK) ON VARIANT.VariantId = IC.VarientId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Model IM WITH(NOLOCK) ON IM.ModelId = VARIANT.ModelId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Make MAKE WITH(NOLOCK) ON MAKE.MakeId = IM.MakeId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FUEL WITH(NOLOCK) ON FUEL.FuelId = VARIANT.FuelID  
  
   WHERE IC.SR_NO = @IcVehicleCode    
  END    
    
  IF (@InsurerId = '16413879-6316-4C1E-93A4-FF8318B14D37') -- BAJAJ      
  BEGIN    
   SELECT 'Bajaj' AS IcName,    
    IC.VariantId HeroVarientID,    
    IC.VehicleCode ICVehicleCode,    
     MAKE.MakeName HEROMake,    
    IM.ModelName HeroModel,    
    VARIANT.VariantName AS HeroVarient,    
    VARIANT.SeatingCapacity AS HeroSC,    
    VARIANT.CubicCapacity AS HCC,    
    FUEl.FuelName AS HeroFuel,    
    IC.VehicleMake ICMake,    
    IC.VehicleModel ICModel,    
    IC.VehicleSubType ICVarient,    
    IC.CarryingCapacity ICSC,    
    IC.CubicCapacity CCC,    
    IC.Fuel ICFuel,    
    VARIANT.GVW AS HeroGVW,    
    0 ICGVW,    
    IC.IsManuallyMapped    
   FROM HeroInsurance.MOTOR.Bajaj_VehicleMaster IC WITH (NOLOCK)   
   LEFT JOIN HeroInsurance.dbo.INSURANCE_VARIANT VARIANT WITH(NOLOCK) ON VARIANT.VariantId = IC.VariantId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Model IM WITH(NOLOCK) ON IM.ModelId = VARIANT.ModelId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Make MAKE WITH(NOLOCK) ON MAKE.MakeId = IM.MakeId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FUEL WITH(NOLOCK) ON FUEL.FuelId = VARIANT.FuelID  
   WHERE IC.VehicleCode = @IcVehicleCode    
  END    
    
  IF (@InsurerId = '77BCE8EC-02D7-4BCF-A971-3E4FBA5C2DBA') -- CHOLA      
  BEGIN    
   SELECT 'CHOLA' AS IcName,    
    IC.VarientId HeroVarientID,    
    IC.ModelCode ICVehicleCode,    
    MAKE.MakeName AS HEROMake,    
    IM.ModelName AS HeroModel,    
    VARIANT.VariantName AS HeroVarient,    
    VARIANT.SeatingCapacity AS HeroSC,    
    VARIANT.CubicCapacity AS HCC,    
    FUEL.FuelName HeroFuel,    
    IC.Make ICMake,    
    IC.VehicleModel ICModel,    
    IC.VehicleModel ICVarient,    
    IC.SEATINGCAPACITY ICSC,    
    IC.CubicCapacity CCC,    
    IC.FuelType ICFuel,    
    VARIANT.GVW AS HeroGVW,    
    0 ICGVW,    
    IC.IsManuallyMapped    
   FROM HeroInsurance.MOTOR.Chola_VehicleMaster IC WITH (NOLOCK)   
   LEFT JOIN HeroInsurance.dbo.Insurance_Variant VARIANT WITH(NOLOCK) ON VARIANT.VariantId = IC.VarientId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Model IM WITH(NOLOCK) ON IM.ModelId = VARIANT.ModelId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Make MAKE WITH(NOLOCK) ON MAKE.MakeId = IM.MakeId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FUEL WITH(NOLOCK) ON FUEL.FuelId = VARIANT.FuelID  
   WHERE IC.ModelCode = @IcVehicleCode    
  END    
    
  IF (@InsurerId = '78190CB2-B325-4764-9BD9-5B9806E99621') -- GO DIGIT      
  BEGIN    
   SELECT 'Go Digit' AS IcName,    
    IC.VariantId HeroVarientID,    
    IC.[Vehicle Code] ICVehicleCode,    
    MAKE.MakeName AS HEROMake,    
    IM.ModelName AS HeroModel,    
    VARIANT.VariantName AS HeroVarient,    
    VARIANT.SeatingCapacity HeroSC,    
    VARIANT.CubicCapacity AS HCC,    
    FUEL.FuelName AS HeroFuel,    
    IC.Make ICMake,    
    IC.Model ICModel,    
    IC.Variant ICVarient,    
    IC.SEATINGCAPACITY ICSC,    
    IC.CubicCapacity CCC,    
    IC.FuelType ICFuel,    
    VARIANT.GVW AS HeroGVW,    
    0 ICGVW,    
    IC.IsManuallyMapped    
   FROM HeroInsurance.MOTOR.GoDigit_VehicleMaster IC WITH (NOLOCK)   
   LEFT JOIN HeroInsurance.dbo.INSURANCE_VARIANT VARIANT WITH(NOLOCK) ON VARIANT.VariantId = IC.VariantId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Model IM WITH(NOLOCK) ON IM.ModelId = VARIANT.ModelId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Make MAKE WITH(NOLOCK) ON MAKE.MakeId = IM.MakeId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FUEL WITH(NOLOCK) ON FUEL.FuelId = VARIANT.FuelID  
   WHERE IC.[Vehicle Code] = @IcVehicleCode    
  END    
    
  IF (@InsurerId = '0A326B77-AFD5-44DA-9871-1742624CFF16') -- HDFC      
  BEGIN    
   SELECT 'HDFC' AS IcName,    
    IC.VariantId HeroVarientID,    
    IC.VEHICLEMODELCODE ICVehicleCode,    
    MAKE.MakeName AS HEROMake,    
    IM.ModelName HeroModel,    
    VARIANT.VariantName AS HeroVarient,    
    VARIANT.SeatingCapacity AS HeroSC,    
    VARIANT.CubicCapacity AS HCC,    
    FUEL.FuelName AS HeroFuel,    
    IC.MANUFACTURER ICMake,    
    IC.VehicleModel ICModel,    
    IC.TXT_VARIANT ICVarient,    
    IC.SEATINGCAPACITY ICSC,    
    IC.CubicCapacity CCC,    
    IC.TXT_FUEL ICFuel,    
    VARIANT.GVW AS HeroGVW,    
    0 ICGVW,    
    IC.IsManuallyMapped    
   FROM HeroInsurance.MOTOR.HDFC_VehicleMaster IC WITH (NOLOCK)  
   LEFT JOIN HeroInsurance.dbo.INSURANCE_VARIANT VARIANT WITH(NOLOCK) ON VARIANT.VariantId = IC.VariantId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Model IM WITH(NOLOCK) ON IM.ModelId = VARIANT.ModelId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Make MAKE WITH(NOLOCK) ON MAKE.MakeId = IM.MakeId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FUEL WITH(NOLOCK) ON FUEL.FuelId = VARIANT.FuelID  
   WHERE IC.VEHICLEMODELCODE = @IcVehicleCode    
  END    
    
  IF (@InsurerId = '372B076C-D9D9-48DC-9526-6EB3D525CAB6') -- Reliance      
  BEGIN    
   SELECT 'Reliance' AS IcName,    
    IC.VarientId HeroVarientID,    
    IC.ModelId ICVehicleCode,    
    MAKE.MakeName AS HEROMake,    
    IM.ModelName AS HeroModel,    
    VARIANT.VariantName AS HeroVarient,    
    VARIANT.SeatingCapacity AS HeroSC,    
    VARIANT.CubicCapacity AS HCC,    
    FUEL.FuelName AS HeroFuel,    
    IC.MakeName ICMake,    
    IC.ModelName ICModel,    
    IC.Variance ICVarient,    
    IC.CarryingCapacity ICSC,    
    IC.CC CCC,    
    IC.OperatedBy ICFuel,    
    VARIANT.GVW AS HeroGVW,    
    0 ICGVW,    
    IC.IsManuallyMapped    
   FROM HeroInsurance.MOTOR.Reliance_VehicleMaster IC WITH (NOLOCK)  
   LEFT JOIN HeroInsurance.dbo.INSURANCE_VARIANT VARIANT WITH(NOLOCK) ON VARIANT.VariantId = IC.VarientId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Model IM WITH(NOLOCK) ON IM.ModelId = VARIANT.ModelId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Make MAKE WITH(NOLOCK) ON MAKE.MakeId = IM.MakeId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FUEL WITH(NOLOCK) ON FUEL.FuelId = VARIANT.FuelID  
   WHERE IC.ModelId = @IcVehicleCode    
  END    
    
  IF (@InsurerId = 'FD3677E5-7938-46C8-9CD2-FAE188A1782C') -- ICICI      
  BEGIN    
   SELECT 'ICICI' AS IcName,    
    IC.VariantId HeroVarientID,    
    IC.VehicleModelCode ICVehicleCode,    
    MAKE.MakeName AS HEROMake,    
    IM.ModelName AS HeroModel,    
    VARIANT.VariantName AS HeroVarient,    
    VARIANT.SeatingCapacity AS HeroSC,    
    VARIANT.CubicCapacity AS HCC,    
    FUEL.FuelName AS HeroFuel,    
    IC.Manufacture ICMake,    
    IC.VehicleModel ICModel,    
    IC.VehicleModel ICVarient,    
    IC.SeatingCapacity ICSC,    
    IC.CubicCapacity CCC,    
    IC.FuelType ICFuel,    
    VARIANT.GVW HeroGVW,    
    0 ICGVW,    
    IC.IsManuallyMapped    
   FROM HeroInsurance.MOTOR.ICICI_VehicleMaster IC WITH (NOLOCK)   
   LEFT JOIN HeroInsurance.dbo.INSURANCE_VARIANT VARIANT WITH(NOLOCK) ON VARIANT.VariantId = IC.VariantId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Model IM WITH(NOLOCK) ON IM.ModelId = VARIANT.ModelId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Make MAKE WITH(NOLOCK) ON MAKE.MakeId = IM.MakeId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FUEL WITH(NOLOCK) ON FUEL.FuelId = VARIANT.FuelID  
   WHERE IC.VehicleModelCode = @IcVehicleCode    
  END    
    
  IF (@InsurerId = 'E656D5D1-5239-4E48-9048-228C67AE3AC3') -- Iffco      
  BEGIN    
   SELECT 'IFFCO' AS IcName,    
    IC.VariantId HeroVarientID,    
    IC.MAKE_CODE ICVehicleCode,    
    MAKE.MakeName AS HEROMake,    
    IM.ModelName AS HeroModel,    
    VARIANT.VariantName AS HeroVarient,    
    VARIANT.SeatingCapacity AS HeroSC,    
    VARIANT.CubicCapacity AS HCC,    
    FUEL.FuelName HeroFuel,    
    IC.Manufacture ICMake,    
    IC.MODEL ICModel,    
    IC.VARIANT ICVarient,    
    IC.SEATING_CAPACITY ICSC,    
    IC.CC CCC,    
    IC.FUEL_TYPE ICFuel,    
    VARIANT.GVW HeroGVW,    
    0 ICGVW,    
    IC.IsManuallyMapped    
   FROM HeroInsurance.MOTOR.ITGI_VehicleMaster IC WITH (NOLOCK)    
   LEFT JOIN HeroInsurance.dbo.INSURANCE_VARIANT VARIANT WITH(NOLOCK) ON VARIANT.VariantId = IC.VariantId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Model IM WITH(NOLOCK) ON IM.ModelId = VARIANT.ModelId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Make MAKE WITH(NOLOCK) ON MAKE.MakeId = IM.MakeId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FUEL WITH(NOLOCK) ON FUEL.FuelId = VARIANT.FuelID  
   WHERE IC.MAKE_CODE = @IcVehicleCode    
  END    
    
  IF (@InsurerId = '5A97C9A3-1CFA-4052-8BA2-6294248EF1E9') -- ORIANTAL      
  BEGIN    
   SELECT 'IFFCO' AS IcName,    
    IC.VariantId HeroVarientID,    
    IC.VEH_MODEL ICVehicleCode,    
    MAKE.MakeName AS HEROMake,    
    IM.ModelName AS HeroModel,    
    VARIANT.VariantName AS HeroVarient,    
    VARIANT.SeatingCapacity AS HeroSC,    
    VARIANT. CubicCapacity AS HCC,    
    FUEL.FuelName AS HeroFuel,    
    IC.VEH_MAKE_DESC ICMake,    
    IC.VEH_MODEL_DESC ICModel,    
    IC.VEH_MODEL_DESC ICVarient,    
    IC.VEH_SEAT_CAP ICSC,    
    IC.VEH_CC CCC,    
    IC.VEH_FUEL_DESC ICFuel,    
    VARIANT.GVW AS HeroGVW,    
    0 ICGVW,    
    IC.IsManuallyMapped    
   FROM HeroInsurance.MOTOR.Oriental_VehicleMaster IC WITH (NOLOCK)  
   LEFT JOIN HeroInsurance.dbo.INSURANCE_VARIANT VARIANT WITH(NOLOCK) ON VARIANT.VariantId = IC.VariantId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Model IM WITH(NOLOCK) ON IM.ModelId = VARIANT.ModelId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Make MAKE WITH(NOLOCK) ON MAKE.MakeId = IM.MakeId  
   LEFT JOIN HeroInsurance.dbo.Insurance_Fuel FUEL WITH(NOLOCK) ON FUEL.FuelId = VARIANT.FuelID  
   WHERE IC.VEH_MODEL = @IcVehicleCode    
  END    
 END TRY    
    
 BEGIN CATCH    
  DECLARE @StrProcedure_Name VARCHAR(500),    
   @ErrorDetail VARCHAR(1000),    
   @ParameterList VARCHAR(2000)    
    
  SET @StrProcedure_Name = ERROR_PROCEDURE()    
  SET @ErrorDetail = ERROR_MESSAGE()    
    
  EXEC Admin_InsertErrorDetail @StrProcedure_Name = @StrProcedure_Name,    
   @ErrorDetail = @ErrorDetail,    
   @ParameterList = @ParameterList    
 END CATCH    
END