  
-- =============================================          
-- Author:  <Suraj Kumar Gupta>          
-- Create date: <02-11-2023>          
-- Description: <GetVariantprobablity>          
/*    
 EXEC Insurance_GetVariantProbabilityForTATA 'CE68D1C4-CE70-4A96-83D5-5A8151362CCA'     
 */  
-- =============================================          
CREATE   PROCEDURE [dbo].[Insurance_GetVariantProbabilityForTATA] (  
@VarientId VARCHAR(50) = NULL  
--,@IsVariantMapped BIT OUTPUT  
)  
AS  
BEGIN  
 DECLARE @PERCENTAGE FLOAT = 0.80;  
 DECLARE @MMVScore FLOAT = @PERCENTAGE,  
  @Mscore FLOAT = @PERCENTAGE,  
  @Moscore FLOAT = @PERCENTAGE,  
  @Vscore FLOAT = @PERCENTAGE  
 DECLARE @TATAMappingExist BIT = 0,  
  @FuelType VARCHAR(10) = '',  
  @IsAMT BIT = 0,  
  @IsDark BIT = 0,  
  @IsTourbo BIT = 0,  
  @IsDualTone BIT = 0,  
  @Is4x4 BIT = 0,  
  @HasDecimal BIT = 0,  
  @HasAbs BIT = 0  
  
 SELECT @TATAMappingExist = CASE   
   WHEN ISNULL(VarientId, '') <> ''  
    THEN 1  
   ELSE 0  
   END  
 FROM MOTOR.TATA_VehicleMaster WITH (NOLOCK)  
 WHERE VarientId = @VarientId  
  
 SELECT @FuelType = FuelName  
   ,  
  @IsAMT = CASE   
   WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%AMT%'  
    THEN 1  
   ELSE 0  
   END,  
  @IsDark = CASE   
   WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%DARK%'  
    THEN 1  
   ELSE 0  
   END,  
  @IsTourbo = CASE   
   WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%TOURBO%'  
    THEN 1  
   ELSE 0  
   END,  
   @HasDecimal = CASE   
   WHEN CHARINDEX('.', VariantName) > 0  
    THEN 1  
   ELSE 0  
   END,  
  @Is4x4 = CASE   
   WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%4X4%'  
    THEN 1  
   ELSE 0  
   END,  
  @HasAbs = CASE   
   WHEN VariantName LIKE '%ABS%'  
    THEN 1  
   ELSE 0  
   END,  
  @IsDualTone = CASE   
   WHEN (  
     dbo.RemoveNonAlphanumeric(VariantName) LIKE '%DUAL%'  
     OR dbo.RemoveNonAlphanumeric(VariantName) LIKE '%DT%'  
     )  
    THEN 1  
   ELSE 0  
   END  
 FROM Insurance_Variant VARIANT WITH (NOLOCK)  
 LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId  
 WHERE VariantId = @VarientId  
  
 --SELECT @IsTourbo IsTurbo,  
 -- @FuelType FuleType,  
 -- @IsAMT AMT,  
 -- @ISDark Dark,  
 -- @IsDualTone DualTone  
  
  
  SELECT *  
 INTO #TEMPVARIANT  
 FROM (  
 SELECT 'TATA' ICName,  
  100 ScoreRank,  
  100.00 MScore,  
  100.00 MoScore,  
  100.00 VScore,  
  100.00 MMVScore,  
  VARIANT.VariantId HeroVarientID,  
  IC.SR_NO ICVehicleCode,  
  MAKE.MakeName HEROMake,  
  MODEL.ModelName HeroModel,  
  VARIANT.VariantName HeroVarient,  
  VARIANT.SeatingCapacity HeroSC,  
  VARIANT.CubicCapacity HCC,  
  FUEL.FuelName HeroFuel,  
  IC.TXT_MANUFACTURERNAME ICMake,  
  IC.TXT_MODEL ICModel,  
  IC.TXT_MODEL_VARIANT ICVarient,  
  IC.NUM_SEATING_CAPACITY ICSC,  
  IC.NUM_CUBIC_CAPACITY CCC,  
  CASE   
   WHEN IC.TXT_FUEL_TYPE = 'PETROL'  
    THEN 'Petrol'  
   WHEN IC.TXT_FUEL_TYPE = 'DIESEL'  
    THEN 'Diesel'  
   WHEN IC.TXT_FUEL_TYPE = 'BATTERY'  
    THEN 'Electric'  
   WHEN IC.TXT_FUEL_TYPE = 'CNG'  
    THEN 'CNG'  
   WHEN IC.TXT_FUEL_TYPE = 'LPG'  
    THEN 'LPG'  
   ELSE IC.TXT_FUEL_TYPE  
   END ICFuel  
 FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)  
 INNER JOIN HeroInsurance.MOTOR.TATA_VehicleMaster IC WITH (NOLOCK) ON VARIANT.VariantId = IC.VarientId  
 LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId  
 LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId  
 LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId  
 WHERE VARIANT.VariantId = @VarientId  
  AND @TATAMappingExist = 1  
   
 UNION ALL  
   
 SELECT 'TATA' ICName,  
  *  
 FROM (  
  SELECT ROW_NUMBER() OVER (  
    PARTITION BY (HEROMake + HEROModel + HeroVarient) ORDER BY MMVScore DESC  
    ) ScoreRank,  
   *  
  FROM (  
   SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.TXT_MANUFACTURERNAME), dbo.RemoveNonAlphanumeric(Hero.MakeName)) MScore,  
    dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.TXT_MODEL_VARIANT), dbo.RemoveNonAlphanumeric(Hero.VariantName)) VScore,  
    dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.TXT_MODEL), dbo.RemoveNonAlphanumeric(Hero.ModelName)) MoScore,  
    dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (  
       IC.TXT_MANUFACTURERNAME,  
       IC.TXT_MODEL,  
       IC.TXT_MODEL_VARIANT  
       )), dbo.RemoveNonAlphanumeric(CONCAT (  
       Hero.MakeName,  
       Hero.ModelName,  
       Hero.VariantName  
       ))) MMVScore,  
    Hero.VariantId HeroVarientID,  
    IC.SR_NO ICVehicleCode,  
    Hero.MakeName HEROMake,  
    Hero.ModelName HeroModel,  
    Hero.VariantName HeroVarient,  
    Hero.SeatingCapacity HeroSC,  
    Hero.CubicCapacity HCC,  
    Hero.FuelName HeroFuel,  
    IC.TXT_MANUFACTURERNAME ICMake,  
    IC.TXT_MODEL ICModel,  
    IC.TXT_MODEL_VARIANT ICVarient,  
    IC.NUM_SEATING_CAPACITY ICSC,  
    IC.NUM_CUBIC_CAPACITY CCC,  
    CASE   
   WHEN IC.TXT_FUEL_TYPE = 'PETROL'  
    THEN 'Petrol'  
   WHEN IC.TXT_FUEL_TYPE = 'DIESEL'  
    THEN 'Diesel'  
   WHEN IC.TXT_FUEL_TYPE = 'BATTERY'  
    THEN 'Electric'  
   WHEN IC.TXT_FUEL_TYPE = 'CNG'  
    THEN 'CNG'  
   WHEN IC.TXT_FUEL_TYPE = 'LPG'  
    THEN 'LPG'  
   ELSE IC.TXT_FUEL_TYPE  
   END ICFuel  
   FROM (  
    SELECT *  
    FROM HeroInsurance.MOTOR.TATA_VehicleMaster WITH (NOLOCK)  
    WHERE @TATAMappingExist <> 1  
     AND VarientId IS NULL  
     AND TXT_FUEL_TYPE = @FuelType  
     AND (  
      TXT_MODEL_VARIANT LIKE CASE   
       WHEN @IsAMT = 1  
        THEN '%AMT%'  
       END  
      OR ISNULL(@IsAMT, 0) = 0  
      )  
     AND (  
      TXT_MODEL_VARIANT LIKE CASE   
       WHEN @IsDark = 1  
        THEN '%DARK%'  
       END  
      OR ISNULL(@IsDark, 0) = 0  
      )  
     AND (  
      TXT_MODEL_VARIANT LIKE CASE   
       WHEN @IsTourbo = 1  
        THEN '%TOURBO%'  
       END  
      OR ISNULL(@IsTourbo, 0) = 0  
      )  
     AND (  
      TXT_MODEL_VARIANT LIKE CASE   
       WHEN @Is4x4 = 1  
        THEN '%4*4%'  
       END  
      OR TXT_MODEL_VARIANT LIKE CASE   
       WHEN @IsDualTone = 1  
        THEN '%4x4%'  
       END  
      OR TXT_MODEL_VARIANT LIKE CASE   
       WHEN @IsDualTone = 1  
        THEN '%4WD%'  
       END  
      OR ISNULL(@Is4x4, 0) = 0  
      )  
     AND (  
      TXT_MODEL_VARIANT LIKE CASE   
       WHEN @HasAbs = 1  
        THEN '%ABS%'  
       END  
      OR ISNULL(@HasAbs, 0) = 0  
      )  
     AND (  
      TXT_MODEL_VARIANT LIKE CASE   
       WHEN @IsDualTone = 1  
        THEN '%DUAL TONE%'  
       END  
      OR TXT_MODEL_VARIANT LIKE CASE   
       WHEN @IsDualTone = 1  
        THEN '%DT%'  
       END  
      OR ISNULL(@IsDualTone, 0) = 0  
      )  
      AND (  
       CHARINDEX('.', TXT_MODEL_VARIANT) > 0  
       OR ISNULL(@HasDecimal, 0) = 0  
       )  
    ) IC  
   CROSS JOIN (  
    SELECT VariantId,  
     MakeName,  
     ModelName,  
     VariantName,  
     SeatingCapacity,  
     CubicCapacity,  
     FuelName  
    FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)  
    LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId  
    LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON MODEL.MakeId = MAKE.MakeId  
    LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId  
    WHERE VariantId = @VarientId  
    ) Hero  
   ) v  
  WHERE v.MMVScore > @MMVScore  
   --AND v.MScore > .50  
   --AND v.VScore > .70  
   --AND v.MoScore > @Moscore        
   AND HeroSC = ICSC  
   AND HCC = CCC  
  ) v  
 -- WHERE ScoreRank = 1  
 ) A  
  
 SELECT * FROM #TEMPVARIANT  
  
 -- IF EXISTS (  
 --  SELECT TOP 1 MMVScore  
 --  FROM #TEMPVARIANT  
 --  WHERE MMVScore >= @PERCENTAGE  
 --  )  
 --BEGIN  
 -- UPDATE TATAVARIANT  
 -- SET VarientId = VARIANT.HeroVarientID, IsManuallyMapped = 0  
 -- FROM #TEMPVARIANT VARIANT  
 -- LEFT JOIN MOTOR.TATA_VehicleMaster TATAVARIANT WITH (NOLOCK) ON TATAVARIANT.SR_NO = VARIANT.ICVehicleCode  
 -- WHERE ISNULL(IsManuallyMapped, 0) = 0  
  
 -- SET @IsVariantMapped = 1  
 --END  
 --ELSE  
 --BEGIN  
 -- SET @IsVariantMapped = 0  
 --END  
END