   /*
   select * from dbo.[Insurance_GetVariantSuggestionForHDFC] ('AE325044-5CDB-443D-9577-F7CE4D5ACDB5')  
   */
CREATE          function [dbo].[Insurance_GetVariantSuggestionForHDFC]   
(      
@VariantId VARCHAR(100)
)      
returns @HDFCVariantMapping TABLE(
	ICName VARCHAR(100),
	ScoreRank int,
	MScore float,
	VScore float,
	MoScore float,
	MMVScore float,
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
as   
BEGIN       
	
DECLARE @HDFCMappingExist BIT = 0,
@FuelType VARCHAR(10) = ''
		,@IsAMT BIT = 0
		,@IsDark BIT = 0
		,@IsTourbo BIT = 0
		,@IsDualTone BIT = 0
		,@HasDecimal BIT = 0
		,@Is4x4 BIT = 0
		,@HasAbs BIT = 0

		SELECT @HDFCMappingExist = CASE 
			WHEN ISNULL(VariantId, '') <> ''
				THEN 1
			ELSE 0
			END
	FROM HeroInsurance.MOTOR.HDFC_VehicleMaster WITH (NOLOCK)
	WHERE VariantId = @VariantId

	SELECT @FuelType =  CASE 
			WHEN FuelName = 'CNG'
				THEN 'CNG'
			WHEN FuelName = 'Petrol'
				THEN 'PETROL'
			WHEN FuelName = 'Diesel'
				THEN 'DIESEL'
			WHEN FuelName = 'LPG'
				THEN 'LPG'
			WHEN FuelName = 'Electric'
				THEN 'ELECTRIC'

			ELSE FuelName
			END
		,@IsAMT = CASE 
			WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%AMT%'
				THEN 1
			ELSE 0
			END
		,@HasDecimal = CASE 
			WHEN CHARINDEX('.', VariantName) > 0
				THEN 1
			ELSE 0
			END
		,@Is4x4 = CASE 
			WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%4X4%'
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
		,@IsDualTone = CASE 
			WHEN (
					dbo.RemoveNonAlphanumeric(VariantName) LIKE '%DUAL%'
					OR dbo.RemoveNonAlphanumeric(VariantName) LIKE '%DT%'
					)
				THEN 1
			ELSE 0
			END
		,@HasAbs = CASE 
			WHEN VariantName LIKE '%ABS%'
				THEN 1
			ELSE 0
			END
	FROM Insurance_Variant VARIANT WITH (NOLOCK)
	LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
	WHERE VariantId = @VariantId



	INSERT INTO @HDFCVariantMapping  
	SELECT 'HDFC' ICName,
			100 ScoreRank,
			100.00 MScore,
			100.00 MoScore,
			100.00 VScore,
			100.00 MMVScore,
			VARIANT.VariantId HeroVarientID,
			IC.VEHICLEMODELCODE ICVehicleCode,
			MAKE.MakeName HEROMake,
			MODEL.ModelName HeroModel,
			VARIANT.VariantName HeroVarient,
			VARIANT.SeatingCapacity HeroSC,
			VARIANT.CubicCapacity HCC,
			FUEL.FuelName HeroFuel,
			IC.MANUFACTURER ICMake,
			IC.VehicleModel ICModel,
			IC.TXT_VARIANT ICVarient,
			IC.SEATINGCAPACITY ICSC,
			IC.CubicCapacity CCC,
			CASE 
				WHEN IC.TXT_FUEL = 'PETROL'
					THEN 'Petrol'
				WHEN IC.TXT_FUEL = 'DIESEL'
					THEN 'Diesel'
				WHEN IC.TXT_FUEL = 'ELECTRIC'
					THEN 'Electric'
				WHEN IC.TXT_FUEL = 'CNG'
					THEN 'CNG'
				WHEN IC.TXT_FUEL = 'LPG'
					THEN 'LPG'
				ELSE IC.TXT_FUEL
				END ICFuel,
	VARIANT.GVW	HeroGVW ,
	IC.GROSSVEHICLEWEIGHT ICGVW,
	IC.IsManuallyMapped
		FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
		INNER JOIN MOTOR.HDFC_VehicleMaster IC WITH (NOLOCK) ON VARIANT.VariantId = IC.VariantId
		LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
		LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId
		LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
		WHERE VARIANT.VariantId = @VariantId
			AND @HDFCMappingExist = 1
		
		UNION ALL
		SELECT 'HDFC' ICName,
			*
		FROM (
			SELECT ROW_NUMBER() OVER (
					PARTITION BY (HEROMake + HEROModel + HeroVarient) ORDER BY MMVScore DESC
					) ScoreRank,
				*
			FROM (
				SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.MANUFACTURER), dbo.RemoveNonAlphanumeric(Hero.MakeName)) MScore,
					dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.TXT_VARIANT), dbo.RemoveNonAlphanumeric(Hero.VariantName)) VScore,
					dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleModel), dbo.RemoveNonAlphanumeric(Hero.ModelName)) MoScore,
					dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
								IC.MANUFACTURER,
								IC.VehicleModel,
								IC.TXT_VARIANT
								)), dbo.RemoveNonAlphanumeric(CONCAT (
								Hero.MakeName,
								Hero.ModelName,
								Hero.VariantName
								)))*100 MMVScore,
					Hero.VariantId HeroVarientID,
					IC.VEHICLEMODELCODE ICVehicleCode,
					Hero.MakeName HEROMake,
					Hero.ModelName HeroModel,
					Hero.VariantName HeroVarient,
					Hero.SeatingCapacity HeroSC,
					Hero.CubicCapacity HCC,
					Hero.FuelName HeroFuel,
					IC.MANUFACTURER ICMake,
					IC.VehicleModel ICModel,
					IC.TXT_VARIANT ICVarient,
					IC.SEATINGCAPACITY ICSC,
					IC.CubicCapacity CCC,
					CASE 
						WHEN IC.TXT_FUEL = 'PETROL'
							THEN 'Petrol'
						WHEN IC.TXT_FUEL = 'DIESEL'
							THEN 'Diesel'
						WHEN IC.TXT_FUEL = 'ELECTRIC'
							THEN 'Electric'
						WHEN IC.TXT_FUEL = 'CNG'
							THEN 'CNG'
						WHEN IC.TXT_FUEL = 'LPG'
							THEN 'LPG'
						ELSE IC.TXT_FUEL
						END ICFuel,
	Hero.GVW	HeroGVW ,
	IC.GROSSVEHICLEWEIGHT ICGVW,
	IC.IsManuallyMapped
				FROM (
					SELECT *
					FROM HeroInsurance.MOTOR.HDFC_VehicleMaster WITH (NOLOCK)
					WHERE  VariantId IS NULL
					AND @HDFCMappingExist <> 1
						AND TXT_FUEL = @FuelType
						AND (
							TXT_VARIANT LIKE CASE 
								WHEN @IsAMT = 1
									THEN '%AMT%'
								END
							OR ISNULL(@IsAMT, 0) = 0
							)
						AND (
							TXT_VARIANT LIKE CASE 
								WHEN @HasAbs = 1
									THEN '%ABS%'
								END
							OR ISNULL(@HasAbs, 0) = 0
							)
						AND (
							TXT_VARIANT LIKE CASE 
								WHEN @IsDark = 1
									THEN '%DARK%'
								END
							OR ISNULL(@IsDark, 0) = 0
							)
						AND (
							TXT_VARIANT LIKE CASE 
								WHEN @IsTourbo = 1
									THEN '%TOURBO%'
								END
							OR ISNULL(@IsTourbo, 0) = 0
							)
						AND (
							CHARINDEX('.', TXT_VARIANT) > 0
							OR ISNULL(@HasDecimal, 0) = 0
							)
						AND (
							TXT_VARIANT LIKE CASE 
								WHEN @Is4x4 = 1
									THEN '%4*4%'
								END
							OR TXT_VARIANT LIKE CASE 
								WHEN @Is4x4 = 1
									THEN '%4x4%'
								END
							OR TXT_VARIANT LIKE CASE 
								WHEN @Is4x4 = 1
									THEN '%4WD%'
								END
							OR ISNULL(@Is4x4, 0) = 0
							)
						AND (
							TXT_VARIANT LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%DUAL TONE%'
								END
							OR TXT_VARIANT LIKE CASE 
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
			WHERE   HeroSC = ICSC
				AND HCC = CCC
			) v
		WHERE ScoreRank = 1
return  
END