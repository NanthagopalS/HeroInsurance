   /*
   select * from dbo.[Insurance_GetVariantSuggestionForReliance] ('4CDB28C1-740C-4E3C-8877-7F137D1D43A8')  
   */
 CREATE          function [dbo].[Insurance_GetVariantSuggestionForReliance]   
(      
@VariantId VARCHAR(100)
)      
returns @RelianceVariantMapping TABLE(
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
	
DECLARE @RelianceMappingExist BIT = 0,
@FuelType VARCHAR(10) = ''
		,@IsAMT BIT = 0
		,@IsDark BIT = 0
		,@IsTourbo BIT = 0
		,@IsDualTone BIT = 0
		,@HasDecimal BIT = 0
		,@Is4x4 BIT = 0
		,@HasAbs BIT = 0

		SELECT @RelianceMappingExist = CASE 
			WHEN ISNULL(VarientId, '') <> ''
				THEN 1
			ELSE 0
			END
	FROM MOTOR.Reliance_VehicleMaster WITH (NOLOCK)
	WHERE VarientId = @VariantId

	SELECT @FuelType =  CASE 
			WHEN FuelName = 'CNG'
				THEN 'CNG'
			WHEN FuelName = 'Petrol'
				THEN 'PETROL'
			WHEN FuelName = 'Diesel'
				THEN 'DIESEL'
			WHEN FuelName = 'LPG'
				THEN 'LPG'
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



	INSERT INTO @RelianceVariantMapping  
	SELECT 'Reliance' ICName,
			100 ScoreRank,
			100.00 MScore,
			100.00 MoScore,
			100.00 VScore,
			100.00 MMVScore,
			VARIANT.VariantId HeroVarientID,
			IC.ModelId ICVehicleCode,
			MAKE.MakeName HEROMake,
			MODEL.ModelName HeroModel,
			VARIANT.VariantName HeroVarient,
			VARIANT.SeatingCapacity HeroSC,
			VARIANT.CubicCapacity HCC,
			FUEL.FuelName HeroFuel,
			IC.MakeName ICMake,
			IC.ModelName ICModel,
			IC.Variance ICVarient,
			IC.CarryingCapacity ICSC,
			IC.CC CCC,
			CASE 
				WHEN IC.OperatedBy = 'PETROL'
					THEN 'Petrol'
				WHEN IC.OperatedBy = 'DIESEL'
					THEN 'Diesel'
				WHEN IC.OperatedBy = 'BATTERY OPERATED'
					THEN 'Electric'
				WHEN IC.OperatedBy = 'CNG'
					THEN 'CNG'
				WHEN IC.OperatedBy = 'LPG'
					THEN 'LPG'
				ELSE IC.OperatedBy
				END ICFuel,
	VARIANT.GVW	HeroGVW ,
	0	ICGVW,
	IC.IsManuallyMapped
		FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
		INNER JOIN MOTOR.Reliance_VehicleMaster IC WITH (NOLOCK) ON VARIANT.VariantId = IC.VarientId
		LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
		LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId
		LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
		WHERE VARIANT.VariantId = @VariantId
			AND @RelianceMappingExist = 1
		
		UNION ALL
		SELECT 'Reliance' ICName,
			*
		FROM (
			SELECT ROW_NUMBER() OVER (
					PARTITION BY (HEROMake + HEROModel + HeroVarient) ORDER BY MMVScore DESC
					) ScoreRank,
				*
			FROM (
				SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.MakeName), dbo.RemoveNonAlphanumeric(Hero.MakeName)) MScore,
					dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.Variance), dbo.RemoveNonAlphanumeric(Hero.VariantName)) VScore,
					dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.ModelName), dbo.RemoveNonAlphanumeric(Hero.ModelName)) MoScore,
					dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
								IC.MakeName,
								IC.ModelName,
								IC.Variance
								)), dbo.RemoveNonAlphanumeric(CONCAT (
								Hero.MakeName,
								Hero.ModelName,
								Hero.VariantName
								)))*100 MMVScore,
					Hero.VariantId HeroVarientID,
					IC.ModelID ICVehicleCode,
					Hero.MakeName HEROMake,
					Hero.ModelName HeroModel,
					Hero.VariantName HeroVarient,
					Hero.SeatingCapacity HeroSC,
					Hero.CubicCapacity HCC,
					Hero.FuelName HeroFuel,
					IC.MakeName ICMake,
					IC.ModelName ICModel,
					IC.Variance ICVarient,
					IC.SeatingCapacity ICSC,
					IC.CC CCC,
					CASE 
						WHEN IC.OperatedBy = 'PETROL'
							THEN 'Petrol'
						WHEN IC.OperatedBy = 'DIESEL'
							THEN 'Diesel'
						WHEN IC.OperatedBy = 'BATTERY OPERATED'
							THEN 'Electric'
						WHEN IC.OperatedBy = 'CNG'
							THEN 'CNG'
						WHEN IC.OperatedBy = 'LPG'
							THEN 'LPG'
						ELSE IC.OperatedBy
						END ICFuel,
	Hero.GVW	HeroGVW ,
	0	ICGVW,
	IC.IsManuallyMapped
				FROM (
					SELECT *
					FROM HeroInsurance.MOTOR.Reliance_VehicleMaster WITH (NOLOCK)
					WHERE  VarientId IS NULL
					AND @RelianceMappingExist <> 1
						AND OperatedBy = @FuelType
						AND (
							Variance LIKE CASE 
								WHEN @IsAMT = 1
									THEN '%AMT%'
								END
							OR Variance LIKE CASE 
								WHEN @IsAMT = 1
									THEN '% AT%'
								END
							OR ISNULL(@IsAMT, 0) = 0
							)
						AND (
							Variance LIKE CASE 
								WHEN @IsDark = 1
									THEN '%DARK%'
								END
							OR ISNULL(@IsDark, 0) = 0
							)
						AND (
							Variance LIKE CASE 
								WHEN @IsTourbo = 1
									THEN '%TOURBO%'
								END
							OR ISNULL(@IsTourbo, 0) = 0
							)
						AND (
							Variance LIKE CASE 
								WHEN @HasAbs = 1
									THEN '%ABS%'
								END
							OR ISNULL(@HasAbs, 0) = 0
							)
						AND (
							Variance LIKE CASE 
								WHEN @Is4x4 = 1
									THEN '%4*4%'
								END
							OR Variance LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%4x4%'
								END
							OR Variance LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%4WD%'
								END
							OR ISNULL(@Is4x4, 0) = 0
							)
						AND (
							CHARINDEX('.', Variance) > 0
							OR ISNULL(@HasDecimal, 0) = 0
							)
						AND (
							Variance LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%DUAL TONE%'
								END
							OR Variance LIKE CASE 
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
				AND LEN(HeroVarient) - LEN(REPLACE(HeroVarient, ' ', '')) = LEN(ICVarient) - LEN(REPLACE(ICVarient, ' ', ''))
			) v
		WHERE ScoreRank = 1
return  
END