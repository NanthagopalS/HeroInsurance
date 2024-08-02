
-- =============================================          
-- Author:  <Ankit Ghosh>          
-- Create date: <02-11-2023>          
-- Description: <GetVariantprobablity>          
--      
/*      
EXEC Insurance_GetVariantProbabilityforCHOLA 'F1846AFD-24C7-46EA-8D56-2C386D006EA9'       
*/
-- =============================================          
CREATE PROCEDURE [dbo].[Insurance_GetVariantProbabilityforCHOLA] (
	@VarientId VARCHAR(50) = NULL
	,@PolicyTypeId VARCHAR(50) = NULL
	,@IsVariantMapped BIT OUTPUT
	)
AS
BEGIN
	DECLARE @PERCENTAGE FLOAT = 0.80;
	DECLARE @MMVScore FLOAT = @PERCENTAGE
		,@Mscore FLOAT = @PERCENTAGE
		,@Moscore FLOAT = @PERCENTAGE
		,@Vscore FLOAT = @PERCENTAGE
	DECLARE @CHOLAMappingExist BIT = 0
		,@FuelType VARCHAR(10) = ''
		,@IsAMT BIT = 0
		,@IsDark BIT = 0
		,@IsTourbo BIT = 0
		,@IsDualTone BIT = 0
		,@IsPlusSign BIT = 0

	SELECT @CHOLAMappingExist = CASE 
			WHEN ISNULL(VarientId, '') <> ''
				THEN 1
			ELSE 0
			END
	FROM HeroInsurance.MOTOR.CHOLA_VehicleMaster WITH (NOLOCK)
	WHERE VarientId = @VarientId

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
			ELSE 'HYBRID'
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
			END
	FROM Insurance_Variant VARIANT WITH (NOLOCK)
	LEFT JOIN Insurance_Fuel FUEL WITH (NOLOCK) ON VARIANT.FuelId = FUEL.FuelId
	WHERE VariantId = @VarientId

	--SELECT @IsAMT,@IsDark,@IsDualTone,@IsPlusSign,@IsTourbo  
	SELECT *
	INTO #TEMPVARIANT
	FROM (
		SELECT 'CHOLA' ICName
			,*
		FROM (
			SELECT ROW_NUMBER() OVER (
					PARTITION BY (dbo.RemoveNonAlphanumeric(HEROMake + HEROModel + HeroVarient)) ORDER BY MMVScore DESC
					) ScoreRank
				,*
			FROM (
				SELECT dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.Make), dbo.RemoveNonAlphanumeric(Hero.MakeName)) MScore
					,dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleModel), dbo.RemoveNonAlphanumeric(Hero.VariantName)) VScore
					,dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(IC.VehicleModel), dbo.RemoveNonAlphanumeric(Hero.ModelName)) MoScore
					,dbo.fn_calculateJaroWinkler(dbo.RemoveNonAlphanumeric(CONCAT (
								IC.Make
								,IC.VehicleModel
								)), dbo.RemoveNonAlphanumeric(CONCAT (
								Hero.MakeName
								,Hero.ModelName
								,Hero.VariantName
								))) MMVScore
					,dbo.RemoveNonAlphanumeric(IC.Make + IC.VehicleModel) MMVBNAME
					,Hero.VariantId HeroVarientID
					,IC.ModelCode ICVehicleCode
					,Hero.MakeName HEROMake
					,Hero.ModelName HeroModel
					,Hero.VariantName HeroVarient
					,Hero.SeatingCapacity HeroSC
					,Hero.CubicCapacity HCC
					,Hero.FuelName HeroFuel
					,IC.Make ICMake
					,IC.VehicleModel ICModel
					,IC.VehicleModel ICVarient
					,IC.SEATINGCAPACITY ICSC
					,IC.CubicCapacity CCC
					,CASE 
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
					FROM (
						SELECT ROW_NUMBER() OVER (
								PARTITION BY VEHICLEMODEL ORDER BY VEHICLEMODEL
								) ROWSTATE
							,StateName
							,VehicleModel
							,FuelType
							,Make
							,SeatingCapacity
							,CubicCapacity
							,ModelCode
						FROM MOTOR.Chola_VehicleMaster IC
							WHERE  VarientId IS NULL   
						) A
					WHERE ROWSTATE = 1
						AND FuelType = @FuelType
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
								WHEN @IsPlusSign = 1
									THEN '%PLUS%'
								END
							OR ISNULL(@IsPlusSign, 0) = 0
							)
						AND (
							VehicleModel LIKE CASE 
								WHEN @IsDualTone = 1
									THEN '%DUAL TONE%'
								END
							OR VehicleModel LIKE CASE 
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
				AND v.MScore > @MScore
				AND v.VScore > @Vscore
				AND v.MoScore > @Moscore
				AND HeroSC = ICSC
				AND HCC = CCC
				--AND HeroFuel LIKE ICFuel + '%'      
			) v
		WHERE ScoreRank = 1
		) A

	IF EXISTS (
			SELECT TOP 1 MMVScore
			FROM #TEMPVARIANT
			WHERE MMVScore >= @PERCENTAGE
			)
	BEGIN
		UPDATE CHOLAVARIANT
		SET VarientId = VARIANT.HeroVarientID, IsManuallyMapped = 0
		FROM #TEMPVARIANT VARIANT
		LEFT JOIN MOTOR.Chola_VehicleMaster CHOLAVARIANT WITH (NOLOCK) ON CHOLAVARIANT.ModelCode = VARIANT.ICVehicleCode
		WHERE ISNULL(IsManuallyMapped, 0) = 0

		SET @IsVariantMapped = 1
	END
	ELSE
	BEGIN
		SET @IsVariantMapped =0
	END
END