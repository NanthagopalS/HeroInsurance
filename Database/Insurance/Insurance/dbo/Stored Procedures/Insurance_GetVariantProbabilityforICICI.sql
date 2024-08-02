
-- =============================================                
/*
EXEC Insurance_GetVariantProbabilityForICICI 'B3A67C7E-BDDA-4C42-B610-EAAF8FAB4E64'  
*/
-- =============================================                
CREATE PROCEDURE [dbo].[Insurance_GetVariantProbabilityforICICI] (
	@VarientId VARCHAR(50) = NULL,
	@IsVariantMapped BIT OUTPUT
	)
AS
BEGIN
	DECLARE @PERCENTAGE FLOAT = 0.80;
	DECLARE @MMVScore FLOAT = @PERCENTAGE,
		@Mscore FLOAT = @PERCENTAGE,
		@Moscore FLOAT = @PERCENTAGE,
		@Vscore FLOAT = @PERCENTAGE
	DECLARE @ICICIMappingExist BIT = 0,
		@FuelType VARCHAR(10) = '',
		@IsAMT BIT = 0,
		@IsDark BIT = 0,
		@IsTourbo BIT = 0,
		@IsDualTone BIT = 0,
		@HasDecimal BIT = 0,
		@Is4x4 BIT = 0

	SELECT @ICICIMappingExist = CASE 
			WHEN VariantId = @VarientId
				THEN 1
			ELSE 0
			END
	FROM HeroInsurance.MOTOR.ICICI_VehicleMaster WITH (NOLOCK)
	WHERE VariantId = @VarientId

	SELECT @FuelType = CASE 
			WHEN FuelName = 'Petrol'
				THEN 'Petrol C'
			WHEN FuelName = 'Diesel '
				THEN 'Diesel C'
			WHEN FuelName = 'Electric '
				THEN 'Electric T'
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
		SELECT 'ICICI' ICName,
			100 ScoreRank,
			100.00 MScore,
			100.00 MoScore,
			100.00 VScore,
			100.00 MMVScore,
			VARIANT.VariantId HeroVarientID,
			IC.VehicleModelCode ICVehicleCode,
			MAKE.MakeName HEROMake,
			MODEL.ModelName HeroModel,
			VARIANT.VariantName HeroVarient,
			VARIANT.SeatingCapacity HeroSC,
			VARIANT.CubicCapacity HCC,
			FUEL.FuelName HeroFuel,
			IC.Manufacture ICMake,
			IC.VehicleModel ICModel
			-- ,IC.VehicleSubType ICVarient              
			,
			IC.VehicleModelcode ICVariantCode,
			IC.SeatingCapacity ICSC,
			IC.CubicCapacity CCC,
			CASE 
				WHEN IC.FuelType LIKE 'Petrol%'
					THEN 'Petrol'
				WHEN IC.FuelType LIKE 'Diesel%'
					THEN 'Diesel'
				WHEN IC.FuelType LIKE 'Electric%'
					THEN 'Electric'
				ELSE IC.FuelType
				END ICFuelType
		FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
		INNER JOIN HeroInsurance.MOTOR.ICICI_VehicleMaster IC WITH (NOLOCK) ON VARIANT.VariantId = IC.VariantId
		LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
		LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId
		LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
		WHERE VARIANT.VariantId = @VarientId
			AND @ICICIMappingExist = 1
		
		UNION ALL
		
		SELECT 'ICICI' ICName,
			*
		FROM (
			SELECT ROW_NUMBER() OVER (
					PARTITION BY (HEROMake + HEROModel + HeroVarient) ORDER BY MMVScore DESC
					) ScoreRank,
				*
			FROM (
				SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.Manufacture), dbo.RemoveNonAlphanumeric(Hero.MakeName)) MScore,
					dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleModel), dbo.RemoveNonAlphanumeric(Hero.ModelName)) MoScore,
					0 VScore,
					dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
								IC.Manufacture,
								IC.VehicleModel
								)), dbo.RemoveNonAlphanumeric(CONCAT (
								Hero.MakeName,
								Hero.ModelName,
								Hero.VariantName
								))) MMVScore,
					Hero.VariantId HeroVarientID,
					IC.VehicleModelCode ICVehicleCode,
					Hero.MakeName HEROMake,
					Hero.ModelName HeroModel,
					Hero.VariantName HeroVarient,
					Hero.SeatingCapacity HeroSC,
					Hero.CubicCapacity HCC,
					Hero.FuelName HeroFuel,
					IC.Manufacture ICMake,
					IC.VehicleModel ICModel
					--  ,IC.VehicleSubType ICVarient              
					,
					IC.VehicleModelCode ICVariantCode,
					IC.SeatingCapacity ICSC,
					IC.CubicCapacity CCC,
					CASE 
						WHEN IC.FuelType LIKE 'Petrol%'
							THEN 'Petrol'
						WHEN IC.FuelType LIKE 'Diesel%'
							THEN 'Diesel'
						WHEN IC.FuelType LIKE 'Electric%'
							THEN 'Electric'
						ELSE IC.FuelType
						END ICFuelType
				FROM (
					SELECT Manufacture,
						VehicleModel,
						SeatingCapacity,
						CubicCapacity,
						VehicleModelCode,
						FuelType
					FROM HeroInsurance.MOTOR.ICICI_VehicleMaster WITH (NOLOCK)
					WHERE VariantId IS NULL
						AND @ICICIMappingExist <> 1
						AND FuelType LIKE '%' + @FuelType + '%'
						AND (
							VehicleModel LIKE CASE 
								WHEN @IsAMT = 1
									THEN '%AMT%'
								END
							OR ISNULL(@IsAMT, 0) = 0
							)
						AND (
							VehicleModel LIKE CASE 
								WHEN @IsDark = 1
									THEN '%DARK%'
								END
							OR ISNULL(@IsDark, 0) = 0
							)
						AND (
							VehicleModel LIKE CASE 
								WHEN @IsTourbo = 1
									THEN '%TOURBO%'
								END
							OR ISNULL(@IsTourbo, 0) = 0
							)
						AND (
							VehicleModel LIKE CASE 
								WHEN @Is4x4 = 1
									THEN '%4*4%'
								END
							OR VehicleModel LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%4x4%'
								END
							OR VehicleModel LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%4WD%'
								END
							OR ISNULL(@Is4x4, 0) = 0
							)
						AND (
							VehicleModel LIKE CASE 
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
				--AND v.MScore > @MScore              
				--AND v.VScore > @Vscore              
				--AND v.MoScore > @Moscore              
				AND HeroSC = ICSC
				--AND HCC = CCC
			) v
		WHERE ScoreRank = 1
		) A

	IF EXISTS (
			SELECT TOP 1 MMVScore
			FROM #TEMPVARIANT
			WHERE MMVScore >= @PERCENTAGE
			)
	BEGIN
		UPDATE VARIANT
		SET VariantId = TEMPVARIANT.HeroVarientID,
			IsManuallyMapped = 0
		FROM #TEMPVARIANT TEMPVARIANT
		LEFT JOIN MOTOR.ICICI_VehicleMaster VARIANT WITH (NOLOCK) ON TEMPVARIANT.ICVariantCode = VARIANT.VehicleModelCode
		WHERE ISNULL(IsManuallyMapped, 0) = 0

		SET @IsVariantMapped = 1
	END
	ELSE
	BEGIN
		SET @IsVariantMapped = 0
	END
END