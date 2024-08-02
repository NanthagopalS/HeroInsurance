
/*
   select * from dbo.[Insurance_GetVariantSuggestionForOriental] ('4CDB28C1-740C-4E3C-8877-7F137D1D43A8')    
   */
CREATE
   

 FUNCTION [dbo].[Insurance_GetVariantSuggestionForOriental] (@VariantId VARCHAR(100))
RETURNS @OrientalVariantMapping TABLE (
   ICName VARCHAR(100),
   ScoreRank INT,
   MScore FLOAT,
   VScore FLOAT,
   MoScore FLOAT,
   MMVScore FLOAT,
   HeroVarientID VARCHAR(100),
   ICVehicleCode VARCHAR(100),
   HEROMake VARCHAR(100),
   HeroModel VARCHAR(100),
   HeroVarient VARCHAR(100),
   HeroSC VARCHAR(100),
   HCC VARCHAR(100),
   HeroFuel VARCHAR(100),
   ICMake VARCHAR(100),
   ICModel VARCHAR(100),
   ICVarient VARCHAR(100),
   ICSC VARCHAR(100),
   CCC VARCHAR(100),
   ICFuel VARCHAR(100),
		HeroGVW VARCHAR(100),
		ICGVW VARCHAR(100),
		IsManuallyMapped bit
   )
AS
BEGIN
   DECLARE @OrientalMappingExist BIT = 0,
   @FuelType VARCHAR(10) = '',
      @IsAMT BIT = 0,
      @IsDark BIT = 0,
      @IsTourbo BIT = 0,
      @IsDualTone BIT = 0,
      @HasDecimal BIT = 0,
      @Is4x4 BIT = 0,
      @HasAbs BIT = 0,
      @IsPlusSign BIT = 0

	   SELECT @OrientalMappingExist = CASE   
   WHEN ISNULL(VariantId, '') <> ''  
    THEN 1  
   ELSE 0  
   END  
 FROM HeroInsurance.MOTOR.Oriental_VehicleMaster WITH (NOLOCK)  
 WHERE VariantId = @VariantId  

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
         ELSE FuelName
         END,
      @IsAMT = CASE 
         WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%AMT%'
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
      @IsPlusSign = CASE 
         WHEN (VariantName) LIKE '%+%'
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
         END,
      @HasAbs = CASE 
         WHEN VariantName LIKE '%ABS%'
            THEN 1
         ELSE 0
         END
   FROM Insurance_Variant VARIANT WITH (NOLOCK)
   LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
   WHERE VariantId = @VariantId

   INSERT INTO @OrientalVariantMapping
   SELECT 'Oriental' ICName
			,100 ScoreRank
			,100.00 MScore
			,100.00 MoScore
			,100.00 VScore
			,100.00 MMVScore
			,VARIANT.VariantId HeroVarientID
			,IC.VEH_MODEL ICVehicleCode
			,MAKE.MakeName HEROMake
			,MODEL.ModelName HeroModel
			,VARIANT.VariantName HeroVarient
			,VARIANT.SeatingCapacity HeroSC
			,VARIANT.CubicCapacity HCC
			,FUEL.FuelName HeroFuel
			,IC.VEH_MAKE_DESC ICMake
			,IC.VEH_MODEL_DESC ICModel
			,'' ICVarient
			,IC.VEH_SEAT_CAP ICSC
			,IC.VEH_CC CCC
			,CASE 
				WHEN IC.VEH_FUEL_DESC LIKE ('P%')
					THEN 'Petrol'
				WHEN IC.VEH_FUEL_DESC = 'D'
					THEN 'Diesel'
				WHEN IC.VEH_FUEL_DESC = 'B'
					THEN 'Electric'
				WHEN IC.VEH_FUEL_DESC = 'C'
					THEN 'CNG'
				ELSE IC.VEH_FUEL_DESC
				END ICFuel,
	VARIANT.GVW	HeroGVW ,
	0	ICGVW,
	IC.IsManuallyMapped
		FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
		INNER JOIN  MOTOR.Oriental_VehicleMaster IC WITH (NOLOCK) ON VARIANT.VariantId = IC.VariantId
		LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
		LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId
		LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
		WHERE VARIANT.VariantId = @VariantId
			AND @OrientalMappingExist = 1
			UNION ALL
   SELECT 'Oriental' ICName,
      *
   FROM (
      SELECT ROW_NUMBER() OVER (
            PARTITION BY (HEROMake + HEROModel + HeroVarient) ORDER BY MMVScore DESC
            ) ScoreRank,
         *
      FROM (
         SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VEH_MAKE_DESC), dbo.RemoveNonAlphanumeric(Hero.MakeName)) MScore,
            dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VEH_MODEL_DESC), dbo.RemoveNonAlphanumeric(Hero.VariantName)) VScore,
            dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VEH_MODEL_DESC), dbo.RemoveNonAlphanumeric(Hero.ModelName)) MoScore,
            dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
                     IC.VEH_MAKE_DESC,
                     IC.VEH_MODEL_DESC
                     )), dbo.RemoveNonAlphanumeric(CONCAT (
                     Hero.MakeName,
                     Hero.ModelName,
                     Hero.VariantName
                     )))*100 MMVScore,
            Hero.VariantId HeroVarientID,
            IC.VEH_MODEL ICVehicleCode,
            Hero.MakeName HEROMake,
            Hero.ModelName HeroModel,
            Hero.VariantName HeroVarient,
            Hero.SeatingCapacity HeroSC,
            Hero.CubicCapacity HCC,
            Hero.FuelName HeroFuel,
            IC.VEH_MAKE_DESC ICMake,
            IC.VEH_MODEL_DESC ICModel,
			'' ICVariant,
            IC.VEH_SEAT_CAP ICSC,
            IC.VEH_CC CCC,
            CASE 
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
               END ICFuel,
	Hero.GVW	HeroGVW ,
	0	ICGVW,
	IC.IsManuallyMapped
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
            SELECT VariantId,
               MakeName,
               ModelName,
               VariantName,
               SeatingCapacity,
               CubicCapacity,
               FuelName,
			   GVW
            FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
            LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
            LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON MODEL.MakeId = MAKE.MakeId
            LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
            WHERE VariantId = @VariantId
            ) Hero
         ) v
      WHERE  HeroSC = ICSC
         AND HCC = CCC
      ) v
   WHERE ScoreRank = 1

   RETURN
END