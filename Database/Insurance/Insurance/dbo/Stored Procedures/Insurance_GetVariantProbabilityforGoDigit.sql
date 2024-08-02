
-- =============================================    
-- Author:  <Ankit Ghosh>    
-- Create date: <02-11-2023>    
-- Description: <GetVariantprobablity>    
--EXEC Insurance_GetVariantProbabilityforGoDigit '0E053F09-AE2A-4E48-97A5-00D3A6AB1418'  
-- =============================================    
CREATE PROCEDURE [dbo].[Insurance_GetVariantProbabilityforGoDigit] (
@VarientId VARCHAR(50) = NULL
,@IsVariantMapped BIT OUTPUT)
AS
BEGIN
	DECLARE @PERCENTAGE FLOAT = 0.75;
	DECLARE @MMVScore FLOAT = @PERCENTAGE,
		@Mscore FLOAT = @PERCENTAGE,
		@Moscore FLOAT = @PERCENTAGE,
		@Vscore FLOAT = @PERCENTAGE
	DECLARE @GoDigitMappingExist BIT = 0,
		@FuelType VARCHAR(10) = '',
		@IsAMT BIT = 0,
		@IsDark BIT = 0,
		@IsTourbo BIT = 0,
		@IsDualTone BIT = 0

	SELECT @GoDigitMappingExist = CASE 
			WHEN ISNULL(VariantId, '') <> ''
				THEN 1
			ELSE 0
			END
	FROM HeroInsurance.MOTOR.GoDigit_VehicleMaster WITH (NOLOCK)
	WHERE VariantId = @VarientId

	SELECT @FuelType = CASE 
			WHEN FuelName = 'PETROL'
				THEN 'Petrol'
			WHEN FuelName = 'DIESEL'
				THEN 'Diesel'
			WHEN FuelName = 'ELECTRIC'
				THEN 'Electric'
			WHEN FuelName = 'CNG'
				THEN 'CNG'
			WHEN FuelName = 'LPG'
				THEN 'LPG'
			WHEN FuelName = 'Battery'
				THEN 'Battery'
			ELSE 'HYBRID'
			END,
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
	SELECT *
	INTO #TEMPVARIANT
	FROM (
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
			END ICFuel
	FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
	INNER JOIN HeroInsurance.MOTOR.GoDigit_VehicleMaster IC WITH (NOLOCK) ON VARIANT.VariantId = IC.VariantId
	LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
	LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId
	LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
	WHERE VARIANT.VariantId = @VarientId
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
							))) MMVScore,
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
					WHEN IC.FuelType = 'HYBRID'
						THEN 'HYBRID'
					WHEN IC.FuelType = 'LPG'
						THEN 'LPG'
					ELSE IC.FuelType
					END ICFuel
			FROM (
				SELECT *
				FROM HeroInsurance.MOTOR.GoDigit_VehicleMaster WITH (NOLOCK)
				WHERE @GoDigitMappingExist <> 1
					AND VariantId IS NULL
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
					FuelName
				FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
				LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
				LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON MODEL.MakeId = MAKE.MakeId
				LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
				WHERE VariantId = @VarientId
				) Hero
			) v
		WHERE v.MMVScore > @MMVScore
			AND HeroSC = ICSC
			AND HCC = CCC
		) v
	WHERE ScoreRank = 1
	) B

	IF EXISTS (
			SELECT TOP 1 MMVScore
			FROM #TEMPVARIANT
			WHERE MMVScore >= @PERCENTAGE
			)
	BEGIN
		UPDATE GODIGITVARIANT
		SET VariantId = VARIANT.HeroVarientID, IsManuallyMapped = 0
		FROM #TEMPVARIANT VARIANT
		LEFT JOIN MOTOR.GoDigit_VehicleMaster GODIGITVARIANT WITH (NOLOCK) ON GODIGITVARIANT.[Vehicle Code] = VARIANT.ICVehicleCode
		WHERE ISNULL(IsManuallyMapped, 0) = 0

		SET @IsVariantMapped = 1
	END
	ELSE
	BEGIN
		SET @IsVariantMapped = 0
	END

END