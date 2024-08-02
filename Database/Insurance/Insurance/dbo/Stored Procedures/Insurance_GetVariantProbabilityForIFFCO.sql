
-- =============================================      
-- Author:  <Vishnu reddy>      
-- Create date: <03-11-2023>      
-- Description: <Insurance_GetVariantProbabilityForITGI>  
-- added vehicle type condition --suraj
/*
EXEC [Insurance_GetVariantProbabilityForIFFCO] '91F29867-C1B4-4CDC-AF90-688A24E945FE'    
*/
-- =============================================      
CREATE PROCEDURE [dbo].[Insurance_GetVariantProbabilityForIFFCO] (
	@VarientId VARCHAR(50) = NULL,
	@IsVariantMapped BIT OUTPUT
	)
AS
BEGIN
	DECLARE @PERCENTAGE FLOAT = 0.86;
	DECLARE @MMVScore FLOAT = @PERCENTAGE,
		@Mscore FLOAT = @PERCENTAGE,
		@Moscore FLOAT = @PERCENTAGE,
		@Vscore FLOAT = @PERCENTAGE
	DECLARE @ITGIMappingExist BIT = 0,
		@FuelType VARCHAR(10) = '',
		@IsAMT BIT = 0,
		@IsDark BIT = 0,
		@IsTourbo BIT = 0,
		@IsDualTone BIT = 0,
		@HasDecimal BIT = 0,
		@Is4x4 BIT = 0,
		@HasAbs BIT = 0

	SELECT @ITGIMappingExist = CASE 
			WHEN ISNULL(VariantId, '') <> ''
				THEN 1
			ELSE 0
			END
	FROM HeroInsurance.MOTOR.ITGI_VehicleMaster WITH (NOLOCK)
	WHERE VariantId = @VarientId

	SELECT @FuelType = CASE 
			WHEN FuelName = 'PETROL'
				THEN 'PETROL'
			WHEN FuelName = 'DIESEL'
				THEN 'DIESEL'
			WHEN FuelName = 'ELECTRICAL'
				OR FuelName = 'BATTREY'
				OR FuelName = 'ELECTRIC'
				THEN 'Electric'
			WHEN FuelName = 'CNG'
				THEN 'CNG'
			WHEN FuelName = 'LPG'
				THEN 'LPG'
			ELSE FuelName
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
			END,
		@HasAbs = CASE 
			WHEN VariantName LIKE '%ABS%'
				THEN 1
			ELSE 0
			END
	FROM Insurance_Variant VARIANT WITH (NOLOCK)
	LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
	WHERE VariantId = @VarientId

	--SELECT @ITGIMappingExist ITGIMappingExist,@IsTourbo IsTourbo,@IsAMT ATM,@FuelType fule,@IsDark dark,@IsDualTone DT,@hasabs abss,@Is4x4 Is4x4,@HasDecimal HasDecimal
	SELECT *
	INTO #TEMPVARIANT
	FROM (
		SELECT 'ITGI' ICName,
			100 ScoreRank,
			100.00 MScore,
			100.00 MoScore,
			100.00 VScore,
			100.00 MMVScore,
			VARIANT.VariantId HeroVarientID,
			IC.MAKE_CODE ICVehicleCode,
			MAKE.MakeName HEROMake,
			MODEL.ModelName HeroModel,
			VARIANT.VariantName HeroVarient,
			VARIANT.SeatingCapacity HeroSC,
			VARIANT.CubicCapacity HCC,
			FUEL.FuelName HeroFuel,
			IC.MANUFACTURE ICMake,
			IC.MODEL ICModel,
			IC.VARIANT ICVarient,
			IC.MAKE_CODE ICVariantCode,
			IC.SEATING_CAPACITY ICSC,
			IC.CC CCC,
			CASE 
				WHEN IC.FUEL_TYPE = 'PETROL'
					THEN 'PETROL'
				WHEN IC.FUEL_TYPE = 'DIESEL'
					THEN 'DIESEL'
				WHEN IC.FUEL_TYPE = 'ELECTRICAL'
					OR IC.FUEL_TYPE = 'BATTERY'
					OR IC.FUEL_TYPE = 'ELECTRIC'
					THEN 'Electric'
				WHEN IC.FUEL_TYPE = 'CNG'
					THEN 'CNG'
				WHEN IC.FUEL_TYPE = 'LPG'
					THEN 'LPG'
				ELSE IC.FUEL_TYPE
				END ICFuelType,
			VARIANT.VehicleTypeId
		FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
		INNER JOIN HeroInsurance.MOTOR.ITGI_VehicleMaster IC WITH (NOLOCK) ON VARIANT.VariantId = IC.VariantId
		LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
		LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId
		LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
		WHERE VARIANT.VariantId = @VarientId
			AND @ITGIMappingExist = 1
		
		UNION ALL
		
		SELECT 'ITGI' ICName,
			*
		FROM (
			SELECT ROW_NUMBER() OVER (
					PARTITION BY (HEROMake + HEROModel + HeroVarient) ORDER BY MMVScore DESC
					) ScoreRank,
				*
			FROM (
				SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.MANUFACTURE), dbo.RemoveNonAlphanumeric(Hero.MakeName)) MScore,
					dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VARIANT), dbo.RemoveNonAlphanumeric(Hero.VariantName)) VScore,
					dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.MODEL), dbo.RemoveNonAlphanumeric(Hero.ModelName)) MoScore,
					dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
								IC.MANUFACTURE,
								IC.MODEL,
								IC.VARIANT
								)), dbo.RemoveNonAlphanumeric(CONCAT (
								Hero.MakeName,
								Hero.ModelName,
								Hero.VariantName
								))) MMVScore,
					Hero.VariantId HeroVarientID,
					IC.MAKE_CODE ICVehicleCode,
					Hero.MakeName HEROMake,
					Hero.ModelName HeroModel,
					Hero.VariantName HeroVarient,
					Hero.SeatingCapacity HeroSC,
					Hero.CubicCapacity HCC,
					Hero.FuelName HeroFuel,
					IC.MANUFACTURE ICMake,
					IC.MODEL ICModel,
					IC.VARIANT ICVarient,
					IC.MAKE_CODE ICVariantCode,
					IC.SEATING_CAPACITY ICSC,
					IC.CC CCC,
					CASE 
						WHEN IC.FUEL_TYPE = 'PETROL'
							THEN 'PETROL'
						WHEN IC.FUEL_TYPE = 'DIESEL'
							THEN 'DIESEL'
						WHEN IC.FUEL_TYPE = 'ELECTRICAL'
							OR IC.FUEL_TYPE = 'BATTERY'
							OR IC.FUEL_TYPE = 'ELECTRIC'
							THEN 'Electric'
						WHEN IC.FUEL_TYPE = 'CNG'
							THEN 'CNG'
						WHEN IC.FUEL_TYPE = 'LPG'
							THEN 'LPG'
						ELSE IC.FUEL_TYPE
						END ICFuelType,
					Hero.VehicleTypeId
				FROM (
					SELECT *
					FROM MOTOR.ITGI_VehicleMaster WITH (NOLOCK)
					WHERE @ITGIMappingExist <> 1
						AND VariantId IS NULL
						AND FUEL_TYPE = @FuelType
						AND (
							VARIANT LIKE CASE 
								WHEN @IsAMT = 1
									THEN '%AMT%'
								END
							OR ISNULL(@IsAMT, 0) = 0
							)
						AND (
							VARIANT LIKE CASE 
								WHEN @IsDark = 1
									THEN '%DARK%'
								END
							OR ISNULL(@IsDark, 0) = 0
							)
						AND (
							VARIANT LIKE CASE 
								WHEN @IsTourbo = 1
									THEN '%TOURBO%'
								END
							OR ISNULL(@IsTourbo, 0) = 0
							)
						AND (
							VARIANT LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%DUAL TONE%'
								END
							OR VARIANT LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%DT%'
								END
							OR ISNULL(@IsDualTone, 0) = 0
							)
						AND (
							VARIANT LIKE CASE 
								WHEN @HasAbs = 1
									THEN '%ABS%'
								END
							OR ISNULL(@HasAbs, 0) = 0
							)
						AND (
							CHARINDEX('.', VARIANT) > 0
							OR ISNULL(@HasDecimal, 0) = 0
							)
						AND (
							VARIANT LIKE CASE 
								WHEN @Is4x4 = 1
									THEN '%4*4%'
								END
							OR VARIANT LIKE CASE 
								WHEN @Is4x4 = 1
									THEN '%4x4%'
								END
							OR VARIANT LIKE CASE 
								WHEN @Is4x4 = 1
									THEN '%4WD%'
								END
							OR ISNULL(@Is4x4, 0) = 0
							)
						AND (
							VARIANT LIKE CASE 
								WHEN @HasAbs = 1
									THEN '%ABS%'
								END
							OR ISNULL(@HasAbs, 0) = 0
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
						VARIANT.VehicleTypeId
					FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
					LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
					LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON MODEL.MakeId = MAKE.MakeId
					LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
					WHERE VariantId = @VarientId
					) Hero
				) v
			WHERE v.MMVScore > @MMVScore
				-- AND v.MScore > @MScore    
				-- AND v.VScore > @Vscore    
				-- AND v.MoScore > @Moscore    
				AND HeroSC = ICSC
				AND HCC = CCC
				--AND (
				--    LEN(HeroVarient) - LEN(REPLACE(HeroVarient, ' ', '')) = LEN(ICVarient) - LEN(REPLACE(ICVarient, ' ', ''))
				--    OR VehicleTypeId = 'D19E7295-CCB7-491E-B397-400A6E20A5BD'
				--    )
			) v
		WHERE ScoreRank = 1
		) A

	IF EXISTS (
			SELECT TOP 1 MMVScore
			FROM #TEMPVARIANT
			WHERE MMVScore >= @PERCENTAGE
			)
	BEGIN
		UPDATE ITGIVARIANT
		SET VariantId = VARIANT.HeroVarientID,
			IsManuallyMapped = 0
		FROM #TEMPVARIANT VARIANT
		LEFT JOIN MOTOR.ITGI_VehicleMaster ITGIVARIANT WITH (NOLOCK) ON ITGIVARIANT.MAKE_CODE = VARIANT.ICVehicleCode
		WHERE ISNULL(IsManuallyMapped, 0) = 0

		SET @IsVariantMapped = 1
	END
	ELSE
	BEGIN
		SET @IsVariantMapped = 0
	END
END