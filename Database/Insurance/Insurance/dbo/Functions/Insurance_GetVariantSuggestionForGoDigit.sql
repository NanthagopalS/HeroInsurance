   /*
   select * from dbo.[Insurance_GetVariantSuggestionForGoDigit] ('C5B93559-CDBF-46F2-83D3-C3A1D469DFF5')  
   */
CREATE   function [dbo].[Insurance_GetVariantSuggestionForGoDigit]   
(      
@VariantId VARCHAR(100)
)      
returns @GoDigitVariantMapping TABLE(
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
	
DECLARE @GoDigitMappingExist BIT = 0,
		@FuelType VARCHAR(10) = ''
		,@IsAMT BIT = 0
		,@IsDark BIT = 0
		,@IsTourbo BIT = 0
		,@IsDualTone BIT = 0
		,@HasDecimal BIT = 0
		,@Is4x4 BIT = 0
		,@HasAbs BIT = 0


		SELECT @GoDigitMappingExist = CASE 
			WHEN ISNULL(VariantId, '') <> ''
				THEN 1
			ELSE 0
			END
	FROM HeroInsurance.MOTOR.GoDigit_VehicleMaster WITH (NOLOCK)
	WHERE VariantId = @VariantId

	SELECT @FuelType =  CASE 
			WHEN FuelName = 'CNG'
				THEN 'CNG'
			WHEN FuelName = 'Petrol'
				THEN 'Petrol'
			WHEN FuelName = 'Diesel'
				THEN 'Diesel'
			WHEN FuelName = 'LPG'
				THEN 'LPG'
			WHEN FuelName = 'Electric'
				THEN 'Electric'
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



	INSERT INTO @GoDigitVariantMapping  
	SELECT 'GoDigit' ICName,
		100 ScoreRank,
		100.00 MScore,
		100.00 MoScore,
		100.00 VScore,
		100.00 MMVScore,
		VARIANT.VariantId HeroVarientID,
		IC.[Vehicle Code] ICVehicleCode,
		MAKE.MakeName HEROMake,
		MODEL.ModelName HeroModel,
		VARIANT.VariantName HeroVarient,
		VARIANT.SeatingCapacity HeroSC,
		VARIANT.CubicCapacity HCC,
		FUEL.FuelName HeroFuel,
		IC.Make ICMake,
		IC.Model ICModel,
		IC.Variant ICVarient,
		IC.SEATINGCAPACITY ICSC,
		IC.CubicCapacity CCC,
		CASE 
			WHEN IC.FuelType = 'PETROL'
				THEN 'Petrol'
			WHEN IC.FuelType = 'DIESEL'
				THEN 'Diesel'
			WHEN IC.FuelType = 'ELECTRIC'
				THEN 'Electric'
			WHEN IC.FuelType = 'CNG'
				THEN 'CNG'
			WHEN IC.FuelType = 'HYBRID'
				THEN 'HYBRID'
			WHEN IC.FuelType = 'LPG'
				THEN 'LPG'
			WHEN IC.FuelType = 'Battery'
				THEN 'Battery'
			ELSE IC.FuelType
			END ICFuel,
	VARIANT.GVW	HeroGVW ,
	IC.GrosssVehicleWeight	ICGVW,
	IC.IsManuallyMapped
	FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
	INNER JOIN MOTOR.GoDigit_VehicleMaster IC WITH (NOLOCK) ON VARIANT.VariantId = IC.VariantId
	LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
	LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId
	LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
	WHERE VARIANT.VariantId = @VariantId
		AND @GoDigitMappingExist = 1
	
	UNION ALL
	
		SELECT 'GoDigit' ICName,
		*
	FROM (
		SELECT ROW_NUMBER() OVER (
				PARTITION BY (HEROMake + HEROModel + HeroVarient) ORDER BY MMVScore DESC
				) ScoreRank,
			*
		FROM (
			SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.Make), dbo.RemoveNonAlphanumeric(Hero.MakeName)) MScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.Variant), dbo.RemoveNonAlphanumeric(Hero.VariantName)) VScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.Model), dbo.RemoveNonAlphanumeric(Hero.ModelName)) MoScore,
				dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
							IC.Make,
							IC.Model,
							IC.Variant
							)), dbo.RemoveNonAlphanumeric(CONCAT (
							Hero.MakeName,
							Hero.ModelName,
							Hero.VariantName
							)))*100 MMVScore,
				Hero.VariantId HeroVarientID,
				IC.[Vehicle Code] ICVehicleCode,
				Hero.MakeName HEROMake,
				Hero.ModelName HeroModel,
				Hero.VariantName HeroVarient,
				Hero.SeatingCapacity HeroSC,
				Hero.CubicCapacity HCC,
				Hero.FuelName HeroFuel,
				IC.Make ICMake,
				IC.Model ICModel,
				IC.Variant ICVarient,
				IC.SEATINGCAPACITY ICSC,
				IC.CubicCapacity CCC,
				CASE 
					WHEN IC.FuelType = 'PETROL'
						THEN 'Petrol'
					WHEN IC.FuelType = 'DIESEL'
						THEN 'Diesel'
					WHEN IC.FuelType = 'ELECTRIC'
						THEN 'Electric'
					WHEN IC.FuelType = 'CNG'
						THEN 'CNG'
					WHEN IC.FuelType = 'LPG'
						THEN 'LPG'
					ELSE IC.FuelType
					END ICFuel,
	Hero.GVW	HeroGVW ,
	IC.GrosssVehicleWeight	ICGVW,
	IC.IsManuallyMapped
			FROM (
				SELECT *
				FROM MOTOR.GoDigit_VehicleMaster WITH (NOLOCK)
				WHERE VariantId IS NULL
				AND @GoDigitMappingExist <> 1
					AND FuelType = @FuelType
					AND (
						Variant LIKE CASE 
							WHEN @IsAMT = 1
								THEN '%AMT%'
							END
						OR ISNULL(@IsAMT, 0) = 0
						)
					AND (
						Variant LIKE CASE 
							WHEN @IsDark = 1
								THEN '%DARK%'
							END
						OR ISNULL(@IsDark, 0) = 0
						)
					AND (
						Variant LIKE CASE 
							WHEN @IsTourbo = 1
								THEN '%TOURBO%'
							END
						OR ISNULL(@IsTourbo, 0) = 0
						)
					AND (
						Variant LIKE CASE 
							WHEN @IsDualTone = 1
								THEN '%DUAL TONE%'
							END
						OR Variant LIKE CASE 
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