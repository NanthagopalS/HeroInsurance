
-- =============================================            
-- Author:  <Suraj Kumar Gupta>            
-- Create date: <01-11-2023>            
-- Description: <GetVariantprobablity>            
/*      
 EXEC Insurance_GetVariantProbabilityForReliance '0E23DAB0-E8EB-4A99-B6D7-855050B4A558'          
 */
-- =============================================            
CREATE PROCEDURE [dbo].[Insurance_GetVariantProbabilityForReliance] @VarientId VARCHAR(50) = NULL,
	@IsVariantMapped BIT OUTPUT
AS
BEGIN
	DECLARE @PERCENTAGE FLOAT = 0.85;
	DECLARE @MMVScore FLOAT = @PERCENTAGE,
		@Mscore FLOAT = @PERCENTAGE,
		@Moscore FLOAT = @PERCENTAGE,
		@Vscore FLOAT = 0.55
	DECLARE @RelianceMappingExist BIT = 0,
		@FuelType VARCHAR(10) = '',
		@IsAMT BIT = 0,
		@IsDark BIT = 0,
		@IsTourbo BIT = 0,
		@IsDualTone BIT = 0,
		@Is4x4 BIT = 0,
		@HasDecimal BIT = 0,
		@HasAbs BIT = 0

	SELECT @RelianceMappingExist = CASE 
			WHEN ISNULL(VarientId, '') <> ''
				THEN 1
			ELSE 0
			END
	FROM MOTOR.Reliance_VehicleMaster WITH (NOLOCK)
	WHERE VarientId = @VarientId

	SELECT @FuelType = CASE 
			WHEN FuelName = 'Petrol'
				THEN 'PETROL'
			WHEN FuelName = 'Diesel'
				THEN 'DIESEL'
			WHEN FuelName = 'CNG'
				THEN 'CNG'
			WHEN FuelName = 'LPG'
				THEN 'LPG'
			WHEN FuelName = 'Electric'
				THEN 'BATTERY OPERATED'
			ELSE FuelName
			END,
		@IsAMT = CASE 
			WHEN dbo.RemoveNonAlphanumeric(VariantName) LIKE '%AMT%'
				OR VariantName LIKE '% AUTO%'
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
		@HasDecimal = CASE 
			WHEN CHARINDEX('.', VariantName) > 0
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

	-- SELECT @IsTourbo IsTurbo,@FuelType FuleType,@IsAMT AMT,@ISDark Dark,@IsDualTone DualTone,@Is4x4 FWD       
	SELECT *
	INTO #TEMPVARIANT
	FROM (
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
				END ICFuel
		FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
		INNER JOIN HeroInsurance.MOTOR.Reliance_VehicleMaster IC WITH (NOLOCK) ON VARIANT.VariantId = IC.VarientId
		LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
		LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON model.MakeId = MAKE.MakeId
		LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
		WHERE VARIANT.VariantId = @VarientId
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
								))) MMVScore,
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
						END ICFuel
				FROM (
					SELECT *
					FROM HeroInsurance.MOTOR.Reliance_VehicleMaster WITH (NOLOCK)
					WHERE @RelianceMappingExist <> 1
						AND VarientId IS NULL
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
						FuelName
					FROM INSURANCE_VARIANT VARIANT WITH (NOLOCK)
					LEFT JOIN Insurance_Model MODEL WITH (NOLOCK) ON VARIANT.ModelId = MODEL.ModelId
					LEFT JOIN Insurance_Make MAKE WITH (NOLOCK) ON MODEL.MakeId = MAKE.MakeId
					LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
					WHERE VariantId = @VarientId
					) Hero
				) v
			WHERE v.MMVScore > @MMVScore
				AND v.MScore > @MScore
				--AND v.VScore > @Vscore          
				AND v.MoScore > @Moscore
				AND HeroSC = ICSC
				AND HCC = CCC
				AND LEN(HeroVarient) - LEN(REPLACE(HeroVarient, ' ', '')) = LEN(ICVarient) - LEN(REPLACE(ICVarient, ' ', ''))
			) v
		WHERE ScoreRank = 1
		) A

	IF EXISTS (
			SELECT TOP 1 MMVScore
			FROM #TEMPVARIANT
			WHERE MMVScore >= @PERCENTAGE
			)
	BEGIN
		UPDATE RelianceVARIANT
		SET VarientId = VARIANT.HeroVarientID,
			IsManuallyMapped = 0
		FROM #TEMPVARIANT VARIANT
		LEFT JOIN MOTOR.Reliance_VehicleMaster RelianceVARIANT WITH (NOLOCK) ON RelianceVARIANT.ModelID = VARIANT.ICVehicleCode
		WHERE ISNULL(IsManuallyMapped, 0) = 0

		SET @IsVariantMapped = 1
	END
	ELSE
	BEGIN
		SET @IsVariantMapped = 0
	END
END