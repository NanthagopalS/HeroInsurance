-- =============================================                
-- Author:  <Ankit Ghosh>                
-- Create date: <02-11-2023>                
-- Description: <GetVariantprobablity>                
--            
/*            
EXEC Insurance_GetVariantProbabilityforOriental '10344191-EB76-467A-88CC-2F12CB2B376D'             
*/  
-- =============================================                
 CREATE    PROCEDURE [dbo].[Insurance_GetVariantProbabilityforOriental] (  
 @VarientId VARCHAR(50) = NULL  
 ,@PolicyTypeId VARCHAR(50) = NULL  
 -- ,@IsVariantMapped BIT OUTPUT  
 )  
AS  
BEGIN  
 DECLARE @PERCENTAGE FLOAT = 0.0;  
 DECLARE @MMVScore FLOAT = @PERCENTAGE  
  ,@Mscore FLOAT = @PERCENTAGE  
  ,@Moscore FLOAT = @PERCENTAGE  
  ,@Vscore FLOAT = @PERCENTAGE  
 DECLARE @OrientalMappingExist BIT = 0  
  ,@FuelType VARCHAR(10) = ''  
  ,@IsAMT BIT = 0  
  ,@IsDark BIT = 0  
  ,@IsTourbo BIT = 0  
  ,@IsDualTone BIT = 0  
  		,@HasDecimal BIT = 0
  ,@IsPlusSign BIT = 0  
  
 SELECT @OrientalMappingExist = CASE   
   WHEN ISNULL(VariantId, '') <> ''  
    THEN 1  
   ELSE 0  
   END  
 FROM HeroInsurance.MOTOR.Oriental_VehicleMaster WITH (NOLOCK)  
 WHERE VariantId = @VarientId  
  
 SELECT @FuelType = CASE   
   WHEN FuelName LIKE 'PETROL%'  
    THEN 'Petrol'  
   WHEN FuelName = 'DIESEL'  
    THEN 'Diesel'  
   WHEN FuelName = 'ELECTRIC'  
    THEN 'Electric'  
   WHEN FuelName = 'CNG'  
    THEN 'CNG'  
   WHEN FuelName = 'LPG'  
    THEN 'LPG'  
   ELSE 'OTHERS'  
   END  
  ,@IsAMT = CASE   
   WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%AMT%'  
    THEN 1  
   ELSE 0  
   END  
  ,@IsDark = CASE   
   WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%DARK%'  
    THEN 1  
   ELSE 0  
   END  
  ,@IsTourbo = CASE   
   WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%TOURBO%'  
    THEN 1  
   ELSE 0  
   END  
  ,@IsPlusSign = CASE   
   WHEN (VariantName) LIKE '%+%'  
    THEN 1  
   ELSE 0  
   END  
  ,@IsDualTone = CASE   
   WHEN (  
     dbo.RemoveNonAlphanumeric(VariantName) LIKE '%DUAL%'  
     OR dbo.RemoveNonAlphanumeric(VariantName) LIKE '%DT%'  
     )  
    THEN 1  
   ELSE 0  
   END , 
   @HasDecimal = CASE 
			WHEN CHARINDEX('.', VariantName) > 0
				THEN 1
			ELSE 0
			END
 FROM Insurance_Variant VARIANT WITH (NOLOCK)  
 LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId  
 WHERE VariantId = @VarientId  
  
 --SELECT @IsAMT,@IsDark,@IsDualTone,@IsPlusSign,@IsTourbo      
 SELECT *  
 INTO #TEMPVARIANT  
 FROM (  
  SELECT 'Oriental' ICName  
   ,*  
  FROM (  
   SELECT ROW_NUMBER() OVER (  
     PARTITION BY (HEROMake + HEROModel + HeroVarient) ORDER BY MMVScore DESC  
     ) ScoreRank  
    ,*  
   FROM (  
    SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VEH_MAKE_DESC), dbo.RemoveNonAlphanumeric(Hero.MakeName)) MScore  
     ,dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VEH_MODEL_DESC), dbo.RemoveNonAlphanumeric(Hero.VariantName)) VScore  
     ,dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VEH_MODEL_DESC), dbo.RemoveNonAlphanumeric(Hero.ModelName)) MoScore  
     ,dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (  
        IC.VEH_MAKE_DESC  
        ,IC.VEH_MODEL_DESC  
        )), dbo.RemoveNonAlphanumeric(CONCAT (  
        Hero.MakeName  
        ,Hero.ModelName  
        ,Hero.VariantName  
        ))) MMVScore  
     ,Hero.VariantId HeroVarientID  
     ,IC.VEH_MODEL ICVehicleCode  
     ,Hero.MakeName HEROMake  
     ,Hero.ModelName HeroModel  
     ,Hero.VariantName HeroVarient  
     ,Hero.SeatingCapacity HeroSC  
     ,Hero.CubicCapacity HCC  
     ,Hero.FuelName HeroFuel  
     ,IC.VEH_MAKE_DESC ICMake  
     ,IC.VEH_MODEL_DESC ICModel  
     ,IC.VEH_SEAT_CAP ICSC  
     ,IC.VEH_CC CCC  
     ,CASE   
      WHEN IC.VEH_FUEL_DESC LIKE 'PETROL%'  
       THEN 'Petrol'  
      WHEN IC.VEH_FUEL_DESC LIKE 'DIESEL%'  
       THEN 'Diesel'  
      WHEN IC.VEH_FUEL_DESC LIKE 'BATTERY POWERED - EL%'  
       THEN 'Electric'  
      WHEN IC.VEH_FUEL_DESC LIKE 'CNG%'  
       THEN 'CNG'  
      WHEN IC.VEH_FUEL_DESC LIKE 'LPG%'  
       THEN 'LPG'  
      ELSE IC.VEH_FUEL_DESC  
      END ICFuel  
    FROM (  
     SELECT *  
     FROM HeroInsurance.MOTOR.Oriental_VehicleMaster WITH (NOLOCK)  
     WHERE VariantId IS NULL  
      AND @OrientalMappingExist <> 1  
      AND VEH_FUEL_DESC LIKE '%' + @FuelType + '%'  
      AND (  
       VEH_MODEL_DESC LIKE CASE   
        WHEN @IsAMT = 1  
         THEN '%AMT%'  
        END  
       OR ISNULL(@IsAMT, 0) = 0  
       )  
      AND (  
       VEH_MODEL_DESC LIKE CASE   
        WHEN @IsDark = 1  
         THEN '%DARK%'  
        END  
       OR ISNULL(@IsDark, 0) = 0  
       )  
      AND (  
       VEH_MODEL_DESC LIKE CASE   
        WHEN @IsTourbo = 1  
         THEN '%TOURBO%'  
        END  
       OR ISNULL(@IsTourbo, 0) = 0  
       ) 
	   AND (
							CHARINDEX('.', VEH_MODEL_DESC) > 0
							OR ISNULL(@HasDecimal, 0) = 0
							)
      AND (  
       VEH_MODEL_DESC LIKE CASE   
        WHEN @IsDualTone = 1  
         THEN '%DUAL TONE%'  
        END  
       OR VEH_MODEL_DESC LIKE CASE   
        WHEN @IsDualTone = 1  
         THEN '%DT%'  
        END  
       OR ISNULL(@IsDualTone, 0) = 0  
       )  
     ) IC  
    CROSS JOIN (  
     SELECT VariantId  
      ,MakeName  
      ,ModelName  
      ,VariantName  
      ,SeatingCapacity  
      ,CubicCapacity  
      ,FuelName  
     FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)  
     LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId  
     LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON MODEL.MakeId = MAKE.MakeId  
     LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId  
     WHERE VariantId = @VarientId  
     ) Hero  
    ) v  
   WHERE v.MMVScore > @MMVScore  
    --AND v.MScore > @MScore      
    --AND v.VScore > @Vscore                      
    --AND v.MoScore > @Moscore      
    AND HeroSC = ICSC  
    AND HCC = CCC  
   ) v  
--  WHERE ScoreRank = 1  
  ) A  
  SELECT * FROM #TEMPVARIANT
 --IF EXISTS (  
 --  SELECT TOP 1 MMVScore  
 --  FROM #TEMPVARIANT  
 --  WHERE MMVScore >= @PERCENTAGE  
 --  )  
 --BEGIN  
 -- UPDATE OrientalVARIANT  
 -- SET VariantId = VARIANT.HeroVarientID  
 --  ,IsManuallyMapped = 0  
 -- FROM #TEMPVARIANT VARIANT  
 -- LEFT JOIN MOTOR.Oriental_VehicleMaster OrientalVARIANT WITH (NOLOCK) ON OrientalVARIANT.VEH_MODEL = VARIANT.ICVehicleCode  
 -- WHERE ISNULL(IsManuallyMapped, 0) = 0  
  
 -- SET @IsVariantMapped = 1  
 --END  
 --ELSE  
 --BEGIN  
 -- SET @IsVariantMapped = 0  
 --END  
END